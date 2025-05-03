using System;
public interface IFactoryFunction
{
    void CreateExample(FunctionExampleData data, string key);
    void CreateFromAssets(string path, string key = null, Action action = null);
}