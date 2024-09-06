using Discord;
using Discord.Rest;

namespace Sally.DiscordBot.Extensions
{
    public static class Extensions
    {
        public async static Task<IUserMessage?> GetHeadMessageAsync(this RestThreadChannel post)
        {
            var postMessages = await post.GetMessagesAsync().FlattenAsync();
            var postMessage = postMessages.Last() as IUserMessage;

            return postMessage;
        }
    }
}
