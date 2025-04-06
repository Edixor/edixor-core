using System;
using System.Collections.Generic;

public interface IDataRegistryService<TData> : IService
{
    List<TData> GetAllItems();
    TData GetItem(Func<TData, bool> predicate);
}
