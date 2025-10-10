# Codacy C# Analysis Configuration

## Current Configuration (.codacy/codacy.yaml)
```yaml
runtimes:
    - python@3.11.11
    - node@22.2.0
tools:
    - lizard@1.17.31
    - pylint@3.3.8
    - semgrep@1.78.0
    - trivy@0.66.0
    - eslint@8.57.0
    - sonarcsharp@9.32
```

## Key Findings
- **SonarC# Tool**: Added `sonarcsharp@9.32` for C# static analysis
- **Repository Setup**: Already configured in Codacy (provider: gh, organization: jessenaiman, repository: chapter-zero)
- **Analysis Trigger**: Requires pushing commits to GitHub for web-based analysis
- **File Analysis**: Use `codacy_get_file_with_analysis` with fileId (not path) after analysis completes

## Available Codacy Tools for C#
- **SonarC# (sonarcsharp)**: Primary C# static analyzer (enabled by default)
- **Semgrep**: Security and code quality rules
- **Trivy**: Security vulnerability scanning
- **Lizard**: Cyclomatic complexity analysis

## Usage Pattern
1. Modify C# files
2. Run `dotnet build` locally to verify compilation
3. Commit and push changes to trigger Codacy analysis
4. Use `codacy_get_file_with_analysis` with fileId to get quality metrics
5. Address any new issues found by analysis

## File IDs for Analysis
- GameState.cs: 170200132811
- Character.cs: 170200132800
- CharacterStats.cs: 170200132819
- PartyData.cs: 170200132834
- Enums.cs: 170200132828

## Quality Metrics Provided
- Grade (0-100) with letter (A, B, C, D, F)
- Total issues count
- Complexity score
- Duplication percentage
- Lines of code and commented lines

## Branch Analysis Status
- Default branch (003-update-omega-spiral): Fully analyzed
- Feature branches: Require push to trigger analysis
- Analysis typically completes within 2-3 minutes after push