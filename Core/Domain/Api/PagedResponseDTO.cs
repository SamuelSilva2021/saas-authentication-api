using System.Text.Json.Serialization;

namespace Authenticator.API.Core.Domain.Api;

/// <summary>
/// DTO padrão para respostas paginadas
/// </summary>
/// <typeparam name="T">Tipo do item</typeparam>
public class PagedResponseDTO<T>
{
    /// <summary>
    /// Itens da página atual
    /// </summary>
    [JsonPropertyName("items")]
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Número da página atual
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    /// Tamanho da página (limite)
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>
    /// Total de itens
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
}