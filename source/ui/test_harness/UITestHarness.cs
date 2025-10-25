using System;
using Godot;

/// <summary>
/// Script for Ui Test Harness. Press F12 to save a screenshot of the current viewport.
/// </summary>
[GlobalClass]
public partial class UiTestHarness : Panel
{
    private const string _DefaultHotkeyScreenshotName = "ui_test_harness_screenshot";
    private const string _ScreenshotDirectory = "user://ui_test_baselines";

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.F12)
        {
            CaptureScreenshotForTest(_DefaultHotkeyScreenshotName);
        }
    }

    /// <summary>
    /// Captures a screenshot of the harness viewport and saves it under <c>user://ui_test_baselines/&lt;screenshotName&gt;.png</c>.
    /// </summary>
    /// <param name="screenshotName">The file stem to use when saving the screenshot.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="screenshotName"/> is null, empty, or whitespace.</exception>
    public void CaptureScreenshotForTest(string screenshotName)
    {
        if (string.IsNullOrWhiteSpace(screenshotName))
        {
            throw new ArgumentException("Screenshot name must be provided.", nameof(screenshotName));
        }

        var viewport = GetViewport();
        if (viewport == null)
        {
            GD.PrintErr("[UiTestHarness] Unable to capture screenshot: viewport is null.");
            return;
        }

        var texture = viewport.GetTexture();
        if (texture == null)
        {
            GD.PrintErr("[UiTestHarness] Unable to capture screenshot: viewport texture is null.");
            return;
        }

        var image = texture.GetImage();
        if (image == null)
        {
            GD.PrintErr("[UiTestHarness] Unable to capture screenshot: could not obtain image from texture.");
            return;
        }

        var directoryError = DirAccess.MakeDirRecursiveAbsolute(ProjectSettings.GlobalizePath(_ScreenshotDirectory));
        if (directoryError != Error.Ok && directoryError != Error.AlreadyExists)
        {
            GD.PrintErr($"[UiTestHarness] Failed to ensure screenshot directory {_ScreenshotDirectory}: {directoryError}");
            return;
        }

        image.FlipY();

        var screenshotPath = $"{_ScreenshotDirectory}/{screenshotName}.png";
        var saveError = image.SavePng(screenshotPath);
        if (saveError != Error.Ok)
        {
            GD.PrintErr($"[UiTestHarness] Failed to save screenshot to {screenshotPath}: {saveError}");
            return;
        }

        GD.Print($"[UiTestHarness] Screenshot saved to {screenshotPath}");
    }
}
