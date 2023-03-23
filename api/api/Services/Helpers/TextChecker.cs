using api.Exceptions;


namespace api.Services.Helpers;


public class TextChecker
{
    public static void MinLength(string text, int minLength)
    {
        if(text.Length < minLength)
            throw new TextCheckerException("must be at least "+ minLength + " characters long!");

    }

    public static void MaxLength(string text, int maxLength)
    {
        if(text.Length > maxLength)
            throw new TextCheckerException("must be at most "+ maxLength + " characters long!");
    }

    public static void ContainsNumber(string text)
    {
        if(!text.Any(char.IsDigit))
            throw new TextCheckerException("must contain at least one digit!");
    }

    public static void ConainsCapitalLetter(string text)
    {
        if(!text.Any(char.IsUpper))
            throw new TextCheckerException("must contain at least one capital letter!");

    }

    public static void ContainsSmallLetter(string text)
    {
        if(!text.Any(char.IsLower))
            throw new TextCheckerException("must contain at least small letter!");

    }

    public static void IsAlphaNumeric(string text)
    {
        foreach (char c in text)
        {
            if(!Char.IsLetterOrDigit(c))
                throw new TextCheckerException("contains non-alphanumeric characters!");

        }
    }

}