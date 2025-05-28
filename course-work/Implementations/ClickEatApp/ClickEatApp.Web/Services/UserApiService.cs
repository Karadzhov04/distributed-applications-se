using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using ClickEatApp.Core.DTOs;


public class UserApiService
{
	private readonly HttpClient _httpClient;

	public UserApiService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<UserDto> GetUserById(int id, string jwtToken)
	{
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

		var response = await _httpClient.GetAsync($"User/{id}");

		if (response.IsSuccessStatusCode)
		{
			var jsonString = await response.Content.ReadAsStringAsync();
			var user = JsonSerializer.Deserialize<UserDto>(jsonString, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});

			return user;
		}

		return null; 
	}

	public async Task<List<UserDto>> GetAllUsersAsync(string jwtToken)
	{
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

		var response = await _httpClient.GetAsync("User");

		if (!response.IsSuccessStatusCode)
		{
			return new List<UserDto>();
		}

		var json = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<List<UserDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
	}
	public async Task<bool> EditUserAsync(int id, UserEditDto user, string jwtToken)
	{
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

		var content = new StringContent(
			JsonSerializer.Serialize(user),
			Encoding.UTF8,
			"application/json"
		);

		var response = await _httpClient.PutAsync($"User/{id}", content);

		return response.IsSuccessStatusCode;
	}

	public async Task<bool> DeleteUserAsync(int id, string jwtToken)
	{
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

		var response = await _httpClient.DeleteAsync($"User/{id}");

		return response.IsSuccessStatusCode;
	}

    public async Task<UserListResultDto> GetFilteredUsersAsync(string? name, string? email, string? sortBy, string? sortDirection, int page, int pageSize)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrEmpty(name))
            query["name"] = name;

        if (!string.IsNullOrEmpty(email))
            query["email"] = email;

        query["sortBy"] = sortBy ?? "id";
        query["sortDirection"] = sortDirection ?? "asc";
        query["page"] = page.ToString();
        query["pageSize"] = pageSize.ToString();

        var response = await _httpClient.GetAsync($"User/filtered?{query}");
        var content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserListResultDto>();
    }
}

