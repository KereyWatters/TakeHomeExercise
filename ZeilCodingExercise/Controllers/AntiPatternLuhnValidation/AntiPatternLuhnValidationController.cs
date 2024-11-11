using Microsoft.AspNetCore.Mvc;
using ZeilCodingExercise.Services;

namespace ZeilCodingExercise.Controllers.LuhnValidation;

// The task is a bit contradictory. It specifies the contract and intention for this endpoint
// and specifies I should write this as I would paid, production code.
// However, I would not write an endpoint with this signature.

// I have labelled this as an antipattern for a several reasons:
// 1) For simple, well known and spec'd algorythms such as Luhn, no code is the best code.
//   You cover more ground, produce less bugs, and have more maintainable code by using a well known library.
// 2) Sending the card number (PAN) extends the PCI-DSS scope to the new system, and all systems between.
//   This means the network stack, anything between it like drivers, proxies, and this service are now in scope
//   for PCI-DSS compliance. This is a huge risk for such a simple task, and should be avoided in favor of local validation
// 3) Returning a simple true/false is unhelpful. In the case of an error, the server should be able to describe
//   what the reason for failure was, without resorting to a non-200 status code, which is unintuitive to respond to
// 4) What is the caller after? Knowing what the Luhn algorythm is, and how to use it means the caller needs
//   to know it helps to validate the card. The endpoint should help the caller validate a card,
//   not perform just one part of the card validation. This is a leaky abstraction.
// 5) The Luhn algorythm is not comprehensive, cards such as China UnionPay and RuPay are valid, but do not pass.
//   A valid Luhn (e.g. 8888) does not mean valid card number, and an invalid Luhn can be a valid card number.

// For my actual recommended implemenation, see CreditCardValidationController.cs

[ApiController]
[Route("[controller]")]
public class AntiPatternLuhnValidationController : ControllerBase {
	private readonly ILogger<AntiPatternLuhnValidationController> _logger;
	private readonly ILuhnValidationService _luhnValidationService;

	public AntiPatternLuhnValidationController(
		ILogger<AntiPatternLuhnValidationController> logger,
		ILuhnValidationService luhnValidationService
	) {
		_logger = logger;
		_luhnValidationService = luhnValidationService;
	}

	[HttpGet(Name = "ValidateCreditCardLuhn")]
	public bool ValidateCreditCardLuhn(string cardNumber) {
		return _luhnValidationService.ValidateCreditCardLuhn(cardNumber).IsValid;
	}

}
