namespace ZeilCodingExercise.Extensions;

public static class StringExtensions {
	public static string RemoveWhitespace(this string input) =>
		new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
}
