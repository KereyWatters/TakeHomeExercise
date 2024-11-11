using CreditCardValidator;
using ZeilCodingExercise.Services;

namespace ZeilCodingExercise.Tests.Services;

public class LuhnValidationServiceTests
{
    LuhnValidationService _service;
    public LuhnValidationServiceTests()
	{
        _service = new LuhnValidationService();
	}

    // Verify malformed strings don't pass
    [Theory]
    [InlineData("asd12asdas223456")]
    [InlineData("1234567890123asd")]
    [InlineData("asd78901d2345678")]
	[InlineData("123456789=01256789")]
    public void MalformedCardNumbersShouldFail(string input)
	{
		var result = _service.ValidateCreditCardLuhn(input);
		Assert.False(result.IsValid);
		Assert.Equal(LuhnValidationService.LuhnValidationErrorType.MalformedNumber, result.Error);
	}

    [Theory]
	[InlineData("")]
	[InlineData("123456789")]
	[InlineData("12345678901234567890")]
    public void CardNumbersWithUnexpectedLengthsShouldFail(string input) {
        var result = _service.ValidateCreditCardLuhn(input);
		Assert.False(result.IsValid);
		Assert.Equal(LuhnValidationService.LuhnValidationErrorType.UnexpectedLength, result.Error);
	}

	// From https://github.com/gustavofrizzo/CreditCardValidator/blob/master/Src/CreditCardValidator.Tests/Data/LuhnNumbers.json
	[Theory]
	[InlineData("5328017940476466")]
	[InlineData("4929174520522064")]
	[InlineData("6011939908785655")]
	[InlineData("349514561709734")]
	[InlineData("3088689936484764")]
	[InlineData("5340328330477582")]
	[InlineData("4916303389920714")]
	[InlineData("370040022538449")]
	public void CardNumbersWithValidChecksumShouldPass(string input) {
		var result = _service.ValidateCreditCardLuhn(input);
		Assert.True(result.IsValid);
		Assert.Equal(LuhnValidationService.LuhnValidationErrorType.Unknown, result.Error);
	}

	// These have all had a digit changed somewhere, resulting in an invalid checksum
	[Theory]
	[InlineData("2234567890123456")]
	[InlineData("5328117940476466")]
	[InlineData("4929174530522064")]
	[InlineData("6011939918785655")]
	[InlineData("349515561709734")]
	[InlineData("3088689936494764")]
	[InlineData("5340328330477592")]
	[InlineData("4916303389920715")]
	[InlineData("370040122538449")]
	public void CardNumbersWithInvalidChecksumShouldFail(string input) {
		var result = _service.ValidateCreditCardLuhn(input);
		Assert.False(result.IsValid);
		Assert.Equal(LuhnValidationService.LuhnValidationErrorType.InvalidCardNumber, result.Error);
	}

	[Fact]
	public void TheLibraryAgreesWithTheCustomLuhnAlgorythm() {
		for (var i = 0; i < 1000; i++) {
			var cardNumber = CreditCardFactory.RandomCardNumber(CardIssuer.Discover);
			// Randomize a digit somewhere, which will still be valid 10% of the time
			var index = new Random().Next(0, cardNumber.Length);
			var newDigit = new Random().Next(0, 10).ToString();
			cardNumber = cardNumber.Remove(index, 1).Insert(index, newDigit);

			var result = _service.ValidateCreditCardLuhn(cardNumber);
			Assert.Equal(result.IsValid, Luhn.CheckLuhn(cardNumber));
		}
	}

}