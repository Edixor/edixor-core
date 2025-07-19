using System;
using ExTools;

public class Status
{
    public StatusData Data { get; private set; }
    public ExStatus Logic { get; private set; }
    public Status(StatusData data, ExStatus logic)
    {
        if (data == null)
        {
            ExDebug.LogError("FunctionData is null in Function constructor.");
            return;
        }

        Data = data;
        Logic = logic;
    }

    public Status(StatusData data, DIContainer container)
    {
        if (data == null)
        {
            ExDebug.LogError("FunctionData is null in Function constructor.");
            return;
        }

        Data = data;

        if (Logic == null)
        {
            try
            {
                ExStatus logic = new ExStatus(data.ScriptLogic, container);
                Logic = logic.GetLogic();
            }
            catch (Exception e)
            {
                ExDebug.LogError($"Error creating logic for function '{data.Name}': {e.Message}");
            }
        }
    }
}
