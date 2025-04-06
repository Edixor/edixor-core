using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
public interface IRegister
{
    void Init<TData>() where TData : ScriptableObject;
    Type RegisterType(string className);
    List<Type> GetRegisteredTypes();
}