using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ubad.Services;

namespace ubad.Modules
{
    public class MaidModule : ModuleBase<SocketCommandContext>
    {
        public MaidService MaidService { get; set; }

        private string maxMessage { get; set; }
        private string reply { get; set; }

        private List<string> messages { get; set; }

        [Command("maid", RunMode = RunMode.Async)]
        public async Task GetStory(params string[] objects)
        {
            if (objects[0] == "get")
            {
                reply = await MaidService.GetResponseAsync();
            }
            else if (objects[0] == "reply")
            {
                reply = await MaidService.PostResponseAsync(string.Join(' ', objects[1..]));
                await Task.Delay(new TimeSpan(0, 0, 10));
                reply = await MaidService.GetResponseAsync();
            }
            else
            {
                await ReplyAsync("Invalid option, please specify either \"get\" or \"reply\" ");
                return;
            }

            if (reply.Length > 1999)
            {
                while (reply.Length > 1999)
                {
                    maxMessage = reply[..1999];
                    // messages.Add(maxMessage);
                    await ReplyAsync(maxMessage);
                    reply = reply[2000..];
                }
            }
            // messages.Add(reply);
            await ReplyAsync(reply);
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

        //[Command("maid print")]
        //public async Task PrintMessages()
        //{
        //    foreach(var x in messages)
        //    {
        //        await ReplyAsync(x);
        //    }

        //}
    }
}