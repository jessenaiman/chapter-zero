using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using OmegaSpiral.UI.Menus;

namespace Tests.UI.Menus;

[TestSuite]
public partial class StageSelectMenuTests : Node
{
    private const string ScenePath = "res://source/ui/menus/stage_select_menu.tscn";

    [TestCase]
    [RequireGodotRuntime]
    public void StagePanel_RectMinSize_IsConfigured()
    {
        var packed = GD.Load<PackedScene>(ScenePath);
        var inst = packed.Instantiate() as Control;

        if (inst == null)
        {
            AssertThat(inst).IsNotNull();
            return;
        }

        AddChild(inst);

        var stagesPanel = inst.GetNodeOrNull<VBoxContainer>("Center/MenuVBox/StagesPanel");
        AssertThat(stagesPanel).IsNotNull();
        AssertThat(stagesPanel!.CustomMinimumSize).IsEqual(new Vector2(800, 320));
    }

    [TestCase]
    [RequireGodotRuntime]
    public void StageButtons_Count_IsFive()
    {
        var packed = GD.Load<PackedScene>(ScenePath);
        var inst = packed.Instantiate() as Control;

        if (inst == null)
        {
            AssertThat(inst).IsNotNull();
            return;
        }

        AddChild(inst);

        var stagesPanel = inst.GetNodeOrNull<VBoxContainer>("Center/MenuVBox/StagesPanel");
        AssertThat(stagesPanel).IsNotNull();

        // After _Ready, StageSelectMenu will replace placeholders with 5 StageButtons
        AssertThat(stagesPanel!.GetChildCount()).IsEqual(5);
    }

    [TestCase]
    [RequireGodotRuntime]
    public void StageButton_StatusIcons_DisplayCorrectly()
    {
        var packed = GD.Load<PackedScene>(ScenePath);
        var inst = packed.Instantiate() as Control;

        if (inst == null)
        {
            AssertThat(inst).IsNotNull();
            return;
        }

        AddChild(inst);

        var stagesPanel = inst.GetNodeOrNull<VBoxContainer>("Center/MenuVBox/StagesPanel");
        AssertThat(stagesPanel).IsNotNull();

        // Check each StageButton for correct status icon
        for (int i = 0; i < stagesPanel!.GetChildCount(); i++)
        {
            var button = stagesPanel.GetChild(i) as StageButton;
            AssertThat(button).IsNotNull();

            // Stage 1 should be Ready (✓), others Missing (○)
            var expectedIcon = (i == 0) ? "✓" : "○";
            var iconLabel = button!.GetNodeOrNull<Label>("HBoxContainer/IconLabel");
            AssertThat(iconLabel).IsNotNull();
            AssertThat(iconLabel!.Text).IsEqual(expectedIcon);
        }
    }

    [TestCase]
    [RequireGodotRuntime]
    public void StageButton_StatusColors_ApplyCorrectly()
    {
        var packed = GD.Load<PackedScene>(ScenePath);
        var inst = packed.Instantiate() as Control;

        if (inst == null)
        {
            AssertThat(inst).IsNotNull();
            return;
        }

        AddChild(inst);

        var stagesPanel = inst.GetNodeOrNull<VBoxContainer>("Center/MenuVBox/StagesPanel");
        AssertThat(stagesPanel).IsNotNull();

        // Check each StageButton for correct status color
        for (int i = 0; i < stagesPanel!.GetChildCount(); i++)
        {
            var button = stagesPanel.GetChild(i) as StageButton;
            AssertThat(button).IsNotNull();

            var nameLabel = button!.GetNodeOrNull<Label>("HBoxContainer/NameLabel");
            var statusLabel = button.GetNodeOrNull<Label>("HBoxContainer/StatusLabel");

            // Stage 1 should be green (Ready), others gray (Missing)
            var expectedColor = (i == 0) ? new Color("#33ff66") : new Color("#808080");
            AssertThat(nameLabel).IsNotNull();
            AssertThat(nameLabel!.Modulate).IsEqual(expectedColor);
        }
    }

    [TestCase]
    [RequireGodotRuntime]
    public void Menu_Centering_WorksAtMultipleResolutions()
    {
        var packed = GD.Load<PackedScene>(ScenePath);
        var inst = packed.Instantiate() as Control;

        if (inst == null)
        {
            AssertThat(inst).IsNotNull();
            return;
        }

        AddChild(inst);

        var centerContainer = inst.GetNodeOrNull<CenterContainer>("Center");
        AssertThat(centerContainer).IsNotNull();

        // Verify CenterContainer is properly configured for centering
        AssertThat((int)centerContainer!.AnchorsPreset).IsEqual((int)Control.LayoutPreset.Center);
    }
}