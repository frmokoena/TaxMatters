using MediatR;
using Microsoft.Extensions.Options;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.PostalCodes.Queries;

namespace Tax.Matters.Web.Core.Modules.PostalCodes.Handlers;

public class GetPostalCodeQueryHandler(IAPIClient client, IOptions<ClientOptions> optionsAccessor) : IRequestHandler<GetPostalCodeQuery, IResponse<PostalCode>>
{
    private readonly IAPIClient _client = client;
    private readonly ClientOptions _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task<IResponse<PostalCode>> Handle(
        GetPostalCodeQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            throw new InvalidOperationException("request id can not be empty");
        }

        var result = await _client.GetAsync<PostalCode>(
            "services/postalcodes/" + request.Id,
            _options.API,
            _options.Name,
            _options.Key,
            cancellationToken);

        return result;
    }
}