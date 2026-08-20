
using Bodde.Common.Extensions;

namespace Bodde.Query.Core;

public static class StringExtensions
{
    extension(string value)
    {
        /// <summary>
        /// Splits the string into tokens using the specified separator.
        /// </summary>
        /// <param name="separator">The character used to separate tokens.</param>
        /// <param name="trim">Indicates whether leading and trailing whitespace should be removed from each token.</param>
        /// <param name="removeEmpty">Indicates whether empty tokens should be removed from the result.</param>
        /// <returns>An array containing the tokens extracted from the string.</returns>
        public string[] Tokenize(char separator, bool trim = true, bool removeEmpty = true)
        {
            var tokens = value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            if(trim)
                tokens = tokens.Select(_ => _.Trim()).ToArray();

            if(removeEmpty)
                tokens = tokens.Where(_ => _.IsNullOrEmpty() == false).ToArray();

            return tokens;
        }
    }
}