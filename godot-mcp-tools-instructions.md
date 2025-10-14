# Godot MCP Server Tools Reference

## Available Tools

Based on the actual connected Godot MCP server (node /home/adam/Documents/Cline/MCP/godot-mcp/build/index.js), the available tools are:

### Core Godot Tools

- `launch_editor`: Launch Godot editor for a specific project
- `run_project`: Run the Godot project and capture output
- `get_debug_output`: Get the current debug output and errors
- `stop_project`: Stop the currently running Godot project
- `get_godot_version`: Get the installed Godot version
- `list_projects`: List Godot projects in a directory
- `get_project_info`: Retrieve metadata about a Godot project
- `create_scene`: Create a new Godot scene file
- `add_node`: Add a node to an existing scene
- `load_sprite`: Load a sprite into a Sprite2D node
- `export_mesh_library`: Export a scene as a MeshLibrary resource
- `save_scene`: Save changes to a scene file
- `get_uid`: Get the UID for a specific file in a Godot project (for Godot 4.4+)
- `update_project_uids`: Update UID references in a Godot project by resaving resources (for Godot 4.4+)

### Usage Examples

#### launch_editor

```json
{
  "projectPath": "/path/to/godot/project"
}
```

Opens the Godot editor for the specified project directory.

#### run_project

```json
{
  "projectPath": "/path/to/godot/project",
  "scene": "res://scenes/main.tscn"
}
```

Runs the Godot project in debug mode. Optional scene parameter specifies which scene to run.

#### get_debug_output

```json
{}
```

Retrieves any debug output or error messages from the currently running Godot process.

#### get_project_info

```json
{
  "projectPath": "/path/to/godot/project"
}
```

Returns metadata about the project including scene count, script count, asset count, etc.

#### create_scene

```json
{
  "projectPath": "/path/to/godot/project",
  "scenePath": "scenes/new_scene.tscn",
  "rootNodeType": "Node2D"
}
```

Creates a new scene file with the specified root node type.

#### add_node

```json
{
  "projectPath": "/path/to/godot/project",
  "scenePath": "scenes/main.tscn",
  "parentNodePath": "root",
  "nodeType": "Sprite2D",
  "nodeName": "MySprite"
}
```

Adds a new node to an existing scene under the specified parent.

#### get_uid

```json
{
  "projectPath": "/path/to/godot/project",
  "filePath": "scenes/main.tscn"
}
```

Gets the UID for a specific file in Godot 4.4+ projects.

## Note

The tools listed above are the actual available tools from the connected Godot MCP server. Previous references to Codacy tools or other MCP servers were incorrect. This server specifically provides Godot editor control and project management capabilities.
