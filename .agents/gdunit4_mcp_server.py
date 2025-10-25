#!/usr/bin/env python3
"""
GDUnit4 MCP Server Integration
Registers GDUnit4 test validation as an MCP tool for GitHub Copilot integration.
"""

import json
from typing import Any
from gdunit4_game_agent import GDUnit4TestValidator, create_gdunit4_tools


class GDUnit4MCPServer:
    """
    <summary>
    MCP Server wrapper for GDUnit4 test validation.
    Provides tools accessible to GitHub Copilot and other MCP clients.
    </summary>
    """
    
    def __init__(self):
        """Initialize the MCP server with GDUnit4 validator."""
        self.validator = GDUnit4TestValidator()
        self.tools = create_gdunit4_tools()
    
    def get_tools(self) -> dict[str, Any]:
        """
        <summary>
        Returns MCP tool definitions for GDUnit4 validation.
        </summary>
        
        <returns>Dictionary of available MCP tools</returns>
        """
        return self.tools
    
    def validate_test(self, test_file_path: str, query: str, rerun_test: bool = False) -> dict[str, Any]:
        """
        <summary>
        Validates a GDUnit4 test file using AI analysis with optional test execution.
        </summary>
        
        <param name="test_file_path">Absolute path to the C# test file</param>
        <param name="query">Validation query (e.g., "Does this test follow best practices?")</param>
        <param name="rerun_test">If true, reruns the test and analyzes results</param>
        
        <returns>Analysis result with findings, screenshots, and test execution status</returns>
        """
        return self.validator.analyze_test(test_file_path, query, rerun_test)


def get_mcp_tool_definitions() -> dict[str, Any]:
    """
    <summary>
    Returns tool definitions compatible with MCP protocol for GitHub Copilot.
    Includes support for screenshot analysis, test execution, and visual validation.
    </summary>
    
    <returns>MCP-compatible tool definitions</returns>
    """
    return {
        'validate_gdunit4_test': {
            'description': 'Validate a GDUnit4 test file with AI analysis, visual confirmation via screenshots, and optional test execution. Analyzes code quality, best practices compliance, and provides specific recommendations.',
            'inputSchema': {
                'type': 'object',
                'properties': {
                    'test_file_path': {
                        'type': 'string',
                        'description': 'Absolute path to the C# test file to validate (e.g., /home/adam/Dev/omega-spiral/chapter-zero/tests/ui/MenuUiTests.cs)'
                    },
                    'query': {
                        'type': 'string',
                        'description': 'Validation question or query (e.g., "Does this test follow GDUnit4 best practices?", "Verify XML documentation compliance", "Is this test comprehensive?")'
                    },
                    'rerun_test': {
                        'type': 'boolean',
                        'description': 'If true, reruns the test to capture new screenshots and verify current status (optional, default false)'
                    }
                },
                'required': ['test_file_path', 'query']
            }
        }
    }


if __name__ == '__main__':
    import sys
    
    server = GDUnit4MCPServer()
    
    # If called with arguments, run validation
    if len(sys.argv) > 2:
        test_file = sys.argv[1]
        rerun = '--rerun' in sys.argv
        query_args = [arg for arg in sys.argv[2:] if arg != '--rerun']
        query = ' '.join(query_args)
        result = server.validate_test(test_file, query, rerun_test=rerun)
        print(json.dumps(result, indent=2))
    else:
        # Print available tools
        print("GDUnit4 MCP Server - Test Validation Tools")
        print("=" * 60)
        print(json.dumps(get_mcp_tool_definitions(), indent=2))
        print("\n" + "=" * 60)
        print("Usage: python3 gdunit4_mcp_server.py [--rerun] <test_file> <query>")
        print("\nExample:")
        print("  python3 gdunit4_mcp_server.py tests/ui/MenuUiTests.cs 'Analyze test quality'")
        print("  python3 gdunit4_mcp_server.py --rerun tests/ui/MenuUiTests.cs 'Verify tests pass'")
