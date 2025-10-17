# Narrative Sequences

This directory contains all narrative sequence implementations for the Ghost Terminal stage refactoring.

## Overview

Each narrative sequence represents a distinct phase or interaction within the Ghost Terminal opening stage. Sequences are orchestrated by `GhostTerminalDirector` and inherit from the `NarrativeSequence` base class.

## Structure

```
sequences/
├── README.md (this file)
├── PlaceholderSequence.cs (reference implementation)
├── OpeningSequence.cs (Phase 2)
├── ThreadBranchSequence.cs (Phase 3)
├── NameInputSequence.cs (Phase 4)
├── SecretSequence.cs (Phase 4)
└── FinaleSequence.cs (Phase 5)
```

## Implementing a New Sequence

### Step 1: Create the Class

Create a new C# file inheriting from `NarrativeSequence`:

```csharp
namespace OmegaSpiral.Source.Scripts.Field.Narrative.Sequences;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class MySequence : NarrativeSequence
{
    // Your implementation here
}
```

### Step 2: Override OnSequenceReady()

Initialize sequence-specific state:

```csharp
protected override void OnSequenceReady()
{
    base.OnSequenceReady(); // Always call base first!

    // Get nodes from your scene
    var myLabel = this.GetNode<Label>("MyLabel");

    // Initialize state
    // ...
}
```

### Step 3: Implement PlayAsync()

This is where your sequence logic goes:

```csharp
public override async Task PlayAsync()
{
    // Display content
    if (this.Dialogue != null)
    {
        await this.DisplayTextWithTypewriterAsync("Some dialogue...").ConfigureAwait(false);
    }

    // Fade effects
    await this.FadeToBlackAsync(1.0f).ConfigureAwait(false);

    // Signal completion
    this.CompleteSequence("next_sequence_id");
}
```

### Step 4: Create the Scene

Create a corresponding `.tscn` file in `Source/Scenes/GhostTerminal/`:

```
MySequence.tscn
├── Control (root)
├── Label (MyLabel)
├── RichTextLabel (for dialogue)
└── Button (for choices)
```

The root node should be a `Control` to inherit from `NarrativeSequence` (which extends `Control`).

### Step 5: Register with Director

In your code that initializes the director:

```csharp
director.RegisterSequenceScene("my_sequence", "res://Source/Scenes/GhostTerminal/MySequence.tscn");
```

## API Reference

### Signals

**SequenceComplete(String nextSequenceId)**
- Emitted when the sequence is done
- Parameter: ID of next sequence to play (empty string = no next)

**SequenceInput(String inputId)**
- Emitted when user makes a choice
- Parameter: ID representing the choice (e.g., "hero", "shadow")

### Protected Methods

**CompleteSequence(String nextSequenceId = "")**
- Call when sequence is finished
- Emits SequenceComplete signal

**OnInput(String inputId)**
- Call when user provides input
- Emits SequenceInput signal
- Director uses this for routing to next sequence

**DisplayTextWithTypewriterAsync(String text, float characterDelay = 0.02f)**
- Shows text with typewriter effect
- Returns after animation completes

**FadeToBlackAsync(float duration = 0.5f)**
- Fades screen to black
- Used for transitioning between sequences

**FadeFromBlackAsync(float duration = 0.5f)**
- Fades screen back to visible
- Used after scene changes

### Protected Properties

**ScreenTransition**
- Access to fade/dissolve effects
- May be null if not available

**Dialogue**
- Access to typewriter effects and dialogue UI
- May be null if not available

**GameState**
- Access to player state (thread choice, name, etc.)
- May be null if not available

**DreamweaverSystem**
- Access to LLM narrative generation (FUTURE)
- May be null if not available

**SequenceId**
- Unique identifier for this sequence
- Set by director during instantiation

## Example: Simple Sequence

```csharp
[GlobalClass]
public partial class SimpleSequence : NarrativeSequence
{
    private Label? contentLabel;
    private Button? continueButton;

    protected override void OnSequenceReady()
    {
        base.OnSequenceReady();
        this.contentLabel = this.GetNode<Label>("ContentLabel");
        this.continueButton = this.GetNode<Button>("ContinueButton");

        if (this.continueButton != null)
        {
            this.continueButton.Pressed += this.OnContinuePressed;
        }
    }

    public override async Task PlayAsync()
    {
        // Fade in
        await this.FadeFromBlackAsync(0.5f).ConfigureAwait(false);

        // Display content
        if (this.contentLabel != null)
        {
            this.contentLabel.Text = "Welcome to the sequence!";
        }

        // Wait for user interaction (button press)
        // OnContinuePressed will call CompleteSequence()
    }

    private void OnContinuePressed()
    {
        this.CompleteSequence("next_sequence");
    }
}
```

## Phase Implementation Schedule

### Phase 2: Opening Sequence
- `OpeningSequence.cs` - Initial narrative with typewriter effects
- Presents thread choice buttons (Hero, Shadow, Ambition)
- Emits user selection as input

### Phase 3: Thread Branch Sequences
- `ThreadBranchSequence.cs` - Common base for thread-specific narratives
- Displays story blocks based on chosen thread
- Handles branching choices within thread

### Phase 4: Input Sequences
- `NameInputSequence.cs` - Player name input with validation
- `SecretSequence.cs` - Secret question with multiple choice

### Phase 5: Polish
- `FinaleSequence.cs` - Conclusion and transition to next stage
- Add AnimationPlayer timelines
- Sound integration
- Optional effects

## Common Patterns

### Pattern 1: Wait for Button Press

```csharp
public override async Task PlayAsync()
{
    var button = this.GetNode<Button>("SubmitButton");
    var taskCompletionSource = new TaskCompletionSource<bool>();

    button.Pressed += () => taskCompletionSource.SetResult(true);

    await taskCompletionSource.Task.ConfigureAwait(false);
    this.CompleteSequence("next_id");
}
```

### Pattern 2: Display Multiple Text Blocks

```csharp
public override async Task PlayAsync()
{
    var textBlocks = new[] { "Line 1", "Line 2", "Line 3" };

    foreach (string block in textBlocks)
    {
        await this.DisplayTextWithTypewriterAsync(block).ConfigureAwait(false);
        await Task.Delay(500).ConfigureAwait(false); // Pause between blocks
    }

    this.CompleteSequence("next_id");
}
```

### Pattern 3: Conditional Branching

```csharp
public override async Task PlayAsync()
{
    if (this.GameState?.DreamweaverThread == DreamweaverThread.Hero)
    {
        await this.PlayHeroContent().ConfigureAwait(false);
    }
    else if (this.GameState?.DreamweaverThread == DreamweaverThread.Shadow)
    {
        await this.PlayShadowContent().ConfigureAwait(false);
    }
    else
    {
        await this.PlayAmbitionContent().ConfigureAwait(false);
    }
}
```

## Testing

Each sequence should have corresponding unit tests:

```csharp
[TestFixture]
public class MySequenceTests
{
    [Test]
    public async Task PlayAsync_DisplaysContent()
    {
        // Arrange
        var sequence = new MySequence();
        // ... setup

        // Act
        var playTask = sequence.PlayAsync();
        // ... simulate user input

        // Assert
        // ... verify signals emitted, content displayed
    }
}
```

## Debugging

Enable debug output in sequences:

```csharp
public override async Task PlayAsync()
{
    GD.Print($"[{this.SequenceId}] Starting playback");

    // ... sequence logic

    GD.Print($"[{this.SequenceId}] Playback complete, next: next_id");
    this.CompleteSequence("next_id");
}
```

## Resources

- **Base Class**: `Source/Scripts/field/narrative/NarrativeSequence.cs`
- **Director**: `Source/Scripts/field/narrative/GhostTerminalDirector.cs`
- **Reference Implementation**: `PlaceholderSequence.cs`
- **Existing UI**: `Source/Scripts/field/ui/UIDialogue.cs`
- **Existing Transitions**: `Source/Scripts/common/screen_transitions/ScreenTransition.cs`
- **Test Examples**: `Tests/Narrative/`

## FAQ

**Q: Can a sequence play sounds?**
A: Yes, add an `AudioStreamPlayer` node to your scene and control it in `PlayAsync()`.

**Q: How do I access JSON content?**
A: Use `NarrativeSceneData` and `NarrativeSceneFactory` as done in the original `NarrativeTerminal`.

**Q: What if a component (UIDialogue, etc.) is null?**
A: Check for null and gracefully degrade functionality. The director will still proceed.

**Q: How do I implement LLM integration?**
A: Use `this.DreamweaverSystem` to call its methods (FUTURE phase).

**Q: Can sequences communicate with each other?**
A: Use the `GameState` singleton to share data between sequences.

## Contributing

When adding a new sequence:

1. ✅ Create class inheriting from `NarrativeSequence`
2. ✅ Implement `OnSequenceReady()` and `PlayAsync()`
3. ✅ Add XML documentation comments
4. ✅ Create corresponding `.tscn` scene
5. ✅ Register with director
6. ✅ Add unit tests
7. ✅ Run `dotnet build` to verify
8. ✅ Run `dotnet test` to verify tests pass
9. ✅ Run Codacy analysis for code quality

---

**Ready to implement Phase 2+!**
