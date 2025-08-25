namespace AskFm.BLL.Services;

public class ServiceResult<T> 
{
    public bool success { get; set; }
    public List<string>? Errors { get; set; }
    public T? Data { get; set; }

    public static async Task<ServiceResult<T>> Success(T data)
    {
        return new ServiceResult<T>
        {
            success = true,
            Data = data
        };
    }
    public static async Task<ServiceResult<T>> Success()
    {
        return new ServiceResult<T>
        {
            success = true,
        };
    }

    public static async Task<ServiceResult<T>> Failure(List<string> errors)
    {
        return new ServiceResult<T>
        {
            success = false,
            Errors = errors
        };
    }
}