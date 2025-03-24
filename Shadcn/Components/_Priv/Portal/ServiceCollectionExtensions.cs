using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shadcn.Components.Priv.Portal;

namespace shadcn.Components.Priv.Portal {

    public static class ServiceCollectionExtensions {
        public static void AddPortals(this IServiceCollection services) => services.TryAddScoped<PortalRegistration>();
    }
}
