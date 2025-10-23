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
        /// Resolves a thread choice from user input.
        /// </summary>
        public static DreamweaverChoice? ResolveThreadChoice(string input, IReadOnlyList<DreamweaverChoice> threadChoices)
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
        /// Attempts to resolve a thread choice by numeric index.
        /// </summary>
        private static DreamweaverChoice? TryResolveNumericChoice(string input, IReadOnlyList<DreamweaverChoice> threadChoices)
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
        /// Attempts to resolve a thread choice by text matching.
        /// </summary>
        private static DreamweaverChoice? TryResolveTextChoice(string input, IReadOnlyList<DreamweaverChoice> threadChoices)
        {
            foreach (DreamweaverChoice option in threadChoices)
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