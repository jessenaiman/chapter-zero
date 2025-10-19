#!/bin/bash

# 🎮 Omega Spiral Bug Squasher - Project Zero Mission
# A gamified script that outputs structured data for Qwen agents to track bug hunting progress

echo "🎮 THE BUG SQUASHER GAME BEGINS!"
echo "🎯 MISSION: REDUCE ALL CRITTERS TO ZERO"
echo "🔥 OBJECTIVE: PROJECT ZERO"
echo ""

# Function to run analysis and output structured JSON
run_analysis() {
    # Run dotnet build to get compiler errors/warnings
    BUILD_OUTPUT=$(dotnet build ../chapter-zero.csproj --no-incremental 2>&1 || true)
    
    # Count errors and warnings (ensure we get numeric values)
    ERROR_COUNT=$(echo "$BUILD_OUTPUT" | grep -c "error " 2>/dev/null | tr -d '\r\n' || echo "0")
    ERROR_COUNT=${ERROR_COUNT:-0}  # Default to 0 if empty
    ERROR_COUNT=$(echo $ERROR_COUNT | sed 's/[^0-9]*//g')  # Ensure it's just numbers
    
    WARNING_COUNT=$(echo "$BUILD_OUTPUT" | grep -c "warning " 2>/dev/null | tr -d '\r\n' || echo "0")
    WARNING_COUNT=${WARNING_COUNT:-0}  # Default to 0 if empty
    WARNING_COUNT=$(echo $WARNING_COUNT | sed 's/[^0-9]*//g')  # Ensure it's just numbers
    
    # Extract specific error and warning lines
    BUILD_ERRORS=$(echo "$BUILD_OUTPUT" | grep "error " 2>/dev/null || echo "")
    BUILD_WARNINGS=$(echo "$BUILD_OUTPUT" | grep "warning " 2>/dev/null || echo "")
    
    # Run dotnet format to check for style issues
    FORMAT_OUTPUT=$(dotnet format --verify-no-changes 2>&1 || echo "")
    FORMAT_COUNT=$(echo "$FORMAT_OUTPUT" | grep -c "Fixing\|formatted" 2>/dev/null | tr -d '\r\n' || echo "0")
    FORMAT_COUNT=${FORMAT_COUNT:-0}  # Default to 0 if empty
    FORMAT_COUNT=$(echo $FORMAT_COUNT | sed 's/[^0-9]*//g')  # Ensure it's just numbers
    
    # Run tests
    TEST_OUTPUT=$(dotnet test 2>&1 || true)
    TEST_FAILURES=$(echo "$TEST_OUTPUT" | grep -c "Failed" 2>/dev/null | tr -d '\r\n' || echo "0")
    TEST_FAILURES=${TEST_FAILURES:-0}  # Default to 0 if empty
    TEST_FAILURES=$(echo $TEST_FAILURES | sed 's/[^0-9]*//g')  # Ensure it's just numbers
    
    # Calculate total critters
    TOTAL_CRITTERS=$((ERROR_COUNT + WARNING_COUNT + FORMAT_COUNT + TEST_FAILURES))
    
    # Output gamified report
    echo "🔍 Running Code Analysis..."
    echo "📋 Building Project (Roslyn Compiler Analysis)..."
    echo "🎨 Checking Code Style (dotnet format)..."
    echo "🧪 Running Tests..."
    echo ""
    echo "🎯 === BUG SQUASHER MISSION REPORT ==="
    echo ""
    echo "🪲 BUILD ERRORS: $ERROR_COUNT"
    echo "⚠️  BUILD WARNINGS: $WARNING_COUNT" 
    echo "🎨 FORMAT ISSUES: $FORMAT_COUNT"
    echo "🧪 TEST FAILURES: $TEST_FAILURES"
    echo ""
    echo "💥 TOTAL CRITTERS IN THE NEST: $TOTAL_CRITTERS"
    echo ""
    
    if [ $TOTAL_CRITTERS -eq 0 ]; then
        echo "🎉 🏆 MISSION ACCOMPLISHED! PROJECT ZERO ACHIEVED! 🏆 🎉"
        echo "   All bugs have been exterminated! The codebase is pristine!"
        echo ""
        echo "   🎮 AGENT STATUS: MISSION COMPLETE"
        echo "   🎯 OBJECTIVE: ACHIEVED"
        echo "   💯 SCORE: PERFECT 100%"
    else
        echo "⚔️ CRITTER COUNT: $TOTAL_CRITTERS"
        echo "🔥 MISSION STATUS: ACTIVE"
        echo "💪 SQUASH UNTIL ZERO!"
        echo ""
        
        # Show progress and motivation
        if [ $TOTAL_CRITTERS -gt 40 ]; then
            echo "💪 HEAVY INFESTATION DETECTED! TIME TO ROLL UP YOUR SLEEVES!"
        elif [ $TOTAL_CRITTERS -gt 20 ]; then
            echo "🔥 WORKING HARD! KEEP SQUASHING THOSE CRITTERS!"
        elif [ $TOTAL_CRITTERS -gt 10 ]; then
            echo "⚡ GOOD PROGRESS! THE END IS NEAR!"
        elif [ $TOTAL_CRITTERS -gt 5 ]; then
            echo "🎯 ALMOST THERE! TAIL END OF THE BUG HUNT!"
        else
            echo "🎯 FINAL COUNTDOWN! SQUASH THE LAST FEW!"
        fi
        echo ""
    fi
    
    # Output structured JSON for Qwen agent
    echo "🤖 === QWEN AGENT DATA ==="
    cat << EOF
{
  "timestamp": "$(date -Iseconds)",
  "project": "Omega Spiral",
  "mission": "Project Zero",
  "total_critters": $TOTAL_CRITTERS,
  "breakdown": {
    "build_errors": $ERROR_COUNT,
    "build_warnings": $WARNING_COUNT,
    "format_issues": $FORMAT_COUNT,
    "test_failures": $TEST_FAILURES
  },
  "details": {
    "build_errors": [
$(echo "$BUILD_ERRORS" | sed 's/^/      "/; s/$/",/' | sed '$s/,$//' | sed 's/      "",//')
    ],
    "build_warnings": [
$(echo "$BUILD_WARNINGS" | sed 's/^/      "/; s/$/",/' | sed '$s/,$//' | sed 's/      "",//')
    ],
    "test_failures": [
$(echo "$TEST_OUTPUT" | grep "Failed" | sed 's/^/      "/; s/$/",/' | sed '$s/,$//' | sed 's/      "",//')
    ]
  },
  "status": "$([ $TOTAL_CRITTERS -eq 0 ] && echo "COMPLETE" || echo "ACTIVE")",
  "mission_log": [
    {
      "type": "build_warning",
      "count": $WARNING_COUNT,
      "message": "⚠️ Fix $WARNING_COUNT build warnings",
      "issues": [
$(echo "$BUILD_WARNINGS" | head -5 | sed 's/^/        "/; s/$/",/' | sed '$s/,$//' | sed 's/        "",//')
      ]
    },
    {
      "type": "motivation",
      "message": "$([ $TOTAL_CRITTERS -gt 40 ] && echo "💪 HEAVY INFESTATION DETECTED! TIME TO ROLL UP YOUR SLEEVES!" || [ $TOTAL_CRITTERS -gt 20 ] && echo "🔥 WORKING HARD! KEEP SQUASHING THOSE CRITTERS!" || [ $TOTAL_CRITTERS -gt 10 ] && echo "⚡ GOOD PROGRESS! THE END IS NEAR!" || [ $TOTAL_CRITTERS -gt 5 ] && echo "🎯 ALMOST THERE! TAIL END OF THE BUG HUNT!" || echo "🎯 FINAL COUNTDOWN! SQUASH THE LAST FEW!")",
      "timestamp": "$(date -Iseconds)"
    }
  ]
}
EOF
}

# Main execution
if [ ! -f "../chapter-zero.csproj" ]; then
    echo "❌ ERROR: chapter-zero.csproj not found!"
    echo "❌ Please run this script from the tools directory."
    exit 1
fi

run_analysis

# Exit with total count as exit code
exit $TOTAL_CRITTERS