using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.Rest;

namespace SteamAppsDiscordBot;

public interface IInteractionServiceAdapter
{
    Task AddModulesAsync(Assembly assembly, IServiceProvider serviceProvider);
    Task<IResult> ExecuteCommandAsync(IInteractionContext socketInteractionContext, IServiceProvider serviceProvider);
    public Task<IReadOnlyCollection<RestGlobalCommand>> RegisterCommandsGloballyAsync(bool deleteMissing = true);
}