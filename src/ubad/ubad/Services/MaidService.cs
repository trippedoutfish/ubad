using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ubad.JsonModels;

namespace ubad.Services
{
    public class MaidService
    {
        private readonly HttpClient _http;
        private readonly static string ApiConnectionError = "MAID Api Error, likely that the MAID service is not running, please contact the MAID tier 3 helpdesk.";

        public MaidService(HttpClient http)
        {
            http.Timeout = new TimeSpan(0, 0, 5);
            _http = http;
        }

        public async Task<string> GetResponseAsync()
        {
            try
            {
                var resp = await _http.GetAsync(Environment.GetEnvironmentVariable("maidApiClient") + "/api/pending_client_actions");
                var json = await resp.Content.ReadAsStringAsync();
                var maid = JsonConvert.DeserializeObject<MaidApi[]>(json);
                maid.Last().Response = "**" + maid.Last().Response + "**";
                return string.Join('\n', maid.Select(x => x.Response).ToList());
            }
            catch (TaskCanceledException em)
            {
            }
            return ApiConnectionError;
        }

        public async Task<string> PostResponseAsync(string response)
        {
            try
            {
                var content = new StringContent("{\"response\": \"" + response + "\"}", Encoding.UTF8, "application/json");
                var resp = await _http.PostAsync(Environment.GetEnvironmentVariable("maidApiClient") + "/api/pending_server_actions", content);
                return "";
            }
            catch (TaskCanceledException em)
            {
            }
            return ApiConnectionError;
        }
    }
}