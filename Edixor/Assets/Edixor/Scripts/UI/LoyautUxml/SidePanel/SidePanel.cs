using System.Collections.Generic;
using System;

public class SidePanel : LayoutLogica
{
    protected override Function[][] functions => new Function[][] 
    {
        new Function[] {LoadFucntion("Restart"), LoadFucntion("Close")},
        new Function[] {LoadFucntion("HotKey"), LoadFucntion("Setting")} 
    };

    protected override Dictionary<string, Function[]> elements => new Dictionary<string, Function[]>
    {
        { "command-function-section", functions[0] },
        { "other-function-section", functions[1] }
    };

}
