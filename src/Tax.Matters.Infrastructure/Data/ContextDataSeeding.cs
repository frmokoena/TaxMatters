using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Domain.Enums;

namespace Tax.Matters.Infrastructure.Data;

public static class ContextDataSeeding
{
    /// <summary>
    /// Note: 
    ///     - For test purpose
    ///     - To be run once for the context data seeding
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task SeedContextDataAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await SeedIntialContextDataAsync();

        async Task SeedIntialContextDataAsync()
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.EnsureCreated();


            // if any calculation type
            if (context.IncomeTax.Any())
            {
                return;   // Context has been seeded
            }

            // Flat rate tax
            var flatRateTax = new IncomeTax
            {
                FlatRate = 17.5m,
                TypeName = TaxCalculationType.FlatRate,
            };

            context.Add(flatRateTax);

            // Flat value tax
            var flatValueTax = new IncomeTax
            {
                TypeName = TaxCalculationType.FlatValue,
                FlatValue = new FlatValueIncomeTax
                {
                    Amount = 10000m,
                    Threshold = 200000,
                    ThresholdRate = 5m
                }
            };

            context.Add(flatValueTax);

            // Progressive tax
            var progressiveTax = new IncomeTax
            {
                TypeName = TaxCalculationType.Progressive
            };

            context.Add(progressiveTax);

            context.AddRange(new ProgressiveIncomeTax
            {
                IncomeTaxId = progressiveTax.Id,
                MinimumIncome = 0m,
                MaximumIncome = 8350m,
                Rate = 10m
            },
            new ProgressiveIncomeTax
            {
                IncomeTaxId = progressiveTax.Id,
                MinimumIncome = 8351m,
                MaximumIncome = 33950m,
                Rate = 15m
            },
            new ProgressiveIncomeTax
            {
                IncomeTaxId = progressiveTax.Id,
                MinimumIncome = 33951m,
                MaximumIncome = 82250m,
                Rate = 25m
            },
            new ProgressiveIncomeTax
            {
                IncomeTaxId = progressiveTax.Id,
                MinimumIncome = 82251m,
                MaximumIncome = 171550m,
                Rate = 28m
            },
            new ProgressiveIncomeTax
            {
                IncomeTaxId = progressiveTax.Id,
                MinimumIncome = 171551m,
                MaximumIncome = 372950m,
                Rate = 33m
            },
            new ProgressiveIncomeTax
            {
                IncomeTaxId = progressiveTax.Id,
                MinimumIncome = 372951,
                Rate = 35m
            });

            // Postal codes
            context.AddRange(new PostalCode
            {
                Code = "7441",
                IncomeTaxId = progressiveTax.Id
            },
            new PostalCode
            {
                Code = "A100",
                IncomeTaxId = flatValueTax.Id
            },
            new PostalCode
            {
                Code = "7000",
                IncomeTaxId = flatRateTax.Id
            },
            new PostalCode
            {
                Code = "1000",
                IncomeTaxId = progressiveTax.Id
            });

            try
            {
                await context.SaveChangesAsync();
            }
            catch(Exception /* ex */)
            {
            }
        }
    }
}
