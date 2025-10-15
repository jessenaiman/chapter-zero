# Omega Spiral - Chapter Zero

A narrative-driven Godot game blending retro aesthetics (DOS terminals, NetHack ASCII, Wizardry party management, dungeon crawling, pixel combat) with modern AI-powered storytelling.

## Overview

**Omega Spiral** is an experimental narrative game where players navigate through five distinct scenes, each representing a different era of gaming aesthetics. The game features dynamic AI-driven narrative personas (Dreamweavers) that adapt to player choices, creating emergent storytelling experiences.

### Key Features

- **5 Unique Scenes**: Each with distinct visual styles and gameplay mechanics
- **AI-Powered Narratives**: Three Dreamweaver personas + Omega system for dynamic storytelling
- **Retro Aesthetics**: CRT effects, terminal interfaces, ASCII art, and pixel graphics
- **Character Progression**: Party management, stats, and persistent save system

## Technology Stack

- **Engine**: Godot 4.5 with .NET/Mono support
- **Language**: C# for game logic
- **AI Integration**: NobodyWho plugin for local LLM inference
- **Testing**: NUnit + Godot test framework

## Prerequisites

### Required Software

- **Godot 4.5 (Mono/.NET version)**: [Download here](https://godotengine.org/download)
- **.NET 8.0 SDK**: [Download here](https://dotnet.microsoft.com/download)
- **Git**: For cloning the repository

### Recommended Tools

- **Visual Studio Code** or **Rider** for C# development
- **Min0**: For handling large assets (if needed)

## Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/jessenaiman/chapter-zero.git
cd chapter-zero
```

### 2. Download LLM Model (Required for AI Features)

The game uses local LLM models for dynamic narrative generation. Due to file size constraints, the model is not included in the repository.

**Recommended Model**: Qwen3-4B-Instruct Q4_K_M (~2.5GB)

#### Option A: Using wget (Linux/Mac)

```bash
mkdir -p models
cd models
wget https://huggingface.co/Qwen/Qwen2.5-3B-Instruct-GGUF/resolve/main/qwen2.5-3b-instruct-q4_k_m.gguf
mv qwen2.5-3b-instruct-q4_k_m.gguf qwen3-4b-instruct-2507-q4_k_m.gguf
cd ..
```

#### Option B: Manual Download

1. Visit [Qwen2.5-3B-Instruct on Hugging Face](https://huggingface.co/Qwen/Qwen2.5-3B-Instruct-GGUF)
2. Download `qwen2.5-3b-instruct-q4_k_m.gguf`
3. Create a `models/` directory in the project root
4. Move the downloaded file to `models/qwen3-4b-instruct-2507-q4_k_m.gguf`

#### Alternative Models

- **Smaller/Faster** (~600MB): Qwen2.5-0.5B-Instruct
- **Larger/Quality** (~8GB): Qwen2.5-7B-Instruct

### 3. Open in Godot

1. Launch Godot 4.5 (Mono version)
2. Click "Import"
3. Navigate to the cloned repository
4. Select `project.godot`
5. Click "Import & Edit"

### 4. Download NobodyWho Plugin Binaries

The NobodyWho plugin requires platform-specific binaries that are too large for git. Download them separately:

1. Visit [NobodyWho Releases](https://github.com/nobodywho-ooo/nobodywho/releases)
2. Download the latest release for your platform
3. Extract the binaries to `addons/nobodywho/` directory

**Expected files**:

- Linux: `nobodywho-godot-x86_64-unknown-linux-gnu-release.so`
- Windows: `nobodywho-godot-x86_64-pc-windows-msvc-release.dll`
- macOS: `nobodywho-godot-x86_64-apple-darwin-release.dylib` and `nobodywho-godot-aarch64-apple-darwin-release.dylib`

### 5. Enable NobodyWho Plugin (First Time Only)

1. In Godot, go to **Project → Project Settings → Plugins**
2. Enable the "NobodyWho" plugin
3. Restart the Godot editor when prompted

### 5. Build the C# Project

Godot should automatically build the C# project. If not:

```bash
dotnet build OmegaSpiral.csproj
```

## Running the Game

### In Godot Editor

Press **F5** or click the **Play** button in the top-right corner.

### From Command Line

```bash
godot --path . --verbose
```

## Project Structure

```text
chapter-zero/
├── Source/
│   ├── Scenes/          # Godot scene files (.tscn)
│   ├── Scripts/         # C# game logic
│   ├── Shaders/         # CRT and visual effects
│   ├── Resources/       # Assets and resources
│   └── Data/            # Game data (JSON, YAML)
├── Tests/               # NUnit test files
├── addons/
│   └── nobodywho/       # LLM integration plugin
├── models/              # LLM models (gitignored)
├── docs/                # Documentation and ADRs
└── specs/               # Feature specifications
```

## Testing

### Run All Tests

```bash
dotnet test
```

**Note**: Some tests require the Godot runtime and cannot be run with `dotnet test` alone. For full integration testing, use Godot's test runner or GdUnit4.

## Development Workflow

### Before Starting Work

1. Review the relevant specification in `specs/`
2. Check Architecture Decision Records (ADRs) in `docs/adr/`
3. Understand the data models in `docs/adr/adr-0002-namespace-review.md`

### Making Changes

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Make your changes
3. Run tests: `dotnet test`
4. Commit with descriptive messages
5. Push and create a Pull Request

### Code Quality

The project uses Codacy for automated code analysis. Ensure your changes:

- Follow C# coding conventions
- Include XML documentation comments
- Pass all existing tests
- Maintain or improve code coverage

## Enforced Pre-Commit Workflow

Before every commit, the following checks are automatically run and must pass:

1. **Code Formatting**: `dotnet format --verify-no-changes` ensures all code is properly formatted.
2. **Static Analysis & Linting**: `dotnet build --warnaserror` enforces all warnings as errors, using Roslyn and StyleCop analyzers.
3. **Automated Tests**: `dotnet test` runs all unit and integration tests.
4. **Security Scan (Optional)**: If Trivy is installed, `trivy fs --exit-code 1 --severity HIGH,CRITICAL .` scans for vulnerabilities and secrets.

If any check fails, the commit is blocked until issues are resolved.

### How it Works

- The `.git/hooks/pre-commit` script enforces these checks automatically.
- All contributors must have these tools installed and configured.
- No code with errors, warnings, failed tests, or critical vulnerabilities can be committed.

### Setup for Contributors

- Ensure you have the latest .NET SDK and Trivy (optional) installed.
- The pre-commit hook is located at `.git/hooks/pre-commit` and should be executable.
- If you clone the repo, verify the hook is present and run `chmod +x .git/hooks/pre-commit` if needed.

This workflow guarantees that only clean, tested, and secure code is committed, preventing error accumulation and improving team velocity.

---

## Key Scenes

1. **Scene 1: Narrative Terminal** - DOS-style terminal with typewriter effect and CRT shader
2. **Scene 2: NetHack Sequence** - ASCII dungeon exploration
3. **Scene 3: Wizardry Party** - Party management and character creation
4. **Scene 4: Tile Dungeon** - Grid-based dungeon crawling
5. **Scene 5: Pixel Combat** - Turn-based combat system

## AI/LLM Integration

The game uses the NobodyWho plugin to run local LLMs for:

- **Three Dreamweaver Personas**: Hero, Shadow, and Ambition guides
- **Omega System**: Meta-narrator weaving player choices into coherent narrative
- **Dynamic Responses**: Context-aware dialogue based on game state

See `docs/adr/adr-0003-nobodywho-llm-integration.md` for full implementation details.

## Troubleshooting

### Godot Can't Find .NET

Ensure you have Godot 4.5 **Mono** version, not the standard version. Check by opening Godot and verifying ".NET" appears in the version string.

### Model Not Loading

1. Verify the model file is in `models/qwen3-4b-instruct-2507-q4_k_m.gguf`
2. Check file size is approximately 2.5GB
3. Ensure the NobodyWho plugin is enabled

### Build Errors

```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### Tests Failing

Some tests depend on Godot nodes and require the Godot runtime. Run them through Godot's test interface or use GdUnit4.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes with tests
4. Ensure code passes quality checks
5. Submit a Pull Request

## License

[License information to be added]

## Credits

- **Technical Lead**: Adam (jessenaiman)
- **NobodyWho Plugin**: [nobodywho-ooo/nobodywho](https://github.com/nobodywho-ooo/nobodywho)
- **Qwen Models**: [Alibaba Cloud](https://huggingface.co/Qwen)

## Support

For issues, questions, or contributions, please open an issue on GitHub.

---

**Last Updated**: October 10, 2025
