namespace Tax.Matters.Client;

public interface IAPIClient
{
    Task<IResponse<T>> CreateAsync<T, TContent>(
        string uri,
        TContent content,
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
        string uri,
        TContent content,
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
