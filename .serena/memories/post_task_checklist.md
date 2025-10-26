# Post-Task Checklist for Omega Spiral Development

After completing any code changes, follow this checklist to ensure quality:

## 1. Build Verification
- Run `dotnet build --no-restore` to ensure no compilation errors
- Check for any warnings in the build output

## 2. Test Execution
- Run `dotnet test --no-build --settings .runsettings` to execute all tests
- Review test results in TestResults/ directory
- Ensure no tests are failing (check test-result.trx)

## 3. Code Quality Checks
- Use VS Code Problems tab to check for any errors/warnings
- Run `dotnet format --verify-no-changes` if formatting hook is enabled
- Ensure XML documentation is complete for public APIs

## 4. Godot-Specific Validation
- Open Godot editor and verify scenes load without errors
- Test gameplay if UI or core mechanics were changed
- Check console output for runtime warnings

## 5. Pre-Commit Validation
- Ensure pre-commit hooks pass (if enabled):
  - Code formatting check
  - Static analysis (warnings as errors)
  - Unit tests
  - Security scan (optional)

## 6. Documentation Updates
- Update README.md if setup or usage changed
- Update code comments if logic became more complex
- Check if ADR documents need updates

## 7. Cross-Platform Testing
- Verify on target platforms (Linux primary, Windows/Mac secondary)
- Test with different Godot versions if applicable

## 8. Performance/Security Review
- Check for performance regressions
- Ensure no security vulnerabilities introduced
- Review memory usage for large assets

## 9. Integration Testing
- Test with AI/LLM features if narrative code changed
- Verify save/load functionality
- Check UI responsiveness

## 10. Final Commit
- Write descriptive commit message
- Push to feature branch
- Create PR with summary of changes

**Critical**: Never commit code with failing tests, build errors, or critical security issues.