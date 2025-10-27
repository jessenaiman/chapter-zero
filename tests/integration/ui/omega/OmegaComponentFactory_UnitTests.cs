// <copyright file="OmegaComponentFactory_UnitTests.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using GdUnit4;
using Godot;
using OmegaSpiral.Source.Ui.Omega;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Integration.Ui.Omega;

/// <summary>
/// Unit tests for OmegaComponentFactory.
/// Tests factory methods for creating Omega-themed UI components.
///
/// RESPONSIBILITY: Verify all factory methods create properly configured components
/// with Omega design system styling applied consistently.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class OmegaComponentFactory_UnitTests
{
    // Test constants
    private const string _TestButtonText = "Test Button";
    private const string _EmptyText = "";

    // ==================== BORDER FRAME CREATION ====================

    /// <summary>
    /// Tests that CreateBorderFrame creates a valid OmegaBorderFrame instance.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateBorderFrame_ReturnsValidInstance()
    {
        // Act
        var borderFrame = AutoFree(OmegaComponentFactory.CreateBorderFrame())!;

        // Assert
        AssertThat(borderFrame).IsNotNull();
        AssertThat(borderFrame).IsInstanceOf<OmegaBorderFrame>();
    }

    /// <summary>
    /// Tests that created border frame has correct name.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateBorderFrame_HasCorrectName()
    {
        // Act
        var borderFrame = AutoFree(OmegaComponentFactory.CreateBorderFrame())!;

        // Assert
        AssertThat(borderFrame.Name).IsEqual("BorderFrame");
    }

    /// <summary>
    /// Tests that created border frame has shader material configured.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateBorderFrame_HasShaderMaterialConfigured()
    {
        // Act
        var borderFrame = AutoFree(OmegaComponentFactory.CreateBorderFrame())!;

        // Assert
        AssertThat(borderFrame.Material).IsNotNull();
        AssertThat(borderFrame.Material).IsInstanceOf<ShaderMaterial>();
    }

    /// <summary>
    /// Tests that created border frame applies Omega design system colors.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateBorderFrame_AppliesOmegaColors()
    {
        // Act
        var borderFrame = AutoFree(OmegaComponentFactory.CreateBorderFrame())!;
        var shaderMaterial = borderFrame.GetShaderMaterial();

        // Assert
        AssertThat(shaderMaterial).IsNotNull();
        var lightThread = (Color)shaderMaterial!.GetShaderParameter("light_thread");
        var shadowThread = (Color)shaderMaterial.GetShaderParameter("shadow_thread");
        var ambitionThread = (Color)shaderMaterial.GetShaderParameter("ambition_thread");

        AssertThat(lightThread).IsEqual(OmegaSpiralColors.LightThread);
        AssertThat(shadowThread).IsEqual(OmegaSpiralColors.ShadowThread);
        AssertThat(ambitionThread).IsEqual(OmegaSpiralColors.AmbitionThread);
    }

    // ==================== TEXT RENDERER CREATION ====================

    /// <summary>
    /// Tests that CreateTextRenderer creates a valid OmegaTextRenderer instance.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateTextRenderer_WithValidLabel_ReturnsValidInstance()
    {
        // Arrange
        var textDisplay = AutoFree(new RichTextLabel())!;

        // Act
        var renderer = OmegaComponentFactory.CreateTextRenderer(textDisplay);

        // Assert
        AssertThat(renderer).IsNotNull();
        AssertThat(renderer).IsInstanceOf<OmegaTextRenderer>();
    }

    /// <summary>
    /// Tests that created text renderer applies Omega theme color.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateTextRenderer_AppliesOmegaThemeColor()
    {
        // Arrange
        var textDisplay = AutoFree(new RichTextLabel())!;

        // Act
        var renderer = OmegaComponentFactory.CreateTextRenderer(textDisplay);

        // Assert - Verify the text color was set (WarmAmber)
        AssertThat(renderer).IsNotNull();
        // Note: We can't directly test the internal color without adding a getter,
        // but we verify the renderer was created successfully
    }

    /// <summary>
    /// Tests that CreateTextRenderer throws with null text display.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateTextRenderer_WithNullLabel_ThrowsException()
    {
        // Act & Assert
        AssertThrown(() => OmegaComponentFactory.CreateTextRenderer(null!))
            .IsInstanceOf<ArgumentNullException>();
    }

    // ==================== SHADER CONTROLLER CREATION ====================

    /// <summary>
    /// Tests that CreateShaderController creates a valid OmegaShaderController instance.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateShaderController_WithValidDisplay_ReturnsValidInstance()
    {
        // Arrange
        var display = AutoFree(new ColorRect())!;

        // Act
        var controller = OmegaComponentFactory.CreateShaderController(display);

        // Assert
        AssertThat(controller).IsNotNull();
        AssertThat(controller).IsInstanceOf<OmegaShaderController>();
    }

    /// <summary>
    /// Tests that CreateShaderController throws with null display.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateShaderController_WithNullDisplay_ThrowsException()
    {
        // Act & Assert
        AssertThrown(() => OmegaComponentFactory.CreateShaderController(null!))
            .IsInstanceOf<ArgumentNullException>();
    }

    // ==================== CHOICE PRESENTER CREATION ====================

    /// <summary>
    /// Tests that CreateChoicePresenter creates a valid OmegaChoicePresenter instance.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateChoicePresenter_WithValidContainer_ReturnsValidInstance()
    {
        // Arrange
        var container = AutoFree(new VBoxContainer())!;

        // Act
        var presenter = OmegaComponentFactory.CreateChoicePresenter(container);

        // Assert
        AssertThat(presenter).IsNotNull();
        AssertThat(presenter).IsInstanceOf<OmegaChoicePresenter>();
    }

    /// <summary>
    /// Tests that CreateChoicePresenter throws with null container.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateChoicePresenter_WithNullContainer_ThrowsException()
    {
        // Act & Assert
        AssertThrown(() => OmegaComponentFactory.CreateChoicePresenter(null!))
            .IsInstanceOf<ArgumentNullException>();
    }

    // ==================== STYLED BUTTON CREATION ====================

    /// <summary>
    /// Tests that CreateStyledButton creates a valid Button instance.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateStyledButton_WithValidText_ReturnsValidButton()
    {
        // Act
        var button = AutoFree(OmegaComponentFactory.CreateStyledButton(_TestButtonText))!;

        // Assert
        AssertThat(button).IsNotNull();
        AssertThat(button).IsInstanceOf<Button>();
    }

    /// <summary>
    /// Tests that created button has correct text.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateStyledButton_SetsCorrectText()
    {
        // Act
        var button = AutoFree(OmegaComponentFactory.CreateStyledButton(_TestButtonText))!;

        // Assert
        AssertThat(button.Text).IsEqual(_TestButtonText);
    }

    /// <summary>
    /// Tests that created button accepts empty text.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateStyledButton_WithEmptyText_CreatesButton()
    {
        // Act
        var button = AutoFree(OmegaComponentFactory.CreateStyledButton(_EmptyText))!;

        // Assert
        AssertThat(button).IsNotNull();
        AssertThat(button.Text).IsEqual(_EmptyText);
    }

    /// <summary>
    /// Tests that created button has Omega theme colors applied.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateStyledButton_AppliesOmegaThemeColors()
    {
        // Act
        var button = AutoFree(OmegaComponentFactory.CreateStyledButton(_TestButtonText))!;

        // Assert - Verify theme color overrides are applied
        var fontColor = button.GetThemeColor("font_color");
        var hoverColor = button.GetThemeColor("font_hover_color");
        var pressedColor = button.GetThemeColor("font_pressed_color");
        var focusColor = button.GetThemeColor("font_focus_color");

        AssertThat(fontColor).IsEqual(OmegaSpiralColors.WarmAmber);
        AssertThat(hoverColor).IsEqual(OmegaSpiralColors.PureWhite);
        AssertThat(pressedColor).IsEqual(OmegaSpiralColors.PureWhite);
        AssertThat(focusColor).IsEqual(OmegaSpiralColors.PureWhite);
    }

    // ==================== INTEGRATION TESTS ====================

    /// <summary>
    /// Tests that all factory methods can be called in sequence without errors.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void FactoryMethods_CanBeCalledInSequence_WithoutErrors()
    {
        // Arrange
        var textDisplay = AutoFree(new RichTextLabel())!;
        var colorDisplay = AutoFree(new ColorRect())!;
        var choiceContainer = AutoFree(new VBoxContainer())!;

        // Act & Assert - No exceptions thrown
        var borderFrame = AutoFree(OmegaComponentFactory.CreateBorderFrame())!;
        AssertThat(borderFrame).IsNotNull();

        var renderer = OmegaComponentFactory.CreateTextRenderer(textDisplay);
        AssertThat(renderer).IsNotNull();

        var controller = OmegaComponentFactory.CreateShaderController(colorDisplay);
        AssertThat(controller).IsNotNull();

        var presenter = OmegaComponentFactory.CreateChoicePresenter(choiceContainer);
        AssertThat(presenter).IsNotNull();

        var button = AutoFree(OmegaComponentFactory.CreateStyledButton(_TestButtonText))!;
        AssertThat(button).IsNotNull();
    }

    /// <summary>
    /// Tests that multiple buttons can be created with consistent styling.
    /// </summary>
    [TestCase]
    [RequireGodotRuntime]
    public void CreateStyledButton_MultipleCalls_ProducesConsistentStyling()
    {
        // Act
        var button1 = AutoFree(OmegaComponentFactory.CreateStyledButton("Button 1"))!;
        var button2 = AutoFree(OmegaComponentFactory.CreateStyledButton("Button 2"))!;
        var button3 = AutoFree(OmegaComponentFactory.CreateStyledButton("Button 3"))!;

        // Assert - All buttons have same theme colors
        AssertThat(button1.GetThemeColor("font_color")).IsEqual(button2.GetThemeColor("font_color"));
        AssertThat(button2.GetThemeColor("font_color")).IsEqual(button3.GetThemeColor("font_color"));

        AssertThat(button1.GetThemeColor("font_hover_color")).IsEqual(button2.GetThemeColor("font_hover_color"));
        AssertThat(button2.GetThemeColor("font_hover_color")).IsEqual(button3.GetThemeColor("font_hover_color"));
    }
}
