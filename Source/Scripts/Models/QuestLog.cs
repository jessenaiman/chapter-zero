namespace OmegaSpiral.Source.Scripts.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Manages a collection of quests for tracking quest progress.
    /// </summary>
    public class QuestLog
    {
        /// <summary>
        /// Quest sort types for sorting quest lists.
        /// </summary>
        public enum SortType
        {
            /// <summary>Sort by quest name.</summary>
            Name,

            /// <summary>Sort by quest level.</summary>
            Level,

            /// <summary>Sort by quest status.</summary>
            Status,

            /// <summary>Sort by date added.</summary>
            DateAdded,
        }

        /// <summary>
        /// Quest filter types for filtering quest lists.
        /// </summary>
        public enum FilterType
        {
            /// <summary>Show all quests.</summary>
            All,

            /// <summary>Show only active quests.</summary>
            Active,

            /// <summary>Show only completed quests.</summary>
            Completed,

            /// <summary>Show only failed quests.</summary>
            Failed,
        }

        /// <summary>
        /// Gets or sets the list of all quests.
        /// </summary>
        public List<Quest> Quests { get; set; } = new List<Quest>();

        /// <summary>
        /// Gets or sets the list of active quests.
        /// </summary>
        public List<Quest> ActiveQuests => Quests.Where(q => q.Status == "Active").ToList();

        /// <summary>
        /// Gets or sets the list of completed quests.
        /// </summary>
        public List<Quest> CompletedQuests => Quests.Where(q => q.Status == "Completed").ToList();

        /// <summary>
        /// Gets or sets the list of failed quests.
        /// </summary>
        public List<Quest> FailedQuests => Quests.Where(q => q.Status == "Failed").ToList();

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestLog"/> class.
        /// </summary>
        public QuestLog()
        {
        }

        /// <summary>
        /// Adds a quest to the log.
        /// </summary>
        /// <param name="quest">The quest to add.</param>
        public void AddQuest(Quest quest)
        {
            if (quest != null && !Quests.Contains(quest))
            {
                Quests.Add(quest);
            }
        }

        /// <summary>
        /// Removes a quest from the log.
        /// </summary>
        /// <param name="quest">The quest to remove.</param>
        /// <returns><see langword="true"/> if the quest was removed; otherwise, <see langword="false"/>.</returns>
        public bool RemoveQuest(Quest quest)
        {
            return Quests.Remove(quest);
        }

        /// <summary>
        /// Gets a quest by ID.
        /// </summary>
        /// <param name="questId">The ID of the quest to get.</param>
        /// <returns>The quest if found; otherwise, <see langword="null"/>.</returns>
        public Quest? GetQuestById(string questId)
        {
            return Quests.FirstOrDefault(q => q.Id == questId);
        }

        /// <summary>
        /// Marks a quest as completed.
        /// </summary>
        /// <param name="questId">The ID of the quest to complete.</param>
        /// <returns><see langword="true"/> if the quest was completed; otherwise, <see langword="false"/>.</returns>
        public bool CompleteQuest(string questId)
        {
            var quest = GetQuestById(questId);
            if (quest != null)
            {
                quest.Status = "Completed";
                return true;
            }

            return false;
        }

        /// <summary>
        /// Marks a quest as failed.
        /// </summary>
        /// <param name="questId">The ID of the quest to fail.</param>
        /// <returns><see langword="true"/> if the quest was failed; otherwise, <see langword="false"/>.</returns>
        public bool FailQuest(string questId)
        {
            var quest = GetQuestById(questId);
            if (quest != null)
            {
                quest.Status = "Failed";
                return true;
            }

            return false;
        }
    }
}
