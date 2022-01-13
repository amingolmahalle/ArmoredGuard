using System;

namespace Common.Helpers;

public static class Randomizer
{
    public static string GenerateOtpCode(int min = 1000, int max = 9999)
    {
        Random random = new Random(DateTime.Now.Millisecond);

        return random.Next(min, max).ToString();
    }

    public static string GeneratePassword(int length = 8, int nonAlphaNumericChars = 4)
    {
        const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
        const string allowedNonAlphaNum = "!@#$%^&*()_-+=[{]};:<>|./?";

        string password = string.Empty;

        Random random = new Random(DateTime.Now.Millisecond);

        for (int i = 0; i < length; i++)
        {
            if (random.Next(1) > 0 && nonAlphaNumericChars > 0)
            {
                password += allowedNonAlphaNum[random.Next(allowedNonAlphaNum.Length)];
                nonAlphaNumericChars--;
            }
            else
            {
                password += allowedChars[random.Next(allowedChars.Length)];
            }
        }

        return password;
    }
}