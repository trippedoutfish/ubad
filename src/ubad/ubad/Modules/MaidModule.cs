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

        [Command("setChannel", RunMode = RunMode.Async)]
        public async Task SetChannel()
        {
            // Get the audio channel
            IVoiceChannel channel = null;
            channel ??= (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument.");
                return;
            }
            await ReplyAsync("Not Implemented yet :^)");
        }


    }
}
