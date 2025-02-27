namespace Assets.Scripts.Framework.Utilities
{
    public class OperationResult
    {
        public bool Success { get; }
        public string Code { get; }
        public string Message { get; }

        /// <summary>
        /// Creates a new OperationResult with the provided success status, code, and message.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public OperationResult(bool success, string code, string message)
        {
            Success = success;
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Creates a successful OperationResult with the provided code and message.
        /// </summary>
        /// <param name="successCode"></param>
        /// <param name="successMessage"></param>
        public static OperationResult SuccessResult(string successCode, string successMessage)
            => new(true, successCode, successMessage);

        /// <summary>
        /// Creates a failed OperationResult with the provided code and message.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        public static OperationResult FailureResult(string errorCode, string errorMessage)
            => new(false, errorCode, errorMessage);
    }
}