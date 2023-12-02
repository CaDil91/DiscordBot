using System.Reflection;
using Discord;
using Discord.Interactions;
namespace DiscordBot;

public interface IInteractionServiceAdapter
{
    Task AddModulesAsync(Assembly assembly, IServiceProvider serviceProvider);
    Task ExecuteCommandAsync(IInteractionContext socketInteractionContext, IServiceProvider serviceProvider);
}