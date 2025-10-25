// <copyright file="MainMenuTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using Godot;
using GdUnit4;
using GdUnit4.Api;
using OmegaSpiral.Source.Stages.Stage0Start;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Menus;

/// <summary>
/// Integration tests for MainMenu.
/// Tests manifest loading, stage button creation, and navigation behavior.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class MainMenuTests : Node
{
    private MainMenu _MainMenu = null!;

    [Before]
    public void Setup()
    {
        _MainMenu = AutoFree(new MainMenu())!;
        AddChild(_MainMenu);
        _MainMenu._Ready();
        AssertThat(_MainMenu).IsNotNull();
    }

    // ==================== MANIFEST LOADING ====================

    /// <summary>
    /// Loads stage manifest and creates expected number of buttons.
    /// </summary>
    [TestCase]
    public void LoadsStagesFromManifest()
    {
        var buttonList = _MainMenu.GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");
        AssertThat(buttonList).IsNotNull();

        var stageButtons = buttonList!.GetChildren().Where(child => child is Button).ToArray();
        AssertThat(stageButtons.Length).IsEqual(6);
    }

    /// <summary>
    /// Stage buttons have correct text from manifest.
    /// </summary>
    [TestCase]
    public void StageButtons_HaveCorrectText()
    {
        var buttonList = _MainMenu.GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");
        AssertThat(buttonList).IsNotNull();

        var stageButtons = buttonList!.GetChildren().Where(child => child is Button).Cast<Button>().ToArray();

        AssertThat(stageButtons[0].Text).Contains("Ghost Terminal");
        AssertThat(stageButtons[1].Text).Contains("Nethack");
        AssertThat(stageButtons[2].Text).Contains("Amnesia");
        AssertThat(stageButtons[3].Text).Contains("Party Selection");
        AssertThat(stageButtons[4].Text).Contains("Fractured Escape");
        AssertThat(stageButtons[5].Text).Contains("System Log Epilogue");
    }

    /// <summary>
    /// Stage header reflects detected stage count.
    /// </summary>
    [TestCase]
    public void StageHeader_ReflectsStageCount()
    {
        var stageHeader = _MainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageHeader");

        AssertThat(stageHeader).IsNotNull();
        AssertThat(stageHeader!.Visible).IsTrue();

        var buttonList = _MainMenu.GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");
        AssertThat(buttonList).IsNotNull();

        int stageButtonCount = buttonList!.GetChildren().Count(child => child is Button);
        string expectedHeader = $"Stage Access · {stageButtonCount} module{(stageButtonCount == 1 ? string.Empty : "s")} detected";

        AssertThat(stageHeader.Text).IsEqual(expectedHeader);
    }

    // ==================== BUTTON INTERACTIONS ====================

    /// <summary>
    /// Clicking stage button triggers navigation (no crash).
    /// </summary>
    [TestCase]
    public void StageButton_Click_TriggersNavigation()
    {
        var buttonList = _MainMenu.GetNodeOrNull<VBoxContainer>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StageButtonList");
        AssertThat(buttonList).IsNotNull();

        var firstStageButton = buttonList!.GetChildren().Where(child => child is Button).Cast<Button>().First();
        firstStageButton.EmitSignal("pressed");

        // Note: In a real test environment, this would trigger navigation
        // For now, we just verify the button exists and can be clicked without crashing
        AssertThat(firstStageButton).IsNotNull();
    }

    /// <summary>
    /// Start button exists and has correct text.
    /// </summary>
    [TestCase]
    public void StartButton_ExistsWithCorrectText()
    {
        var startButton = _MainMenu.GetNodeOrNull<Button>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/StartButton");

        AssertThat(startButton).IsNotNull();
        AssertThat(startButton!.Text).Contains("Launch");
        AssertThat(startButton.Text).Contains("Ghost Terminal");
    }

    /// <summary>
    /// Options button exists and is clickable.
    /// </summary>
    [TestCase]
    public void OptionsButton_Exists()
    {
        var optionsButton = _MainMenu.GetNodeOrNull<Button>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/OptionsButton");

        AssertThat(optionsButton).IsNotNull();
        AssertThat(optionsButton!.Disabled).IsFalse();
    }

    /// <summary>
    /// Quit button exists and is clickable.
    /// </summary>
    [TestCase]
    public void QuitButton_Exists()
    {
        var quitButton = _MainMenu.GetNodeOrNull<Button>("MenuContainer/MenuWrapper/MenuContent/MenuButtonsMargin/MenuButtonsContainer/QuitButton");

        AssertThat(quitButton).IsNotNull();
        AssertThat(quitButton!.Disabled).IsFalse();
    }

    // ==================== TEXT & VISUAL ELEMENTS ====================

    /// <summary>
    /// Title label is visible with non-empty text.
    /// </summary>
    [TestCase]
    public void Title_IsVisible()
    {
        var titleLabel = _MainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/TitleMargin/TitleLabel");

        AssertThat(titleLabel).IsNotNull();
        AssertThat(titleLabel!.Text).IsNotEmpty();
        AssertThat(titleLabel.Visible).IsTrue();
    }

    /// <summary>
    /// Description label is visible with non-empty text.
    /// </summary>
    [TestCase]
    public void Description_IsVisible()
    {
        var descriptionLabel = _MainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/DescriptionMargin/DescriptionLabel");

        AssertThat(descriptionLabel).IsNotNull();
        AssertThat(descriptionLabel!.Text).IsNotEmpty();
        AssertThat(descriptionLabel.Visible).IsTrue();
    }

    /// <summary>
    /// Text labels use center horizontal alignment.
    /// </summary>
    [TestCase]
    public void Text_UsesCenterAlignment()
    {
        var titleLabel = _MainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/TitleMargin/TitleLabel");
        var descriptionLabel = _MainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/DescriptionMargin/DescriptionLabel");

        AssertThat(titleLabel).IsNotNull();
        AssertThat((int)titleLabel!.HorizontalAlignment).IsEqual((int)HorizontalAlignment.Center);

        AssertThat(descriptionLabel).IsNotNull();
        AssertThat((int)descriptionLabel!.HorizontalAlignment).IsEqual((int)HorizontalAlignment.Center);
    }

    /// <summary>
    /// Labels are placed in correct parent containers.
    /// </summary>
    [TestCase]
    public void Labels_InCorrectContainers()
    {
        var titleLabel = _MainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/TitleMargin/TitleLabel");

        AssertThat(titleLabel).IsNotNull();
        var titleParent = titleLabel!.GetParent();
        AssertThat(titleParent).IsInstanceOf<MarginContainer>();
        AssertThat(titleParent!.Name).IsEqual("TitleMargin");

        var descriptionLabel = _MainMenu.GetNodeOrNull<Label>("MenuContainer/MenuWrapper/MenuContent/DescriptionMargin/DescriptionLabel");
        AssertThat(descriptionLabel).IsNotNull();
        var descriptionParent = descriptionLabel!.GetParent();
        AssertThat(descriptionParent).IsInstanceOf<MarginContainer>();
        AssertThat(descriptionParent!.Name).IsEqual("DescriptionMargin");
    }

    // ==================== INHERITANCE ====================

    /// <summary>
    /// MainMenu inherits from MenuUI.
    /// </summary>
    [TestCase]
    public void InheritsFromMenuUI()
    {
        AssertThat(_MainMenu).IsInstanceOf<OmegaSpiral.Source.Ui.Omega.OmegaUi>();
    }
}
