#!/bin/bash

# Local Code Quality Analysis Script
# Uses dotnet build and SonarScanner for immediate feedback before pushing

set -e

echo "🔍 Running local code quality analysis..."
echo "========================================"

# Build the project first
echo "📦 Building project..."
if dotnet build; then
    echo "✅ Build successful"
else
    echo "❌ Build failed - fix compilation errors first"
    exit 1
fi

echo ""

# Run basic tests if they exist
echo "🧪 Running tests..."
echo "------------------"
if dotnet test --verbosity quiet --no-build; then
    echo "✅ Tests passed"
else
    echo "⚠️  Some tests failed - review above"
fi

echo ""

# Run SonarScanner analysis
echo "🎯 Running SonarScanner analysis..."
echo "-----------------------------------"

# Check if SonarQube server is running
if curl -s http://localhost:9000/api/system/status | grep -q '"status":"UP"'; then
    echo "✅ SonarQube server is running at localhost:9000"

    # Check if we can authenticate (server might need initial setup)
    if curl -s -u admin:admin http://localhost:9000/api/system/info >/dev/null 2>&1; then
        echo "✅ SonarQube authentication successful"

        # Run analysis with SonarQube server (using correct .NET prefixes and full path)
        if dotnet sonarscanner begin /s:"/home/adam/Dev/omega-spiral/chapter-zero/SonarQube.Analysis.xml" /k:"chapter-zero-local" /n:"Chapter Zero - Omega Spiral" /v:"1.0.0" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="admin" /d:sonar.password="admin" 2>/dev/null; then
            dotnet build --verbosity quiet
            if dotnet sonarscanner end /d:sonar.login="admin" /d:sonar.password="admin" 2>/dev/null; then
                echo "✅ SonarScanner analysis completed successfully"
                echo "📊 View results at: http://localhost:9000/dashboard?id=chapter-zero-local"
            else
                echo "⚠️  SonarScanner end failed - check server logs"
            fi
        else
            echo "⚠️  SonarScanner begin failed - check project configuration"
        fi
    else
        echo "⚠️  Cannot authenticate with SonarQube (default admin:admin may have changed)"
        echo "💡 Please ensure SonarQube is set up with admin/admin credentials"
        echo "   Or update the credentials in this script"
    fi
else
    echo "❌ SonarQube server not running at localhost:9000"
    echo "💡 To start SonarQube locally:"
    echo "   docker run -d --name sonarqube -p 9000:9000 -e SONAR_ES_BOOTSTRAP_CHECKS_DISABLE=true sonarqube:latest"
    echo "   Wait 1-2 minutes for startup, then re-run this script"
    exit 1
fi

echo ""

# Summary
echo "🎉 Local analysis complete!"
echo "📊 Summary:"
echo "   • Build: ✅ Passed"
echo "   • Tests: ⚠️  Check output above"
echo "   • Code Analysis: See SonarQube dashboard or local reports"
echo ""
echo "🚀 Ready to push? Run: git push origin 004-implement-omega-spiral"
echo "📈 Codacy analysis will run automatically after push"
