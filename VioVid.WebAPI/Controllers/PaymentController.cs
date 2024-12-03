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
    private readonly IPaymentService _paymentService;
    private readonly IStripeService _stripeService;
    private readonly IVnPayService _vnPayService;

    public PaymentController(IVnPayService vnPayService, IPaymentService paymentService, IStripeService stripeService)
    {
        _vnPayService = vnPayService;
        _paymentService = paymentService;
        _stripeService = stripeService;
    }

    [HttpPost("vn-pay")]
    public async Task<IActionResult> CreateVnPayPaymentUrl(CreatePaymentRequest createPaymentRequest)
    {
        //Create payment
        var payment = await _paymentService.CreatePayment(createPaymentRequest);
        Console.WriteLine(payment);

        // Access HttpContext directly
        var context = HttpContext;

        // Generate the payment URL via the VnPayService
        var paymentUrl = await _vnPayService.CreatePaymentUrl(payment, context);

        // Return the result wrapped in ApiResult
        return Ok(ApiResult<string>.Success(paymentUrl));
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> CreateStripePaymentUrl(CreatePaymentRequest createPaymentRequest)
    {
        //Create payment
        var payment = await _paymentService.CreatePayment(createPaymentRequest);
        Console.WriteLine(payment);

        // Access HttpContext directly
        var context = HttpContext;

        // Generate the payment URL via the VnPayService
        var paymentUrl = await _stripeService.CreatePaymentSession(payment);

        // Return the result wrapped in ApiResult
        return Ok(ApiResult<string>.Success(paymentUrl));
    }

    [HttpGet("stripe-callback-success")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeCallbackSuccess([FromQuery] string session_id)
    {
        var isSuccess = await _stripeService.Success(session_id);
        if (isSuccess) return Ok(ApiResult<string>.Success("Payment was successful!"));
        return BadRequest(ApiResult<string>.Success("Payment failed!"));
    }

    [HttpGet("stripe-callback-cancelled")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeCallbackCancelled([FromQuery] string session_id)
    {
        return BadRequest(ApiResult<string>.Success("Payment was cancelled!"));
    }
}