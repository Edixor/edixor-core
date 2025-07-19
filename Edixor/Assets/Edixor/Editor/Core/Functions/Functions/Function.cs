using System;
using ExTools;

public class Function
{
    public FunctionData Data { get; private set; }
    public ExFunction Logic { get; private set; }

    public Function(FunctionData data, ExFunction logic)
    {
        if (data == null)
        {
            ExDebug.LogError("FunctionData is null.");
            return;
        }

        Data = data;
        Logic = logic;
    }

    public Function(FunctionData data, DIContainer container)
    {
        if (data == null)
        {
            ExDebug.LogError("FunctionData is null.");
            return;
        }

        Data = data;

        if (Logic == null)
        {
            try
            {
                ExFunction logic = new ExFunction(data.ScriptLogic, container);
                Logic = logic.GetLogic();
            }
            catch (Exception e)
            {
                ExDebug.LogError($"Failed to create logic for '{data.Name}': {e.Message}");
            }
        }
    }

    public void Execute()
    {
        if (Logic != null)
        {
            Logic.Execute();
        }
        else
        {
            ExDebug.LogError($"ExFunction is null for function: {Data?.Name ?? "null"}");
        }
    }
}
