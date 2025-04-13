using System.Collections.Generic;
using System;

public class SidePanel : LayoutLogic
{
    protected override Function[][] functions => new Function[][] 
    {
        new Function[] {LoadFunction("Restart"), LoadFunction("Close")},
        new Function[] {LoadFunction("HotKey"), LoadFunction("Setting")} 
    };

    protected override Dictionary<string, Function[]> elements => new Dictionary<string, Function[]>
    {
        { "command-function-section", functions[0] },
        { "other-function-section", functions[1] }
    };

}
