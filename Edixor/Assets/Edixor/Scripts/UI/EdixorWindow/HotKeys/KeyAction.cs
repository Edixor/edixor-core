using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class KeyAction
{
    // Поле с именем должно быть первым и сериализуемым
    [Header("Basic Information")]
    [SerializeField]
    private string _actionName = "Default Action Name";

    // Свойство для имени – теперь виртуальное, чтобы можно было менять значение через инспектор
    public virtual string Name
    {
        get { return _actionName; }
        set { _actionName = value; }
    }

    // Далее идут другие настройки
    [Header("Setting Options")]
    [SerializeField]
    private bool _enable = true;
    public bool enable
    {
        get => _enable;
        set => _enable = value;
    }

    // Абстрактное свойство для комбинации клавиш
    public abstract List<KeyCode> Combination { get; }

    [NonSerialized]
    protected EdixorWindow window;

    public void SetWindow(EdixorWindow window)
    {
        this.window = window;
    }

    public virtual void Execute()
    {
        if (window == null)
        {
            Debug.LogError("Window is null in action: " + Name);
            return;
        }
        Action?.Invoke();
    }

    // Делегат, вызываемый при срабатывании действия
    public virtual Action Action { get; }

    // Обязательный конструктор для сериализации
    protected KeyAction() { }

    // Конструктор с передачей окна
    protected KeyAction(EdixorWindow window)
    {
        this.window = window;
    }
}
