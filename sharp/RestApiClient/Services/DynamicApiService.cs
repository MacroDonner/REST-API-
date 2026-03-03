using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RestApiClient.Services
{
    public class DynamicApiService
    {
        private readonly HttpClient _httpClient;

        public DynamicApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<(string Content, HttpStatusCode Status)> FetchDataAsync(string url, string token = null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrWhiteSpace(token))
            {
                // Добавляем токен в заголовок Authorization
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            using var response = await _httpClient.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();

            return (content, response.StatusCode);
        }
    }
}
