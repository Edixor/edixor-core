using System;

using ExTools.Edixor.Interface;
using ExTools;

namespace ExTools.HotKeys
{
    public class Close : ExHotKey
    {
        private IClosable _edixor;
        public override void Activate()
        {
            if (_edixor != null)
            {
                _edixor.CloseEdixor();
            }
            else
            {
                _edixor = container.ResolveNamed<IClosable>(container.Resolve<ServiceNameResolver>().Edixor_Close);
                
                _edixor.CloseEdixor();
            }
        }
    }
    
}
