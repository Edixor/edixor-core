using System;

using ExTools.Edixor.Interface;
using ExTools.Settings;
using ExTools;

public class Restart : ExHotKey
{
   
    private IRestartable _edixor;

    public override void Activate()
    {
        if (_edixor != null)
        {
            _edixor.RestartWindow();
        }
        else
        {
            _edixor = container.ResolveNamed<IRestartable>(container.Resolve<ServiceNameResolver>().Edixor_Restart);
            
            _edixor.RestartWindow();
        }
    }
}
