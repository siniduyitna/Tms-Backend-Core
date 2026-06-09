using System.ComponentModel.DataAnnotations;

namespace TmsApi.Options;

public class PaymentOptions
{
    [Required(ErrorMessage = "The GatewayUrl field is required.")] // 👈 የግድ መኖር አለበት
    public required string GatewayUrl { get; init; }

    [Range(100, 100000, ErrorMessage = "MaxDepositBirr must be between 100 and 100,000 Birr.")] //
    public decimal MaxDepositBirr { get; init; }
}