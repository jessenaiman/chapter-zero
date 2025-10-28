using Godot;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Lightweight autoload shim for SceneManager used by the project autoloads.
    /// Ensures tests and runtime can instantiate this autoload without requiring the
    /// full implementation.
    /// </summary>
    public partial class SceneManager : Node
    {
        // Minimal placeholder
    }
}
