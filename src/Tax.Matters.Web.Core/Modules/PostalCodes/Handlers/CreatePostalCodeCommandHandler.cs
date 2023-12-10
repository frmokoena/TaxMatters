using MediatR;
using Microsoft.Extensions.Options;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.PostalCodes.Commands;
using Tax.Matters.Web.Core.Modules.PostalCodes.Models;

namespace Tax.Matters.Web.Core.Modules.PostalCodes.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="CreatePostalCodeCommandHandler"/> handler class
/// </summary>
/// <param name="client"></param>
/// <param name="optionsAccessor"></param>
public class CreatePostalCodeCommandHandler(IAPIClient client, IOptions<ClientOptions> optionsAccessor) : IRequestHandler<CreatePostalCodeCommand, IResponse<PostalCode>>
{
    private readonly IAPIClient _client = client;
    private readonly ClientOptions _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task<IResponse<PostalCode>> Handle(
        CreatePostalCodeCommand request, CancellationToken cancellationToken)
    {
        var result = await _client.CreateAsync<PostalCode, PostalCodeInputModel>(
            request.Model,
            "services/PostalCodes",
            _options.API,
            _options.Name,
            _options.Key,
            cancellationToken);

        return result;
    }
}