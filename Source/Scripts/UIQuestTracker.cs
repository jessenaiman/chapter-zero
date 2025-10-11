using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmegaSpiral.Source.Scripts.Models;
using OmegaSpiral.Source.Scripts;
using OmegaSpiral.Source.Scripts.Interfaces;

/// <summary>
/// Container for the player quest tracker display.
/// The UIQuestTracker manages the display of active quests and their objectives in a compact
/// overlay that stays visible during gameplay. It provides players with constant awareness
/// of their current goals and progress without interrupting gameplay flow.
/// It handles the presentation of quest titles, objective progress, and completion status
/// in an unobtrusive but informative manner.
/// </summary>
public partial class UIQuestTracker : Control
{
    /// <summary>
    /// Emitted when the player clicks on a quest in the tracker.
    /// </summary>
    [Signal]
    public delegate void QuestClickedEventHandler(Quest quest);

    /// <summary>
    /// Emitted when the quest tracker visibility is toggled.
    /// </summary>
    [Signal]
    public delegate void TrackerToggledEventHandler(bool isVisible);

    /// <summary>
    /// The player's quest log.
    /// </summary>
    public QuestLog PlayerQuestLog { get; private set; }

    /// <summary>
    /// Whether the quest tracker is currently visible.
    /// </summary>
    public bool TrackerVisible
    {
        get => Visible;
        set => Visible = value;
    }

    /// <summary>
    /// Whether the quest tracker is currently expanded to show more details.
    /// </summary>
    public bool IsExpanded { get; private set; } = false;

    /// <summary>
    /// The container for quest displays.
    /// </summary>
    private VBoxContainer questContainer;

    /// <summary>
    /// The toggle button to expand/collapse the tracker.
    /// </summary>
    private Button toggleButton;

    /// <summary>
    /// The maximum number of quests to display in the tracker.
    /// </summary>
    [Export]
    public int MaxQuestsDisplayed { get; set; } = 5;

    /// <summary>
    /// The time in seconds to show a quest update notification.
    /// </summary>
    [Export]
    public float NotificationDuration { get; set; } = 3.0f;

    /// <summary>
    /// Dictionary mapping quests to their UI displays.
    /// </summary>
    private Dictionary<Quest, Control> questDisplays = new Dictionary<Quest, Control>();

    /// <summary>
    /// The currently tracked quest.
    /// </summary>
    private Quest trackedQuest;

    public override void _Ready()
    {
        // Get references to child UI elements
        questContainer = GetNode<VBoxContainer>("QuestContainer");
        toggleButton = GetNode<Button>("ToggleButton");

        // Initially show the quest tracker
        Visible = true;

        // Connect to any necessary signals
        ConnectSignals();
    }

    /// <summary>
    /// Connect to necessary signals.
    /// </summary>
    private void ConnectSignals()
    {
        // Connect the toggle button
        if (toggleButton != null)
        {
            toggleButton.Pressed += OnToggleButtonPressed;
        }

        // Connect to quest events
        // QuestEvents.QuestAccepted += OnQuestAccepted;
        // QuestEvents.QuestUpdated += OnQuestUpdated;
        // QuestEvents.QuestCompleted += OnQuestCompleted;
        // QuestEvents.QuestFailed += OnQuestFailed;
        // QuestEvents.QuestAbandoned += OnQuestAbandoned;
    }

    /// <summary>
    /// Setup the UI quest tracker with the given player quest log.
    /// </summary>
    /// <param name="questLog">The player's quest log</param>
    public void Setup(QuestLog questLog)
    {
        PlayerQuestLog = questLog;

        // Clear any existing quest displays
        ClearQuestDisplays();

        // Create displays for all active quests
        if (PlayerQuestLog != null && PlayerQuestLog.ActiveQuests != null)
        {
            var questsToShow = PlayerQuestLog.ActiveQuests.Take(MaxQuestsDisplayed).ToList();
            foreach (var quest in questsToShow)
            {
                CreateQuestDisplay(quest, questContainer);
            }
        }

        // Show the quest tracker
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

        // Clear container
        if (questContainer != null)
        {
            foreach (var child in questContainer.GetChildren())
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
    private void CreateQuestDisplay(Quest quest, VBoxContainer container)
    {
        if (quest == null || container == null)
        {
            return;
        }

        // Create a new quest display control
        var display = new PanelContainer();
        display.Name = quest.Title;

        // Add the display to the container
        container.AddChild(display);

        // Store the display in the dictionary
        questDisplays[quest] = display;

        // Set up the display with initial values
        UpdateQuestDisplay(quest, display);

        // Connect input events to allow clicking on the quest
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
        // titleLabel.AddThemeColorOverride("font_color", quest.IsTracked ? Colors.Yellow : Colors.White);

        // var progressLabel = display.GetNode<Label>("Progress");
        // progressLabel.Text = $"{quest.CompletedObjectives}/{quest.TotalObjectives}";

        // var progressBar = display.GetNode<TextureProgressBar>("ProgressBar");
        // progressBar.Value = quest.Progress;
        // progressBar.MaxValue = 100.0f;

        // var statusLabel = display.GetNode<Label>("Status");
        // statusLabel.Text = quest.Status.ToString();
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
    }

    /// <summary>
    /// Refresh the quest tracker with current quest data.
    /// </summary>
    public void Refresh()
    {
        // Clear and recreate all quest displays
        ClearQuestDisplays();

        if (PlayerQuestLog != null && PlayerQuestLog.ActiveQuests != null)
        {
            var questsToShow = PlayerQuestLog.ActiveQuests.Take(MaxQuestsDisplayed).ToList();
            foreach (var quest in questsToShow)
            {
                CreateQuestDisplay(quest, questContainer);
            }
        }

        // Update all displays
        UpdateAllDisplays();
    }

    /// <summary>
    /// Show a notification when a quest is updated.
    /// </summary>
    /// <param name="quest">The updated quest</param>
    /// <param name="message">The notification message</param>
    public async void ShowQuestNotification(Quest quest, string message)
    {
        if (quest == null || string.IsNullOrEmpty(message))
        {
            return;
        }

        // Show a temporary notification for the quest update
        // This would typically involve showing a label or panel with the message

        // For example:
        // var notificationPanel = GetNode<PanelContainer>("NotificationPanel");
        // var notificationLabel = notificationPanel.GetNode<Label>("NotificationLabel");
        // notificationLabel.Text = message;
        // notificationPanel.Show();

        // Wait for the notification duration
        await Task.Delay(TimeSpan.FromSeconds(NotificationDuration));

        // Hide the notification
        // notificationPanel.Hide();
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
    /// Toggle the quest tracker visibility.
    /// </summary>
    public void ToggleTracker()
    {
        Visible = !Visible;
        EmitSignal(SignalName.TrackerToggled, Visible);
    }

    /// <summary>
    /// Expand or collapse the quest tracker to show more details.
    /// </summary>
    public void ToggleExpansion()
    {
        IsExpanded = !IsExpanded;

        // Update the UI to show more or fewer details
        if (IsExpanded)
        {
            // Show expanded view with more quest details
            // This might involve increasing the size of quest displays or showing additional information
        }
        else
        {
            // Show collapsed view with less quest details
            // This might involve decreasing the size of quest displays or hiding additional information
        }

        // Update the toggle button text or icon
        if (toggleButton != null)
        {
            toggleButton.Text = IsExpanded ? "Collapse" : "Expand";
        }
    }

    /// <summary>
    /// Track a specific quest in the tracker.
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

        // Set the tracked quest
        trackedQuest = quest;

        // Show a notification
        ShowQuestNotification(quest, $"Tracking Quest: {quest.Title}");
    }

    /// <summary>
    /// Untrack the currently tracked quest.
    /// </summary>
    public void UntrackQuest()
    {
        if (trackedQuest == null || PlayerQuestLog == null)
        {
            return;
        }

        // Unset the quest as tracked in the player's quest log
        PlayerQuestLog.UntrackQuest(trackedQuest);

        // Update the quest display to show it's no longer being tracked
        if (questDisplays.ContainsKey(trackedQuest))
        {
            UpdateQuestDisplay(trackedQuest, questDisplays[trackedQuest]);
        }

        // Clear the tracked quest
        trackedQuest = null;

        // Show a notification
        ShowQuestNotification(trackedQuest, $"Stopped Tracking Quest");
    }

    /// <summary>
    /// Get the currently tracked quest.
    /// </summary>
    /// <returns>The currently tracked quest, or null if none</returns>
    public Quest GetTrackedQuest()
    {
        return trackedQuest;
    }

    /// <summary>
    /// Get all quests currently displayed in the tracker.
    /// </summary>
    /// <returns>A list of quests currently displayed in the tracker</returns>
    public List<Quest> GetDisplayedQuests()
    {
        return new List<Quest>(questDisplays.Keys);
    }

    /// <summary>
    /// Add a quest to the tracker.
    /// </summary>
    /// <param name="quest">The quest to add</param>
    public void AddQuest(Quest quest)
    {
        if (quest == null || questContainer == null)
        {
            return;
        }

        // If we're already at the maximum number of quests, remove the oldest one
        if (questDisplays.Count >= MaxQuestsDisplayed)
        {
            var oldestQuest = questDisplays.Keys.First();
            RemoveQuest(oldestQuest);
        }

        // Create a display for the new quest
        CreateQuestDisplay(quest, questContainer);

        // Update the quest display
        if (questDisplays.ContainsKey(quest))
        {
            UpdateQuestDisplay(quest, questDisplays[quest]);
        }

        // Show a notification
        ShowQuestNotification(quest, $"New Quest: {quest.Title}");
    }

    /// <summary>
    /// Remove a quest from the tracker.
    /// </summary>
    /// <param name="quest">The quest to remove</param>
    public void RemoveQuest(Quest quest)
    {
        if (quest == null)
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

        // If this was the tracked quest, clear it
        if (quest == trackedQuest)
        {
            trackedQuest = null;
        }

        // Show a notification
        ShowQuestNotification(quest, $"Quest Removed: {quest.Title}");
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

        // Add the quest to the tracker
        AddQuest(quest);

        // Show a notification
        ShowQuestNotification(quest, $"Quest Accepted: {quest.Title}");
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

        // Show a notification
        ShowQuestNotification(quest, $"Quest Updated: {quest.Title}");
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

        // Update the quest display to show completed status
        if (questDisplays.ContainsKey(quest))
        {
            UpdateQuestDisplay(quest, questDisplays[quest]);
        }

        // Show a notification
        ShowQuestNotification(quest, $"Quest Completed: {quest.Title}");

        // Remove the quest after a delay
        CallDeferred("RemoveQuest", quest);
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
        if (questDisplays.ContainsKey(quest))
        {
            UpdateQuestDisplay(quest, questDisplays[quest]);
        }

        // Show a notification
        ShowQuestNotification(quest, $"Quest Failed: {quest.Title}");

        // Remove the quest after a delay
        CallDeferred("RemoveQuest", quest);
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

        // Remove the quest from the tracker
        RemoveQuest(quest);

        // Show a notification
        ShowQuestNotification(quest, $"Quest Abandoned: {quest.Title}");
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
            // Click on a quest to select it
            EmitSignal(SignalName.QuestClicked, quest);
        }
    }

    /// <summary>
    /// Callback when the toggle button is pressed.
    /// </summary>
    private void OnToggleButtonPressed()
    {
        ToggleExpansion();
    }

    /// <summary>
    /// Callback when the close button is pressed.
    /// </summary>
    private void OnCloseButtonPressed()
    {
        ToggleTracker();
    }

    /// <summary>
    /// Hide the quest tracker.
    /// </summary>
    public void HideTracker()
    {
        Visible = false;
        EmitSignal(SignalName.TrackerToggled, false);
    }

    /// <summary>
    /// Show the quest tracker.
    /// </summary>
    public void ShowTracker()
    {
        Visible = true;
        EmitSignal(SignalName.TrackerToggled, true);
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

        // Refresh the quest tracker display
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

        // Refresh the quest tracker display
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

        // Refresh the quest tracker display
        Refresh();
    }

    /// <summary>
    /// Update the quest tracker position.
    /// </summary>
    /// <param name="position">The new position for the quest tracker</param>
    public void UpdatePosition(Vector2 position)
    {
        Position = position;
    }

    /// <summary>
    /// Update the quest tracker size.
    /// </summary>
    /// <param name="size">The new size for the quest tracker</param>
    public void UpdateSize(Vector2 size)
    {
        Size = size;
    }

    /// <summary>
    /// Update the quest tracker theme.
    /// </summary>
    /// <param name="theme">The new theme for the quest tracker</param>
    public void UpdateTheme(Theme theme)
    {
        if (theme == null)
        {
            return;
        }

        // Apply the new theme to the quest tracker
        // This would typically involve updating the theme of all child controls
        // The exact implementation depends on the UI structure being used.

        // For example:
        // AddThemeOverride("theme", theme);
        // foreach (var child in GetChildren())
        // {
        //     if (child is Control control)
        //     {
        //         control.AddThemeOverride("theme", theme);
        //     }
        // }
    }

    /// <summary>
    /// Update the quest tracker transparency.
    /// </summary>
    /// <param name="alpha">The new alpha value for the quest tracker (0.0 to 1.0)</param>
    public void UpdateTransparency(float alpha)
    {
        // Clamp the alpha value to the valid range
        alpha = Mathf.Clamp(alpha, 0.0f, 1.0f);

        // Update the modulate property to change transparency
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, alpha);
    }

    /// <summary>
    /// Animate the quest tracker entrance.
    /// </summary>
    public async void AnimateEntrance()
    {
        // Animate the quest tracker sliding in from the side
        // This would typically involve creating a tween to animate the position

        // For example:
        // var startPosition = new Vector2(-Size.X, Position.Y);
        // Position = startPosition;

        // var tween = CreateTween();
        // tween.TweenProperty(this, "position:x", 0.0f, 0.5f);
        // tween.SetTrans(Tween.TransitionType.Back);
        // tween.SetEase(Tween.EaseType.Out);

        // Wait for the animation to complete
        await ToSignal(GetTree().CreateTimer(0.5f), Timer.SignalName.Timeout);
    }

    /// <summary>
    /// Animate the quest tracker exit.
    /// </summary>
    public async void AnimateExit()
    {
        // Animate the quest tracker sliding out to the side
        // This would typically involve creating a tween to animate the position

        // For example:
        // var tween = CreateTween();
        // tween.TweenProperty(this, "position:x", -Size.X, 0.5f);
        // tween.SetTrans(Tween.TransitionType.Back);
        // tween.SetEase(Tween.EaseType.In);

        // Wait for the animation to complete
        await ToSignal(GetTree().CreateTimer(0.5f), Timer.SignalName.Timeout);

        // Hide the quest tracker
        HideTracker();
    }

    /// <summary>
    /// Flash the quest tracker to draw attention.
    /// </summary>
    /// <param name="color">The color to flash</param>
    /// <param name="duration">The duration of the flash</param>
    public async void Flash(Color color, float duration = 0.5f)
    {
        if (duration <= 0)
        {
            return;
        }

        // Store the original modulate color
        var originalColor = Modulate;

        // Flash to the specified color
        Modulate = color;

        // Wait for half the duration
        await ToSignal(GetTree().CreateTimer(duration / 2.0f), Timer.SignalName.Timeout);

        // Return to the original color
        Modulate = originalColor;

        // Wait for the remaining duration
        await ToSignal(GetTree().CreateTimer(duration / 2.0f), Timer.SignalName.Timeout);
    }

    /// <summary>
    /// Pulse the quest tracker to draw attention.
    /// </summary>
    /// <param name="scale">The scale factor for the pulse</param>
    /// <param name="duration">The duration of the pulse</param>
    public async void Pulse(float scale = 1.1f, float duration = 0.5f)
    {
        if (duration <= 0 || scale <= 0)
        {
            return;
        }

        // Store the original scale
        var originalScale = Scale;

        // Create a tween to animate the scale
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", originalScale * scale, duration / 2.0f);
        tween.TweenProperty(this, "scale", originalScale, duration / 2.0f);
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.InOut);

        // Wait for the animation to complete
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    /// <summary>
    /// Shake the quest tracker to draw attention.
    /// </summary>
    /// <param name="amplitude">The amplitude of the shake</param>
    /// <param name="duration">The duration of the shake</param>
    /// <param name="frequency">The frequency of the shake</param>
    public async void Shake(float amplitude = 5.0f, float duration = 0.5f, float frequency = 10.0f)
    {
        if (duration <= 0 || amplitude <= 0 || frequency <= 0)
        {
            return;
        }

        // Store the original position
        var originalPosition = Position;

        // Create a timer for the shake duration
        var shakeTimer = GetTree().CreateTimer(duration);

        // Shake for the specified duration
        while (!shakeTimer.IsStopped())
        {
            // Calculate shake offset using Perlin noise for natural-looking shake
            var time = (float)GetTimeSinceStartup();
            var offsetX = Mathf.PerlinNoise(time * frequency, 0) * 2.0f - 1.0f;
            var offsetY = Mathf.PerlinNoise(0, time * frequency) * 2.0f - 1.0f;
            var offset = new Vector2(offsetX, offsetY) * amplitude;

            // Apply the shake offset
            Position = originalPosition + offset;

            // Wait for the next frame
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        // Return to the original position
        Position = originalPosition;
    }

    /// <summary>
    /// Get the quest tracker's current position.
    /// </summary>
    /// <returns>The quest tracker's current position</returns>
    public new Vector2 GetPosition()
    {
        return Position;
    }

    /// <summary>
    /// Get the quest tracker's current size.
    /// </summary>
    /// <returns>The quest tracker's current size</returns>
    public new Vector2 GetSize()
    {
        return Size;
    }

    /// <summary>
    /// Get the quest tracker's current transparency.
    /// </summary>
    /// <returns>The quest tracker's current alpha value (0.0 to 1.0)</returns>
    public float GetTransparency()
    {
        return Modulate.A;
    }

    /// <summary>
    /// Get the quest tracker's current scale.
    /// </summary>
    /// <returns>The quest tracker's current scale</returns>
    public new Vector2 GetScale()
    {
        return Scale;
    }

    /// <summary>
    /// Get the quest tracker's current theme.
    /// </summary>
    /// <returns>The quest tracker's current theme</returns>
    public new Theme GetTheme()
    {
        // Return the current theme
        // This would typically involve getting the theme from the control
        // The exact implementation depends on the UI structure being used.

        // For example:
        // return GetTheme();

        return null;
    }

    /// <summary>
    /// Get the number of quests currently displayed in the tracker.
    /// </summary>
    /// <returns>The number of quests currently displayed</returns>
    public int GetQuestCount()
    {
        return questDisplays.Count;
    }

    /// <summary>
    /// Check if a quest is currently displayed in the tracker.
    /// </summary>
    /// <param name="quest">The quest to check</param>
    /// <returns>True if the quest is displayed, false otherwise</returns>
    public bool IsQuestDisplayed(Quest quest)
    {
        if (quest == null)
        {
            return false;
        }

        return questDisplays.ContainsKey(quest);
    }

    /// <summary>
    /// Check if the quest tracker is currently tracking a quest.
    /// </summary>
    /// <returns>True if a quest is being tracked, false otherwise</returns>
    public bool IsTrackingQuest()
    {
        return trackedQuest != null;
    }

    /// <summary>
    /// Get the currently tracked quest.
    /// </summary>
    /// <returns>The currently tracked quest, or null if none</returns>
    public Quest GetCurrentTrackedQuest()
    {
        return trackedQuest;
    }

    /// <summary>
    /// Set the maximum number of quests to display in the tracker.
    /// </summary>
    /// <param name="maxQuests">The maximum number of quests to display</param>
    public void SetMaxQuestsDisplayed(int maxQuests)
    {
        if (maxQuests <= 0)
        {
            return;
        }

        MaxQuestsDisplayed = maxQuests;

        // Refresh the quest tracker to respect the new limit
        Refresh();
    }

    /// <summary>
    /// Set the notification duration for quest updates.
    /// </summary>
    /// <param name="duration">The duration to show notifications for</param>
    public void SetNotificationDuration(float duration)
    {
        if (duration <= 0)
        {
            return;
        }

        NotificationDuration = duration;
    }

    /// <summary>
    /// Get the maximum number of quests that can be displayed in the tracker.
    /// </summary>
    /// <returns>The maximum number of quests that can be displayed</returns>
    public int GetMaxQuestsDisplayed()
    {
        return MaxQuestsDisplayed;
    }

    /// <summary>
    /// Get the notification duration for quest updates.
    /// </summary>
    /// <returns>The duration to show notifications for</returns>
    public float GetNotificationDuration()
    {
        return NotificationDuration;
    }
}
