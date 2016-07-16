using System.Collections.Generic;
using System.Linq;

namespace SmartParkAndroid.Core
{
    public class SmartJsonResult
    {
        public bool IsValid => ValidationErrors == null || !ValidationErrors.Any();
        public IEnumerable<string> SuccessNotifications { get; set; }
        public IEnumerable<string> ValidationErrors { get; set; }
    }


    public class SmartJsonResult<T> : SmartJsonResult
    {
        public T Result { get; set; }
    }


    public class SmartJsonResult<T, T2> : SmartJsonResult<T>
    {
        public T2 SecondResult { get; set; }
    }
}