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
                    throw new Exception($"Login failed: {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                return result.Token ?? throw new Exception("Token is missing in response");
            }
            catch (Exception e)
            {
                throw new Exception($"Error during logging in: {e.Message}");
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
                    throw new Exception($"Registration failed: {error}");
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Error during registration: {e.Message}");
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
                    throw new Exception($"Change password failed: {error}");
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Error during password change: {e.Message}");
            }
        }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
    }
}
