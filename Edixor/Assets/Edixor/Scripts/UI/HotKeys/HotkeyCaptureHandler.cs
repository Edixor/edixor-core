using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HotkeyCaptureHandler
{
    private bool isCapturing = false;
    private List<KeyCode> capturedKeys = new List<KeyCode>();
    private Action<List<KeyCode>> onCaptureComplete;

    /// <summary>
    /// Запускает захват новой комбинации клавиш с указанным callback.
    /// </summary>
    public void StartCapture(Action<List<KeyCode>> captureCompleteCallback)
    {
        isCapturing = true;
        capturedKeys.Clear();
        onCaptureComplete = captureCompleteCallback;
        Debug.Log("Начался захват новой комбинации клавиш. Введите сочетание (максимум 3 клавиши) и нажмите Enter для подтверждения.");
    }

    /// <summary>
    /// Обработка событий ввода клавиш. Вызывается из OnGUI.
    /// </summary>
    public void Process(Event e)
    {
        if (!isCapturing)
            return;

        if (e.type == EventType.KeyDown)
        {
            // Если нажата клавиша None, удаляем её (если присутствует) и игнорируем событие
            if (e.keyCode == KeyCode.None)
            {
                if (capturedKeys.Contains(KeyCode.None))
                    capturedKeys.Remove(KeyCode.None);
                e.Use();
                return;
            }

            // Если нажата клавиша Enter, завершаем захват независимо от набранного количества клавиш.
            if (e.keyCode == KeyCode.Return)
            {
                Debug.Log("Захваченная комбинация: " + string.Join(" + ", capturedKeys));
                onCaptureComplete?.Invoke(new List<KeyCode>(capturedKeys));
                FinishCapture();
                e.Use();
                return;
            }

            // Если уже набрано 3 клавиши, завершаем захват (игнорируем дальнейшие клавиши)
            if (capturedKeys.Count >= 3)
            {
                Debug.Log("Набрано 3 клавиши, дополнительные игнорируются. Принимаем комбинацию: " + string.Join(" + ", capturedKeys));
                onCaptureComplete?.Invoke(new List<KeyCode>(capturedKeys));
                FinishCapture();
                e.Use();
                return;
            }

            // Если такая клавиша ещё не была добавлена, добавляем её.
            if (!capturedKeys.Contains(e.keyCode))
            {
                capturedKeys.Add(e.keyCode);
                Debug.Log("Добавлена клавиша: " + e.keyCode);

                // Если после добавления достигли 3 клавиш, завершаем захват автоматически.
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

    private void FinishCapture()
    {
        isCapturing = false;
        capturedKeys.Clear();
    }

    public bool IsCapturing() => isCapturing;

    /// <summary>
    /// Возвращает строковое представление текущей комбинации для отображения в UI.
    /// </summary>
    public string GetCurrentCombinationString() => string.Join(" + ", capturedKeys);
}
