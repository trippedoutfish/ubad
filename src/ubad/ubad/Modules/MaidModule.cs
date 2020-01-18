using Discord;
using Discord.Commands;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ubad.Services;

namespace ubad.Modules
{
    public class MaidModule : ModuleBase<SocketCommandContext>
    {
        public MaidService MaidService { get; set; }

        [Command("maid", RunMode = RunMode.Async)]
        public async Task GetStory(params string[] objects)
        {
            if (objects[0] == "get")
            {
                var reply = await MaidService.GetResponseAsync();
                await ReplyAsync(reply);
            }
            else if (objects[0] == "reply")
            {
                await MaidService.PostResponseAsync(string.Join(' ', objects[1..]));
                await Task.Delay(new TimeSpan(0, 0, 10));
                var reply = await MaidService.GetResponseAsync();
                await ReplyAsync(reply);
            }
            else
            {
                await ReplyAsync("Invalid option, please spcify either \"get\" or \"reply\" ");
            }
        }

        //[Command("activeChannel", RunMode = RunMode.Async)]
        //public async Task GetStory(params string[] objects)
        //{
        // TODO: Add Ability for bot to restrict commands to live comments from people in a chat channel
        //}


    }
}
