using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HotkeyCaptureHandler : IHotkeyCaptureHandler
{
    private bool isCapturing = false;
    private List<KeyCode> capturedKeys = new List<KeyCode>();
    private Action<List<KeyCode>> onCaptureComplete;

    // Реализация свойства для обновления UI
    public Action<List<KeyCode>> OnCaptureChanged { get; set; }

    public void StartCapture(Action<List<KeyCode>> captureCompleteCallback)
    {
        isCapturing = true;
        capturedKeys.Clear();
        onCaptureComplete = captureCompleteCallback;
        // При старте обновляем UI (даже если комбинация пустая)
        OnCaptureChanged?.Invoke(new List<KeyCode>(capturedKeys));
    }

    public void Process(Event e)
    {
        if (!isCapturing)
            return;

        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.None)
            {
                if (capturedKeys.Contains(KeyCode.None))
                    capturedKeys.Remove(KeyCode.None);
                e.Use();
                return;
            }

            if (e.keyCode == KeyCode.Return)
            {
                onCaptureComplete?.Invoke(new List<KeyCode>(capturedKeys));
                FinishCapture();
                e.Use();
                return;
            }

            if (capturedKeys.Count >= 3)
            {
                onCaptureComplete?.Invoke(new List<KeyCode>(capturedKeys));
                FinishCapture();
                e.Use();
                return;
            }

            if (!capturedKeys.Contains(e.keyCode))
            {
                capturedKeys.Add(e.keyCode);
                // Обновляем UI после каждого нового нажатия
                OnCaptureChanged?.Invoke(new List<KeyCode>(capturedKeys));

                if (capturedKeys.Count == 3)
                {
                    Debug.Log("Достигнуто 3 клавиши, принимаем комбинацию: " + string.Join(" + ", capturedKeys));
                    onCaptureComplete?.Invoke(new List<KeyCode>(capturedKeys));
                    FinishCapture();
                    e.Use();
                    return;
                }
            }
            e.Use();
        }
    }

    public void FinishCapture()
    {
        isCapturing = false;
        capturedKeys.Clear();
    }

    public bool IsCapturing() => isCapturing;

    public string GetCurrentCombinationString() => string.Join(" + ", capturedKeys);
}
