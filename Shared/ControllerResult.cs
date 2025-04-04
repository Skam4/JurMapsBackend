public class ControllerResult<T>
{
    public bool Success { get; private set; }
    public int StatusCode { get; private set; }
    public string Message { get; private set; }
    public T? Data { get; }

    private ControllerResult(bool success, int statusCode, string message, T? data = default)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message;
        Data = data;
    }

    public static ControllerResult<T> Ok(T data, string message = "Operacja zakończona sukcesem")
            => new ControllerResult<T>(true, 200, message, data);

    public static ControllerResult<T> NotFound(string message)
        => new ControllerResult<T>(false, 404, message);

    public static ControllerResult<T> BadRequest(string message)
        => new ControllerResult<T>(false, 400, message);
    public static ControllerResult<T> ServerError(string message) 
        => new ControllerResult<T>(false, 500, message);
}
