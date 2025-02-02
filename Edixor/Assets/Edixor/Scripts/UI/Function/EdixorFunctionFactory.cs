using System;
public class EdixorFunctionFactory : Factory<EdixorFunction, EdixorWindow>
{
    public void ActivateFunction<T>() where T : EdixorFunction
    {
        var function = GetItem<T>();
        function.Activate();
    }

    
}
