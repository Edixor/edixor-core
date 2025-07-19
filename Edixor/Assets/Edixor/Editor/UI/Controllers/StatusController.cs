using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

using ExTools.Settings;
using ExTools;

public class StatusController : ListControllerBase<Status>, IStatusController
{
    private IUIController _ui;
    private DIContainer _container;

    public StatusController(IUIController ui, DIContainer c)
    {
        _ui = ui;
        _container = c;
    }

    public void Initialize(IUIController ui, DIContainer c = null)
    {
        if (ui != null) _ui = ui;
        if (c != null) _container = c;
    }

    public override void Process()
    {
        var bar = _ui.GetElement("status-section");
        if (bar == null) return;
        bar.Clear();

        var uniqueItems = new List<Status>();
        var duplicates = new List<Status>();
        var seen = new HashSet<Status>();

        foreach (var status in items)
        {
            if (!seen.Add(status))
                duplicates.Add(status);
            else
                uniqueItems.Add(status);
        }

        if (duplicates.Count > 0)
        {
            items.Clear();
            items.AddRange(uniqueItems);
        }

        foreach (var status in items)
        {
            try
            {
                var logic = status.Logic;
                logic.Init(bar);
                logic.InvokeAwake();
                var element = logic.LoadUI();
                bar.Add(element);
                logic.InvokeStart();
                logic.InvokeOnEnable();
            }
            catch (System.Exception ex)
            {
                var errorLabel = new Label("error");
                errorLabel.style.color = Color.red;
                errorLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                errorLabel.RegisterCallback<ClickEvent>(_ => Debug.LogError(ex));
                bar.Add(errorLabel);
            }
        }
    }

    public override void AddItem(Status status)
    {
        if (status == null || string.IsNullOrEmpty(status.Data?.Name))
        {
            ExDebug.LogWarning("Attempt to add invalid Status.");
            return;
        }

        if (items.Any(f => f.Data.Name == status.Data.Name))
        {
            bool coldStart = _container.ResolveNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting)
                .GetCorrectItem()
                .IsColdStart;
            if (!coldStart) ExDebug.LogWarning($"Status '{status.Data.Name}' already added.");
            else ExDebug.Log($"Status '{status.Data.Name}' already added.");
            return;
        }

        base.AddItem(status);
    }

    public void OnGUI()
    {
        foreach (var status in items)
            status.Logic.InvokeUpdate();
    }

    public void ResetConfiguration() => ClearAll();
}
