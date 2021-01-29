using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Xamflix.Core.AsyncVoid
{
    public static class AsyncVoidExtensions
    {
        public static IEnumerable<MethodInfo> GetAsyncVoidMethods(this Assembly assembly)
        {
            return assembly.GetLoadableTypes()
                           .SelectMany(type => type.GetMethods(
                                                               BindingFlags.NonPublic
                                                               | BindingFlags.Public
                                                               | BindingFlags.Instance
                                                               | BindingFlags.Static
                                                               | BindingFlags.DeclaredOnly))
                           .Where(method => method.HasAttribute<AsyncStateMachineAttribute>() && method.HasAttribute<AsyncVoidCheckExemptionAttribute>() == false)
                           .Where(method => method.ReturnType == typeof(void));
        }

        public static void AssertNoAsyncVoidMethods(this Assembly assembly)
        {
            List<MethodInfo> messages = assembly
                                        .GetAsyncVoidMethods()
                                        .ToList();
            if(messages.Any())
            {
                throw new AsyncVoidUsageException(messages.First());
            }
        }
        
        private static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if(assembly == null) throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch(ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        private static bool HasAttribute<TAttribute>(this MethodInfo method) where TAttribute : Attribute
        {
            return method.GetCustomAttributes(typeof(TAttribute), false).Any();
        }
    }
}