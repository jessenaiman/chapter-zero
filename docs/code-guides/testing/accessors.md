---
layout: default
title: Accessors
parent: Scene Runner
grand_parent: Advanced Testing
nav_order: 8
---

# Scene Accessors

In addition to simulating the scene, the SceneRunner provides functions to access the scene's nodes.
These functions are useful for debugging and testing purposes.

For example, you can use **find_child()** to retrieve a specific node in the scene, and then call its methods or change its properties to test its behavior.

By using these functions, you can gain greater control over the scene and test various scenarios,
making it easier to find and fix bugs and improve the overall quality of your game or application.

## Function Overview

|Function|Description|
|---|---|
|[GetProperty]({{site.baseurl}}/advanced_testing/sceneRunner/#get_property) | Return the current value of a property. |
|[SetProperty]({{site.baseurl}}/advanced_testing/sceneRunner/#set_property) | Sets the value of the property with the specified name. |
|[FindChild]({{site.baseurl}}/advanced_testing/sceneRunner/#find_child) | Searches for the specified node with the name in the current scene. |
|[Invoke]({{site.baseurl}}/advanced_testing/sceneRunner/#invoke) | Executes the function specified by name in the scene and returns the result. |

### get_property

The **get_property** function returns the current value of the property from the current scene.

It takes the following arguments:

```cs
/// <summary>
/// Returns the property by given name.
/// </summary>
/// <typeparam name="T">The type of the property</typeparam>
/// <param name="name">The parameter name</param>
/// <returns>The value of the property or throws a MissingFieldException</returns>
/// <exception cref="MissingFieldException"/>
public T GetProperty<T>(string name);
```

Here is an example of how to use GetProperty:

```cs
ISceneRunner runner = ISceneRunner.Load("res://test_scene.tscn");

// Returns the current property `_door_color` from the scene
ColorRect color = runner.GetProperty("_door_color");
```

### set_property

The **set_property** function sets the value of a property with the specified name.

It takes the following arguments:

```cs
/// <summary>
/// Sets the value of the property with the specified name.
/// </summary>
/// <param name="name">The name of the property.</param>
/// <param name="value">The value to set for the property.</param>
/// <exception cref="MissingFieldException"/>
public T SetProperty<T>(string name, Variant value);
```

Here is an example of how to use SetProperty:

```cs
ISceneRunner runner = ISceneRunner.Load("res://test_scene.tscn");

// Sets the property `_door_color` to Red
runner.SetProperty("_door_color", Colors.Red);
```

### find_child

The **find_child** function searches for a node with the specified name in the current scene and returns it. If the node is not found, it returns null.

```cs
/// <summary>
/// Find a child located in the current scene.
/// </summary>
/// <param name="name">The name of the node to find.</param>
/// <param name="recursive">Enables/disables searching recursively.</param>
/// <param name="owned">If set to true, it only finds nodes who have an assigned owner.</param>
/// <returns>The node if found, otherwise null.</returns>
Node FindChild(string name, bool recursive = true, bool owned = false) -> Node:
```

```cs
var runner = ISceneRunner.Load("res://test_scene.tscn");

// Searches for the node `Health` inside the scene tree
var output = runner.FindChild("Health", true, true) as HealthComponent;
```

### invoke

The **invoke** function runs the function specified by given name in the scene and returns the result.

It takes the following arguments:

```cs
/// <summary>
/// Invokes the method by given name and arguments.
/// </summary>
/// <param name="name">The name of method to invoke</param>
/// <param name="args">The function arguments</param>
/// <returns>The return value of invoked method</returns>
/// <exception cref="MissingMethodException"/>
public object Invoke(string name, params object[] args);
```

Here is an example of how to use Invoke:

```cs
ISceneRunner runner = ISceneRunner.Load("res://test_scene.tscn");

// Invokes the function `start_color_cycle`
runner.Invoke("start_color_cycle");
```
