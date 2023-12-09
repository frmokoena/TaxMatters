using MediatR;
using Microsoft.Extensions.Options;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Models.Dto;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Queries;

namespace Tax.Matters.Web.Core.Modules.TaxCalculations.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="GetTaxCalculationsQueryHandler"/> handler class
/// </summary>
/// <param name="client"></param>
/// <param name="optionsAccessor"></param>
public class GetTaxCalculationsQueryHandler(IAPIClient client, IOptions<ClientOptions> optionsAccessor) : IRequestHandler<GetTaxCalculationsQuery, IResponse<PageListDto<TaxCalculation>>>
{
    private readonly IAPIClient _client = client;  
    private readonly ClientOptions _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task<IResponse<PageListDto<TaxCalculation>>> Handle(
        GetTaxCalculationsQuery request, CancellationToken cancellationToken)
    {
        string queryString = $"pageNumber={request.PageNumber}&limit={request.Limit}";

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            queryString += $"&keyword={request.Keyword}";
        }

        if (!string.IsNullOrWhiteSpace(request.SortOrder))
        {
            queryString += $"&sortOrder={request.SortOrder}";
        }

        var result = await _client.GetAsync<PageListDto<TaxCalculation>>(
            "services/taxcalculations?" + queryString,
            _options.API, 
            _options.Name, 
            _options.Key, 
            cancellationToken);

        return result;
    }
}
