using Godot;
using GdUnit4;
using static GdUnit4.Assertions;

/// <summary>
/// Helper for taking screenshots in GdUnit4 Ui tests.
/// </summary>
public static class BackgroundComponentScreenshotHelper
{
    /// <summary>
    /// Takes a screenshot for a Ui test and always overwrites the file for that test.
    /// The screenshot file is named uniquely per test and always reflects the latest run.
    /// </summary>
    public static void TakeScreenshot(Node root, string testName)
    {
        if (root == null)
        {
            AssertThat(root).OverrideFailureMessage("Cannot take screenshot: root node is null").IsNotNull();
            return;
        }

        var viewport = root.GetViewport();
        if (viewport == null)
        {
            AssertThat(viewport).OverrideFailureMessage("Cannot take screenshot: viewport is null (node not in scene tree?)").IsNotNull();
            return;
        }

        var texture = viewport.GetTexture();
        if (texture == null)
        {
            AssertThat(texture).OverrideFailureMessage("Cannot take screenshot: viewport texture is null").IsNotNull();
            return;
        }

        var img = texture.GetImage();
        if (img == null)
        {
            AssertThat(img).OverrideFailureMessage("Cannot take screenshot: could not get image from texture").IsNotNull();
            return;
        }

        img.FlipY();
        var path = $"res://TestResults/ui_screenshots/{testName}.png";
        var error = img.SavePng(path);
        if (error != Error.Ok)
        {
            AssertThat(error).OverrideFailureMessage($"Failed to save screenshot to {path}: {error}").IsEqual(Error.Ok);
            return;
        }

        GD.Print($"Screenshot saved: {path}");
    }
}
