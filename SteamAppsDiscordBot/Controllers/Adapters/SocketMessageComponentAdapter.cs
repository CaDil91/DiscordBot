using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordBot.Controllers.Adapters;

    /// <summary>
    /// Adapter class to adapt the SocketMessageComponent to the IMessageComponentAdapter interface.
    /// </summary>
    public class SocketMessageComponentAdapter : ICommandAdapter<IComponentInteractionData>
    {
        private readonly SocketMessageComponent _socketMessageComponent;
        public IComponentInteractionData CommandData => _socketMessageComponent.Data;
        public string CommandName => CommandData.CustomId;

        /// <summary>
        /// Initializes a new instance of the SocketMessageComponentAdapter class.
        /// </summary>
        /// <param name="socketMessageComponent">The SocketMessageComponent instance to be adapted.</param>
        public SocketMessageComponentAdapter(SocketMessageComponent socketMessageComponent)
        {
            _socketMessageComponent = socketMessageComponent;
        }
        
        
        public SocketUserMessage Message => _socketMessageComponent.Message;
        public SocketUser User => _socketMessageComponent.User;
        public ISocketMessageChannel Channel => _socketMessageComponent.Channel;
        public ulong? GuildId => _socketMessageComponent.GuildId;

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
            return _socketMessageComponent.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
        }

        /// <inheritdoc/>
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
            return _socketMessageComponent.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
        }

        /// <inheritdoc/>
        public Task DeferAsync(bool ephemeral = false, RequestOptions? options = null)
        {
            // Call the corresponding method of the adapted SocketMessageComponent
            return _socketMessageComponent.DeferAsync(ephemeral, options);
        }

        /// <summary>
        /// Base implementation of the ValidateCommand method always returns true.
        /// </summary>
        /// <returns></returns>
        public bool ValidateCommand() => true;

        public string? GetCommandName() => _socketMessageComponent.Data?.CustomId;
    }