// <copyright file="NameValidationHarness.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Source.Narrative
{
    /// <summary>
    /// Provides reusable validation logic for player-provided names within narrative flows.
    /// Ensures consistent enforcement of formatting and length rules across UI surfaces.
    /// </summary>
    public sealed class NameValidationHarness
    {
        private static readonly HashSet<char> DefaultAllowedSymbols = new() { ' ', '-', '\'' };
        private readonly HashSet<char> allowedSymbols;

        /// <summary>
        /// Initializes a new instance of the <see cref="NameValidationHarness"/> class.
        /// </summary>
        /// <param name="maximumLength">The maximum allowed length for a name.</param>
        /// <param name="additionalAllowedSymbols">
        /// Optional extra symbols permitted in addition to letters, digits, space, hyphen, and apostrophe.
        /// </param>
        public NameValidationHarness(int maximumLength, IEnumerable<char>? additionalAllowedSymbols = null)
        {
            if (maximumLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumLength), "Maximum length must be positive.");
            }

            this.MaximumLength = maximumLength;

            this.allowedSymbols = new HashSet<char>(DefaultAllowedSymbols);
            if (additionalAllowedSymbols != null)
            {
                foreach (char symbol in additionalAllowedSymbols)
                {
                    this.allowedSymbols.Add(symbol);
                }
            }
        }

        /// <summary>
        /// Gets the maximum length allowed for a name.
        /// </summary>
        public int MaximumLength { get; }

        /// <summary>
        /// Validates the provided name and returns the result of the validation check.
        /// </summary>
        /// <param name="input">The name supplied by the user.</param>
        /// <returns>
        /// A <see cref="NameValidationResult"/> indicating whether validation succeeded along with an error message.
        /// </returns>
        public NameValidationResult SubmitName(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return NameValidationResult.Failure("Name cannot be empty.");
            }

            if (input.Length > this.MaximumLength)
            {
                return NameValidationResult.Failure($"Name must be {this.MaximumLength} characters or fewer.");
            }

            if (input.Any(this.IsDisallowedCharacter))
            {
                return NameValidationResult.Failure("Name contains unsupported characters.");
            }

            return NameValidationResult.Success();
        }

        private bool IsDisallowedCharacter(char character)
        {
            return !(char.IsLetterOrDigit(character) || this.allowedSymbols.Contains(character));
        }
    }

    /// <summary>
    /// Represents the outcome of a name validation attempt.
    /// </summary>
    public readonly struct NameValidationResult
    {
        private NameValidationResult(bool isAccepted, string errorMessage)
        {
            this.IsAccepted = isAccepted;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets a value indicating whether the name was accepted.
        /// </summary>
        public bool IsAccepted { get; }

        /// <summary>
        /// Gets the error message when validation fails; empty when validation succeeds.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Creates a successful validation result.
        /// </summary>
        /// <returns>A <see cref="NameValidationResult"/> indicating success.</returns>
        public static NameValidationResult Success() => new(true, string.Empty);

        /// <summary>
        /// Creates a failed validation result with the provided message.
        /// </summary>
        /// <param name="message">The error message describing why validation failed.</param>
        /// <returns>A <see cref="NameValidationResult"/> indicating failure.</returns>
        public static NameValidationResult Failure(string message) => new(false, message);
    }
}
