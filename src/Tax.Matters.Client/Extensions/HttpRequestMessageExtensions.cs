using System.Net.Http.Headers;

namespace Tax.Matters.Client.Extensions;

public static class HttpRequestMessageExtensions
{
    public static void AddBearerAuthorization(this HttpRequestMessage httpRequestMessage, string token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public static void AddBasicAuthorization(
        this HttpRequestMessage httpRequestMessage,
        string username,
        string secret)
    {
        if (!(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(secret)))
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes(
                            $"{username}:{secret}")));
        }
    }
}
