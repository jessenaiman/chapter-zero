using GdUnit4;
using OmegaSpiral.Source.Scripts.Infrastructure;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Unit.Services;

/// <summary>
/// Tests for GameAppConfig UI layout configuration loading from JSON.
/// Verifies that source/resources/ui_layout_config.json is properly parsed and cached.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public class GameAppConfigTests
{
    private GameAppConfig? _AppConfig;

    [Before]
    public void Setup()
    {
        _AppConfig = AutoFree(new GameAppConfig());
        if (_AppConfig != null)
        {
            _AppConfig.CallDeferred(Godot.Node.MethodName._Ready);
        }
    }

    [After]
    public void Teardown()
    {
        _AppConfig?.QueueFree();
    }

    [TestCase]
    public void TestUiLayoutConfigLoads()
    {
        AssertThat(_AppConfig).IsNotNull();
        // Give the config time to load
        AssertThat(_AppConfig!.BezelMargin).IsGreater(0.0f);
    }

    [TestCase]
    public void TestBezelMarginLoaded()
    {
        AssertThat(_AppConfig!.BezelMargin).IsEqual(0.05f);
    }

    [TestCase]
    public void TestStageNameWidthRatioLoaded()
    {
        AssertThat(_AppConfig!.StageNameWidthRatio).IsEqual(0.15f);
    }

    [TestCase]
    public void TestStatusLabelWidthRatioLoaded()
    {
        AssertThat(_AppConfig!.StatusLabelWidthRatio).IsEqual(0.15f);
    }

    [TestCase]
    public void TestButtonHeightLoaded()
    {
        AssertThat(_AppConfig!.ButtonHeight).IsEqual(80);
    }

    [TestCase]
    public void TestButtonPaddingHLoaded()
    {
        AssertThat(_AppConfig!.ButtonPaddingH).IsEqual(16);
    }

    [TestCase]
    public void TestButtonPaddingVLoaded()
    {
        AssertThat(_AppConfig!.ButtonPaddingV).IsEqual(8);
    }

    [TestCase]
    public void TestButtonSpacingLoaded()
    {
        AssertThat(_AppConfig!.ButtonSpacing).IsEqual(16);
    }

    [TestCase]
    public void TestAllConfigValuesArePositive()
    {
        AssertThat(_AppConfig!.BezelMargin).IsGreater(0.0f);
        AssertThat(_AppConfig!.StageNameWidthRatio).IsGreater(0.0f);
        AssertThat(_AppConfig!.StatusLabelWidthRatio).IsGreater(0.0f);
        AssertThat(_AppConfig!.ButtonHeight).IsGreater(0);
        AssertThat(_AppConfig!.ButtonPaddingH).IsGreaterEqual(0);
        AssertThat(_AppConfig!.ButtonPaddingV).IsGreaterEqual(0);
        AssertThat(_AppConfig!.ButtonSpacing).IsGreater(0);
    }
}
