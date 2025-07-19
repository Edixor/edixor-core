using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using ExTools.Style;

public class ExElementStyle<TState> : ISerializationCallbackReceiver
    where TState : struct, IExStyleState
{
    [SerializeField] List<ExStateEntry<TState>> states = new List<ExStateEntry<TState>>();

    void ISerializationCallbackReceiver.OnBeforeSerialize() { }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        EnsureNormal();
    }

    void EnsureNormal()
    {
        if (states == null)
            states = new List<ExStateEntry<TState>>();
        if (!states.Any(e => e.StateName == StyleState.Normal))
            states.Insert(0, new ExStateEntry<TState>(StyleState.Normal));
    }

    public void ApplyTo(VisualElement element, StyleState state = StyleState.Normal)
    {
        var entry = GetEntry(state);
        entry.State.ApplyTo(element);
    }

    public void ApplyWithStates(VisualElement element)
    {
        ApplyTo(element, StyleState.Normal);
        element.RegisterCallback<MouseEnterEvent>(_ => ApplyTo(element, StyleState.Hover));
        element.RegisterCallback<MouseLeaveEvent>(_ => ApplyTo(element, StyleState.Normal));
        element.RegisterCallback<MouseDownEvent>(_ => ApplyTo(element, StyleState.Active), TrickleDown.TrickleDown);
        element.RegisterCallback<MouseUpEvent>(evt =>
        {
            var next = element.worldBound.Contains(evt.mousePosition) ? StyleState.Hover : StyleState.Normal;
            ApplyTo(element, next);
        }, TrickleDown.TrickleDown);
    }

    ExStateEntry<TState> GetEntry(StyleState state)
    {
        EnsureNormal();
        var found = states.FirstOrDefault(e => e.StateName == state);
        if (found != null)
            return found;
        var normal = states.First(e => e.StateName == StyleState.Normal);
        return normal;
    }
}

[Serializable]
public class ExStateEntry<TState>
    where TState : struct, IExStyleState
{
    public StyleState StateName;
    public TState State;

    public ExStateEntry(StyleState stateName)
    {
        StateName = stateName;
        State = default;
    }
}
