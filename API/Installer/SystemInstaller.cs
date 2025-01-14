using API.Installer;

namespace API.InstallerExtension;

public static class SystemInstaller 
{
    public static void Installer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        services.AddSwaggerGen();
    }
}
