using System;

public class GitHubIndexProviderFactory : IExtensionIndexProviderFactory
{
    public IGenericIndexProvider<T> Create<T>(string indexUrl) where T : class
    {
        var provider = new GenericIndexProvider<T>();
        provider.Init(indexUrl);
        return provider;
    }
}
