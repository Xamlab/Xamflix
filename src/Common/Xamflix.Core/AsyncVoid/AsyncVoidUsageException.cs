using System;
using System.Reflection;

namespace Xamflix.Core.AsyncVoid
{
    public class AsyncVoidUsageException : Exception
    {
        public AsyncVoidUsageException(MethodInfo method) : base($"'{method.DeclaringType!.Name}.{method.Name}' is an async void method. If that was intended, explicitly specify ${nameof(AsyncVoidCheckExemptionAttribute)} and describe the reason of async void usage.")
        {
        }
    }
}