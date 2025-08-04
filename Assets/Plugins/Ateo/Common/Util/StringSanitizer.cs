using System;
using System.Collections.Generic;
using System.Text;
using Ateo.Extensions;

namespace Plugins.Ateo.Common.Util
{
	/// <summary>
	/// A class which sanitizes any provided string based on rules defined in <see cref="StringSanitizerOptions"/>
	/// </summary>
	public class StringSanitizer
	{
		/// <summary>
		/// A class holding all the options which are needed by <see cref="StringSanitizer"/> to sanitize strings.
		/// </summary>
		[Serializable]
		public class StringSanitizerOptions
		{
			#region Fields

			/// <summary>
			/// Specifies whether tags "<tag></tag>" are allowed.
			/// </summary>
			public bool RemoveTags;

			/// <summary>
			/// Specifies whether line breaks are allowed
			/// </summary>
			public bool RemoveLineBreaks;

			/// <summary>
			/// Specifies whether consecutive white spaces are allowed.
			/// </summary>
			public bool RemoveMultipleWhiteSpaces;

			/// <summary>
			/// Specifies whether letters are allowed.
			/// </summary>
			public bool Letters;

			/// <summary>
			/// Specifies whether digits are allowed.
			/// </summary>
			public bool Digits;

			/// <summary>
			/// Specifies whether white spaces are allowed.
			/// </summary>
			public bool WhiteSpaces;

			/// <summary>
			/// Specifies whether special characters are allowed.
			/// </summary>
			public bool SpecialCharacters;

			/// <summary>
			/// The maximum length of the string which will be returned.
			/// </summary>
			public int Length;

			/// <summary>
			/// List of decimals that represent Unicode characters which are supported. If a character is not represented in this list,
			/// it will be discarded before doing any other checks.
			/// See <a href="https://en.wikipedia.org/wiki/List_of_Unicode_characters#Basic_Latin">Unicode characters</a>
			/// </summary>
			public List<int> SupportedCharacters;

			/// <summary>
			/// List of decimals that represent Unicode characters which are supported. If a character is not represented in this list,
			/// it will be discarded if <see cref="SpecialCharacters"/> is enabled.
			/// See <a href="https://en.wikipedia.org/wiki/List_of_Unicode_characters#Basic_Latin">Unicode characters</a>
			/// </summary>
			public List<int> AllowedSpecialCharacters;

			#endregion

			#region Constructor

			public StringSanitizerOptions(bool removeTags = true, bool removeLineBreaks = true, bool removeMultipleWhiteSpaces = true,
				bool letters = true, bool digits = true, bool whiteSpaces = true, bool specialCharacters = true, int length = 0,
				List<int> supportedCharacters = null, List<int> allowedSpecialCharacters = null)
			{
				RemoveTags = removeTags;
				RemoveLineBreaks = removeLineBreaks;
				RemoveMultipleWhiteSpaces = removeMultipleWhiteSpaces;
				Letters = letters;
				Digits = digits;
				WhiteSpaces = whiteSpaces;
				SpecialCharacters = specialCharacters;
				Length = length;
				SupportedCharacters = supportedCharacters ?? new List<int>();
				AllowedSpecialCharacters = allowedSpecialCharacters ?? new List<int>();
			}

			#endregion

			#region Public Methods
			
			/// <summary>
			/// Adds a character to the list of supported characters.
			/// </summary>
			/// <param name="character">The character to add</param>
			public void AddSupportedCharacter(char character)
			{
				if (!SupportedCharacters.Contains(character))
				{
					SupportedCharacters.Add(character);
					SupportedCharacters.Sort();
				}
			}

			/// <summary>
			/// Adds a character to the list of supported characters.
			/// </summary>
			/// <param name="number">The decimal number representing the character to add</param>
			public void AddSupportedCharacter(int number)
			{
				if (!SupportedCharacters.Contains(number))
				{
					SupportedCharacters.Add(number);
					SupportedCharacters.Sort();
				}
			}

			/// <summary>
			/// Adds a range of characters to the list of supported characters.
			/// </summary>
			/// <param name="from">The decimal number representing the character from which the range starts (inclusive)</param>
			/// <param name="to">The decimal number representing the character at which the range ends (inclusive)</param>
			public void AddSupportedCharacters(int from, int to)
			{
				for (int i = from; i <= to; i++)
				{
					if (!SupportedCharacters.Contains(i))
					{
						SupportedCharacters.Add(i);
					}
				}

				SupportedCharacters.Sort();
			}
			
			/// <summary>
			/// Adds a character to the list of allowed special characters.
			/// </summary>
			/// <param name="character">The character to add</param>
			public void AddAllowedSpecialCharacter(char character)
			{
				if (!AllowedSpecialCharacters.Contains(character))
				{
					AllowedSpecialCharacters.Add(character);
					AllowedSpecialCharacters.Sort();
				}
			}

			/// <summary>
			/// Adds a character to the list of allowed special characters.
			/// </summary>
			/// <param name="number">The decimal number representing the character to add</param>
			public void AddAllowedSpecialCharacter(int number)
			{
				if (!AllowedSpecialCharacters.Contains(number))
				{
					AllowedSpecialCharacters.Add(number);
					AllowedSpecialCharacters.Sort();
				}
			}

			/// <summary>
			/// Adds a range of characters to the list of allowed special characters.
			/// </summary>
			/// <param name="from">The decimal number representing the character from which the range starts (inclusive)</param>
			/// <param name="to">The decimal number representing the character at which the range ends (inclusive)</param>
			public void AddAllowedSpecialCharacters(int from, int to)
			{
				for (int i = from; i <= to; i++)
				{
					if (!AllowedSpecialCharacters.Contains(i))
					{
						AllowedSpecialCharacters.Add(i);
					}
				}

				AllowedSpecialCharacters.Sort();
			}

			#endregion
		}

		#region Properties

		/// <summary>
		/// The options used by this <see cref="StringSanitizer"/>.
		/// </summary>
		public StringSanitizerOptions Options { get; }

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a new instance of <see cref="StringSanitizer"/>
		/// </summary>
		public StringSanitizer()
		{
			Options = new StringSanitizerOptions();
		}

		/// <summary>
		/// Creates a new instance of <see cref="StringSanitizer"/>
		/// </summary>
		public StringSanitizer(StringSanitizerOptions options)
		{
			Options = options;
		}

		/// <summary>
		/// Creates a new instance of <see cref="StringSanitizer"/>
		/// </summary>
		public StringSanitizer(bool removeTags = true, bool removeLineBreaks = true, bool removeMultipleWhiteSpaces = true,
			bool letters = true, bool digits = true, bool whiteSpaces = true, bool specialCharacters = true, int length = 0,
			List<int> supportedCharacters = null, List<int> allowedSpecialCharacters = null)
		{
			Options = new StringSanitizerOptions(removeTags, removeLineBreaks, removeMultipleWhiteSpaces, letters, digits, whiteSpaces,
				specialCharacters, length, supportedCharacters, allowedSpecialCharacters);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sanitize the provided string based on the rules defined in <see cref="Options"/>.
		/// </summary>
		/// <param name="input">The string to be sanitized</param>
		/// <returns>A sanitized string</returns>
		public string Sanitize(string input)
		{
			if (Options.RemoveTags)
			{
				input = input.RemoveTags();
			}

			if (Options.RemoveLineBreaks)
			{
				input = input.RemoveLineBreaks();
			}

			if (Options.RemoveMultipleWhiteSpaces)
			{
				input = input.RemoveMultipleWhiteSpaces();
			}

			StringBuilder sb = new StringBuilder();

			foreach (char c in input)
			{
				if (!Options.SupportedCharacters.Contains(c)) continue;

				if ((Options.Digits && char.IsDigit(c)) ||
				    (Options.Letters && char.IsLetter(c)) ||
				    (Options.WhiteSpaces && char.IsWhiteSpace(c)) ||
				    (Options.SpecialCharacters && (Options.AllowedSpecialCharacters.Count > 0
					    ? Options.AllowedSpecialCharacters.Contains(c)
					    : !char.IsDigit(c) || !char.IsLetter(c) || !char.IsWhiteSpace(c))))
				{
					sb.Append(c);
				}
			}

			input = sb.ToString();

			input = input.Trim(); // Returns a string which equals the input string with all white-spaces trimmed from start and end

			if (Options.Length > 0)
			{
				input = input.Truncate(Options.Length);
			}

			return input;
		}

		#endregion
	}
}