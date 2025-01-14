namespace API.InstallerExtension;

public static class InstallerExtension
{
    public static void InstallerServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
    {
        // var installer = typeof(Startup).Assembly.ExportedTypes.Where(x => typeof(IInstaller). IsAssignableFrom(x) && !x.IsInterface
        // && !x.IsAbstract). Select (Activator.CreateInstance). Cast<IInstaller>().ToList();
        // installer.ForEach(installer => installer.InstallServices(services, configuration));
    }
}
