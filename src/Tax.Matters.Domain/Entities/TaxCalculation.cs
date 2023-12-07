namespace Tax.Matters.Domain.Entities;

public class TaxCalculation : Auditable
{
    public string PostalCodeId { get; set; } = default!;
    public PostalCode PostalCode { get; set; } = default!;
    public decimal AnnualIncome { get; set; }
    public decimal TaxAmount { get; set;}
}
