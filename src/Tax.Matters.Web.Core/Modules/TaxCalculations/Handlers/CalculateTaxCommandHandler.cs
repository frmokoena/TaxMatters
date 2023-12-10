using MediatR;
using Microsoft.Extensions.Options;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Commands;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Models;

namespace Tax.Matters.Web.Core.Modules.TaxCalculations.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="CalculateTaxCommandHandler"/> handler class
/// </summary>
/// <param name="client"></param>
/// <param name="optionsAccessor"></param>
public class CalculateTaxCommandHandler(IAPIClient client, IOptions<ClientOptions> optionsAccessor) : IRequestHandler<CalculateTaxCommand, IResponse<TaxCalculation>>
{
    private readonly IAPIClient _client = client;
    private readonly ClientOptions _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task<IResponse<TaxCalculation>> Handle(
        CalculateTaxCommand request, CancellationToken cancellationToken)
    {
        var result = await _client.CreateAsync<TaxCalculation, TaxCalculationInputModel>(
            request.Model,
            "services/taxcalculations/calculate",
            _options.API,
            _options.Name,
            _options.Key,
            cancellationToken);

        return result;
    }
}
