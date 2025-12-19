using Microsoft.AspNetCore.Mvc;

namespace Payments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(ILogger<PaymentsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult ProcessPayment([FromBody] PaymentRequest request)
    {
        // Mock payment processing
        _logger.LogInformation("Processing payment for Order {OrderId}", request.OrderId);
        
        return Ok(new 
        { 
            PaymentId = Guid.NewGuid(), 
            OrderId = request.OrderId,
            Status = "Approved",
            TransactionDate = DateTime.UtcNow 
        });
    }
}

public record PaymentRequest(Guid OrderId, decimal Amount, string CardNumber, string Expiry, string CVV);
