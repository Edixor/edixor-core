using System;

using ExTools.Edixor.Interface;
using ExTools;

namespace ExTools.Functions
{
    public class Minimization : ExFunction
    {
        private IMinimizable _edixor;
        public override void Activate()
        {
            if (_edixor != null)
            {
                _edixor.MinimizeWindow();
            }
            else
            {
                _edixor = container.ResolveNamed<IMinimizable>(container.Resolve<ServiceNameResolver>().Edixor_Mini);
                
                _edixor.MinimizeWindow();
            }
        }
    }
}
