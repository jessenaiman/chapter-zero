using System.Collections.ObjectModel;
using System.Globalization;
using OmegaSpiral.Source.Narrative;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Handles resolution of user input for narrative choices and thread selections.
    /// </summary>
    public static class NarrativeInputResolver
    {
        /// <summary>
        /// Resolves a choice from user input.
        /// </summary>
        public static ChoiceOption? ResolveThreadChoice(string input, IReadOnlyList<ChoiceOption> threadChoices)
        {
            if (threadChoices.Count == 0)
            {
                return null;
            }

            // Try numeric selection first
            var numericChoice = TryResolveNumericChoice(input, threadChoices);
            if (numericChoice != null)
            {
                return numericChoice;
            }

            // Try text-based selection
            return TryResolveTextChoice(input, threadChoices);
        }

        /// <summary>
        /// Resolves a choice option from user input.
        /// </summary>
        public static ChoiceOption? ResolveChoiceOption(string input, IReadOnlyList<ChoiceOption> activeChoices)
        {
            if (activeChoices.Count == 0)
            {
                return null;
            }

            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
            {
                index -= 1;
                if (index >= 0 && index < activeChoices.Count)
                {
                    return activeChoices[index];
                }
            }

            foreach (ChoiceOption option in activeChoices)
            {
                if (option.Text != null && option.Text.Equals(input, StringComparison.OrdinalIgnoreCase))
                {
                    return option;
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to resolve a choice by numeric index.
        /// </summary>
        private static ChoiceOption? TryResolveNumericChoice(string input, IReadOnlyList<ChoiceOption> threadChoices)
        {
            if (int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int index))
            {
                index -= 1;
                if (index >= 0 && index < threadChoices.Count)
                {
                    return threadChoices[index];
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to resolve a choice by text matching.
        /// </summary>
        private static ChoiceOption? TryResolveTextChoice(string input, IReadOnlyList<ChoiceOption> threadChoices)
        {
            foreach (ChoiceOption option in threadChoices)
            {
                if ((option.Id != null && option.Id.Equals(input, StringComparison.OrdinalIgnoreCase)) ||
                    (option.Text != null && option.Text.Equals(input, StringComparison.OrdinalIgnoreCase)))
                {
                    return option;
                }
            }

            return null;
        }
    }
}
