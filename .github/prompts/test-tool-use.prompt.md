**Instructions:**

1. **Gather Error Reports:**
   - Open the Problems tab in VS Code and the latest test result file (e.g., test-result.html or `.trx`).
   - List every error, warning, and failed test, including file names and line numbers.

2. **Analyze and Plan:**
   - For each error, identify the root cause (naming, missing XML docs, symbol resolution, test assertion failure, etc.).
   - Group similar errors to avoid redundant fixes (e.g., all naming violations, all missing comments).

3. **Fix in Bulk:**
   - Edit files to resolve all issues in one pass:
     - Rename classes, methods, and fields to match naming conventions.
     - Add required XML documentation comments.
     - Remove unused usings.
     - Correct test assertions and logic based on failure messages.
     - Address any Godot-specific issues (e.g., missing nodes, incorrect scene paths).
   - For test failures, use the error message and stack trace to directly address the cause.

4. **Validate Once:**
   - After all fixes, run a single build and test cycle.
   - Review the Problems tab and test results to confirm all errors are resolved.

5. **Report:**
   - Summarize what was fixed, referencing the original error list.
   - If any errors remain, repeat only for those specific issues.

**Key Principles:**
- Do not repeat build/test cycles for each individual fix—batch all changes first.
- Use the Problems tab and test result files as your authoritative source for errors.
- Minimize context usage by grouping and fixing similar issues together.
- Only check results after all fixes are applied.

---

This approach ensures you fix all errors efficiently, without wasting time or context on repeated cycles.
