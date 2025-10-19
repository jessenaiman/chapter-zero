/home/adam/Dev/omega-spiral/chapter-zero/
├── OmegaSpiral.csproj                    # Single project file (no solution needed)
├── Source/
│   ├── Scripts/
│   │   └── Stages/
│   │       └── Stage1/                   # C# scripts for Stage 1
│   │           ├── TerminalBase.cs
│   │           ├── DreamweaverScore.cs
│   │           ├── BootSequence.cs
│   │           ├── OpeningMonologue.cs
│   │           ├── Question1_Name.cs
│   │           ├── Question2_Bridge.cs
│   │           ├── Question3_Voice.cs
│   │           ├── Question4_Name.cs
│   │           ├── Question5_Secret.cs
│   │           └── Question6_Continue.cs
│   └── Stages/
│       └── Stage1/                       # Godot scene files for Stage 1
│           ├── TerminalBase.tscn
│           ├── BootSequence.tscn
│           ├── OpeningMonologue.tscn
│           ├── Question1_Name.tscn
│           ├── Question2_Bridge.tscn
│           ├── Question3_Voice.tscn
│           ├── Question4_Name.tscn
│           ├── Question5_Secret.tscn
│           ├── Question6_Continue.tscn
│           └── SecretQuestion.tscn       # (old file to remove)
└── Tests/
    └── Stages/
        └── Stage1/                       # Test files for Stage 1
            ├── ContentBlockTests.cs      # Actual tests ✓
            ├── ErrorHandlingTests.cs     # Actual tests ✓
            ├── NarrativeScriptFunctionalTests.cs
            ├── NeverGoAloneControllerTests.cs
            ├── GamepadInput.cs           # Test helper ✓
            ├── KeyInput.cs               # Test helper ✓
            ├── MouseInput.cs             # Test helper ✓
            ├── GamepadNavigation.cs      # Test helper ✓
            ├── KeyboardNavigation.cs     # Test helper ✓
            ├── InputMethodType.cs        # Test helper ✓
            ├── TestInputSpamHarness.cs   # Test helper ✓
            ├── DialogueFlowTests.cs.disabled      # TO REMOVE
            └── FirstActIntegrationTests.cs.disabled  # TO REMOVE