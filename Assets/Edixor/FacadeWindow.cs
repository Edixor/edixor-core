/*using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Commands;
using System;

public abstract class FacadeWindow : EditorWindow
{
    public event Action OnStyleChanged;

    protected StyleMenu menuStyleFacade;
    protected StyleWindow windowStyleFacade;
    protected CustomMenu menu;

    protected List<IStyle> styles = new List<IStyle>();
    protected List<Command> commands = new List<Command>();
    protected Stack<Command> removedCommands = new Stack<Command>();

    protected void InitializeStyle(List<IStyle> styles)
    {
        if (styles.Count > 0)
        {
            this.styles = styles;
            IComponents component = styles[0].GetStyleComponent("window");

            if (component is StyleWindow styleWindow)
            {
                windowStyleFacade = styleWindow;
                Debug.Log("Window style initialized");
                windowStyleFacade.ComponentInitialization(true);
            }
            else
            {
                Debug.LogError("The retrieved component is not of type 'StyleWindow'.");
            }
        }
        else
        {
            Debug.LogWarning("Styles list is empty.");
        }
    }

    public StyleWindow GetStyleComponent(string a)
    {
        if (a == "window")
        {
            return windowStyleFacade;
        }
        else
        {
            return null;
        }
    }

    protected void ButtonRollbackCommands()
    {
        if (windowStyleFacade != null)
        {
            var labelStyle = windowStyleFacade.GetElement("LabelDeGu").ElementStyle("label");
            var buttonStyle = windowStyleFacade.GetElement("ButtonDeGu").ElementStyle("button");

            if (labelStyle != null)
            {
                if (GUILayout.Button("Rollback", buttonStyle, GUILayout.Width(75)))
                {
                    RemoveLastTask();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Element or style not found.");
            }
        }
        else
        {
            EditorGUILayout.LabelField("Window is not initialized.");
        }
    }

    protected void ButtonReturnCommands()
    {
        var buttonElement = windowStyleFacade?.GetElement("ButtonDeGu");
        var buttonStyle = buttonElement?.ElementStyle("button");

        if (GUILayout.Button("Return", buttonStyle, GUILayout.Width(75)))
        {
            ResumeTask();
        }
    }

    protected void WindowCheckingTasks()
    {
        var buttonElement = windowStyleFacade?.GetElement("ButtonDeGu");
        var buttonStyle = buttonElement?.ElementStyle("button");

        if (GUILayout.Button("[]", buttonStyle, GUILayout.Width(25)))
        {
            // Implement CheckingTasks() if needed
        }
    }

    protected void SwitchStyle()
    {
        var buttonElement = windowStyleFacade?.GetElement("ButtonDeGu");
        var buttonStyle = buttonElement?.ElementStyle("button");

        if (GUILayout.Button("!S", buttonStyle, GUILayout.Width(25)))
        {
            ChangeOfStyle();
        }
    }

    protected void SwitchKeyCode()
    {
        var buttonElement = windowStyleFacade?.GetElement("ButtonDeGu");
        var buttonStyle = buttonElement?.ElementStyle("button");

        if (GUILayout.Button("Kc", buttonStyle, GUILayout.Width(25)))
        {
            //ChangeOfStyle();
        }
    }

    protected abstract void DrawCommandButtons();

    protected void AddCommand(Command command)
    {
        removedCommands.Clear();
        command.Tasks();
        commands.Add(command);
        command.CallAction();
    }

    protected void RemoveLastTask()
    {
        if (commands.Count > 0)
        {
            var lastTask = commands[^1];
            lastTask.JobRollback();
            lastTask.RollbackAction();
            removedCommands.Push(lastTask);
            commands.RemoveAt(commands.Count - 1);
        }
    }

    protected void ResumeTask()
    {
        if (removedCommands.Count > 0)
        {
            var taskToResume = removedCommands.Pop();
            taskToResume.Tasks();
            commands.Add(taskToResume);
            taskToResume.CallAction();
        }
    }

    protected void ChangeOfStyle()
    {
        if (menu == null)
        {
            menu = ScriptableObject.CreateInstance<CustomMenu>();
            menu.Initialize(menuStyleFacade);
        }

        for (int i = 0; i < styles.Count; i++)
        {
            int capturedIndex = i;
            string styleName = styles[capturedIndex].styleName;

            bool isCurrentStyle = styles[capturedIndex].GetStyleComponent("window") == windowStyleFacade;
            menu.AddItem(new CMItemAction(isCurrentStyle ? "main: " + styleName : styleName, !isCurrentStyle, () => ApplyStyle(capturedIndex), "ASD2"));
        }

        Vector2 position = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        menu.ShowMenu(position);
    }

    private void ApplyStyle(int index)
    {
        windowStyleFacade = (StyleWindow)styles[0].GetStyleComponent("window");

        //FacadeDataPlagin.SchangeStyletStyles(index);
        Repaint();

        // Trigger the style change event
        OnStyleChanged?.Invoke();
    }

    // Обработка сочетаний клавиш
    protected void HandleHotkeys()
    {
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.control)
        {
            if (e.keyCode == KeyCode.W && e.keyCode == KeyCode.R)
            {
                Close();
            }
            else if (e.keyCode == KeyCode.W && e.keyCode == KeyCode.S)
            {
                ChangeOfStyle();
            }
            else if (e.keyCode == KeyCode.W && e.keyCode == KeyCode.Z)
            {
                RemoveLastTask();
            }
            else if (e.keyCode == KeyCode.W && e.keyCode == KeyCode.X)
            {
                ResumeTask();
            }
            else if (e.keyCode == KeyCode.W && e.keyCode == KeyCode.F)
            {
                // Открытие на полный экран
                SetFullscreen(true);
            }
            else if (e.keyCode == KeyCode.W && e.keyCode == KeyCode.V)
            {
                // Закрытие полноэкранного режима
                SetFullscreen(false);
            }
        }
    }

    private void SetFullscreen(bool fullscreen)
    {
        if (fullscreen)
        {
            maxSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            minSize = maxSize;
        }
        else
        {
            // Возвращаем стандартные размеры окна
            minSize = new Vector2(400, 300);
            maxSize = new Vector2(800, 600);
        }
    }
}*/
