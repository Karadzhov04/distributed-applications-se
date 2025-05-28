using ClickEatApp.Core.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ClickEatApp.Core;
using System.Web;

namespace ClickEatApp.Web.Services
{
	public class OrderApiService
	{
        private readonly HttpClient _httpClient;

        public OrderApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateOrderAsync(OrderCreateDto dto, string jwtToken)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.PostAsync("Orders", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.GetAsync("Orders/UserOrders");

            if (!response.IsSuccessStatusCode)
                return new List<OrderDto>();

            var json = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return orders ?? new List<OrderDto>();
        }

        public async Task<OrderListResultDto> GetFilteredOrdersAsync(string? clientName, DateTime? orderDate, decimal? minPrice, string? sortBy, string? sortDirection, int page, int pageSize)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrEmpty(clientName))
                query["clientName"] = clientName;

            if (orderDate.HasValue)
                query["orderDate"] = orderDate.Value.ToString("yyyy-MM-dd");

            if (minPrice.HasValue)
                query["minPrice"] = minPrice.Value.ToString();

            query["sortBy"] = sortBy ?? "id";
            query["sortDirection"] = sortDirection ?? "asc";
            query["page"] = page.ToString();
            query["pageSize"] = pageSize.ToString();

            var response = await _httpClient.GetAsync($"Orders?{query}");
            var content = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OrderListResultDto>();
        }

    }
}
