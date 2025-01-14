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
    private readonly IMomoService _momoService;

    public PaymentController(IVnPayService vnPayService, IPaymentService paymentService, IStripeService stripeService, IMomoService momoService)
    {
        _vnPayService = vnPayService;
        _paymentService = paymentService;
        _stripeService = stripeService;
        _momoService = momoService;
    }

    [HttpPost("vn-pay")]
    public async Task<IActionResult> CreateVnPayPaymentUrl(CreatePaymentRequest createPaymentRequest)
    {
        //Create payment
        var payment = await _paymentService.CreatePayment(createPaymentRequest, "VNPAY");

        // Access HttpContext directly
        var context = HttpContext;

        // Generate the payment URL via the VnPayService
        var paymentUrl = await _vnPayService.CreatePaymentUrl(payment, context);

        // Return the result wrapped in ApiResult
        return Ok(ApiResult<string>.Success(paymentUrl));
    }

    [HttpPost("validate-vnpay-result")]
    [AllowAnonymous]
    public async Task<IActionResult> VnPayCallback([FromBody] Dictionary<string, string> vnpParams)
    {
        Console.WriteLine("Verify payment using the VnPayService");
        return Ok(ApiResult<bool>.Success(await _vnPayService.VerifyPayment(vnpParams)));
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> CreateStripePaymentUrl(CreatePaymentRequest createPaymentRequest)
    {
        //Create payment
        var payment = await _paymentService.CreatePayment(createPaymentRequest, "STRIPE");
    
        // Generate the payment URL via the VnPayService
        var paymentUrl = await _stripeService.CreatePaymentSession(payment);
    
        // Return the result wrapped in ApiResult
        return Ok(ApiResult<string>.Success(paymentUrl));
    }
    
    [HttpPost("stripe-callback-success")]
    [AllowAnonymous]
    public async Task<IActionResult> StripeCallbackSuccess(StripeCallbackRequest stripeCallbackRequest)
    {
        return Ok(ApiResult<bool>.Success(await _stripeService.HandleRecord(stripeCallbackRequest)));
    }
    
    [HttpPost("stripe-callback-cancelled")]
    [AllowAnonymous]
    public async Task StripeCallbackCancelled(StripeCallbackRequest stripeCallbackRequest)
    {
        await _stripeService.ExpireSession(stripeCallbackRequest);
    }

    [HttpPost("momo")]
    public async Task<IActionResult> CreateMomoPaymentUrl(CreatePaymentRequest createPaymentRequest)
    {
        var payment = await _paymentService.CreatePayment(createPaymentRequest, "MOMO");
        
        var paymentUrl = await _momoService.CreatePaymentUrl(payment);

        // Return the result wrapped in ApiResult
        return Ok(ApiResult<string>.Success(paymentUrl));
    }
    
    [HttpPost("momo-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> MomoCallback(MomoCallbackRequest momoCallbackRequest)
    {
        return Ok(ApiResult<bool>.Success(await _momoService.HandleRecord(momoCallbackRequest)));
    }
}