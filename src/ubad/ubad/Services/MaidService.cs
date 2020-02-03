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
            get => participantsInStory.Count;
        }

        internal string ListParticipants
        {
            get
            {
                if(participantsInStory.Count == 0)
                {
                    return "There are no participants";
                }
                return string.Join(',', participantsInStory.Select(x => x.Username).ToList());
            }
        }
        private List<IGuildUser> participantsInStory { get; set; }

        private List<string> messages { get; set; }


        public MaidService(HttpClient http)
        {
            http.Timeout = new TimeSpan(0, 0, 5);
            _http = http;

            participantsInStory = new List<IGuildUser>();
            currentSpeaker = 0;
            advancePaused = false;
            lastAdvancement = DateTime.UtcNow;
        }

        public bool Contains(IGuildUser user)
        {
            return participantsInStory.Contains(user);
        }

        public void Clear()
        {
            participantsInStory.Clear();
            currentSpeaker = 0;
        }
        public void AddParticipant(IGuildUser user)
        {
            participantsInStory.Add(user);
        }

        public void RemoveParticipant(IGuildUser user)
        {
            participantsInStory.Remove(user);
            if (currentSpeaker >= participantsInStory.Count)
            {
                currentSpeaker = participantsInStory.Count - 1;
            }
        }

        public bool IsCurrentSpeaker(IGuildUser user)
        {
            if(participantsInStory.Count == 0)
            {
                return false;
            }
            return participantsInStory[currentSpeaker] == user;
        }

        public string CurrentSpeaker()
        {
            if (participantsInStory.Count == 0)
            {
                return "Nobody";
            }
            return participantsInStory[currentSpeaker].Mention;
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
                Console.WriteLine(em);
            }
            return ApiConnectionError;
        }

        internal void AdvanceSpeaker()
        {
            if(currentSpeaker + 1 >= participantsInStory.Count)
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
                Console.WriteLine(em);
            }
            return ApiConnectionError;
        }

        internal IGuildUser FindParticipant(string userId)
        {
            ulong convertedId;
            if(UInt64.TryParse(userId, out convertedId))
            {
                return participantsInStory.Find(x => x.Id == convertedId);
            }
            return null;
        }
    }
}