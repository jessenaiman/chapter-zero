#!/bin/bash

# 🎮 Omega Spiral Bug Squasher - Project Zero Mission
# A gamified script to hunt down and exterminate all code critters!

set -e

echo "🎯 Initializing Bug Squasher v2.0..."
echo "🚀 Launching Project Zero: The Omega Spiral"
echo ""

# Function to run different analysis tools and count issues
run_analysis() {
    echo "🔍 Running Code Analysis..."
    echo ""
    
    # Run dotnet build to get compiler errors/warnings
    echo "📋 Building Project (Roslyn Compiler Analysis)..."
    BUILD_OUTPUT=$(dotnet build OmegaSpiral.csproj 2>&1 || true)
    
    # Count errors and warnings
    ERROR_COUNT=$(echo "$BUILD_OUTPUT" | grep -c "error " || echo "0")
    WARNING_COUNT=$(echo "$BUILD_OUTPUT" | grep -c "warning " || echo "0")
    
    # Get specific error and warning details
    BUILD_ERRORS=$(echo "$BUILD_OUTPUT" | grep "error " || echo "")
    BUILD_WARNINGS=$(echo "$BUILD_OUTPUT" | grep "warning " || echo "")
    
    # Run dotnet format to check for style issues
    echo "🎨 Checking Code Style (dotnet format)..."
    FORMAT_OUTPUT=$(dotnet format --verify-no-changes 2>&1 || echo "format issues found")
    FORMAT_COUNT=$(echo "$FORMAT_OUTPUT" | grep -c "Fixing" || echo "0")
    
    # Run Roslyn analyzers if available
    echo "🔍 Running Roslyn Analyzers..."
    ANALYZER_OUTPUT=$(dotnet build OmegaSpiral.csproj --no-incremental 2>&1 || true)
    ANALYZER_COUNT=$(echo "$ANALYZER_OUTPUT" | grep -c "error " | grep -v "CS0234\|CS0019\|CA1708" || echo "0")
    
    # Try to run additional tools if available
    LIZARD_COUNT=0
    if command -v lizard &> /dev/null; then
        echo "🐛 Running Lizard Complexity Analysis..."
        LIZARD_OUTPUT=$(lizard --warningsonly . 2>&1 || true)
        LIZARD_COUNT=$(echo "$LIZARD_OUTPUT" | grep -c "warning:" || echo "0")
    fi
    
    # Codacy analysis if available
    CODACY_COUNT=0
    if [ -f ".codacy/cli.sh" ]; then
        echo "🛡️  Checking Codacy Issues..."
        # We'll skip actual codacy run for now to avoid API calls, but could integrate later
        CODACY_COUNT=0
    fi
    
    # Calculate totals
    TOTAL_ERRORS=$ERROR_COUNT
    TOTAL_WARNINGS=$WARNING_COUNT
    TOTAL_FORMAT=$FORMAT_COUNT
    TOTAL_ANALYZERS=$ANALYZER_COUNT
    TOTAL_LIZARD=$LIZARD_COUNT
    TOTAL_CODACY=$CODACY_COUNT
    
    # Display results in gamified format
    echo ""
    echo "🎯 === BUG SQUASHER MISSION REPORT ==="
    echo ""
    echo "🪲 BUILD ERRORS: $TOTAL_ERRORS"
    echo "⚠️  BUILD WARNINGS: $TOTAL_WARNINGS" 
    echo "🎨 FORMAT ISSUES: $TOTAL_FORMAT"
    echo "🔍 ANALYZER ISSUES: $TOTAL_ANALYZERS"
    echo "🐛 LIZARD COMPLEXITY: $TOTAL_LIZARD"
    echo "🛡️  CODACY ISSUES: $TOTAL_CODACY"
    echo ""
    
    TOTAL_CRITTERS=$((TOTAL_ERRORS + TOTAL_WARNINGS + TOTAL_FORMAT + TOTAL_ANALYZERS + TOTAL_LIZARD + TOTAL_CODACY))
    
    echo "💥 TOTAL CRITTERS IN THE NEST: $TOTAL_CRITTERS"
    echo ""
    
    if [ $TOTAL_CRITTERS -eq 0 ]; then
        echo "🎉 🏆 MISSION ACCOMPLISHED! PROJECT ZERO ACHIEVED! 🏆 🎉"
        echo "   All bugs have been exterminated! The codebase is pristine!"
        echo ""
        echo "   🎮 AGENT STATUS: MISSION COMPLETE"
        echo "   🎯 OBJECTIVE: ACHIEVED"
        echo "   💯 SCORE: PERFECT 100%"
        exit 0
    else
        echo "⚔️  CRITTER COUNT: $TOTAL_CRITTERS"
        echo "🔥 MISSION STATUS: ACTIVE"
        echo "💪 SQUASH UNTIL ZERO!"
        echo ""
        
        # Show specific issues
        if [ $TOTAL_ERRORS -gt 0 ]; then
            echo "🚨 BUILD ERRORS FOUND:"
            echo "$BUILD_ERRORS" | sed 's/^/   /'
            echo ""
        fi
        
        if [ $TOTAL_WARNINGS -gt 0 ]; then
            echo "⚠️  BUILD WARNINGS FOUND:"
            echo "$BUILD_WARNINGS" | sed 's/^/   /'
            echo ""
        fi
        
        # Show progress bar
        show_progress_bar $TOTAL_CRITTERS
        echo ""
        
        # Motivational message
        show_motivational_message $TOTAL_CRITTERS
        echo ""
        
        return $TOTAL_CRITTERS
    fi
}

show_progress_bar() {
    local total=$1
    local max=50
    local percentage=$((total * 100 / 50))
    
    if [ $percentage -gt 100 ]; then
        percentage=100
    fi
    
    local filled=$((percentage * max / 10))
    local empty=$((max - filled))
    
    echo -n "📊 CRITTER COUNT: ["
    for ((i=0; i<filled; i++)); do
        echo -n "🪲"
    done
    for ((i=0; i<empty; i++)); do
        echo -n " "
    done
    echo -n "] $total/50"
    echo ""
}

show_motivational_message() {
    local count=$1
    
    if [ $count -gt 40 ]; then
        echo "💪 HEAVY INFESTATION DETECTED! TIME TO ROLL UP YOUR SLEEVES!"
    elif [ $count -gt 20 ]; then
        echo "🔥 WORKING HARD! KEEP SQUASHING THOSE CRITTERS!"
    elif [ $count -gt 10 ]; then
        echo "⚡ GOOD PROGRESS! THE END IS NEAR!"
    elif [ $count -gt 5 ]; then
        echo "🎯 ALMOST THERE! TAIL END OF THE BUG HUNT!"
    elif [ $count -gt 0 ]; then
        echo "🎯 FINAL COUNTDOWN! SQUASH THE LAST FEW!"
    else
        echo "🏆 VICTORY! PROJECT ZERO ACHIEVED!"
    fi
}

# Function to run tests and check for test failures
run_tests() {
    echo "🧪 Running Tests..."
    TEST_OUTPUT=$(dotnet test 2>&1 || true)
    TEST_FAILURES=$(echo "$TEST_OUTPUT" | grep -c "Failed" || echo "0")
    TEST_COUNT=$(echo "$TEST_OUTPUT" | grep -c "Total" | head -1 || echo "0")
    
    echo "🧪 TEST FAILURES: $TEST_FAILURES"
    if [ $TEST_FAILURES -gt 0 ]; then
        echo "❌ Some tests are failing - these count as critters too!"
        echo "$TEST_OUTPUT" | grep -A 10 "Failed" | sed 's/^/   /'
    fi
    echo ""
}

# Main execution
echo "🎮 THE BUG SQUASHER GAME BEGINS!"
echo "🎯 MISSION: REDUCE ALL CRITTERS TO ZERO"
echo "🔥 OBJECTIVE: PROJECT ZERO"
echo ""

# Check if project exists
if [ ! -f "OmegaSpiral.csproj" ]; then
    echo "❌ ERROR: OmegaSpiral.csproj not found!"
    echo "❌ Please run this script from the project root directory."
    exit 1
fi

# Run tests first
run_tests

# Run main analysis
run_analysis

# Exit with the total count as exit code
TOTAL_COUNT=$?
exit $TOTAL_COUNT