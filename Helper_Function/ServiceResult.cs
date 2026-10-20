namespace University_Academic.Helper_Function;

public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string SuccessMessage { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;

    public static ServiceResult Success(string SuccMessage = "") 
    {
        return new ServiceResult
        {
            IsSuccess = true,
            SuccessMessage = SuccMessage

        };
        
        }
    public static ServiceResult Failure(string message) => new ServiceResult { IsSuccess = false, ErrorMessage = message };
}
public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }
    public static ServiceResult<T> Success(T data) => new ServiceResult<T> { IsSuccess = true, Data = data };
    public static new ServiceResult<T> Failure(string message) => new ServiceResult<T> { IsSuccess = false, ErrorMessage = message };
}
