using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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



        internal int currentSpeaker { get; set; }

        internal bool advancePaused { get; set; }



        internal DateTime lastAdvancement { get; set; }

        internal int CountParticipants
        {
            get => membersInStory.Count;
        }

        internal string ListParticipants
        {
            get => string.Join(',', membersInStory.Select(x => x.Username).ToList());
        }
        private List<IGuildUser> membersInStory { get; set; }

        private List<string> messages { get; set; }


        public MaidService(HttpClient http)
        {
            http.Timeout = new TimeSpan(0, 0, 5);
            _http = http;

            membersInStory = new List<IGuildUser>();
            currentSpeaker = 0;
            advancePaused = false;
            lastAdvancement = DateTime.UtcNow;
        }

        public bool Contains(IGuildUser user)
        {
            return membersInStory.Contains(user);
        }

        public void Clear()
        {
            membersInStory.Clear();
            currentSpeaker = 0;
        }
        public void AddParticipant(IGuildUser user)
        {
            membersInStory.Add(user);
        }

        public void RemoveParticipant(IGuildUser user)
        {
            membersInStory.Remove(user);
        }

        public bool IsCurrentSpeaker(IGuildUser user)
        {
            if(membersInStory.Count == 0)
            {
                return false;
            }
            return membersInStory[currentSpeaker] == user;
        }

        public string CurrentSpeaker()
        {
            if (membersInStory.Count == 0)
            {
                return "Nobody participating";
            }
            return membersInStory[currentSpeaker].Mention;
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

        internal void AdvanceSpeaker()
        {
            if(currentSpeaker + 1 >= membersInStory.Count)
            {
                currentSpeaker = 0;
            }
            else
            {
                currentSpeaker += 1;
            }
            lastAdvancement = DateTime.UtcNow;
        }

        public async Task<string> PostResponseAsync(string response)
        {
            advancePaused = true;
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