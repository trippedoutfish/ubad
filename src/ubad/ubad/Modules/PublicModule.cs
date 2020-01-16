using Discord;
using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using ubad.Services;

namespace ubad.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public PictureService PictureService { get; set; }
        public XkcdService XkcdService { get; set; }
        public WebHookExampleService WebHookExampleService { get; set; }
        public JokeService JokeService { get; set; }

        //private Random randomGenerator = new Random();

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("cat")]
        public async Task CatAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await PictureService.GetCatPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        [Command("xkcd")]
        public async Task XkcdAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await XkcdService.GetXkcdComicAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "xkcd.png");
        }

        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user ??= Context.User;

            await ReplyAsync(user.ToString());
        }

        [Command("commands")]
        public async Task CommandsAsync()
        {
            string[] commands = { "ping - try me", "cat - picture of a car", "xkcd - todays xkcd", "userinfo <username> -  returns info on user", "echo <someText> - EchoooO", "list <word1> <word2> - type words with spaces between 'em" };

            await ReplyAsync(string.Join('\n', commands));
        }

        //// Ban a user
        //[Command("ban")]
        //[RequireContext(ContextType.Guild)]
        //// make sure the user invoking the command can ban
        //[RequireUserPermission(GuildPermission.BanMembers)]
        //// make sure the bot itself can ban
        //[RequireBotPermission(GuildPermission.BanMembers)]
        //public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null)
        //{
        //    await user.Guild.AddBanAsync(user, reason: reason);
        //    await ReplyAsync("ok!");
        //}

        // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
        [Command("echo")]
        public Task EchoAsync([Remainder] string text)
            // Insert a ZWSP before the text to prevent triggering other bots!
            => ReplyAsync('\u200B' + text);

        // 'params' will parse space-separated elements into a list
        [Command("list")]
        public Task ListAsync(params string[] objects)
            => ReplyAsync($"You listed: {objects.Length} items.");

        //// Setting a custom ErrorMessage property will help clarify the precondition error
        //[Command("guild_only")]
        //[RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        //public Task GuildOnlyCommand()
        //    => ReplyAsync("Nothing to see here!");

        [Command("webhook")]
        public async Task WebHookAsync(string Title = "Default Title", string Description = "Default Description")
        {
            await WebHookExampleService.PostWebhook(Title, Description);
        }

        //[Command("joke")]
        public async Task JokeTaskAsync([Remainder] string text)
        {
            var punchLine = await JokeService.GetJokeAsync(text);
            Console.WriteLine(punchLine);
            punchLine = punchLine.Replace("<br />", "");
            var periodIndex = punchLine.IndexOf('.') > 0 ? punchLine.IndexOf('.') : punchLine.Substring(50).IndexOf(' ');
            var sampleIndex = punchLine.LastIndexOf('=', 150);
            var trimmedJoke = punchLine.Substring(sampleIndex, periodIndex);
            await ReplyAsync(trimmedJoke);
        }
    }
}