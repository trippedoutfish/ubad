using Discord;
using Discord.Webhook;
using System;
using System.Threading.Tasks;

namespace ubad.Services
{
    public class WebHookExampleService
    {
        private readonly string webHookId = Environment.GetEnvironmentVariable("webHookId");
        private readonly string webHookToken = Environment.GetEnvironmentVariable("webHookToken");
        public async Task PostWebhook(string Title, string Description)
        {
            using var client = new DiscordWebhookClient($"https://discordapp.com/api/webhooks/{webHookId}/{webHookToken}");
            var embed = new EmbedBuilder();

            embed.AddField(Title,
            Description)
            .WithColor(Color.Gold)
            .WithTitle(Title)
            .WithCurrentTimestamp()
            .Build();

            // Webhooks are able to send multiple embeds per message
            // As such, your embeds must be passed as a collection. 
            await client.SendMessageAsync(embeds: new[] { embed.Build() });
        }
    }
}