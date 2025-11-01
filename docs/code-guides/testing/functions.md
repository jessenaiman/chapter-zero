---
layout: default
title: Functions
parent: Scene Runner
grand_parent: Advanced Testing
nav_order: 7
---

## Functions

In asynchronous programming, it's often necessary to wait for a function to complete and obtain its result before continuing with your program.
The Scene Runner provides functions that allow you to wait for specific methods to return a value, with a specified timeout.
This is particularly useful in scenarios where you want to test or ensure the result of a method call within a certain timeframe.

## Function Overview

|Function|Description|
|---|---|
|[AwaitMethod](#await_func) |Waits for a function in the scene to return a value. Returns a GdUnitFuncAssert object, which allows you to verify the result of the function call.|
|[AwaitMethodOn](#await_func_on) |Waits for a function of a specific source node to return a value. Returns a GdUnitFuncAssert object, which allows you to verify the result of the function call.|

### await_func

The **await_func** function pauses execution until a specified function in the scene returns a value.
It returns a [GdUnitFuncAssert]({{site.baseurl}}/testing/assert-function/#functionmethod-assertions) object, which provides a suite of
assertion methods to verify the returned value.

It takes the following arguments:

```cs
/// <summary>
/// Returns a method awaiter to wait for a specific method result.
/// </summary>
/// <typeparam name="V">The expected result type</typeparam>
/// <param name="methodName">The name of the method to wait</param>
/// <returns>GodotMethodAwaiter</returns>
GdUnitAwaiter.GodotMethodAwaiter<V> AwaitMethod<V>(string methodName);
```

Here is an example of how to use AwaitMethod:

```cs
ISceneRunner runner = ISceneRunner.Load("res://test_scene.tscn");

// Waits until the function `color_cycle()` returns black or fails after an timeout of 5s
await runner.AwaitMethod<bool>("color_cycle").IsEqual("black").WithTimeout(5000);
```
