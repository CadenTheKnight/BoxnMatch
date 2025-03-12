namespace Assets.Scripts.Framework.Utilities
{
    public enum ResultStatus
    {
        Success,
        Warning,
        Failure
    }

    public class OperationResult
    {
        public string Code { get; }
        public string Message { get; }
        public ResultStatus Status { get; }

        /// <summary>
        /// Creates a new OperationResult with the provided success status, code, and message.
        /// </summary>
        /// <param name="status">The status of the operation result.</param>
        /// <param name="code">The code of the operation result.</param>
        /// <param name="message">The message of the operation result.</param>
        public OperationResult(ResultStatus status, string code, string message)
        {
            Status = status;
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Creates a successful OperationResult with the provided code and message.
        /// </summary>
        /// <param name="successCode">The code of the success result.</param>
        /// <param name="successMessage">The message of the success result.</param>
        public static OperationResult SuccessResult(string successCode, string successMessage)
            => new(ResultStatus.Success, successCode, successMessage);

        /// <summary>
        /// Creates a warning OperationResult with the provided code and message.
        /// </summary>
        /// <param name="warningCode">The code of the warning result.</param>
        /// <param name="warningMessage">The message of the warning result.</param>
        public static OperationResult WarningResult(string warningCode, string warningMessage)
            => new(ResultStatus.Warning, warningCode, warningMessage);

        /// <summary>
        /// Creates a failed OperationResult with the provided code and message.
        /// </summary>
        /// <param name="errorCode">The code of the error result.</param>
        /// <param name="errorMessage">The message of the error result.</param>
        public static OperationResult FailureResult(string errorCode, string errorMessage)
            => new(ResultStatus.Failure, errorCode, errorMessage);
    }
}