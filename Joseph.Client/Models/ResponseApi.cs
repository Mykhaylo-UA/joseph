namespace Joseph.Client.Models;

public class ResponseApi<T>
{
    public T Entity { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}