using System;
using SharpMASM;

public class MASMWrapper
{
    private readonly object _wrappedInstance;

    public MASMWrapper(object wrappedInstance)
    {
        _wrappedInstance = wrappedInstance;
    }

    public void ExecuteWithExceptionHandling(Action action)
    {
        try
        {
            action();
        }
        catch (MASMException ex)
        {
            // Log the exception to a custom location
            Console.WriteLine($"Caught MASMException: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            // Optionally rethrow or handle the exception
        }
        catch (Exception ex)
        {
            // Handle other exceptions if needed
            Console.WriteLine($"Caught general exception: {ex.Message}");
        }
    }
}
