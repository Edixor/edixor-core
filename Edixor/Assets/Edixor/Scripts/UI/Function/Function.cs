using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Function
{
    public FunctionData Data { get; private set; }
    public FunctionLogica Logic { get; private set; }

    public Function(FunctionData data, FunctionLogica logic)
    {
        Data = data;
        Logic = logic;
    }
}
