using System.Threading.Tasks;
public interface IExtensionInstaller
{
    Task InstallAsync(ExtensionEntry entry);
    Task UninstallAsync(ExtensionEntry ext);
}