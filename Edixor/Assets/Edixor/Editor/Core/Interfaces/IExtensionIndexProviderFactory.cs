public interface IExtensionIndexProviderFactory
{
    IGenericIndexProvider<T> Create<T>(string indexUrl) where T : class;
}
