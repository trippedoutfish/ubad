using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ubad.Services;

namespace ubad.Modules
{
    public class MaidModule : ModuleBase<SocketCommandContext>
    {
        

        public MaidService MaidService { get; set; }

        private string reply { get; set; }
        private string maxMessage { get; set; }


        [Command("maid", RunMode = RunMode.Async)]
        public async Task GetStory(params string[] objects)
        {
            if (objects[0] == "join")
            {
                if (MaidService.Contains(Context.User as IGuildUser))
                {
                    await ReplyAsync($"{Context.User.Username} you silly goose, you are already in the story rotation.");
                    return;
                }
                MaidService.AddParticipant(Context.User as IGuildUser);
                await ReplyAsync($"{Context.User.Username} you have been added to the story rotation, there  are {MaidService.CountParticipants} people participating,");
                return;
            }
            else if (objects[0] == "leave")
            {
                if (!MaidService.Contains(Context.User as IGuildUser))
                {
                    MaidService.RemoveParticipant(Context.User as IGuildUser);
                    await ReplyAsync($"{Context.User.Username} you have been removed from the participants list.");
                    return;
                }
                await ReplyAsync($"{Context.User.Username} you are not participating.");
                return;
            }

            if (!MaidService.Contains(Context.User as IGuildUser))
            {
                await ReplyAsync($"{Context.User.Username} you silly goose, participate by joining the rotation with **!maid join**.");
                return;
            }


            if (objects[0] == "get")
            {
                reply = await MaidService.GetResponseAsync();
                MaidService.advancePaused = false;
                await DisplayWholeReply();
                return;
            }
            else if (objects[0] == "reply")
            {
                if ((MaidService.IsCurrentSpeaker(Context.User as IGuildUser) && !MaidService.advancePaused) || DateTime.UtcNow.Subtract(MaidService.lastAdvancement).TotalSeconds > 30)
                {
                    reply = await MaidService.PostResponseAsync(string.Join(' ', objects[1..]));
                    await Task.Delay(new TimeSpan(0, 0, 10));
                    reply = await MaidService.GetResponseAsync();
                    if (reply != "Still Generating...")
                    {
                        MaidService.advancePaused = false;
                        MaidService.AdvanceSpeaker();
                    }
                    await DisplayWholeReply();
                    return;
                }
            }
            else if (objects[0] == "participants")
            {
                await ReplyAsync(MaidService.ListParticipants);
                return;
            }
            else if (objects[0] == "clear")
            {
                MaidService.Clear();
                return;
            }
            else if (objects[0] == "speaker")
            {
                await ReplyAsync($"{MaidService.CurrentSpeaker()} is up!");
                return;
            }
            else
            {
                await ReplyAsync("Invalid option, please specify either \"get\" or \"reply\" ");
                return;
            }

            
        }

        private async Task DisplayWholeReply()
        {
            if (reply.Length > 1999)
            {
                while (reply.Length > 1999)
                {
                    maxMessage = reply[..1999];
                    await ReplyAsync(maxMessage);
                    reply = reply[2000..];
                }
            }
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

        [Command("mr", RunMode = RunMode.Async)]
        public async Task PostReply(params string[] objects)
        {
            if ((MaidService.IsCurrentSpeaker(Context.User as IGuildUser) && !MaidService.advancePaused) || DateTime.UtcNow.Subtract(MaidService.lastAdvancement).TotalSeconds > 30)
            {
                reply = await MaidService.PostResponseAsync(string.Join(' ', objects[1..]));
                await Task.Delay(new TimeSpan(0, 0, 10));
                reply = await MaidService.GetResponseAsync();
                if (reply != "Still Generating...")
                {
                    MaidService.advancePaused = false;
                    MaidService.AdvanceSpeaker();
                }
                await DisplayWholeReply();
                return;
            }
            else
            { 
                if(DateTime.UtcNow.Subtract(MaidService.lastAdvancement).TotalSeconds > 0)
                {
                    await ReplyAsync($"It's not your turn to reply or you need to wait {30 - Math.Floor(DateTime.UtcNow.Subtract(MaidService.lastAdvancement).TotalSeconds)} seconds.");
                }
                else
                {
                    await ReplyAsync($"It's not your turn, **{MaidService.CurrentSpeaker()}** get typing, you will lose  your turn in {30 - Math.Floor(DateTime.UtcNow.Subtract(MaidService.lastAdvancement).TotalSeconds)} seconds.");
                }
            }
        }

        [Command("mg", RunMode = RunMode.Async)]
        public async Task GetReply(params string[] objects)
        {
            if (!MaidService.Contains(Context.User as IGuildUser))
            {
                await ReplyAsync($"{Context.User.Username} you silly goose, participate by joining the rotation with **!maid join**.");
                return;
            }
            reply = await MaidService.GetResponseAsync();
            MaidService.advancePaused = false;
            await DisplayWholeReply();
            return;
        }

        //[Command("mp")]
        //public async Task PrintMessages()
        //{
        //    foreach(var x in messages)
        //    {
        //        await ReplyAsync(x);
        //    }

        //}
    }
}