using System;

namespace Xamflix.MediaProcessor.Models
{
    public class TaskResult<T>
    {
        public string TaskName { get; set; }
        public T Result { get; set; }
        public bool IsSuccessful { get; set; }
        public Exception Error { get; set; }
    }
}