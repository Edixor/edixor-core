using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Function
{
    public FunctionData Data { get; private set; }
    public FunctionLogic Logic { get; private set; }

    public Function(FunctionData data, FunctionLogic logic)
    {
        Data = data;
        Logic = logic;
    }
}
