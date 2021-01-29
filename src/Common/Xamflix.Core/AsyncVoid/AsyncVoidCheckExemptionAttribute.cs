using System;

namespace Xamflix.Core.AsyncVoid
{
    public class AsyncVoidCheckExemptionAttribute : Attribute
    {
        public string ExemptionDescription { get; }

        public AsyncVoidCheckExemptionAttribute(string exemptionDescription)
        {
            ExemptionDescription = exemptionDescription;
        }
    }
}