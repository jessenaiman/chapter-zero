using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace OmegaSpiral.Source.Scripts
{
    /// <summary>
    /// Processes dialogue and choice rules from JSON, converting story blocks into queued lines.
    /// Pushes lines to UI scenes via signals without hardcoded narrative content.
    /// </summary>
    public class NarratorEngine : Node
    {
        // Signals for UI updates
        [Signal]
        public delegate void NarratorLineQueuedEventHandler(string line);

        [Signal]
        public delegate void NarratorChoiceAvailableEventHandler(JObject choiceData);

        [Signal]
        public delegate void NarratorQuestionAvailableEventHandler(string question, JArray options);

        [Signal]
        public delegate void NarratorSceneCompleteEventHandler();

        // Internal state
        private Queue<string> _dialogueQueue = new Queue<string>();
        private JObject _currentSceneData;
        private int _currentBlockIndex = 0;
        private bool _isProcessing = false;

        public override void _Ready()
        {
            GD.Print("NarratorEngine initialized");
        }

        /// <summary>
        /// Loads scene data and initializes the narrator engine.
        /// </summary>
        /// <param name="sceneData">JSON scene data to process</param>
        public void LoadSceneData(JObject sceneData)
        {
            try
            {
                _currentSceneData = sceneData;
                _currentBlockIndex = 0;
                _dialogueQueue.Clear();

                GD.Print("NarratorEngine loaded scene data");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error loading scene data in NarratorEngine: {ex.Message}");
            }
        }

        /// <summary>
        /// Starts processing the narrative scene data.
        /// </summary>
        public void StartNarrative()
        {
            try
            {
                if (_currentSceneData == null)
                {
                    GD.PrintErr("No scene data loaded in NarratorEngine");
                    return;
                }

                _isProcessing = true;

                // Process opening lines
                if (_currentSceneData.ContainsKey("openingLines"))
                {
                    JArray openingLines = (JArray)_currentSceneData["openingLines"];
                    foreach (string line in openingLines)
                    {
                        QueueLine(line);
                    }
                }

                // Process initial choice if available
                if (_currentSceneData.ContainsKey("initialChoice"))
                {
                    JObject initialChoice = (JObject)_currentSceneData["initialChoice"];
                    EmitSignal(SignalName.NarratorChoiceAvailable, initialChoice);
                }

                GD.Print("NarratorEngine started narrative processing");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error starting narrative in NarratorEngine: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes a story block and queues its content.
        /// </summary>
        /// <param name="blockIndex">Index of the story block to process</param>
        public void ProcessStoryBlock(int blockIndex)
        {
            try
            {
                if (_currentSceneData == null || !_currentSceneData.ContainsKey("storyBlocks"))
                {
                    GD.PrintErr("No story blocks available in NarratorEngine");
                    return;
                }

                JArray storyBlocks = (JArray)_currentSceneData["storyBlocks"];
                if (blockIndex >= storyBlocks.Count)
                {
                    GD.PrintErr($"Story block index {blockIndex} out of range");
                    return;
                }

                JObject block = (JObject)storyBlocks[blockIndex];

                // Process paragraphs
                if (block.ContainsKey("paragraphs"))
                {
                    JArray paragraphs = (JArray)block["paragraphs"];
                    foreach (string paragraph in paragraphs)
                    {
                        QueueLine(paragraph);
                    }
                }

                // Process question if available
                if (block.ContainsKey("question"))
                {
                    string question = (string)block["question"];
                    JArray choices = (JArray)block["choices"];
                    EmitSignal(SignalName.NarratorQuestionAvailable, question, choices);
                }

                GD.Print($"Processed story block {blockIndex}");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error processing story block {blockIndex} in NarratorEngine: {ex.Message}");
            }
        }

        /// <summary>
        /// Queues a line for narration.
        /// </summary>
        /// <param name="line">Line to queue</param>
        private void QueueLine(string line)
        {
            try
            {
                _dialogueQueue.Enqueue(line);
                EmitSignal(SignalName.NarratorLineQueued, line);
                GD.Print($"Queued line: {line}");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error queuing line in NarratorEngine: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the next line from the dialogue queue.
        /// </summary>
        /// <returns>Next line to narrate, or null if queue is empty</returns>
        public string GetNextLine()
        {
            try
            {
                if (_dialogueQueue.Count > 0)
                {
                    string line = _dialogueQueue.Dequeue();
                    GD.Print($"Dequeued line: {line}");
                    return line;
                }
                return null;
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error getting next line from NarratorEngine: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if there are more lines in the queue.
        /// </summary>
        /// <returns>True if queue has more lines, false otherwise</returns>
        public bool HasMoreLines()
        {
            return _dialogueQueue.Count > 0;
        }

        /// <summary>
        /// Processes a player choice and continues the narrative.
        /// </summary>
        /// <param name="choiceIndex">Index of the chosen option</param>
        public void ProcessChoice(int choiceIndex)
        {
            try
            {
                GD.Print($"Processing choice {choiceIndex}");

                // In a real implementation, we would process the choice and determine
                // the next story block to load based on the choice
                // For now, we'll just simulate continuing the narrative

                // For demonstration, let's process the first story block after a choice
                ProcessStoryBlock(0);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error processing choice {choiceIndex} in NarratorEngine: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes a player answer to a question.
        /// </summary>
        /// <param name="answer">Player's answer</param>
        public void ProcessAnswer(string answer)
        {
            try
            {
                GD.Print($"Processing answer: {answer}");

                // In a real implementation, we would store the answer and potentially
                // influence the narrative based on it
                // For now, we'll just continue processing

                // Signal that the scene is complete (for demonstration)
                EmitSignal(SignalName.NarratorSceneComplete);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error processing answer '{answer}' in NarratorEngine: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles player name input.
        /// </summary>
        /// <param name="name">Player's name</param>
        public void ProcessPlayerName(string name)
        {
            try
            {
                GD.Print($"Processing player name: {name}");

                // In a real implementation, we would store the name in GameState
                // For now, we'll just log it

                // Continue with name prompt if available
                if (_currentSceneData.ContainsKey("namePrompt"))
                {
                    string namePrompt = (string)_currentSceneData["namePrompt"];
                    QueueLine(namePrompt);
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error processing player name '{name}' in NarratorEngine: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles secret question answer.
        /// </summary>
        /// <param name="answer">Player's answer to secret question</param>
        public void ProcessSecretAnswer(string answer)
        {
            try
            {
                GD.Print($"Processing secret answer: {answer}");

                // In a real implementation, we would store the secret answer
                // For now, we'll just log it and signal completion

                // Show exit line if available
                if (_currentSceneData.ContainsKey("exitLine"))
                {
                    string exitLine = (string)_currentSceneData["exitLine"];
                    QueueLine(exitLine);
                }

                // Signal scene completion
                EmitSignal(SignalName.NarratorSceneComplete);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error processing secret answer '{answer}' in NarratorEngine: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all queued dialogue.
        /// </summary>
        public void ClearQueue()
        {
            try
            {
                _dialogueQueue.Clear();
                GD.Print("NarratorEngine dialogue queue cleared");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error clearing NarratorEngine queue: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the current queue size.
        /// </summary>
        /// <returns>Number of lines in the queue</returns>
        public int GetQueueSize()
        {
            return _dialogueQueue.Count;
        }

        /// <summary>
        /// Pauses narrative processing.
        /// </summary>
        public void Pause()
        {
            _isProcessing = false;
            GD.Print("NarratorEngine paused");
        }

        /// <summary>
        /// Resumes narrative processing.
        /// </summary>
        public void Resume()
        {
            _isProcessing = true;
            GD.Print("NarratorEngine resumed");
        }

        /// <summary>
        /// Checks if narrative processing is paused.
        /// </summary>
        /// <returns>True if paused, false otherwise</returns>
        public bool IsPaused()
        {
            return !_isProcessing;
        }

        public override void _Process(double delta)
        {
            // Process any queued dialogue if not paused
            if (_isProcessing && HasMoreLines())
            {
                // In a real implementation, we would process lines at a controlled pace
                // For now, we'll just log that we're processing
                // GD.Print("Processing queued dialogue...");
            }
        }
    }
}