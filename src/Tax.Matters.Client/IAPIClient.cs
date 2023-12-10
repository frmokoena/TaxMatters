namespace Tax.Matters.Client;

/// <summary>
/// Defines the common actions for the client
/// </summary>
public interface IAPIClient
{
    Task<IResponse<T>> CreateAsync<T, TContent>(
        TContent content,
        string uri,
        string? BaseUri = null,
        string? clientName = null,
        string? apiKey = null,            
        CancellationToken cancellationToken = default) where TContent : class;

    Task<IResponse<T>> DeleteAsync<T>(
        string uri,
        string? baseUri = null,
        string? clientName = null,
        string? apiKey = null,
        CancellationToken cancellationToken = default);

    Task<IResponse<T>> EditAsync<T, TContent>(
        TContent content,
        string uri,
        string? baseUri = null,
        string? clientName = null,
        string? apiKey = null,
        CancellationToken cancellationToken = default) where TContent : class;

    Task<IResponse<T>> GetAsync<T>(
        string uri,
        string? baseUri = null,
        string? clientName = null,
        string? apiKey = null, 
        CancellationToken cancellationToken = default);
}
