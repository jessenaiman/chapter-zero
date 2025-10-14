#!/bin/bash

# Comprehensive Coverage Report Generation Script
# Generates coverage reports in multiple formats for Codacy and SonarQube

set -e

echo "📊 Setting up code coverage reports..."
echo "====================================="

# Clean previous coverage reports
echo "🧹 Cleaning previous coverage reports..."
rm -rf coverage/
mkdir -p coverage/

echo "✅ Cleaned previous reports"
echo ""

# Build the project first
echo "📦 Building project..."
if dotnet build --verbosity quiet; then
    echo "✅ Build successful"
else
    echo "❌ Build failed - fix compilation errors first"
    exit 1
fi

echo ""

# Run tests with coverage collection
echo "🧪 Running tests with coverage..."
echo "----------------------------------"

# Run tests with Coverlet to collect coverage data
# This will generate coverage reports in multiple formats as configured in .csproj
if dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage/test-results --verbosity quiet; then
    echo "✅ Tests completed successfully"
else
    echo "❌ Tests failed - fix test errors first"
    exit 1
fi

echo ""

# Generate additional coverage formats if needed
echo "📋 Generating coverage reports..."
echo "---------------------------------"

# The coverlet collector should have already generated the reports based on .csproj configuration
# Let's verify what was generated and copy to expected locations for SonarQube
if [ -d "coverage/" ]; then
    echo "📁 Coverage reports generated in: $(pwd)/coverage/"

    # List generated files
    echo "📄 Generated coverage files:"
    ls -la coverage/ || echo "No coverage files found"

    # Copy OpenCover format for SonarQube (if it exists)
    if [ -f "coverage/coverage.opencover.xml" ]; then
        cp coverage/coverage.opencover.xml coverage.xml
        echo "✅ Copied OpenCover report for SonarQube: coverage.xml"
    fi

    # Show coverage statistics if available
    if command -v dotnet-coverage &> /dev/null; then
        echo ""
        echo "📊 Coverage Summary:"
        echo "-------------------"
        dotnet-coverage merge coverage/test-results/*.xml -o coverage/merged.coverage.xml -f cobertura
        echo "Coverage reports ready for:"
        echo "  • SonarQube: coverage.xml (OpenCover format)"
        echo "  • Codacy: coverage.cobertura.xml (Cobertura format)"
        echo "  • General: coverage.lcov.info (LCOV format)"
    fi
else
    echo "❌ Coverage directory not found"
    exit 1
fi

echo ""
echo "🎉 Coverage setup complete!"
echo "==========================="
echo "📊 Coverage reports are ready at: $(pwd)/coverage/"
echo ""
echo "📋 Next steps:"
echo "  • For SonarQube: Run SonarScanner analysis"
echo "  • For Codacy: Reports will be uploaded automatically via GitHub Actions"
echo "  • For local viewing: Open coverage/coverage.cobertura.xml in coverage tools"
echo ""
echo "🚀 Ready to run: ./analyze-code.sh"
