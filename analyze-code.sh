#!/bin/bash

# SonarC# Local Analysis Script
# Run this before pushing to check code quality

set -e

echo "🔍 Running SonarC# analysis..."

# Build the project first
echo "📦 Building project..."
dotnet build

# Run SonarScanner analysis
echo "🎯 Starting SonarC# analysis..."
dotnet sonarscanner begin \
    /k:"chapter-zero" \
    /d:sonar.host.url="https://sonarcloud.io" \
    /d:sonar.login="${SONAR_TOKEN:-}" \
    /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" \
    /d:sonar.exclusions="**/*.Generated.cs,**/*.Designer.cs,**/*.g.cs"

# Build with coverage (if dotnet-coverage is available)
if command -v dotnet-coverage &> /dev/null; then
    echo "📊 Running tests with coverage..."
    dotnet-coverage collect "dotnet test" -f opencover -o coverage.opencover.xml
else
    echo "⚠️  dotnet-coverage not found, running tests without coverage..."
    dotnet test
fi

# End analysis
dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN:-}"

echo "✅ SonarC# analysis complete!"
echo "📋 Check results at: https://sonarcloud.io/dashboard?id=chapter-zero"