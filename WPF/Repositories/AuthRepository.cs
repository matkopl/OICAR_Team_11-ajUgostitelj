using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebAPI.DTOs;

namespace WPF.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "http://localhost:5207/api/auth";

        public AuthRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            try
            {
                LoginDto loginDto = new()
                {
                    Username = username,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync($"{_apiKey}/login", loginDto);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception(error);
                }

                var token = await response.Content.ReadAsStringAsync();
                return token ?? throw new Exception("Token is missing in response");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiKey}/register", registerDto);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception(error);
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> ChangePasswordAsync(string token, ChangePasswordDto changePasswordDto)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync($"{_apiKey}/change_password", changePasswordDto);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception(error);
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<UserDto?> GetUserDetailsAsync(string username, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"api/user/{username}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to load user details: {error}");
                }

                return await response.Content.ReadFromJsonAsync<UserDto>();
            }
            catch (Exception e)
            {
                throw new Exception($"Error loading user details: {e.Message}");
            }
        }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
    }
}
