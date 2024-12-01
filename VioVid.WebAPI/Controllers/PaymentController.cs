using Application.DTOs;
using Application.DTOs.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IVnPayService _vnPayService;

    public PaymentController(IVnPayService vnPayService)
    {
        _vnPayService = vnPayService;
    }

    [HttpPost("vn-pay")]
    public async Task<IActionResult> CreateVnPayPaymentUrl(CreatePaymentRequest createPaymentRequest)
    {
        // Access HttpContext directly
        var context = HttpContext;

        // Generate the payment URL via the VnPayService
        var paymentUrl = await _vnPayService.CreatePaymentUrl(createPaymentRequest, context);

        // Return the result wrapped in ApiResult
        return Ok(ApiResult<string>.Success(paymentUrl));
    }


    [HttpGet("vn-pay-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> VnPayCallback([FromQuery] Dictionary<string, string> vnpParams)
    {
        // Verify payment using the VnPayService
        var isValid = await _vnPayService.VerifyPayment(vnpParams);

        if (isValid) return Ok(ApiResult<string>.Success("Payment was successful!"));

        return BadRequest(ApiResult<string>.Success("Payment failed!"));
    }
}