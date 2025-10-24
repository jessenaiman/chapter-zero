// <copyright file="UiTestHarnessScreenshotTests.cs" company="Omega Spiral">
// Copyright (c) Omega Spiral. All rights reserved.
// </copyright>

namespace OmegaSpiral.Tests.Ui.TestHarness;

using Godot;
using GdUnit4;
using GdUnit4.Api;
using static GdUnit4.Assertions;

/// <summary>
/// Verifies that the Ui test harness can generate a baseline screenshot for the terminal layout shell.
/// SHOULD FAIL until UiTestHarness implements automated screenshot capture for tests.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public partial class UiTestHarnessScreenshotTests : Node
{
    private const string HarnessScenePath = "res://source/ui/test_harness/ui_test_harness.tscn";
    private const string ScreenshotName = "UiTestHarness_TerminalLayoutShell";

    /// <summary>
    /// The harness should render the terminal layout shell and persist a screenshot to the test results directory.
    /// </summary>
    [TestCase]
    public async Task UiTestHarness_CaptureScreenshot_SavesImage()
    {
        using ISceneRunner runner = ISceneRunner.Load(HarnessScenePath);
        await runner.SimulateFrames(2).ConfigureAwait(false);

        var harnessPanel = runner.Scene() as UiTestHarness;
        AssertThat(harnessPanel).IsNotNull();
        var harness = harnessPanel!;

        harness.CaptureScreenshotForTest(ScreenshotName);

        var screenshotPath = $"user://ui_test_baselines/{ScreenshotName}.png";
        AssertBool(FileAccess.FileExists(screenshotPath))
            .OverrideFailureMessage($"Expected screenshot at {screenshotPath} to exist after capture.")
            .IsTrue();
    }
}
