using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Infrastructure.Data.Repositories;

/// <summary>
/// Describe the repository for the tax calculation
/// </summary>
public interface ICalculationRepository
{
    Task<TaxCalculation> AddAsync(TaxCalculation taxCalculation, CancellationToken token);

    Task<TaxCalculation?> GetCalculationAsync(
        string postalCode, 
        decimal income, 
        CancellationToken cancellationToken = default);

    Task<PostalCode?> GetPostalCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<IList<ProgressiveIncomeTax>> GetProgressiveIncomeTaxTableAsync(
        string incomeTaxId, 
        decimal income,
        CancellationToken cancellationToken = default);
}
