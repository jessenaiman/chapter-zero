using System;

namespace OmegaSpiral.Source.Scripts.Domain.Dungeon
{
    /// <summary>
    /// Represents domain validation failures that occur when constructing dungeon aggregates or value objects.
    /// </summary>
    [Serializable]
    public class DungeonValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DungeonValidationException"/> class.
        /// </summary>
        public DungeonValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DungeonValidationException"/> class with a message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public DungeonValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DungeonValidationException"/> class with a message and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DungeonValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
