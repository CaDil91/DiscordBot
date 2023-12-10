using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Adapters;

/// <summary>
/// Adapter for DiscordNet's Discord.WebSocket.DiscordSocketClient.
/// </summary>
public class DiscordSocketClientAdapter
{
    private readonly DiscordSocketClient _clientAdaptee;
    public DiscordSocketRestClient Rest => _clientAdaptee.Rest;
    public IEnumerable<SocketGuild?> Guilds => _clientAdaptee.Guilds;
    public ConnectionState ConnectionState => _clientAdaptee.ConnectionState;
    
    /// <summary>
    ///     Fired when an Interaction is created. This event covers all types of interactions including but not limited to: buttons, select menus, slash commands, autocompletes.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This event is fired when an interaction is created. The event handler must return a
    ///         <see cref="T:System.Threading.Tasks.Task" /> and accept a <see cref="T:Discord.WebSocket.SocketInteraction" /> as its parameter.
    ///     </para>
    ///     <para>
    ///         The interaction created will be passed into the <see cref="T:Discord.WebSocket.SocketInteraction" /> parameter.
    ///     </para>
    /// </remarks>
    public event Func<SocketInteraction, Task>? InteractionCreated
    {
        add => _clientAdaptee.InteractionCreated += value;
        remove => _clientAdaptee.InteractionCreated -= value;
    }
    
    /// <summary>Fired when guild data has finished downloading.</summary>
    /// <remarks>
    ///     It is possible that some guilds might be unsynced if <see cref="P:Discord.WebSocket.DiscordSocketConfig.MaxWaitBetweenGuildAvailablesBeforeReady" />
    ///     was not long enough to receive all GUILD_AVAILABLEs before READY.
    /// </remarks>
    public event Func<Task>? Ready
    {
        add => _clientAdaptee.Ready += value;
        remove => _clientAdaptee.Ready -= value;
    }
    
    public DiscordSocketClientAdapter(DiscordSocketClient clientAdaptee)
    {
        _clientAdaptee = clientAdaptee;
    }

    /// <summary>
    /// Creates a new socket interaction context using our adapted DiscordSocketClient.
    /// </summary>
    /// <param name="interaction">The socket interaction.</param>
    /// <returns>
    /// A new instance of the <see cref="T:Discord.Interactions.SocketInteractionContext" />
    /// class using the provided socket interaction.
    /// </returns>
    public IInteractionContext CreateSocketInteractionContext(SocketInteraction interaction) 
        => new SocketInteractionContext(_clientAdaptee, interaction);

    /// <summary>
    /// Adapted class has no documentation.
    /// </summary>
    /// <param name="tokenType"></param>
    /// <param name="token"></param>
    /// <param name="validateToken"></param>
    public async Task LoginAsync(TokenType tokenType, string? token, bool validateToken = true) => 
        await _clientAdaptee.LoginAsync(tokenType, token, validateToken);

    /// <summary>
    /// Adapted class has no documentation.
    /// </summary>
    public async Task StartAsync() => await _clientAdaptee.StartAsync();
}