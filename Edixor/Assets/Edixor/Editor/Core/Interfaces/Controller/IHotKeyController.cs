using System;
public interface IHotKeyController
{
    void InitHotKeys();
    void ResetConfiguration();
    bool IsHotKeyEnabled();
    void OnKeys();
    event Action<string> OnHotKeyExecuted;
}