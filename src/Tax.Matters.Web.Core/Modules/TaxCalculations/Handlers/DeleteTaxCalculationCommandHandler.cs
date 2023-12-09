using MediatR;
using Microsoft.Extensions.Options;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Commands;

namespace Tax.Matters.Web.Core.Modules.TaxCalculations.Handlers;

/// <summary>
/// Initializes a new instance of the <see cref="DeleteTaxCalculationCommandHandler"/> handler class
/// </summary>
/// <param name="client"></param>
/// <param name="optionsAccessor"></param>
public class DeleteTaxCalculationCommandHandler(IAPIClient client, IOptions<ClientOptions> optionsAccessor) : IRequestHandler<DeleteTaxCalculationCommand, IResponse<TaxCalculation>>
{
    private readonly IAPIClient _client = client;
    private readonly ClientOptions _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

    public async Task<IResponse<TaxCalculation>> Handle(
        DeleteTaxCalculationCommand request, CancellationToken cancellationToken)
    {
        var result = await _client.DeleteAsync<TaxCalculation>(
            "services/taxcalculations/" + request.Id,
            _options.API,
            _options.Name,
            _options.Key,
            cancellationToken);

        return result;
    }
}
