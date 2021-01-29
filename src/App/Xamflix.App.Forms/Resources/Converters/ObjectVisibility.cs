using System.Collections;
using Xamflix.Core.Extensions;

namespace Xamflix.App.Forms.Resources.Converters
{
    public static class ObjectVisibility
    {
        public static bool IsObjectVisible(object value)
        {
            if(value is bool b)
            {
                return b;
            }

            var isVisible = true;
            switch(value)
            {
                case null:
                    isVisible = false;
                    break;
                case IEnumerable enumerable:
                    isVisible = enumerable.Count() != 0;
                    break;
                default:
                    if(string.IsNullOrEmpty(value.ToString()))
                    {
                        isVisible = false;
                    }

                    break;
            }

            return isVisible;
        }
    }
}