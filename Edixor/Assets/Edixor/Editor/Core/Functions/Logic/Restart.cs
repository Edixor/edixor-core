using ExTools;

using ExTools.Edixor.Interface;
using System;

namespace ExTools.Functions
{
    public class Restart : ExFunction
    {
        private IRestartable _edixor;

        public override void Activate()
        {
            _edixor = container.ResolveNamed<IRestartable>(container.Resolve<ServiceNameResolver>().Edixor_Restart);
            if (_edixor != null)
            {
                _edixor.RestartWindow();
            }
            else
            {
                ExDebug.LogError("Window is null in Restart action");
            }
        }
    }
}