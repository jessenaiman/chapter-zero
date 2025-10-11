# Codacy Local Analysis Workflow

## Correct Understanding of Codacy Usage

**Codacy is NOT a standalone CLI tool** - it operates through MCP (Model Context Protocol) tools only. The MCP tools provide access to Codacy's analysis capabilities.

## Local Pre-Push Workflow

### 1. **Local Quality Checks (Before Push)**
Use MCP Codacy tools locally to analyze code before committing:

```bash
# Check specific file analysis
codacy_get_file_with_analysis(fileId: "170200132811", ...)

# Run CLI analysis on current code
codacy_cli_analyze(rootPath: "/path/to/project", tool: "sonarcsharp")

# Analyze security vulnerabilities
codacy_cli_analyze(rootPath: "/path/to/project", tool: "trivy")
```

### 2. **Available MCP Tools**
- `mcp_codacy_mcp_se_codacy_cli_analyze` - Run analysis on files/code
- `mcp_codacy_mcp_se_codacy_get_file_with_analysis` - Get detailed metrics for analyzed files
- `mcp_codacy_mcp_se_codacy_list_files` - List files with their analysis status
- `mcp_codacy_mcp_se_codacy_get_repository_with_analysis` - Get repo-wide analysis

### 3. **Analysis Types Available**
- **SonarC# (sonarcsharp)**: C# code quality, bugs, maintainability
- **Trivy**: Security vulnerabilities, dependencies
- **Semgrep**: Security rules, code patterns
- **Lizard**: Cyclomatic complexity
- **ESLint**: JavaScript/TypeScript (if applicable)

## Workflow Integration

### Pre-Push Checklist:
1. ✅ **Build succeeds**: `dotnet build`
2. ✅ **Tests pass**: `dotnet test`
3. ✅ **Codacy analysis**: Run MCP tools above
4. ✅ **No new issues**: Compare with baseline analysis
5. ✅ **Push to GitHub**: Triggers full web analysis

### Post-Push Analysis:
- Codacy web interface provides detailed reports
- PR comments with analysis results
- Quality gates and blocking rules

## Configuration

**Codacy is configured via `.codacy/codacy.yaml`**:
```yaml
tools:
  - sonarcsharp@9.32    # C# analysis
  - trivy@0.66.0       # Security
  - semgrep@1.78.0     # Code patterns
  - lizard@1.17.31     # Complexity
```

## Key Points

- **Local analysis**: Use MCP tools before pushing
- **Web analysis**: Automatic after push to GitHub
- **No standalone CLI**: Must use MCP interface
- **File IDs**: Required for `get_file_with_analysis` (get from `list_files`)
- **Branch analysis**: Requires push to trigger web analysis

## Quality Gates

- **Grade A**: 90-100 (Excellent)
- **Grade B**: 80-89 (Good)
- **Grade C**: 70-79 (Needs improvement)
- **Grade D-F**: <70 (Action required)

## Integration with Development

- **Before commit**: Run local MCP analysis
- **After push**: Review Codacy web reports
- **PR reviews**: Codacy comments on changes
- **CI/CD**: Can block merges based on quality gates
