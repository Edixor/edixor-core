using System.Collections.Generic;
using System;
public interface IVersions
{
    Dictionary<int, Action> versions { get; }
    int countVersion { get; }
    void InitializeVersionActions();
    void ChangeVersion(int version);
}