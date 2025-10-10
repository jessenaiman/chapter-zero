using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using FileAccess = Godot.FileAccess;
using Timer = Godot.Timer;

public partial class NarrativeTerminal : Node2D
{
    // UI elements for the terminal
    private RichTextLabel _outputLabel;
    private LineEdit _inputField;
    private Button _submitButton;
    private Label _promptLabel;
    
    // Game state and scene data
    private NarrativeSceneData _sceneData;
    private SceneManager _sceneManager;
    private NarratorEngine _narratorEngine;
    private int _currentBlockIndex = 0;
    private bool _waitingForInput = false;
    private string _currentPromptType = ""; // "choice", "name", "secret", "question"

    public override void _Ready()
    {
        // Initialize UI elements (in a real Godot scene, these would be connected)
        _outputLabel = GetNode<RichTextLabel>("OutputLabel"); // This would be connected in the actual scene
        _inputField = GetNode<LineEdit>("InputField");
        _submitButton = GetNode<Button>("SubmitButton");
        _promptLabel = GetNode<Label>("PromptLabel");
        
        // Connect button press signal
        _submitButton.Pressed += OnSubmitPressed;
        _inputField.TextSubmitted += (string text) => OnSubmitPressed();
        
        // Get references to singletons
        _sceneManager = GetNode<SceneManager>("/root/SceneManager");
        _narratorEngine = GetNode<NarratorEngine>("/root/NarratorEngine");
        
        // Load scene data
        LoadSceneData();
        
        // Start the narrative
        StartNarrative();
    }
    
    private void LoadSceneData()
    {
        try
        {
            // Determine which data file to load based on the current thread
            string thread = _sceneManager.GetNode<GameState>("/root/GameState").DreamweaverThread.ToString().ToLower();
            string dataPath = $"res://Source/Data/scenes/scene1_narrative/{thread}.json";
            
            if (!FileAccess.FileExists(dataPath))
            {
                GD.PrintErr($"Narrative data file does not exist: {dataPath}");
                // Fallback to hero thread if specific thread doesn't exist
                dataPath = "res://Source/Data/scenes/scene1_narrative/hero.json";
            }
            
            string jsonData = FileAccess.GetFileAsString(dataPath);
            JsonNode jsonNode = JsonNode.Parse(jsonData);
            
            _sceneData = new NarrativeSceneData();
            _sceneData.Type = jsonNode["type"]?.ToString();
            
            // Load opening lines
            if (jsonNode["openingLines"] != null)
            {
                foreach (var line in jsonNode["openingLines"].AsArray())
                {
                    _sceneData.OpeningLines.Add(line.ToString());
                }
            }
            
            // Load initial choice
            if (jsonNode["initialChoice"] != null)
            {
                var choiceNode = jsonNode["initialChoice"];
                _sceneData.InitialChoice = new NarrativeChoice
                {
                    Prompt = choiceNode["prompt"]?.ToString()
                };
                
                if (choiceNode["options"] != null)
                {
                    foreach (var option in choiceNode["options"].AsArray())
                    {
                        var choice = new DreamweaverChoice
                        {
                            Id = option["id"]?.ToString(),
                            Text = option["label"]?.ToString(),
                            Description = option["description"]?.ToString()
                        };
                        _sceneData.InitialChoice.Options.Add(choice);
                    }
                }
            }
            
            // Load story blocks
            if (jsonNode["storyBlocks"] != null)
            {
                foreach (var block in jsonNode["storyBlocks"].AsArray())
                {
                    var storyBlock = new StoryBlock();
                    
                    if (block["paragraphs"] != null)
                    {
                        foreach (var para in block["paragraphs"].AsArray())
                        {
                            storyBlock.Paragraphs.Add(para.ToString());
                        }
                    }
                    
                    storyBlock.Question = block["question"]?.ToString();
                    
                    if (block["choices"] != null)
                    {
                        foreach (var choice in block["choices"].AsArray())
                        {
                            var choiceOption = new ChoiceOption
                            {
                                Text = choice["text"]?.ToString(),
                                NextBlock = int.Parse(choice["nextBlock"]?.ToString() ?? "0")
                            };
                            storyBlock.Choices.Add(choiceOption);
                        }
                    }
                    
                    _sceneData.StoryBlocks.Add(storyBlock);
                }
            }
            
            _sceneData.NamePrompt = jsonNode["namePrompt"]?.ToString();
            
            if (jsonNode["secretQuestion"] != null)
            {
                var secretNode = jsonNode["secretQuestion"];
                _sceneData.SecretQuestion = new SecretQuestion
                {
                    Prompt = secretNode["prompt"]?.ToString()
                };
                
                if (secretNode["options"] != null)
                {
                    foreach (var opt in secretNode["options"].AsArray())
                    {
                        _sceneData.SecretQuestion.Options.Add(opt.ToString());
                    }
                }
            }
            
            _sceneData.ExitLine = jsonNode["exitLine"]?.ToString();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error loading narrative scene data: {ex.Message}");
        }
    }
    
    private void StartNarrative()
    {
        // Display opening lines
        foreach (string line in _sceneData.OpeningLines)
        {
            DisplayText(line);
        }
        
        // Present initial choice
        if (_sceneData.InitialChoice != null)
        {
            DisplayText(_sceneData.InitialChoice.Prompt);
            _promptLabel.Text = "Choose your story type:";
            
            // Display options
            string optionsText = "";
            foreach (var option in _sceneData.InitialChoice.Options)
            {
                optionsText += $"[{option.Id.ToUpper()}] {option.Text} - {option.Description}\n";
            }
            DisplayText(optionsText);
            
            _waitingForInput = true;
            _currentPromptType = "choice";
            _inputField.PlaceholderText = "Enter your choice (hero/shadow/ambition)";
        }
    }
    
    private void ContinueNarrative()
    {
        if (_currentBlockIndex < _sceneData.StoryBlocks.Count)
        {
            var block = _sceneData.StoryBlocks[_currentBlockIndex];
            
            // Display paragraphs
            foreach (string paragraph in block.Paragraphs)
            {
                DisplayText(paragraph);
            }
            
            if (!string.IsNullOrEmpty(block.Question))
            {
                DisplayText(block.Question);
                _promptLabel.Text = "Your response:";
                _waitingForInput = true;
                _currentPromptType = "question";
                _inputField.PlaceholderText = "Enter your answer";
            }
            else if (_currentBlockIndex == _sceneData.StoryBlocks.Count - 1)
            {
                // Last block - prompt for name
                DisplayText(_sceneData.NamePrompt);
                _promptLabel.Text = "Enter your name:";
                _waitingForInput = true;
                _currentPromptType = "name";
                _inputField.PlaceholderText = "Your name";
            }
        }
    }
    
    private void OnSubmitPressed()
    {
        if (!_waitingForInput) return;
        
        string input = _inputField.Text.Trim().ToLower();
        _inputField.Text = "";
        
        switch (_currentPromptType)
        {
            case "choice":
                HandleChoiceInput(input);
                break;
            case "name":
                HandleNameInput(input);
                break;
            case "secret":
                HandleSecretInput(input);
                break;
            case "question":
                HandleQuestionInput(input);
                break;
        }
    }
    
    private void HandleChoiceInput(string input)
    {
        // Find the matching choice
        DreamweaverChoice selectedChoice = null;
        foreach (var choice in _sceneData.InitialChoice.Options)
        {
            if (choice.Id.ToLower() == input)
            {
                selectedChoice = choice;
                break;
            }
        }
        
        if (selectedChoice != null)
        {
            // Update game state with the chosen thread
            _sceneManager.SetDreamweaverThread(selectedChoice.Id);
            
            // Display confirmation
            DisplayText($"You have chosen the {selectedChoice.Text} path: {selectedChoice.Description}");
            
            // Move to first story block
            _currentBlockIndex = 0;
            _waitingForInput = false;
            _currentPromptType = "";
            
            ContinueNarrative();
        }
        else
        {
            DisplayText("Invalid choice. Please select hero, shadow, or ambition.");
            _inputField.PlaceholderText = "Enter your choice (hero/shadow/ambition)";
        }
    }
    
    private void HandleNameInput(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            _sceneManager.SetPlayerName(input);
            DisplayText($"Welcome, {input}.");
            
            // Show secret question
            if (_sceneData.SecretQuestion != null)
            {
                DisplayText(_sceneData.SecretQuestion.Prompt);
                _promptLabel.Text = "Your response:";
                
                string optionsText = "";
                for (int i = 0; i < _sceneData.SecretQuestion.Options.Count; i++)
                {
                    optionsText += $"[{i}] {_sceneData.SecretQuestion.Options[i]}\n";
                }
                DisplayText(optionsText);
                
                _waitingForInput = true;
                _currentPromptType = "secret";
                _inputField.PlaceholderText = "Enter option number or custom response";
            }
            else
            {
                CompleteNarrativeScene();
            }
        }
        else
        {
            DisplayText("Please enter a valid name.");
        }
    }
    
    private void HandleSecretInput(string input)
    {
        DisplayText("Your secret is safe with us.");
        CompleteNarrativeScene();
    }
    
    private void HandleQuestionInput(string input)
    {
        // For now, just proceed to the next block
        _currentBlockIndex++;
        _waitingForInput = false;
        _currentPromptType = "";
        
        if (_currentBlockIndex < _sceneData.StoryBlocks.Count)
        {
            ContinueNarrative();
        }
        else
        {
            // Reached the end of story blocks, prompt for name
            DisplayText(_sceneData.NamePrompt);
            _promptLabel.Text = "Enter your name:";
            _waitingForInput = true;
            _currentPromptType = "name";
            _inputField.PlaceholderText = "Your name";
        }
    }
    
    private void CompleteNarrativeScene()
    {
        if (_sceneData.ExitLine != null)
        {
            DisplayText(_sceneData.ExitLine);
        }
        
        DisplayText("\nMoving to the next part of your journey...");
        
        // Update scene manager that we're moving to scene 2
        _sceneManager.UpdateCurrentScene(2);
        
        // Schedule scene transition after a short delay
        var timer = new Timer();
        timer.WaitTime = 3.0f;
        timer.Timeout += () => {
            _sceneManager.TransitionToScene("Scene2NethackSequence");
        };
        AddChild(timer);
        timer.Start();
    }
    
    private void DisplayText(string text)
    {
        // In a real implementation, this would add text to the output label
        // with typewriter effect. For now, we'll just add it directly.
        if (_outputLabel != null)
        {
            _outputLabel.Text += text + "\n";
        }
        else
        {
            GD.Print(text);
        }
    }
    
    // Simulate typewriter effect
    private async void DisplayTextWithTypewriter(string text)
    {
        if (_outputLabel == null) return;
        
        _outputLabel.Text += "\n";
        foreach (char c in text)
        {
            _outputLabel.Text += c;
            await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
        }
        _outputLabel.Text += "\n";
    }
}