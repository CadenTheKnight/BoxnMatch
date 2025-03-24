using Assets.Scripts.Framework.Types;

namespace Assets.Scripts.Framework.Utilities
{
    /// <summary>
    /// Represents the result of an operation, including the code, message, and status.
    /// </summary>
    public class OperationResult
    {
        public string Code { get; }
        public string Message { get; }
        public ResultStatus Status { get; }

        /// <summary>
        /// Creates a new OperationResult with the provided success status, code, and message.
        /// </summary>
        public OperationResult(ResultStatus status, string code, string message)
        {
            Status = status;
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Creates a successful OperationResult with the provided code and message.
        /// </summary>
        public static OperationResult SuccessResult(string successCode, string successMessage)
            => new(ResultStatus.Success, successCode, successMessage);

        /// <summary>
        /// Creates a warning OperationResult with the provided code and message.
        /// </summary>
        public static OperationResult WarningResult(string warningCode, string warningMessage)
            => new(ResultStatus.Warning, warningCode, warningMessage);

        /// <summary>
        /// Creates a error OperationResult with the provided code and message.
        /// </summary>
        public static OperationResult ErrorResult(string errorCode, string errorMessage)
            => new(ResultStatus.Error, errorCode, errorMessage);
    }
}