using MediatR;
using Microsoft.Extensions.Options;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.TaxManagement.Queries;

namespace Tax.Matters.Web.Core.Modules.TaxManagement.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="ListTaxCalculationTypesQueryHandler"/> handler class
/// </summary>
/// <param name="client"></param>
/// <param name="optionsAccessor"></param>
public class ListTaxCalculationTypesQueryHandler(IAPIClient client, IOptions<ClientOptions> optionsAccessor) : IRequestHandler<ListTaxCalculationTypesQuery, IResponse<IEnumerable<IncomeTax>>>
{
    private readonly IAPIClient _client = client;
    private readonly ClientOptions _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task<IResponse<IEnumerable<IncomeTax>>> Handle(
        ListTaxCalculationTypesQuery request, CancellationToken cancellationToken)
    {
        var result = await _client.GetAsync<IEnumerable<IncomeTax>>(
            "services/taxmanagement/listtaxcalculationtypes",
            _options.API,
            _options.Name,
            _options.Key,
            cancellationToken);

        return result;
    }
}
