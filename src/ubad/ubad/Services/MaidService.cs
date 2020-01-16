using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ubad.JsonModels;
using System.Linq;

namespace ubad.Services
{
    public class MaidService
    {
        private readonly HttpClient _http;

        public MaidService(HttpClient http)
            => _http = http;

        public async Task<string> GetResponseAsync()
        {
            var resp = await _http.GetAsync(Environment.GetEnvironmentVariable("maidApiClient") + "/api/pending_client_actions");
            var json = await resp.Content.ReadAsStringAsync();
            var maid = JsonConvert.DeserializeObject<MaidApi[]>(json);
            maid.Last().Response = "**" + maid.Last().Response + "**";
            return string.Join('\n', maid.Select(x => x.Response).ToList());
        }

        public async Task<HttpStatusCode> PostResponseAsync(string response)
        {
            var content = new StringContent("{\"response\": \"" + response + "\"}", Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync(Environment.GetEnvironmentVariable("maidApiClient") + "/api/pending_server_actions", content);
            return resp.StatusCode;
        }
    }
}
