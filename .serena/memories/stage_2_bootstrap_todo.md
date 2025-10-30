Observations for Stage 2 (Nethack):
- .tscn ext_resource paths point to res://source/stages/stage_2/... but actual files are at res://source/frontend/stages/stage_2_nethack/; fix all scene script paths.
- NethackHub.cs script is missing (uid file exists). Either restore or remove hub scene usage.
- NethackCinematicDirector.cs missing using OmegaSpiral.Source.Frontend (StageBase location); update namespaces/usings to compile.
- NethackExploration.cs references OmegaSpiral.Source.Scripts.Infrastructure and Stage2 assets under res://source/stages/stage_2/...; update to Backend.SceneBase and correct resource/data paths.
- Data references: source/data/stages/ghost_terminal_archives/scene_flow.json points to res://source/stages/stage_2/scenes/nethack_hub.tscn; update to frontend path or route via GameManager.StageScenes.
- After wiring, add Stage 2 PackedScene to GameManager.StageScenes in main scene or autoload configuration.
- Validate by building, then running gdUnit tests focusing on stage_2, and ensuring no warnings in build output.
