using System;

namespace Xamflix.MediaProcessor.GenerateData
{
    public class GenerateDataResult
    {
        public GenerateDataResult(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
        
        public GenerateDataResult(string failureMessage)
        {
            FailureMessage = failureMessage;
        }
        
        public GenerateDataResult(string failureMessage, Exception exception)
        {
            FailureMessage = failureMessage;
            Exception = exception;
        }

        public bool IsSuccessful { get; }
        public string? FailureMessage { get; }
        public Exception? Exception { get; }
    }
}