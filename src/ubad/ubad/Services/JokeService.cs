using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ubad.Services
{
    public class JokeService
    {
        private readonly HttpClient _http;

        public JokeService(HttpClient http)
            => _http = http;

        public async Task<string> GetJokeAsync(string prompt)
        {
            var resp = await _http.GetAsync(Environment.GetEnvironmentVariable("JokeApi") + prompt.Replace(" ", "+"));
            return await resp.Content.ReadAsStringAsync();
        }
    }
}