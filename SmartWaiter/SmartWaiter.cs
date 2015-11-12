using System;
using System.Diagnostics;

public class SmartWaiter
{
    public static int DefaultTimeout = 60; // by seconds

    public static void TryWaitFor(Func<bool> delegateFunc)
    {
        TryWaitFor(delegateFunc, DefaultTimeout);
    }

    public static void TryWaitFor(Func<bool> delegateFunc, int timeoutSeconds)
    {
        var message = delegateFunc.Method.Name; ;
        TryWaitFor(delegateFunc, message, timeoutSeconds);
    }

    public static void TryWaitFor(Func<bool> delegateFunc, string message)
    {
        TryWaitFor(delegateFunc, message, DefaultTimeout);
    }

    public static void TryWaitFor(Func<bool> delegateFunc, string message, int timeoutSeconds)
    {
        var start = DateTime.Now;
        var exMessage = string.Empty;
        do
        {
            try
            {
                if (delegateFunc())
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                exMessage = ex.Message;
            }
            System.Threading.Thread.Sleep(1 * 1000);
        } while (DateTime.Now - start <= TimeSpan.FromSeconds(timeoutSeconds));

        Debug.WriteLine("Exception occurred when try {0} {1}", message, exMessage);
        throw new TimeoutException(string.Format("Timeout <{0}> seconds to {1}", timeoutSeconds, message));
    }

    public static void WaitFor(Func<bool> delegateFunc, int timeoutSeconds)
    {
        var message = delegateFunc.Method.Name;
        WaitFor(delegateFunc, message, timeoutSeconds);
    }

    public static void WaitFor(Func<bool> delegateFunc)
    {
        var message = delegateFunc.Method.Name;
        WaitFor(delegateFunc, message, DefaultTimeout);
    }

    public static void WaitFor(Func<bool> delegateFunc, string message, int timeoutSeconds)
    {
        var start = DateTime.Now;
        do
        {
            if (delegateFunc())
            {
                return;
            }
            System.Threading.Thread.Sleep(1 * 1000);
        } while (DateTime.Now - start <= TimeSpan.FromSeconds(timeoutSeconds));

        throw new TimeoutException(string.Format("Timeout <{0}> seconds to {1}", timeoutSeconds, message));
    }
}