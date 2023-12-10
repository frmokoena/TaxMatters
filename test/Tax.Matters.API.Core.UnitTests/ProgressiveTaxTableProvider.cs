using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.UnitTests
{
    internal static class ProgressiveTaxTableProvider
    {
        private readonly static IList<ProgressiveIncomeTax> _progressiveTable = new List<ProgressiveIncomeTax>
        {
            new() {
                IncomeTaxId = "progressive-tax",
                MinimumIncome = 0m,
                MaximumIncome = 8350m,
                Rate = 10m
            },
            new() {
                IncomeTaxId = "progressive-tax",
                MinimumIncome = 8351m,
                MaximumIncome = 33950m,
                Rate = 15m
            },
            new() {
                IncomeTaxId = "progressive-tax",
                MinimumIncome = 33951m,
                MaximumIncome = 82250m,
                Rate = 25m
            },
            new() {
                IncomeTaxId = "progressive-tax",
                MinimumIncome = 82251m,
                MaximumIncome = 171550m,
                Rate = 28m
            },
            new() {
                IncomeTaxId = "progressive-tax",
                MinimumIncome = 171551m,
                MaximumIncome = 372950m,
                Rate = 33m
            },
            new() {
                IncomeTaxId = "progressive-tax",
                MinimumIncome = 372951,
                Rate = 35m
            }
        };

        public static IList<ProgressiveIncomeTax> GetProgressiveTable(string incomeTaxId, decimal income)
            => _progressiveTable.Where(m => m.IncomeTaxId == incomeTaxId && m.MinimumIncome < income).OrderBy(m => m.MinimumIncome).ToList();

    }
}
