using Godot;
using GdUnit4;
using static GdUnit4.Assertions;

namespace OmegaSpiral.Tests.Shared;

/// <summary>
/// Base class for terminal window layout tests that provides common setup and utility methods.
/// </summary>
[TestSuite]
[RequireGodotRuntime]
public abstract partial class OmegaWindowLayoutTestBase : Node
{
    /// <summary>
    /// Gets the tolerance for floating-point comparisons.
    /// </summary>
    protected const float Tolerance = 0.001f;

    /// <summary>
    /// Gets the tolerance for centering calculations (2px).
    /// </summary>
    protected const float CenteringTolerance = 2.1f;

    /// <summary>
    /// Gets the minimum margin percentage for content (8%).
    /// </summary>
    protected const float MinimumMarginPercentage = 0.08f;

    /// <summary>
    /// Gets the minimum difference for size comparisons to account for frame bounds.
    /// </summary>
    protected const float SizeComparisonTolerance = -0.1f;

    /// <summary>
    /// Gets the tolerance for size comparisons.
    /// </summary>
    protected const float SizeComparisonTolerancePositive = 0.1f;

    /// <summary>
    /// Verifies that content fits within frame boundaries with tolerance.
    /// </summary>
    /// <param name="frameRect">The frame rectangle.</param>
    /// <param name="contentRect">The content rectangle.</param>
    protected void AssertContentFitsInFrame(Rect2 frameRect, Rect2 contentRect)
    {
        // Content must fit within frame bounds (or be equal)
        AssertThat(frameRect.Size.X - contentRect.Size.X).IsGreaterEqual(SizeComparisonTolerance);
        AssertThat(frameRect.Size.Y - contentRect.Size.Y).IsGreaterEqual(SizeComparisonTolerance);
    }

    /// <summary>
    /// Verifies that content is centered within frame with tolerance.
    /// </summary>
    /// <param name="frameRect">The frame rectangle.</param>
    /// <param name="contentRect">The content rectangle.</param>
    protected void AssertContentCenteredInFrame(Rect2 frameRect, Rect2 contentRect)
    {
        var frameCenter = frameRect.GetCenter();
        var contentCenter = contentRect.GetCenter();

        AssertThat(Mathf.Abs(contentCenter.X - frameCenter.X)).IsLess(CenteringTolerance);
        AssertThat(Mathf.Abs(contentCenter.Y - frameCenter.Y)).IsLess(CenteringTolerance);
    }

    /// <summary>
    /// Calculates expected margins based on frame size and minimum percentage.
    /// </summary>
    /// <param name="frameSize">The frame size.</param>
    /// <returns>The expected margin size.</returns>
    protected Vector2 CalculateExpectedMargins(Vector2 frameSize)
    {
        return new Vector2(
            frameSize.X * MinimumMarginPercentage,
            frameSize.Y * MinimumMarginPercentage
        );
    }

    /// <summary>
    /// Verifies that all child elements of a container are within the container bounds.
    /// </summary>
    /// <param name="container">The parent container.</param>
    protected void AssertChildrenWithinBounds(Control container)
    {
        var containerRect = container.GetGlobalRect();

        for (int i = 0; i < container.GetChildCount(); i++)
        {
            var child = container.GetChild(i);
            if (child is Control childControl)
            {
                var childRect = childControl.GetGlobalRect();

                // Child should be completely within container bounds
                AssertThat(childRect.Position.X - containerRect.Position.X).IsGreaterEqual(SizeComparisonTolerance);
                AssertThat(childRect.Position.Y - containerRect.Position.Y).IsGreaterEqual(SizeComparisonTolerance);
                AssertThat(containerRect.End.X - childRect.End.X).IsGreaterEqual(SizeComparisonTolerance);
                AssertThat(containerRect.End.Y - childRect.End.Y).IsGreaterEqual(SizeComparisonTolerance);
            }
        }
    }
}
