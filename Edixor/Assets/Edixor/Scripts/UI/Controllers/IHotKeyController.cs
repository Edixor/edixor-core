using System.Collections.Generic;
using UnityEngine;

public interface IHotKeyController
{
    void InitHotKeys();

    bool IsHotKeyEnabled();

    void OnKeys();

    void AddKey(KeyAction key);

    List<KeyAction> GetKeys();
}
