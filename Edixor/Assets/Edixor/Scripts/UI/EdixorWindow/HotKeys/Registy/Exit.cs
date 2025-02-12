using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Exit : KeyAction
{
    [SerializeField]
    private List<KeyCode> combination = new List<KeyCode> { KeyCode.LeftShift, KeyCode.E };

    // Используем базовое поле имени, поэтому не нужно вводить дополнительное пол
    public override List<KeyCode> Combination => combination;

    public override Action Action => () =>
    {
        if (window != null)
        {
            window.Close();
        }
        else
        {
            Debug.LogError("Window is null in Exit action");
        }
    };

    // Параметрless конструктор для сериализации
    public Exit() : base()
    {
        Name = "Exit";  // задаём имя по умолчанию
    }

    // Конструктор с передачей окна
    public Exit(EdixorWindow window) : base(window)
    {
        Name = "Exit";
    }
}
