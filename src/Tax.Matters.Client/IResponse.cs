using System.Net;
using System.Net.Http.Headers;

namespace Tax.Matters.Client;

/// <summary>
/// Wrapper for the client responses
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IResponse<T>
{
    T? Content { get; }
    string? Error { get; }
    bool IsError { get; }
    HttpStatusCode HttpStatusCode { get; }
    string? HttpReasonPhrase { get; }
    string? Raw { get; }
    Exception? Exception { get; }
    ResponseError ResponseError { get; }

    /// <summary>
    /// To relay response headers
    /// 
    /// </summary>
    HttpResponseHeaders? Headers { get; }
}
