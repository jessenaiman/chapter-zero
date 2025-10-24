graph TD
    subgraph "Godot Singletons (Autoload)"
        GameManager("[GameManager (Singleton)]")
    end

    subgraph "Class Inheritance"
        BaseStage["`StageController (Base Class)`"]
        BaseStage --> Stage1Controller["`Stage1Controller`"]
        BaseStage --> Stage2Controller["`Stage2Controller`"]
        BaseStage --> StageNController["`...etc`"]
    end

    subgraph "Scene Structure (Example: Stage 1)"
        Stage1Scene["`stage_1.tscn (Node)`"]
        Stage1Scene -- "has script" --> Stage1Controller
        Stage1Controller -- "manages child node" --> NarrativeTerminal["`NarrativeTerminal`"]
    end

    GameManager -- "1. Holds array of [PackedScene]" --> Stage1Scene
    GameManager -- "2. Calls `_currentStage.ExecuteStageAsync()`" --> Stage1Controller
    Stage1Controller -- "3. Emits `StageComplete` signal" --> GameManager
    GameManager -- "4. Catches signal, loads next stage" --> GameManager