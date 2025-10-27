using Godot;
using System;

namespace OmegaSpiral.Source.Ui.Omega
{
    /// <summary>
    /// Background component for Omega UI with design system integration.
    /// Manages the background ColorRect that sets the overall UI tone.
    ///
    /// RESPONSIBILITY: Configure background color from design system.
    /// Assumes Background node already exists in scene (defined in .tscn files).
    ///
    /// Design System Integration:
    /// - Reads background color from OmegaSpiralColors (JSON config)
    /// - Applies color to the Background ColorRect node
    /// - Color is inherited by all subclasses via OmegaUi
    /// </summary>
    [GlobalClass]
    public partial class OmegaBackground : Node
    {
        /// <summary>
        /// Configures the background ColorRect with design system color.
        /// Called by OmegaUi during _Ready() to apply color from config.
        /// </summary>
        /// <param name="backgroundNode">The Background ColorRect from the scene.</param>
        /// <exception cref="ArgumentNullException">Thrown if backgroundNode is null.</exception>
        public static void ConfigureBackground(ColorRect backgroundNode)
        {
            if (backgroundNode == null)
            {
                throw new ArgumentNullException(nameof(backgroundNode), "Background node cannot be null");
            }

            try
            {
                // Apply design system color from JSON config
                backgroundNode.Color = OmegaSpiralColors.DeepSpace;
            }
            catch (Exception ex)
            {
                GD.PushError($"[OmegaBackground] Failed to configure background: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the current background color from a background node.
        /// </summary>
        /// <param name="backgroundNode">The Background ColorRect.</param>
        /// <returns>Current color value.</returns>
        public static Color GetBackgroundColor(ColorRect? backgroundNode)
        {
            return backgroundNode?.Color ?? OmegaSpiralColors.DeepSpace;
        }

        /// <summary>
        /// Resets the background color to the design system default (DeepSpace).
        /// </summary>
        /// <param name="backgroundNode">The Background ColorRect.</param>
        public static void ResetToDesignSystemColor(ColorRect? backgroundNode)
        {
            if (backgroundNode != null)
            {
                backgroundNode.Color = OmegaSpiralColors.DeepSpace;
            }
        }
    }
}
