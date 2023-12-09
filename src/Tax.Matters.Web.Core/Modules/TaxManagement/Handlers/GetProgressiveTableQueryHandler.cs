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
using Tax.Matters.Web.Core.Modules.TaxManagement.Queries;

namespace Tax.Matters.Web.Core.Modules.TaxManagement.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="GetProgressiveTableQueryHandler"/> handler class
/// </summary>
/// <param name="client"></param>
/// <param name="optionsAccessor"></param>
public class GetProgressiveTableQueryHandler(IAPIClient client, IOptions<ClientOptions> optionsAccessor) : IRequestHandler<GetProgressiveTableQuery, IResponse<PageListDto<ProgressiveIncomeTax>>>
{
    private readonly IAPIClient _client = client;
    private readonly ClientOptions _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task<IResponse<PageListDto<ProgressiveIncomeTax>>> Handle(
        GetProgressiveTableQuery request, CancellationToken cancellationToken)
    {
        string queryString = $"pageNumber={request.PageNumber}&limit={request.Limit}&incomeTaxId={request.IncomeTaxId}";

        var result = await _client.GetAsync<PageListDto<ProgressiveIncomeTax>>(
            "services/taxmanagement/progressive?" + queryString,
            _options.API,
            _options.Name,
            _options.Key,
            cancellationToken);

        return result;
    }
}
