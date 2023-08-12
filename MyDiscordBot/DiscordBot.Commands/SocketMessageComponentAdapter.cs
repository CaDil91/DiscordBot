using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.DiscordBot.Core;

namespace DiscordBot.DiscordBot.Commands;

    /// <summary>
    /// Adapter class to adapt the SocketMessageComponent to the IMessageComponentAdapter interface.
    /// </summary>
    public class SocketMessageComponentAdapter
    {
        internal readonly SocketMessageComponent SocketMessageComponent;
        
        IComponentInteractionData Data => SocketMessageComponent.Data;
        public SocketUserMessage Message => SocketMessageComponent.Message;
        public SocketUser User => SocketMessageComponent.User;
        public ISocketMessageChannel Channel => SocketMessageComponent.Channel;
        public ulong? GuildId => SocketMessageComponent.GuildId;

        /// <summary>
        /// Initializes a new instance of the SocketMessageComponentAdapter class.
        /// </summary>
        /// <param name="socketMessageComponent">The SocketMessageComponent instance to be adapted.</param>
        public SocketMessageComponentAdapter(SocketMessageComponent socketMessageComponent)
        {
            SocketMessageComponent = socketMessageComponent;
        }

        public Task RespondAsync(
            string? text = null,
            Embed[]? embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions? allowedMentions = null,
            MessageComponent? components = null,
            Embed? embed = null,
            RequestOptions? options = null)
        {
            // Call the corresponding method of the adapted SocketMessageComponent
            return SocketMessageComponent.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
        }

        /// <inheritdoc />
        public Task<RestFollowupMessage> FollowupAsync(
            string? text = null,
            Embed[]? embeds = null,
            bool isTTS = false,
            bool ephemeral = false,
            AllowedMentions? allowedMentions = null,
            MessageComponent? components = null,
            Embed? embed = null,
            RequestOptions? options = null)
        {
            // Call the corresponding method of the adapted SocketMessageComponent
            return SocketMessageComponent.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
        }

        /// <inheritdoc/>
        public Task DeferAsync(bool ephemeral = false, RequestOptions? options = null)
        {
            // Call the corresponding method of the adapted SocketMessageComponent
            return SocketMessageComponent.DeferAsync(ephemeral, options);
        }

        /// <summary>
        /// Base implementation of the ValidateCommand method always returns true.
        /// </summary>
        /// <returns></returns>
        public bool ValidateCommand() => true;

        public string? GetCommandName() => SocketMessageComponent.Data?.CustomId;
    }