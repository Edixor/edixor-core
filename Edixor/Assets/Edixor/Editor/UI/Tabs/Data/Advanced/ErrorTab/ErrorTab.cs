using UnityEngine.UIElements;
using UnityEngine;
using System;

using ExTools;

public class ErrorTab : EdixorTab
{
    private Label errorMessageLabel;
    private Button copyButton;

    private string errorText;
    private Exception error;

    public ErrorTab(Exception ex)
    {
        error = ex;
        tabName = "ErrorTab: " + ex.GetType().Name;
    }

    private void Start()
    {
        errorText = error.ToString();
        errorMessageLabel = root.Q<Label>("error-message");
        copyButton = root.Q<Button>("copy-button");
        errorMessageLabel.text = errorText;
        copyButton.clicked += () => GUIUtility.systemCopyBuffer = errorText;
    }
}
