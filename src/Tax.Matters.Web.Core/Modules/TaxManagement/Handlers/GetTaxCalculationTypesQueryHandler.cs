using MediatR;
using Microsoft.Extensions.Options;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Models.Dto;
using Tax.Matters.Web.Core.Modules.TaxManagement.Queries;

namespace Tax.Matters.Web.Core.Modules.TaxManagement.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="GetTaxCalculationTypesQueryHandler"/> handler class
/// </summary>
/// <param name="client"></param>
/// <param name="optionsAccessor"></param>
public class GetTaxCalculationTypesQueryHandler(IAPIClient client, IOptions<ClientOptions> optionsAccessor) : IRequestHandler<GetTaxCalculationTypesQuery, IResponse<PageListDto<IncomeTax>>>
{
    private readonly IAPIClient _client = client;
    private readonly ClientOptions _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task<IResponse<PageListDto<IncomeTax>>> Handle(
        GetTaxCalculationTypesQuery request, CancellationToken cancellationToken)
    {
        string queryString = $"pageNumber={request.PageNumber}&limit={request.Limit}";

        var result = await _client.GetAsync<PageListDto<IncomeTax>>(
            "services/taxmanagement/taxcalculationtypes?" + queryString,
            _options.API,
            _options.Name,
            _options.Key,
            cancellationToken);

        return result;
    }
}
