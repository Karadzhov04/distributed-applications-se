using ClickEatApp.Core.DTOs;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;

namespace ClickEatApp.Web.Services
{
	public class AuthApiService
	{
		private readonly HttpClient _httpClient;

		public AuthApiService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		public async Task<bool> RegisterUserAsync(RegisterDto user)
		{
			var content = new StringContent(
				JsonSerializer.Serialize(user),
				Encoding.UTF8,
				"application/json"
				);

			var response = await _httpClient.PostAsync("User", content);

			return response.IsSuccessStatusCode;
		}

		public async Task<string?> LoginUserAsync(LoginDto user)
		{
			var content = new StringContent(
				JsonSerializer.Serialize(user),
				Encoding.UTF8,
				"application/json"
				);

			var response = await _httpClient.PostAsync("Auth", content);

			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = JsonSerializer.Deserialize<TokenResponseDto>(json, options);
            return result?.Token;
        }
	}
}
