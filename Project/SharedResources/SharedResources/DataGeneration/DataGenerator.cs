using System;

namespace SharedResources.DataGeneration
{
    public static class DataGenerator
    {
        private static Random rand = new Random();

        public static string GetRandomString(int length)
        {
            var word = String.Empty;
            var letters = @"abcdefghijklmnopqrstuvwxyz";

            for (int characterIndex = 0; characterIndex <= length; characterIndex++)
            {
                int letterIndex = rand.Next(0, letters.Length - 1);
                word += letters[letterIndex];
            }

            return word;
        }

        public static int GetRandomNumber(int length)
        {
            var number = String.Empty;
            var digits = @"0123456789";

            for (int digitIndex = 0; digitIndex <= length; digitIndex++)
            {
                int numberIndex = rand.Next(0, digits.Length - 1);
                number += digits[numberIndex];
            }

            return Int32.Parse(number);
        }
    }
}