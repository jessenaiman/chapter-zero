namespace OmegaSpiral.Source.Scripts.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a character's skills and proficiencies.
    /// </summary>
    public class Skills
    {
        /// <summary>
        /// Gets or sets the list of individual skills.
        /// </summary>
        public List<Skill> SkillList { get; set; } = new List<Skill>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Skills"/> class.
        /// </summary>
        public Skills()
        {
        }

        /// <summary>
        /// Gets a skill by name.
        /// </summary>
        /// <param name="skillName">The name of the skill to get.</param>
        /// <returns>The skill if found; otherwise, <see langword="null"/>.</returns>
        public Skill? GetSkill(string skillName)
        {
            return SkillList.Find(s => s.Name == skillName);
        }

        /// <summary>
        /// Adds a skill to the list.
        /// </summary>
        /// <param name="skill">The skill to add.</param>
        public void AddSkill(Skill skill)
        {
            if (skill != null && !SkillList.Contains(skill))
            {
                SkillList.Add(skill);
            }
        }

        /// <summary>
        /// Removes a skill from the list.
        /// </summary>
        /// <param name="skill">The skill to remove.</param>
        /// <returns><see langword="true"/> if the skill was removed; otherwise, <see langword="false"/>.</returns>
        public bool RemoveSkill(Skill skill)
        {
            return SkillList.Remove(skill);
        }
    }
}
