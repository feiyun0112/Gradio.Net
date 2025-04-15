using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Demo.AspNetCore.Launcher;

internal static class DependencyInjectionExtension
{
    public static IServiceCollection AddLauncher(this IServiceCollection services)
    {
        services.AddGradio();

        var blockBuilders = AssemblyModuleReference.Assembly.GetTypes().Where(type => type is { IsAbstract: false, IsInterface: false }
                                && type.IsAssignableTo(typeof(IGradioBlockBuilder)))
                            .Select(type => ServiceDescriptor.Singleton(typeof(IGradioBlockBuilder), type));
        services.TryAddEnumerable(blockBuilders);
        return services;
    }

    public static WebApplication UseLauncher(this WebApplication app)
    {
        var builders = app.Services.GetRequiredService<IEnumerable<IGradioBlockBuilder>>();
        app.UseGradio(GradioBlockLoader.Load(builders).Result);
        app.UseStaticFiles();
        app.UseRouting();
        return app;
    }
}