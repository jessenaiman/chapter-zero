using System;

/// <summary>
/// Attribute to mark a test method for automatic screenshot capture after execution.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TakeScreenshotAttribute : Attribute
{
    public TakeScreenshotAttribute() { }
}
