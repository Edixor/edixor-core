using ExTools;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
public class StyleLogicEdixor : StyleLogic
{
    protected EdixorParameters _edixParameters;
    public StyleLogicEdixor(VisualElement root = null, EdixorParameters parameter = null) : base(root, parameter)
    {
        _edixParameters = parameter;
    }
    public void FunctionStyling(Button[] buttonArray)
    {
        if (buttonArray == null || _edixParameters == null)
        {
            ExDebug.LogError("List or parameters are null.");
            return;
        }

        foreach (var button in buttonArray)
        {
            _edixParameters.FunctionStyle.ApplyWithStates(button);
        }
    }
}
