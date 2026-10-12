using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuntAndPeck.Extensions;
using HuntAndPeck.Services.Interfaces;

namespace HuntAndPeck.Services
{
    internal class HintLabelService : IHintLabelService
    {

        public IList<string> GetHintStrings(int hintCount)
        {
            var hintStrings = new List<string>();
            if (hintCount <= 0)
            {
                return hintStrings;
            }

            var hintCharacters = new[] { 'S', 'A', 'D', 'F', 'J', 'K', 'L', 'E', 'W', 'C', 'M', 'P', 'G', 'H' };
            var digitsNeeded = (int)Math.Ceiling(Math.Log(hintCount) / Math.Log(hintCharacters.Length));

            var wholeHintCount = (int)Math.Pow(hintCharacters.Length, digitsNeeded);
            var shortHintCount = (wholeHintCount - hintCount) / hintCharacters.Length;
            var longHintCount = hintCount - shortHintCount;

            var longHintPrefixCount = wholeHintCount / hintCharacters.Length - shortHintCount;
            for (int i = 0, j = 0; i < longHintCount; ++i, ++j)
            {
                hintStrings.Add(new string(NumberToHintString(j, hintCharacters, digitsNeeded).Reverse().ToArray()));
                if (longHintPrefixCount > 0 && (i + 1) % longHintPrefixCount == 0)
                {
                    j += shortHintCount;
                }
            }

            if (digitsNeeded > 1)
            {
                for (var i = 0; i < shortHintCount; ++i)
                {
                    hintStrings.Add(new string(NumberToHintString(i + longHintPrefixCount, hintCharacters, digitsNeeded - 1).Reverse().ToArray()));
                }
            }

            return hintStrings.ToList();
        }

        private string NumberToHintString(int number, char[] characterSet, int noHintDigits = 0)
        {
            var divisor = characterSet.Length;
            var hintString = new StringBuilder();

            do
            {
                var remainder = number % divisor;
                hintString.Insert(0, characterSet[remainder]);
                number -= remainder;
                number /= (int)Math.Floor((double)divisor);
            } while (number > 0);

            var length = hintString.Length;
            for (var i = 0; i < (noHintDigits - length); ++i)
            {
                hintString.Insert(0, characterSet[0]);
            }

            return hintString.ToString();
        }
    }
}
