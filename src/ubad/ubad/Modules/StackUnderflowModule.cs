using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using ubad.JsonModels;
using ubad.Services;

namespace ubad.Modules
{
    public class StackUnderflowModule : ModuleBase<SocketCommandContext>
    {
        private QuestionAndAnswer reply { get; set; }
        public StackUnderflowService StackUnderflowService { get; set; }

        [Command("suqna", RunMode = RunMode.Async)]
        public async Task FtsFtQuery([Remainder] string text)
        {
            reply = await StackUnderflowService.FtsFtQuery(text);
            if (reply.Question != null)
            { 
                await ReplyAsync("Question:");
                await ReplyLargeText(reply.Question);
                if (reply.Answer != null)
                {
                    await ReplyAsync("Answer:");
                    await ReplyLargeText(reply.Answer);
                }
                else
                {
                    await ReplyAsync("No Accepted Answer.");
                }
            }else
            {
                await ReplyAsync("No suitable results.");
            }
        }

        public async Task ReplyLargeText(string text)
        {
            int index = 0;
            while(text.Length - index > 1999)
            {
                await ReplyAsync(text[index..(index+1999)]);
                index += 1999;
            }
            await ReplyAsync(text[index..]);
        }
    }
}
