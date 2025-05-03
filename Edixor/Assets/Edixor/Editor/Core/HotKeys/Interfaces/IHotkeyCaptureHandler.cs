using System;
using System.Collections.Generic;
using UnityEngine;

public interface IHotkeyCaptureHandler
{
    
    void StartCapture(Action<List<KeyCode>> captureCompleteCallback);
    void Process(Event e);
    void FinishCapture();
    bool IsCapturing();
    string GetCurrentCombinationString();
    Action<List<KeyCode>> OnCaptureChanged { get; set; }
}