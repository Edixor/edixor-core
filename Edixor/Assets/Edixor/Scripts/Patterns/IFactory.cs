using System;
using System.Collections.Generic;
using UnityEngine;

public interface IFactory
{
    void Init<TData, TLogic>(Func<TData, string> logicPathSelector)
        where TData : ScriptableObject
        where TLogic : class;
    
    void Init<TData, TLogic, TFull>(Func<TData, string> logicPathSelector)
        where TData : ScriptableObject
        where TLogic : class
        where TFull : class;
    
    object Create(ScriptableObject data);
    object CreateLogic(ScriptableObject data);
    List<object> CreateAllFromProject();
}
