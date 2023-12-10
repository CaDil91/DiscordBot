using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.Rest;

namespace SteamAppsDiscordBot.Adapters;

/// <summary>
/// InteractionServiceAdapter is a class that acts as an adapter (wrapper) for the InteractionService class.
/// This class implements the IInteractionServiceAdapter interface.
/// Each method in this class corresponds directly to a method in InteractionService, and just forwards the call.
/// The purpose of this class is to allow InteractionService, which doesn't have a parameterless constructor, to be used indirectly through an interface, which can be mocked during testing.
/// </summary>
public class InteractionServiceAdapter : IInteractionServiceAdapter
{
    private readonly InteractionService _interactionService;

    public InteractionServiceAdapter(InteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    public Task AddModulesAsync(Assembly assembly, IServiceProvider serviceProvider)
    {
        return _interactionService.AddModulesAsync(assembly, serviceProvider);
    }

    public Task<IResult> ExecuteCommandAsync(IInteractionContext socketInteractionContext, IServiceProvider serviceProvider)
    {
        return _interactionService.ExecuteCommandAsync(socketInteractionContext, serviceProvider);
    }
    
    public Task<IReadOnlyCollection<RestGlobalCommand>> RegisterCommandsGloballyAsync(bool deleteMissing = true)
    {
        return _interactionService.RegisterCommandsGloballyAsync(deleteMissing);
    }
}