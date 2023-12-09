using Microsoft.EntityFrameworkCore;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Infrastructure.Data.Repositories;

public class CalculationRepository(AppDbContext context) : ICalculationRepository
{
    private readonly AppDbContext _context = context;

    public async Task<TaxCalculation> AddAsync(TaxCalculation calculation, CancellationToken cancellationToken = default)
    {
        _context.TaxCalculation.Add(calculation);

        await _context.SaveChangesAsync(cancellationToken);

        return calculation;
    }

    public async Task<TaxCalculation?> GetCalculationAsync(
        string postalCode,
        decimal income,
        CancellationToken cancellationToken = default)
    {
        var calculation = await _context.TaxCalculation
        .Where(
        m =>
                m.AnnualIncome == income
                && m.PostalCode.Code == postalCode).Select(m => new TaxCalculation
                {
                    Id = m.Id,
                    AnnualIncome = m.AnnualIncome,
                    TaxAmount = m.TaxAmount,
                    DateCreated = m.DateCreated,
                    PostalCode = new PostalCode
                    {
                        Code = m.PostalCode.Code,
                        IncomeTax = new IncomeTax
                        {
                            TypeName = m.PostalCode.IncomeTax.TypeName
                        }
                    },
                    Version = m.Version
                })
        .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return calculation;
    }

    public async Task<PostalCode?> GetPostalCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var postalCode = await _context.PostalCode
            .Where(m => m.Code == code)
            .Select(m => new PostalCode
            {
                Id = m.Id,
                Code = m.Code,
                IncomeTax = new IncomeTax
                {
                    TypeName = m.IncomeTax.TypeName,
                    FlatRate = m.IncomeTax.FlatRate,
                    FlatValue = m.IncomeTax.FlatValue == null ? null : new FlatValueIncomeTax
                    {
                        Amount = m.IncomeTax.FlatValue.Amount,
                        Threshold = m.IncomeTax.FlatValue.Threshold,
                        ThresholdRate = m.IncomeTax.FlatValue.ThresholdRate
                    }
                }
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return postalCode;
    }

    public async Task<IList<ProgressiveIncomeTax>> GetProgressiveIncomeTaxTableAsync(
        string incomeTaxId,
        decimal income,
        CancellationToken cancellationToken = default)
    {
        var table = await _context.ProgressiveIncomeTax.Where(m => m.IncomeTaxId == incomeTaxId && m.MinimumIncome <= income)
            .OrderBy(m => m.MinimumIncome)
            .Select(m => new ProgressiveIncomeTax
            {
                MinimumIncome = m.MinimumIncome,
                MaximumIncome = m.MaximumIncome,
                Rate = m.Rate
            }).ToListAsync(cancellationToken: cancellationToken);

        return table;
    }
}
