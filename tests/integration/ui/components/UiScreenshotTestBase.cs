using Godot;
using System;
using System.Reflection;
using GdUnit4;

/// <summary>
/// Base class for Ui tests that automatically takes screenshots for methods marked with [TakeScreenshot].
/// </summary>
public abstract partial class UiScreenshotTestBase : Node
{
    protected void MaybeTakeScreenshot(string testName, Node root)
    {
        var method = GetType().GetMethod(testName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method != null && method.GetCustomAttribute<TakeScreenshotAttribute>() != null)
        {
            BackgroundComponentScreenshotHelper.TakeScreenshot(root, testName);
        }
    }
}
