using Godot;
using GdUnit4;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.UI;

[TestSuite]
[RequireGodotRuntime]
public partial class MenuTests
{
    [TestCase(Timeout = 2000)]
    public void MenuSceneLoadsWithoutHanging()
    {
        // Load scene inline - auto-freed by GdUnit API
        ISceneRunner runner = ISceneRunner.Load("res://source/scenes/menus/main_menu/main_menu_with_animations.tscn");

        // Verify scene loaded
        AssertThat(runner.Scene()).IsNotNull();

        // NO Dispose() - runner is auto-freed
    }
}
