
using Authenticator.API.Infrastructure.CrossCutting.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// Resposta padrão da API
/// </summary>
/// <typeparam name="T">Tipo dos dados retornados</typeparam>
[ExcludeFromCodeCoverage]
public class ResponseDTO<T> : IResponseDTO
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    [JsonPropertyName("succeeded")]
    public bool Succeeded { get; set; }

    /// <summary>
    /// Resultado em caso de sucesso (pode ser qualquer objeto)
    /// </summary>
    [JsonPropertyName("successResult")]
    public object? SuccessResult { get; set; }

    /// <summary>
    /// Código de status HTTP da resposta
    /// </summary>
    [JsonIgnore]
    public int Code { get; set; }

    /// <summary>
    /// Lista de erros ocorridos durante a operação
    /// </summary>
    [JsonPropertyName("errors")]
    public IList<ErrorDTO> Errors { get; set; } = new List<ErrorDTO>();

    /// <summary>
    /// Cabeçalhos adicionais da resposta
    /// </summary>
    [JsonPropertyName("headers")]
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Dados retornados na resposta
    /// </summary>
    private dynamic? _data;

    /// <summary>
    /// Dados retornados na resposta
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { 
        get => _data; 
        set => _data = value!; 
    }

    /// <summary>
    /// Dados retornados na resposta
    /// </summary>
    /// <returns></returns>
    public dynamic? GetData() => _data;

    /// <summary>
    /// URL da requisição original
    /// </summary>
    [JsonPropertyName("requestUrl")]
    public string? RequestUrl { get; set; }

    /// <summary>
    /// Corpo da requisição original
    /// </summary>
    [JsonPropertyName("requestBody")]
    public string? RequestBody { get; set; }

    /// <summary>
    /// Corpo bruto da requisição original (antes de qualquer processamento)
    /// </summary>
    [JsonPropertyName("rawRequestBody")]
    public string? RawRequestBody { get; set; }

    [JsonIgnore]
    public string? DataAsJson => ((T?)GetData()).ToJson();

    public ExceptionDTO? Exception { get; set; }

}