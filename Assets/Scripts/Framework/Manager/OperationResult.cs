namespace Assets.Scripts.Framework.Manager
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public OperationResult(bool success, int errorCode = 0, string errorMessage = null)
        {
            Success = success;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}