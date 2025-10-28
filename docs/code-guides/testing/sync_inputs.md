---
layout: default
title: Synchronize Inputs
parent: Scene Runner
grand_parent: Advanced Testing
nav_order: 5
---

## Synchronize Inputs Events

Waits for all input events to be processed by flushing any buffered input events and then awaiting a full cycle of both the process and physics frames.<br>

This is typically used to ensure that any simulated or queued inputs are fully processed before proceeding with the next steps in the scene.<br>
**It's essential for reliable input simulation or when synchronizing logic based on inputs.**<br>

### await_input_processed

The **await_input_processed** function do wait until all input events are processed.<br>

```cs
/// <summary>
///     Waits for all input events to be processed by flushing any buffered input events and then awaiting a full cycle of
///     both the process and physics frames.
///     This is typically used to ensure that any simulated or queued inputs are fully processed before proceeding with the
///     next steps in the scene.
///     It's essential for reliable input simulation or when synchronizing logic based on inputs.
/// </summary>
public static Task AwaitInputProcessed()
```

Here is an example of how to use AwaitInputProcessed:

```cs
ISceneRunner runner = ISceneRunner.Load("res://test_scene.tscn");

// Simulates key combination ctrl+C is pressed
runner
    .SimulateKeyPress(Key.Ctrl)
    .SimulateKeyPress(Key.C);

// finalize the input event processing
await runner.AwaitInputProcessed();
```
