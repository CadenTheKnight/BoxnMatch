using System;
using Assets.Scripts.Framework.Types;

namespace Assets.Scripts.Framework.Utilities
{
    /// <summary>
    /// Represents the result of an operation, including the status, code and message.
    /// </summary>
    public class OperationResult
    {
        public ResultStatus Status { get; }
        public string Code { get; }
        public string Message { get; }
        public object Data { get; }
        public Action Retry { get; }

        /// <summary>
        /// Creates a new OperationResult with the provided success status, code, and message.
        /// </summary>
        public OperationResult(ResultStatus status, string code, string message, object data = null, Action retry = null)
        {
            Status = status;
            Code = code;
            Message = message;
            Data = data;
            Retry = retry;
        }

        /// <summary>
        /// Creates a successful OperationResult with the provided code, data, and message.
        /// </summary>
        public static OperationResult SuccessResult(string code, string message, object data = null) => new(ResultStatus.Success, code, message, data);

        /// <summary>
        /// Creates a warning OperationResult with the provided code, message, and data.
        /// </summary>
        public static OperationResult WarningResult(string code, string message, object data = null) => new(ResultStatus.Warning, code, message, data);
        /// <summary>
        /// Creates an error OperationResult with the provided code, message, data, and retry action.
        /// </summary>
        public static OperationResult ErrorResult(string code, string message, object data = null, Action retry = null) => new(ResultStatus.Error, code, message, data, retry);
    }
}