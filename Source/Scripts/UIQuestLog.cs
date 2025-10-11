using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Models;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Interfaces;

/// <summary>
/// Container for the player quest log display.
/// The UIQuestLog manages the display and tracking of player quests, objectives,
/// and progress throughout the game. It provides a centralized interface for
/// players to view their current quests, completed quests, and track their progress.
/// It also handles quest notifications and updates to keep players informed of
/// their quest status changes.
/// </summary>
public partial class UIQuestLog : Control
{
    /// <summary>
    /// Emitted when the player selects a quest.
    /// </summary>
    [Signal]
    public delegate void QuestSelectedEventHandler(Quest quest);

    /// <summary>
    /// Emitted when the player accepts a quest.
    /// </summary>
    [Signal]
    public delegate void QuestAcceptedEventHandler(Quest quest);

    /// <summary>
    /// Emitted when the player completes a quest.
    /// </summary>
    [Signal]
    public delegate void QuestCompletedEventHandler(Quest quest);

    /// <summary>
    /// Emitted when the player abandons a quest.
    /// </summary>
    [Signal]
    public delegate void QuestAbandonedEventHandler(Quest quest);

    /// <summary>
    /// Emitted when the quest log is closed.
    /// </summary>
    [Signal]
    public delegate void QuestLogClosedEventHandler();

    /// <summary>
    /// The player's quest log.
    /// </summary>
    public QuestLog PlayerQuestLog { get; private set; }

    /// <summary>
    /// Whether the quest log is currently visible.
    /// </summary>
    public bool QuestLogVisible
    {
        get => Visible;
        set => Visible = value;
    }

    /// <summary>
    /// The container for active quests.
    /// </summary>
    private Control activeQuestsContainer;

    /// <summary>
    /// The container for completed quests.
    /// </summary>
    private Control completedQuestsContainer;

    /// <summary>
    /// The container for quest details.
    /// </summary>
    private Control questDetailsContainer;

    /// <summary>
    /// The close button.
    /// </summary>
    private Button closeButton;

    /// <summary>
    /// Dictionary mapping quests to their UI displays.
    /// </summary>
    private Dictionary<Quest, Control> questDisplays = new Dictionary<Quest, Control>();

    /// <summary>
    /// The currently selected quest.
    /// </summary>
    private Quest selectedQuest;

    public override void _Ready()
    {
        // Get references to child UI elements
        activeQuestsContainer = GetNode<Control>("ActiveQuestsContainer");
        completedQuestsContainer = GetNode<Control>("CompletedQuestsContainer");
        questDetailsContainer = GetNode<Control>("QuestDetailsContainer");
        closeButton = GetNode<Button>("CloseButton");

        // Initially hide the quest log
        Visible = false;

        // Connect to any necessary signals
        ConnectSignals();
    }

    /// <summary>
    /// Connect to necessary signals.
    /// </summary>
    private void ConnectSignals()
    {
        // Connect the close button
        if (closeButton != null)
        {
            closeButton.Pressed += OnCloseButtonPressed;
        }

        // Connect to quest events
        // QuestEvents.QuestAccepted += OnQuestAccepted;
        // QuestEvents.QuestCompleted += OnQuestCompleted;
        // QuestEvents.QuestFailed += OnQuestFailed;
        // QuestEvents.QuestUpdated += OnQuestUpdated;
        // QuestEvents.QuestAbandoned += OnQuestAbandoned;
    }

    /// <summary>
    /// Setup the UI quest log with the given player quest log.
    /// </summary>
    /// <param name="questLog">The player's quest log</param>
    public void Setup(QuestLog questLog)
    {
        PlayerQuestLog = questLog;

        // Clear any existing quest displays
        ClearQuestDisplays();

        // Create displays for all quests in the quest log
        if (PlayerQuestLog.ActiveQuests != null)
        {
            foreach (var quest in PlayerQuestLog.ActiveQuests)
            {
                CreateQuestDisplay(quest, activeQuestsContainer);
            }
        }

        if (PlayerQuestLog.CompletedQuests != null)
        {
            foreach (var quest in PlayerQuestLog.CompletedQuests)
            {
                CreateQuestDisplay(quest, completedQuestsContainer);
            }
        }

        // Show the quest log
        Visible = true;

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Clear all quest displays.
    /// </summary>
    private void ClearQuestDisplays()
    {
        // Remove all existing quest displays
        foreach (var display in questDisplays.Values)
        {
            display.QueueFree();
        }

        questDisplays.Clear();

        // Clear containers
        if (activeQuestsContainer != null)
        {
            foreach (var child in activeQuestsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (completedQuestsContainer != null)
        {
            foreach (var child in completedQuestsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        if (questDetailsContainer != null)
        {
            foreach (var child in questDetailsContainer.GetChildren())
            {
                child.QueueFree();
            }
        }
    }

    /// <summary>
    /// Create a display for a quest.
    /// </summary>
    /// <param name="quest">The quest to create a display for</param>
    /// <param name="container">The container to add the display to</param>
    private void CreateQuestDisplay(Quest quest, Control container)
    {
        if (quest == null || container == null)
        {
            return;
        }

        // Create a new quest display control
        var display = new Control();
        display.Name = quest.Title;

        // Add the display to the container
        container.AddChild(display);

        // Store the display in the dictionary
        questDisplays[quest] = display;

        // Set up the display with initial values
        UpdateQuestDisplay(quest, display);

        // Connect input events to allow selecting the quest
        display.GuiInput += (inputEvent) => OnQuestDisplayInput(quest, inputEvent);
    }

    /// <summary>
    /// Update a quest display with current values.
    /// </summary>
    /// <param name="quest">The quest to update the display for</param>
    /// <param name="display">The display to update</param>
    private void UpdateQuestDisplay(Quest quest, Control display)
    {
        if (quest == null || display == null)
        {
            return;
        }

        // Update the display with the quest's current properties
        // This would typically involve updating labels, progress bars, icons, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var titleLabel = display.GetNode<Label>("Title");
        // titleLabel.Text = quest.Title;

        // var descriptionLabel = display.GetNode<Label>("Description");
        // descriptionLabel.Text = quest.Description;

        // var progressLabel = display.GetNode<Label>("Progress");
        // progressLabel.Text = $"{quest.CompletedObjectives}/{quest.TotalObjectives}";

        // var progressBar = display.GetNode<TextureProgressBar>("ProgressBar");
        // progressBar.Value = quest.Progress;
        // progressBar.MaxValue = 100.0f;

        // var statusLabel = display.GetNode<Label>("Status");
        // statusLabel.Text = quest.Status.ToString();
    }

    /// <summary>
    /// Update the quest details panel.
    /// </summary>
    /// <param name="quest">The quest to display details for</param>
    private void UpdateQuestDetailsPanel(Quest quest)
    {
        if (quest == null || questDetailsContainer == null)
        {
            return;
        }

        // Update the details panel with the quest's properties
        // This would typically involve updating labels, descriptions, objectives, rewards, etc.
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var titleLabel = questDetailsContainer.GetNode<Label>("Title");
        // titleLabel.Text = quest.Title;

        // var descriptionLabel = questDetailsContainer.GetNode<Label>("Description");
        // descriptionLabel.Text = quest.Description;

        // var objectivesContainer = questDetailsContainer.GetNode<VBoxContainer>("Objectives");
        // foreach (var child in objectivesContainer.GetChildren())
        // {
        //     child.QueueFree();
        // }

        // foreach (var objective in quest.Objectives)
        // {
        //     var objectiveLabel = new Label();
        //     objectiveLabel.Text = objective.Description;
        //     objectiveLabel.AddThemeColorOverride("font_color", objective.IsCompleted ? Colors.Green : Colors.White);
        //     objectivesContainer.AddChild(objectiveLabel);
        // }

        // var rewardsLabel = questDetailsContainer.GetNode<Label>("Rewards");
        // rewardsLabel.Text = string.Join(", ", quest.Rewards.Select(r => r.Name));

        // var statusLabel = questDetailsContainer.GetNode<Label>("Status");
        // statusLabel.Text = quest.Status.ToString();
    }

    /// <summary>
    /// Show the available actions for a quest.
    /// </summary>
    /// <param name="quest">The quest to show actions for</param>
    private void ShowQuestActions(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        // Show the appropriate actions for the quest
        // This would typically involve showing/hiding buttons based on the quest status
        // The exact implementation depends on the UI structure being used.

        // For example:
        // var abandonButton = GetNode<Button>("AbandonButton");
        // var trackButton = GetNode<Button>("TrackButton");
        // var shareButton = GetNode<Button>("ShareButton");

        // abandonButton.Visible = quest.Status == QuestStatus.Active;
        // trackButton.Visible = quest.Status == QuestStatus.Active;
        // shareButton.Visible = quest.Status == QuestStatus.Active || quest.Status == QuestStatus.Completed;

        // Connect button signals
        // abandonButton.Pressed += () => OnAbandonQuest(quest);
        // trackButton.Pressed += () => OnTrackQuest(quest);
        // shareButton.Pressed += () => OnShareQuest(quest);
    }

    /// <summary>
    /// Hide the quest actions.
    /// </summary>
    private void HideQuestActions()
    {
        // Hide all action buttons
        // var abandonButton = GetNode<Button>("AbandonButton");
        // var trackButton = GetNode<Button>("TrackButton");
        // var shareButton = GetNode<Button>("ShareButton");

        // abandonButton.Visible = false;
        // trackButton.Visible = false;
        // shareButton.Visible = false;
    }

    /// <summary>
    /// Callback when a quest is selected.
    /// </summary>
    /// <param name="quest">The selected quest</param>
    private void OnQuestSelected(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        selectedQuest = quest;
        UpdateQuestDetailsPanel(quest);
        ShowQuestActions(quest);
        EmitSignal(SignalName.QuestSelected, quest);
    }

    /// <summary>
    /// Callback when input is received on a quest display.
    /// </summary>
    /// <param name="quest">The quest associated with the display</param>
    /// <param name="inputEvent">The input event</param>
    private void OnQuestDisplayInput(Quest quest, InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            // Select the quest when clicked
            OnQuestSelected(quest);
        }
    }

    /// <summary>
    /// Callback when the close button is pressed.
    /// </summary>
    private void OnCloseButtonPressed()
    {
        HideQuestLog();
        EmitSignal(SignalName.QuestLogClosed);
    }

    /// <summary>
    /// Callback when a quest is accepted.
    /// </summary>
    /// <param name="quest">The accepted quest</param>
    private void OnQuestAccepted(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Add the quest to the active quests display
        CreateQuestDisplay(quest, activeQuestsContainer);

        // Update the quest display
        if (questDisplays.ContainsKey(quest))
        {
            UpdateQuestDisplay(quest, questDisplays[quest]);
        }

        // Show a notification
        ShowNotification($"Quest Accepted: {quest.Title}", Colors.Green);
    }

    /// <summary>
    /// Callback when a quest is completed.
    /// </summary>
    /// <param name="quest">The completed quest</param>
    private void OnQuestCompleted(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Move the quest from active to completed
        if (questDisplays.ContainsKey(quest))
        {
            var display = questDisplays[quest];
            display.Reparent(completedQuestsContainer);
        }

        // Update the quest display
        UpdateQuestDisplay(quest, questDisplays[quest]);

        // Show a notification
        ShowNotification($"Quest Completed: {quest.Title}", Colors.Gold);
    }

    /// <summary>
    /// Callback when a quest is failed.
    /// </summary>
    /// <param name="quest">The failed quest</param>
    private void OnQuestFailed(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Update the quest display to show failed status
        UpdateQuestDisplay(quest, questDisplays[quest]);

        // Show a notification
        ShowNotification($"Quest Failed: {quest.Title}", Colors.Red);
    }

    /// <summary>
    /// Callback when a quest is updated.
    /// </summary>
    /// <param name="quest">The updated quest</param>
    private void OnQuestUpdated(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Update the quest display
        if (questDisplays.ContainsKey(quest))
        {
            UpdateQuestDisplay(quest, questDisplays[quest]);
        }

        // If this is the selected quest, also update the details panel
        if (quest == selectedQuest)
        {
            UpdateQuestDetailsPanel(quest);
        }

        // Show a notification
        ShowNotification($"Quest Updated: {quest.Title}", Colors.Blue);
    }

    /// <summary>
    /// Callback when a quest is abandoned.
    /// </summary>
    /// <param name="quest">The abandoned quest</param>
    private void OnQuestAbandoned(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Remove the quest from the displays
        if (questDisplays.ContainsKey(quest))
        {
            var display = questDisplays[quest];
            display.QueueFree();
            questDisplays.Remove(quest);
        }

        // Clear the selected quest if it was the abandoned quest
        if (quest == selectedQuest)
        {
            selectedQuest = null;
            HideQuestDetails();
        }

        // Show a notification
        ShowNotification($"Quest Abandoned: {quest.Title}", Colors.Gray);
    }

    /// <summary>
    /// Hide the quest log.
    /// </summary>
    public void HideQuestLog()
    {
        Visible = false;
        selectedQuest = null;
        HideQuestDetails();
        HideQuestActions();
    }

    /// <summary>
    /// Show the quest log.
    /// </summary>
    public void ShowQuestLog()
    {
        Visible = true;
        selectedQuest = null;
        HideQuestDetails();
        HideQuestActions();
    }

    /// <summary>
    /// Toggle the quest log visibility.
    /// </summary>
    public void ToggleQuestLog()
    {
        if (Visible)
        {
            HideQuestLog();
        }
        else
        {
            ShowQuestLog();
        }
    }

    /// <summary>
    /// Hide the quest details panel.
    /// </summary>
    private void HideQuestDetails()
    {
        if (questDetailsContainer != null)
        {
            questDetailsContainer.Hide();
        }
    }

    /// <summary>
    /// Update all quest displays.
    /// </summary>
    public void UpdateAllDisplays()
    {
        foreach (var kvp in questDisplays)
        {
            UpdateQuestDisplay(kvp.Key, kvp.Value);
        }

        // If there's a selected quest, update the details panel
        if (selectedQuest != null)
        {
            UpdateQuestDetailsPanel(selectedQuest);
        }
    }

    /// <summary>
    /// Get the display for a specific quest.
    /// </summary>
    /// <param name="quest">The quest to get the display for</param>
    /// <returns>The display control for the quest, or null if not found</returns>
    public Control GetQuestDisplay(Quest quest)
    {
        if (quest == null)
        {
            return null;
        }

        return questDisplays.GetValueOrDefault(quest, null);
    }

    /// <summary>
    /// Refresh the quest log with current quest data.
    /// </summary>
    public void Refresh()
    {
        // Clear and recreate all quest displays
        ClearQuestDisplays();

        if (PlayerQuestLog != null)
        {
            if (PlayerQuestLog.ActiveQuests != null)
            {
                foreach (var quest in PlayerQuestLog.ActiveQuests)
                {
                    CreateQuestDisplay(quest, activeQuestsContainer);
                }
            }

            if (PlayerQuestLog.CompletedQuests != null)
            {
                foreach (var quest in PlayerQuestLog.CompletedQuests)
                {
                    CreateQuestDisplay(quest, completedQuestsContainer);
                }
            }
        }

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Show a notification in the quest log.
    /// </summary>
    /// <param name="message">The notification message</param>
    /// <param name="color">The color of the notification</param>
    public async void ShowNotification(string message, Color color)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        // Show a temporary notification in the quest log
        // This would typically involve showing a label or panel with the message

        // For example:
        // var notificationLabel = GetNode<Label>("NotificationLabel");
        // notificationLabel.Text = message;
        // notificationLabel.AddThemeColorOverride("font_color", color);
        // notificationLabel.Show();

        // Wait for a few seconds
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Hide the notification
        // notificationLabel.Hide();
    }

    /// <summary>
    /// Show an effect label (like quest completion or objective updates).
    /// </summary>
    /// <param name="text">The text to show</param>
    /// <param name="position">The position to show the text at</param>
    /// <param name="color">The color of the text</param>
    public void ShowEffectLabel(string text, Vector2 position, Color color)
    {
        // Show a floating label at the specified position
        // This would typically involve creating a temporary label that floats upward and fades out

        // For example:
        // var label = new Label();
        // label.Text = text;
        // label.AddThemeColorOverride("font_color", color);
        // label.Position = position;
        // AddChild(label);

        // Create a tween to animate the label
        // var tween = CreateTween();
        // tween.TweenProperty(label, "position:y", position.Y - 50, 1.0f);
        // tween.Parallel().TweenProperty(label, "modulate:a", 0.0f, 1.0f);
        // tween.TweenCallback(new Callable(label, "queue_free"));
    }

    /// <summary>
    /// Accept a new quest.
    /// </summary>
    /// <param name="quest">The quest to accept</param>
    public void AcceptQuest(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Add the quest to the player's quest log
        PlayerQuestLog.AcceptQuest(quest);

        // Add the quest to the active quests display
        CreateQuestDisplay(quest, activeQuestsContainer);

        // Update the quest display
        if (questDisplays.ContainsKey(quest))
        {
            UpdateQuestDisplay(quest, questDisplays[quest]);
        }

        // Emit the quest accepted signal
        EmitSignal(SignalName.QuestAccepted, quest);
    }

    /// <summary>
    /// Complete a quest.
    /// </summary>
    /// <param name="quest">The quest to complete</param>
    public void CompleteQuest(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Complete the quest in the player's quest log
        PlayerQuestLog.CompleteQuest(quest);

        // Move the quest from active to completed
        if (questDisplays.ContainsKey(quest))
        {
            var display = questDisplays[quest];
            display.Reparent(completedQuestsContainer);
        }

        // Update the quest display
        UpdateQuestDisplay(quest, questDisplays[quest]);

        // Emit the quest completed signal
        EmitSignal(SignalName.QuestCompleted, quest);

        // Grant quest rewards
        GrantQuestRewards(quest);
    }

    /// <summary>
    /// Abandon a quest.
    /// </summary>
    /// <param name="quest">The quest to abandon</param>
    public void AbandonQuest(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Remove the quest from the player's quest log
        PlayerQuestLog.AbandonQuest(quest);

        // Remove the quest from the displays
        if (questDisplays.ContainsKey(quest))
        {
            var display = questDisplays[quest];
            display.QueueFree();
            questDisplays.Remove(quest);
        }

        // Clear the selected quest if it was the abandoned quest
        if (quest == selectedQuest)
        {
            selectedQuest = null;
            HideQuestDetails();
        }

        // Emit the quest abandoned signal
        EmitSignal(SignalName.QuestAbandoned, quest);
    }

    /// <summary>
    /// Grant rewards for completing a quest.
    /// </summary>
    /// <param name="quest">The completed quest</param>
    private void GrantQuestRewards(Quest quest)
    {
        if (quest == null || quest.Rewards == null)
        {
            return;
        }

        // Grant all rewards for the quest
        foreach (var reward in quest.Rewards)
        {
            if (reward is ItemReward itemReward)
            {
                // Add items to the player's inventory
                // PlayerInventory.AddItem(itemReward.Item, itemReward.Quantity);
            }
            else if (reward is ExperienceReward expReward)
            {
                // Add experience to the player's character
                // PlayerCharacter.AddExperience(expReward.Amount);
            }
            else if (reward is CurrencyReward currencyReward)
            {
                // Add currency to the player's wallet
                // PlayerWallet.AddCurrency(currencyReward.CurrencyType, currencyReward.Amount);
            }
        }

        // Show a notification about the rewards
        ShowNotification($"Quest Rewards Received: {quest.Title}", Colors.Gold);
    }

    /// <summary>
    /// Track a quest (show it in a prominent location).
    /// </summary>
    /// <param name="quest">The quest to track</param>
    public void TrackQuest(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Set the quest as tracked in the player's quest log
        PlayerQuestLog.TrackQuest(quest);

        // Update the quest display to show it's being tracked
        if (questDisplays.ContainsKey(quest))
        {
            UpdateQuestDisplay(quest, questDisplays[quest]);
        }

        // Show a notification
        ShowNotification($"Tracking Quest: {quest.Title}", Colors.Blue);
    }

    /// <summary>
    /// Untrack a quest.
    /// </summary>
    /// <param name="quest">The quest to untrack</param>
    public void UntrackQuest(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Unset the quest as tracked in the player's quest log
        PlayerQuestLog.UntrackQuest(quest);

        // Update the quest display to show it's no longer being tracked
        if (questDisplays.ContainsKey(quest))
        {
            UpdateQuestDisplay(quest, questDisplays[quest]);
        }

        // Show a notification
        ShowNotification($"Stopped Tracking Quest: {quest.Title}", Colors.Gray);
    }

    /// <summary>
    /// Share a quest with other players (in multiplayer games).
    /// </summary>
    /// <param name="quest">The quest to share</param>
    public void ShareQuest(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        // Share the quest with other players
        // This would typically involve sending a network message to other players

        // For example:
        // NetworkManager.SendQuestShared(quest);

        // Show a notification
        ShowNotification($"Shared Quest: {quest.Title}", Colors.Purple);
    }

    /// <summary>
    /// Get all active quests.
    /// </summary>
    /// <returns>A list of all active quests</returns>
    public List<Quest> GetActiveQuests()
    {
        if (PlayerQuestLog == null || PlayerQuestLog.ActiveQuests == null)
        {
            return new List<Quest>();
        }

        return new List<Quest>(PlayerQuestLog.ActiveQuests);
    }

    /// <summary>
    /// Get all completed quests.
    /// </summary>
    /// <returns>A list of all completed quests</returns>
    public List<Quest> GetCompletedQuests()
    {
        if (PlayerQuestLog == null || PlayerQuestLog.CompletedQuests == null)
        {
            return new List<Quest>();
        }

        return new List<Quest>(PlayerQuestLog.CompletedQuests);
    }

    /// <summary>
    /// Get the tracked quest.
    /// </summary>
    /// <returns>The currently tracked quest, or null if none</returns>
    public Quest GetTrackedQuest()
    {
        if (PlayerQuestLog == null)
        {
            return null;
        }

        return PlayerQuestLog.TrackedQuest;
    }

    /// <summary>
    /// Get the number of active quests.
    /// </summary>
    /// <returns>The number of active quests</returns>
    public int GetActiveQuestCount()
    {
        if (PlayerQuestLog == null || PlayerQuestLog.ActiveQuests == null)
        {
            return 0;
        }

        return PlayerQuestLog.ActiveQuests.Count;
    }

    /// <summary>
    /// Get the number of completed quests.
    /// </summary>
    /// <returns>The number of completed quests</returns>
    public int GetCompletedQuestCount()
    {
        if (PlayerQuestLog == null || PlayerQuestLog.CompletedQuests == null)
        {
            return 0;
        }

        return PlayerQuestLog.CompletedQuests.Count;
    }

    /// <summary>
    /// Check if a quest is active.
    /// </summary>
    /// <param name="quest">The quest to check</param>
    /// <returns>True if the quest is active, false otherwise</returns>
    public bool IsQuestActive(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null || PlayerQuestLog.ActiveQuests == null)
        {
            return false;
        }

        return PlayerQuestLog.ActiveQuests.Contains(quest);
    }

    /// <summary>
    /// Check if a quest is completed.
    /// </summary>
    /// <param name="quest">The quest to check</param>
    /// <returns>True if the quest is completed, false otherwise</returns>
    public bool IsQuestCompleted(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null || PlayerQuestLog.CompletedQuests == null)
        {
            return false;
        }

        return PlayerQuestLog.CompletedQuests.Contains(quest);
    }

    /// <summary>
    /// Check if a quest is tracked.
    /// </summary>
    /// <param name="quest">The quest to check</param>
    /// <returns>True if the quest is tracked, false otherwise</returns>
    public bool IsQuestTracked(Quest quest)
    {
        if (quest == null || PlayerQuestLog == null)
        {
            return false;
        }

        return PlayerQuestLog.TrackedQuest == quest;
    }

    /// <summary>
    /// Get the progress of a quest.
    /// </summary>
    /// <param name="quest">The quest to get progress for</param>
    /// <returns>The progress of the quest as a percentage (0.0 to 1.0)</returns>
    public float GetQuestProgress(Quest quest)
    {
        if (quest == null)
        {
            return 0.0f;
        }

        return quest.Progress;
    }

    /// <summary>
    /// Get the time remaining for a timed quest.
    /// </summary>
    /// <param name="quest">The timed quest to get time remaining for</param>
    /// <returns>The time remaining for the quest, or TimeSpan.MaxValue if not timed</returns>
    public TimeSpan GetTimeRemaining(Quest quest)
    {
        if (quest == null || !quest.IsTimed)
        {
            return TimeSpan.MaxValue;
        }

        return quest.EndTime - DateTime.UtcNow;
    }

    /// <summary>
    /// Sort quests by a specific criterion.
    /// </summary>
    /// <param name="sortType">The type of sorting to apply</param>
    public void SortQuests(QuestLog.SortType sortType)
    {
        if (PlayerQuestLog == null)
        {
            return;
        }

        // Sort the quests in the player's quest log
        PlayerQuestLog.SortQuests(sortType);

        // Refresh the quest log display
        Refresh();
    }

    /// <summary>
    /// Filter quests by a specific criterion.
    /// </summary>
    /// <param name="filterType">The type of filtering to apply</param>
    public void FilterQuests(QuestLog.FilterType filterType)
    {
        if (PlayerQuestLog == null)
        {
            return;
        }

        // Filter the quests in the player's quest log
        PlayerQuestLog.FilterQuests(filterType);

        // Refresh the quest log display
        Refresh();
    }

    /// <summary>
    /// Search for quests by title or description.
    /// </summary>
    /// <param name="searchTerm">The term to search for</param>
    public void SearchQuests(string searchTerm)
    {
        if (PlayerQuestLog == null || string.IsNullOrEmpty(searchTerm))
        {
            return;
        }

        // Search for quests in the player's quest log
        PlayerQuestLog.SearchQuests(searchTerm);

        // Refresh the quest log display
        Refresh();
    }
}
