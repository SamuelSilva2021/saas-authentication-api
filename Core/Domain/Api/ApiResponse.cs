namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// Resposta padrão da API
/// </summary>
/// <typeparam name="T">Tipo dos dados retornados</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Se a operação foi bem-sucedida
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Dados retornados
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Lista de erros (se houver)
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Timestamp da resposta
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Cria uma resposta de sucesso
    /// </summary>
    public static ApiResponse<T> SuccessResult(T data, string message = "Operação realizada com sucesso")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Cria uma resposta de erro
    /// </summary>
    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}