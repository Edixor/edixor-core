using System.Threading.Tasks;
public interface IExtensionInstaller
{
    Task InstallAsync(IndexEntry entry);
    Task UninstallAsync(IndexEntry ext);
}