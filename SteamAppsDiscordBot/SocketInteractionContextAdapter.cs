using System.Diagnostics.CodeAnalysis;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot;

/// <summary>
/// Adapter for DiscordNet's Discord.Interactions.SocketInteractionContext.
/// </summary>
[ExcludeFromCodeCoverage]
public class SocketInteractionContextAdapter : ISocketInteractionContextAdapter
{
    private readonly SocketInteractionContext _context;

    public SocketInteractionContextAdapter(DiscordSocketClient client, SocketInteraction interaction)
    {
        _context = new SocketInteractionContext(client, interaction);
    }

    public DiscordSocketClient Client => _context.Client;

    public SocketInteraction Interaction => _context.Interaction;
}