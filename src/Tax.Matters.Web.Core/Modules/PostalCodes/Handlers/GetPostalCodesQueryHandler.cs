using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Models.Dto;
using Tax.Matters.Web.Core.Modules.PostalCodes.Queries;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Queries;

namespace Tax.Matters.Web.Core.Modules.PostalCodes.Handlers;

public class GetPostalCodesQueryHandler(IAPIClient client, IOptions<ClientOptions> optionsAccessor) : IRequestHandler<GetPostalCodesQuery, IResponse<PageListDto<PostalCode>>>
{
    private readonly IAPIClient _client = client;
    private readonly ClientOptions _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task<IResponse<PageListDto<PostalCode>>> Handle(
        GetPostalCodesQuery request, CancellationToken cancellationToken)
    {
        string queryString = $"pageNumber={request.PageNumber}&limit={request.Limit}";

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            queryString += $"&keyword={request.Keyword}";
        }

        var result = await _client.GetAsync<PageListDto<PostalCode>>(
            "services/postalcodes?" + queryString,
            _options.API,
            _options.Name,
            _options.Key,
            cancellationToken);

        return result;
    }
}
