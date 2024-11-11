using CreditCardValidator;
using Microsoft.AspNetCore.Mvc;

namespace ZeilCodingExercise.Controllers.LuhnValidation;

// Use of a library simplies the controller, removes a service, the need for a few extension methods,
// an entire batch of tests, gives us richer functionality to provide to callers, takes less time,
// incentivises contribution to open source libraries, and allows for code updates without dedicated dev time.

[ApiController]
[Route("[controller]")]
public class CreditCardValidationController : ControllerBase {
	private readonly ILogger<CreditCardValidationController> _logger;

	public CreditCardValidationController(ILogger<CreditCardValidationController> logger) {
		_logger = logger;
	}

	public enum CardValidationErrorType {
		Unknown,
		Malformed,
		InvalidCardNumber,
		UnsupportedCardType,
	}

	public record CardValidationResult(bool IsValid, CardValidationErrorType Error);

	// Note: In actual production code, you'd never extend PCI-DSS compliance scope to simply validate a number.
	// Such a task should be done simply locally, avoiding risks of leaking the PII via logs, tracing, memory dumps, etc.
	// This would be better off as a core/shared project/package that the caller can tap into.
	[HttpGet(Name = "ValidateCard")]
	public CardValidationResult ValidateCard(string cardNumber) {
		try {
			var validator = new CreditCardDetector(cardNumber);
			if (!validator.IsValid()) {
				return new CardValidationResult(false, CardValidationErrorType.InvalidCardNumber);
			}

			// Some cards, such as China UnionPay, and RuPay do not use Luhn validation
			// There is a conscious decision to be made here, whether these cards are supported or not.
			// They are potentially valid cards regardless of luhn validation.
			// The nice thing about using a library, is we have all this information available.
			// It's a choice to make, instead of one limited by our own implementation.
			if (Luhn.CheckLuhn(cardNumber) == false) {
				return new CardValidationResult(false, CardValidationErrorType.UnsupportedCardType);
			}

			return new CardValidationResult(true, CardValidationErrorType.Unknown);

		} catch (ArgumentException) {
			// The CreditCardDetector class has an antipattern where it throws an exception
			// to handle the case where the card number is malformed.. Ah well.
			return new CardValidationResult(false, CardValidationErrorType.Malformed);
		} catch (Exception e) {
			_logger.LogError(e, "Unexpected exception while validating card number");
			return new CardValidationResult(false, CardValidationErrorType.Unknown);
		}

	}
}
