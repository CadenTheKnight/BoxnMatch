using Assets.Scripts.Framework.Enums;

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
        public string Category { get; }

        /// <summary>
        /// Creates a new OperationResult with the provided success status, code, and message.
        /// </summary>
        public OperationResult(ResultStatus status, string code, string message, string category = null)
        {
            Status = status;
            Code = code;
            Message = message;
            Category = category;
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
        public static OperationResult ErrorResult(string errorCode, string errorMessage, string category = null)
            => new(ResultStatus.Error, errorCode, errorMessage, category);
    }
}