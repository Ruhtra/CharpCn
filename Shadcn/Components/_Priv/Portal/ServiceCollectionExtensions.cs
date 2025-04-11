using Shadcn.Components.Priv.Portal;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shadcn.Components.Priv.Portal;


public static class ServiceCollectionExtensions {
    public static void AddPortals(this IServiceCollection services) => services.TryAddScoped<PortalRegistration>();
}

