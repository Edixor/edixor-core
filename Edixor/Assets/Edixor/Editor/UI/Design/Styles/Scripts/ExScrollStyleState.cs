using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct ExScrollStyleState
{
    public ExStyleState dragger;
    public ExStyleState tracker;

    public void ApplyTo(VisualElement element)
    {
        foreach (var draggerElement in element.Query<VisualElement>(className: "unity-base-slider__dragger").ToList())
        {
            draggerElement.style.backgroundImage = null;
            draggerElement.style.color = new StyleColor(dragger.textColor);
            draggerElement.style.backgroundColor = new StyleColor(dragger.backgroundColor);
            draggerElement.style.borderTopColor = new StyleColor(dragger.borderTopColor);
            draggerElement.style.borderBottomColor = new StyleColor(dragger.borderBottomColor);
            draggerElement.style.borderLeftColor = new StyleColor(dragger.borderLeftColor);
            draggerElement.style.borderRightColor = new StyleColor(dragger.borderRightColor);
            draggerElement.style.borderTopWidth = new StyleFloat(dragger.borderWidth);
            draggerElement.style.borderBottomWidth = new StyleFloat(dragger.borderWidth);
            draggerElement.style.borderLeftWidth = new StyleFloat(dragger.borderWidth);
            draggerElement.style.borderRightWidth = new StyleFloat(dragger.borderWidth);
            draggerElement.style.borderTopLeftRadius = new StyleLength(dragger.borderRadius);
            draggerElement.style.borderTopRightRadius = new StyleLength(dragger.borderRadius);
            draggerElement.style.borderBottomLeftRadius = new StyleLength(dragger.borderRadius);
            draggerElement.style.borderBottomRightRadius = new StyleLength(dragger.borderRadius);
        }

        foreach (var trackerElement in element.Query<VisualElement>(className: "unity-base-slider__tracker").ToList())
        {
            trackerElement.style.backgroundImage = null;
            trackerElement.style.color = new StyleColor(tracker.textColor);
            trackerElement.style.backgroundColor = new StyleColor(tracker.backgroundColor);
            trackerElement.style.borderTopColor = new StyleColor(tracker.borderTopColor);
            trackerElement.style.borderBottomColor = new StyleColor(tracker.borderBottomColor);
            trackerElement.style.borderLeftColor = new StyleColor(tracker.borderLeftColor);
            trackerElement.style.borderRightColor = new StyleColor(tracker.borderRightColor);
            trackerElement.style.borderTopWidth = new StyleFloat(tracker.borderWidth);
            trackerElement.style.borderBottomWidth = new StyleFloat(tracker.borderWidth);
            trackerElement.style.borderLeftWidth = new StyleFloat(tracker.borderWidth);
            trackerElement.style.borderRightWidth = new StyleFloat(tracker.borderWidth);
            trackerElement.style.borderTopLeftRadius = new StyleLength(tracker.borderRadius);
            trackerElement.style.borderTopRightRadius = new StyleLength(tracker.borderRadius);  
            trackerElement.style.borderBottomLeftRadius = new StyleLength(tracker.borderRadius);
            trackerElement.style.borderBottomRightRadius = new StyleLength(tracker.borderRadius);
        }
    }
}