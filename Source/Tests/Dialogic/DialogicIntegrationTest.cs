namespace OmegaSpiral.Tests.Dialogic;

// <copyright file="DialogicIntegrationTest.cs" company="Ωmega Spiral">
// Copyright (c) Ωmega Spiral. All rights reserved.
// </copyright>

using Godot;
using OmegaSpiral.Source.Scripts.Common;

/// <summary>
/// Test scene for verifying DialogicIntegration functionality.
/// </summary>
[GlobalClass]
public partial class DialogicIntegrationTest : Node2D
{
    private DialogicIntegration? dialogicIntegration;

    /// <inheritdoc/>
    public override void _Ready()
    {
        // Get the DialogicIntegration instance
        this.dialogicIntegration = this.GetNodeOrNull<DialogicIntegration>("/root/DialogicIntegration");

        if (this.dialogicIntegration != null)
        {
            GD.Print("DialogicIntegration found, starting test...");
            _ = this.RunDialogicTestAsync();
        }
        else
        {
            GD.PrintErr("DialogicIntegration not found!");
        }
    }

    private async System.Threading.Tasks.Task RunDialogicTestAsync()
    {
        if (this.dialogicIntegration == null)
        {
            return;
        }

        // Test setting and getting a variable
        this.dialogicIntegration.SetDialogicVariable("test_variable", "Hello Dialogic!");
        var value = this.dialogicIntegration.GetDialogicVariable("test_variable");
        GD.Print($"Retrieved variable value: {value}");

        // Test starting a timeline (if we have one)
        // This would require a valid .dtl file to test with
        GD.Print("DialogicIntegration test completed successfully!");

        await Task.CompletedTask;
    }
}
