using ZeilCodingExercise.Extensions;

namespace ZeilCodingExercise.Services;

public interface ILuhnValidationService {
	LuhnValidationService.LuhnValidationResult ValidateCreditCardLuhn(string cardNumber);
}

public class LuhnValidationService : ILuhnValidationService {

	public enum LuhnValidationErrorType {
		Unknown,
		MalformedNumber,
		UnexpectedLength,
		InvalidCardNumber,
	}
	public record LuhnValidationResult(bool IsValid, LuhnValidationErrorType Error);

	public LuhnValidationResult ValidateCreditCardLuhn(string cardNumber) {
		cardNumber = cardNumber.RemoveWhitespace();

		if (cardNumber.Length < 13 || cardNumber.Length > 19) {
			return new LuhnValidationResult(false, LuhnValidationErrorType.UnexpectedLength);
		}

		if (cardNumber.All(char.IsDigit) == false) {
			return new LuhnValidationResult(false, LuhnValidationErrorType.MalformedNumber);
		}

		if (ValidateLuhn(cardNumber) == false) {
			return new LuhnValidationResult(false, LuhnValidationErrorType.InvalidCardNumber);
		}

		return new LuhnValidationResult(true, LuhnValidationErrorType.Unknown);
	}

	// Ported from the pseudo code at https://en.wikipedia.org/wiki/Luhn_algorithm
	private int CharToInt(char c) => c - '0';
	private bool ValidateLuhn(string cardNumber) {
		var sum = 0;
		var parity = cardNumber.Length % 2;
		for (var i = 0; i < cardNumber.Length - 1; i++) {
			var digit = CharToInt(cardNumber[i]);
			if (i % 2 != parity) {
				sum += digit;
			} else if (digit > 4) {
				sum += 2 * digit - 9;
			} else {
				sum += 2 * digit;
			}
		}

		return CharToInt(cardNumber[^1]) == (10 - (sum % 10)) % 10;
	}
}
