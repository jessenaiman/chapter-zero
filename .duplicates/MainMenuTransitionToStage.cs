// Archived for review: previous MainMenu.TransitionToStage implementation
// This code duplicated stage-entry logic that is now handled by stage managers.
/*
        /// <summary>
        /// Transitions to a stage using the first scene of that stage.
        /// For Stage 1, loads the manifest to get the first beat.
        /// </summary>
        private void TransitionToStage(int stageId)
        {
            string? scenePath = null;

            // For Stage 1, load the stage_manifest.json to get the first beat
            if (stageId == 1)
            {
                var stageManifestLoader = new StageManifestLoader();
                var stageManifest = stageManifestLoader.LoadManifest("res://source/stages/stage_1/stage_manifest.json");

                if (stageManifest != null && stageManifest.Scenes.Count > 0)
                {
                    var firstBeat = stageManifest.GetFirstScene();
                    if (firstBeat != null)
                    {
                        scenePath = firstBeat.SceneFile;
                        GD.Print($"[MainMenu] Stage 1 first beat: {scenePath}");
                    }
                }
                else
                {
                    GD.PrintErr("[MainMenu] Failed to load Stage 1 manifest. Falling back to default.");
                }
            }

            // Fallback to hardcoded paths for other stages
            if (string.IsNullOrEmpty(scenePath))
            {
                var firstSceneMap = new System.Collections.Generic.Dictionary<int, string>
                {
                    { 1, "res://source/stages/stage_1/beats/boot_sequence.tscn" },
                    { 2, "res://source/stages/stage_2/echo_hub.tscn" },
                    { 3, "res://source/stages/stage_3/echo_vault_hub.tscn" },
                    { 4, "res://source/stages/stage_4/stage_4_main.tscn" },
                    { 5, "res://source/stages/stage_5/stage5.tscn" },
                };

                if (!firstSceneMap.TryGetValue(stageId, out scenePath))
                {
                    GD.PrintErr($"[MainMenu] No entry scene configured for Stage {stageId}.");
                    return;
                }
            }

            GD.Print($"[MainMenu] Transitioning to Stage {stageId}: {scenePath}");

            if (_sceneManager != null)
            {
                _sceneManager.TransitionToScene(scenePath);
            }
            else
            {
                GD.PrintErr("[MainMenu] SceneManager not found! Using fallback scene change.");
                var nextScene = GD.Load<PackedScene>(scenePath);
                if (nextScene != null)
                {
                    GetTree().ChangeSceneToPacked(nextScene);
                }
            }
        }
*/
