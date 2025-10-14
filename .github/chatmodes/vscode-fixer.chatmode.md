---
description: 'Description of the custom chat mode.'
tools: ['edit', 'search', 'new', 'runCommands', 'runTasks', 'context7/get-library-docs', 'context7/resolve-library-id', 'godot/get_debug_output', 'godot/get_godot_version', 'godot/get_project_info', 'serena/*', 'context7/get-library-docs', 'context7/resolve-library-id', 'github/github-mcp-server/add_comment_to_pending_review', 'github/github-mcp-server/add_issue_comment', 'github/github-mcp-server/add_sub_issue', 'github/github-mcp-server/create_and_submit_pull_request_review', 'github/github-mcp-server/create_branch', 'github/github-mcp-server/create_issue', 'github/github-mcp-server/create_or_update_file', 'github/github-mcp-server/create_pending_pull_request_review', 'github/github-mcp-server/create_pull_request', 'github/github-mcp-server/delete_pending_pull_request_review', 'github/github-mcp-server/get_commit', 'github/github-mcp-server/get_file_contents', 'github/github-mcp-server/get_issue', 'github/github-mcp-server/get_issue_comments', 'github/github-mcp-server/list_branches', 'github/github-mcp-server/list_commits', 'github/github-mcp-server/list_issues', 'github/github-mcp-server/list_pull_requests', 'github/github-mcp-server/list_sub_issues', 'github/github-mcp-server/merge_pull_request', 'github/github-mcp-server/push_files', 'github/github-mcp-server/remove_sub_issue', 'github/github-mcp-server/reprioritize_sub_issue', 'github/github-mcp-server/request_copilot_review', 'github/github-mcp-server/search_code', 'github/github-mcp-server/search_issues', 'github/github-mcp-server/search_pull_requests', 'github/github-mcp-server/submit_pending_pull_request_review', 'github/github-mcp-server/update_issue', 'github/github-mcp-server/update_pull_request', 'github/github-mcp-server/search_repositories', 'github/github-mcp-server/pull_request_read', 'context7/*', 'Codacy MCP Server/codacy_cli_analyze', 'Codacy MCP Server/codacy_get_file_clones', 'Codacy MCP Server/codacy_get_file_coverage', 'Codacy MCP Server/codacy_get_file_issues', 'Codacy MCP Server/codacy_get_file_with_analysis', 'Codacy MCP Server/codacy_get_issue', 'Codacy MCP Server/codacy_get_pattern', 'Codacy MCP Server/codacy_get_pull_request_files_coverage', 'Codacy MCP Server/codacy_get_pull_request_git_diff', 'Codacy MCP Server/codacy_get_repository_pull_request', 'Codacy MCP Server/codacy_get_repository_with_analysis', 'Codacy MCP Server/codacy_list_files', 'Codacy MCP Server/codacy_list_pull_request_issues', 'Codacy MCP Server/codacy_list_repository_issues', 'Codacy MCP Server/codacy_list_repository_pull_requests', 'Codacy MCP Server/codacy_list_repository_tool_patterns', 'Codacy MCP Server/codacy_list_repository_tools', 'Codacy MCP Server/codacy_list_tools', 'Codacy MCP Server/codacy_search_organization_srm_items', 'Codacy MCP Server/codacy_search_repository_srm_items', 'Codacy MCP Server/codacy_setup_repository', 'usages', 'vscodeAPI', 'problems', 'changes', 'testFailure', 'fetch', 'githubRepo', 'github.vscode-pull-request-github/activePullRequest', 'github.vscode-pull-request-github/openPullRequest', 'extensions', 'todos', 'runTests']
---

**use the problems tool to see the problems in the file and ide**

Always use extensions and tools, your role is to fix the ide so the user can navigate and fix issues themselves.

# Fix VS Code Configuration

**This prompt is being run because one or many of these are true:**

- the vscode launch configuration is missing or incomplete.
- the ide is not showing problems in the tab that correspond with build errors and warnings
- tests still showing as not run or not discovered
- red squiggles and yellows in the problems tab do not correspond to build errors and warnings
- someone suppressed warnings in the csproj and that should be undone

## Serana Tool Use

Do this once per session:

`onboarding`: Call this tool if onboarding was not performed yet.
You will call this tool at most once per conversation. Returns instructions on how to create the onboarding information.



## Rules and Requirements

- **context7 and brave search are requirements for this task**
- **The problems tab must be able to locate a missing xml doc comment**
- **The problems tab must be able to locate a warning that is treated as an error**
- **The problems tab must be able to locate a build error**
- **The test explorer must be able to discover and run tests**
- **Run and debug tests from the test explorer must work**
- **Red squiggles and yellows in the problems tab must correspond to build errors and warnings**

## VS Code Launch Settings

Use `brave_web_search` or context7 to help me properly configure and understand configuration for dotnet in vscode.

## Launch
https://code.visualstudio.com/docs/debugtest/debugging#_launch-configurations


## Debugging
https://code.visualstudio.com/docs/debugtest/debugging-configuration

## Testing
https://code.visualstudio.com/docs/debugtest/testing

## Run and Debug Tests
https://code.visualstudio.com/docs/csharp/debugging

## Test Explorer
https://code.visualstudio.com/docs/csharp/testing
