using ClickEatApp.Core.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Web;

namespace ClickEatApp.Web.Services
{
    public class RestaurantApiService
    {
        private readonly HttpClient _httpClient;

        public RestaurantApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RestaurantViewDto> GetRestaurantById(int id, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync($"Restaurant/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var restaurant = JsonSerializer.Deserialize<RestaurantViewDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return restaurant;
            }

            return null;
        }

        public async Task<List<RestaurantViewDto>> GetAllRestaurantsAsync(string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync("Restaurant");

            if (!response.IsSuccessStatusCode)
            {
                // логика при грешка
                return new List<RestaurantViewDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<RestaurantViewDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CreateRestaurantAsync(RestaurantCreateDto restaurantDto, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var form = new MultipartFormDataContent();

            form.Add(new StringContent(restaurantDto.Name), "Name");
            form.Add(new StringContent(restaurantDto.Description), "Description");
            form.Add(new StringContent(restaurantDto.IsOpen.ToString()), "IsOpen");
            form.Add(new StringContent(restaurantDto.OwnerId.ToString()), "OwnerId");

            if (restaurantDto.Image != null && restaurantDto.Image.Length > 0)
            {
                var streamContent = new StreamContent(restaurantDto.Image.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(restaurantDto.Image.ContentType);
                form.Add(streamContent, "Image", restaurantDto.Image.FileName);
            }

            var response = await _httpClient.PostAsync("Restaurant", form);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> EditRestaurantAsync(int id, RestaurantEditDto restaurantDto, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var form = new MultipartFormDataContent();

            form.Add(new StringContent(restaurantDto.Name), "Name");
            form.Add(new StringContent(restaurantDto.Description), "Description");
            form.Add(new StringContent(restaurantDto.IsOpen.ToString()), "IsOpen");
            if (restaurantDto.OwnerId.HasValue)
                form.Add(new StringContent(restaurantDto.OwnerId.Value.ToString()), "OwnerId");

            if (restaurantDto.Image != null && restaurantDto.Image.Length > 0)
            {
                var streamContent = new StreamContent(restaurantDto.Image.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(restaurantDto.Image.ContentType);
                form.Add(streamContent, "Image", restaurantDto.Image.FileName);
            }

            var response = await _httpClient.PutAsync($"Restaurant/{id}", form);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRestaurantAsync(int id, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.DeleteAsync($"Restaurant/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<RestaurantListResultDto> GetFilteredRestaurantsAsync(string? name, string? ownerName, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrEmpty(name))
                query["name"] = name;

            if (!string.IsNullOrEmpty(ownerName))
                query["ownerName"] = ownerName;

            query["sortBy"] = sortBy ?? "id";
            query["sortDirection"] = sortDirection ?? "asc";
            query["page"] = page.ToString();
            query["pageSize"] = pageSize.ToString();

            var response = await _httpClient.GetAsync($"Restaurant/filtered?{query}");
            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<RestaurantListResultDto>();
        }
    }
}
