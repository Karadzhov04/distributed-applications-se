using ClickEatApp.Core.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace ClickEatApp.Web.Services
{
	public class FoodApiService
	{
        private readonly HttpClient _httpClient;

        public FoodApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<FoodListResultDto> GetFilteredFoodsAsync(
             int restaurantId,
             string? name,
             int? minCalories,
             string? sortBy,
             string? sortDirection,
             int page,
             int pageSize)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["restaurantId"] = restaurantId.ToString();

            if (!string.IsNullOrEmpty(name))
                query["name"] = name;

            if (minCalories.HasValue)
                query["minCalories"] = minCalories.Value.ToString();

            query["sortBy"] = sortBy ?? "id";
            query["sortDirection"] = sortDirection ?? "asc";
            query["page"] = page.ToString();
            query["pageSize"] = pageSize.ToString();

            var response = await _httpClient.GetAsync($"Food/filtered?{query}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<FoodListResultDto>();
        }

        public async Task<FoodEditDto> GetFoodByIdAsync(int id, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync($"Food/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var food = JsonSerializer.Deserialize<FoodEditDto>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return food;
            }

            return null;
        }

        public async Task<List<FoodViewDto>> GetFoodsByRestaurantIdAsync(int restaurantId, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync($"Food/GetAllByRestaurantId/{restaurantId}");

            if (!response.IsSuccessStatusCode)
            {
                // логика при грешка
                return new List<FoodViewDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<FoodViewDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> CreateFoodAsync(FoodCreateDto foodDto, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var form = new MultipartFormDataContent();

            form.Add(new StringContent(foodDto.Name), "Name");
            form.Add(new StringContent(foodDto.Description), "Description");
            form.Add(new StringContent(foodDto.Calories.ToString()), "Calories");
            form.Add(new StringContent(foodDto.Price.ToString()), "Price");
            form.Add(new StringContent(foodDto.RestaurantId.ToString()), "RestaurantId");

            if (foodDto.ImageFile != null && foodDto.ImageFile.Length > 0)
            {
                var streamContent = new StreamContent(foodDto.ImageFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(foodDto.ImageFile.ContentType);
                form.Add(streamContent, "ImageFile", foodDto.ImageFile.FileName);
            }

            var response = await _httpClient.PostAsync("Food", form);

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Error: " + error);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> EditFoodAsync(int id, FoodEditDto foodDto, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var form = new MultipartFormDataContent();

            form.Add(new StringContent(foodDto.Name), "Name");
            form.Add(new StringContent(foodDto.Description), "Description");
            form.Add(new StringContent(foodDto.Calories.ToString()), "Calories");
            form.Add(new StringContent(foodDto.Price.ToString()), "Price");
            form.Add(new StringContent(foodDto.RestaurantId.ToString()), "RestaurantId");

            if (foodDto.ImageFile != null && foodDto.ImageFile.Length > 0)
            {
                var streamContent = new StreamContent(foodDto.ImageFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(foodDto.ImageFile.ContentType);
                form.Add(streamContent, "ImageFile", foodDto.ImageFile.FileName);
            }

            var response = await _httpClient.PutAsync($"Food/{id}", form);

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Error: " + error);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteFoodAsync(int id, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.DeleteAsync($"Food/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
