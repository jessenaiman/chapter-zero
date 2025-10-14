# Codacy Tools Usage Guide

## Correct Tool Usage with Serena Integration

### Codacy MCP Tools
- `codacy_list_repository_issues`: Gets all quality issues for repository
- `codacy_search_repository_srm_items`: Finds security risk management items
- `codacy_get_repository_with_analysis`: Provides repository-level metrics
- `codacy_list_files`: Lists files with quality metrics
- `codacy_get_file_with_analysis`: Detailed analysis for specific files
- `codacy_list_tools`: Available analysis tools
- `codacy_list_repository_tools`: Repository-specific tool configuration
- `codacy_get_pattern`: Pattern definitions
- `codacy_list_repository_tool_patterns`: Tool pattern configurations

### Parameters Always Required
- `provider`: "gh"
- `organization`: "jessenaiman"
- `repository`: "chapter-zero"

### Quality Metrics Interpretation
- Grade A: 90-100 (Excellent)
- Grade B: 80-89 (Good)
- Grade C: 70-79 (Needs improvement)
- Grade D: 60-69 (Poor)
- Grade F: <60 (Critical)

### Security Priorities
- Critical: Hardcoded passwords, injection vulnerabilities
- High: Authentication bypass, data exposure
- Medium: Weak encryption, improper error handling
- Low: Information disclosure, best practice violations

### File Analysis Workflow
1. Use `codacy_list_files` to identify problematic files
2. Use `codacy_get_file_with_analysis` for detailed metrics
3. Review issues and prioritize fixes
4. Monitor impact on overall repository grade
