using MediatR;
using Microsoft.Extensions.Options;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Queries;

namespace Tax.Matters.Web.Core.Modules.TaxCalculations.Handlers;

public class GetTaxCalculationQueryHandler(IAPIClient client, IOptions<ClientOptions> optionsAccessor) : IRequestHandler<GetTaxCalculationQuery, IResponse<TaxCalculation>>
{
    private readonly IAPIClient _client = client;
    private readonly ClientOptions _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task<IResponse<TaxCalculation>> Handle(
        GetTaxCalculationQuery request, CancellationToken cancellationToken)
    {
        var result = await _client.GetAsync<TaxCalculation>(
            "services/taxcalculations/" + request.Id,
            _options.API,
            _options.Name,
            _options.Key,
            cancellationToken);

        return result;
    }
}
