namespace API.Installer;

public interface IInstaller
{
    void Installer(IServiceCollection services , IConfiguration configuration);
}
