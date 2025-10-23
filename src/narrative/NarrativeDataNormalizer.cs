using System.Collections.ObjectModel;
using OmegaSpiral.Source.Narrative;
using OmegaSpiral.Source.Scripts.Common;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Handles normalization of narrative scene data to ensure consistency and provide defaults.
    /// </summary>
    public static class NarrativeDataNormalizer
    {
        /// <summary>
        /// Normalizes the provided narrative scene data.
        /// </summary>
        public static void Normalize(NarrativeSceneData data, out IReadOnlyList<DreamweaverChoice> threadChoices)
        {
            NormalizeBasicData(data);
            NormalizeInitialChoice(data, out threadChoices);
            NormalizeStoryBlocks(data);
        }

        /// <summary>
        /// Normalizes basic narrative data fields.
        /// </summary>
        private static void NormalizeBasicData(NarrativeSceneData data)
        {
            data.OpeningLines ??= new List<string>();
            data.StoryBlocks ??= new List<StoryBlock>();
            data.SecretQuestion ??= new SecretQuestion { Options = new List<string>() };
        }

        /// <summary>
        /// Normalizes the initial choice data.
        /// </summary>
        private static void NormalizeInitialChoice(NarrativeSceneData data, out IReadOnlyList<DreamweaverChoice> threadChoices)
        {
            if (data.InitialChoice?.Options != null && data.InitialChoice.Options.Count > 0)
            {
                foreach (DreamweaverChoice option in data.InitialChoice.Options)
                {
                    if (string.IsNullOrWhiteSpace(option.Id) ||
                        !Enum.TryParse(option.Id, true, out DreamweaverThread parsedThread))
                    {
                        parsedThread = DreamweaverThread.Hero;
                    }

                    option.Thread = parsedThread;
                    option.Text ??= option.Id;
                    option.Description ??= string.Empty;
                }

                threadChoices = new ReadOnlyCollection<DreamweaverChoice>(
                    new List<DreamweaverChoice>(data.InitialChoice.Options));
            }
            else
            {
                threadChoices = Array.Empty<DreamweaverChoice>();
            }
        }

        /// <summary>
        /// Normalizes story block data.
        /// </summary>
        private static void NormalizeStoryBlocks(NarrativeSceneData data)
        {
            foreach (StoryBlock block in data.StoryBlocks)
            {
                block.Paragraphs ??= new List<string>();
                block.Choices ??= new List<ChoiceOption>();
            }
        }
    }
}
