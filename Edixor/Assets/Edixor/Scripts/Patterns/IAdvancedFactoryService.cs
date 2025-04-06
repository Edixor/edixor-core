using System.Collections.Generic;
using System;
public interface IAdvancedRegistryService<TData, TLogic, TFull> : IService
{
    List<TFull> GetAllItems();
    List<TData> GetAllData();
    List<TLogic> GetAllLogics();
    TFull GetItem(Func<TFull, bool> predicate);
    TData GetData(Func<TData, bool> predicate);
    TLogic GetLogic(Func<TLogic, bool> predicate);
}

