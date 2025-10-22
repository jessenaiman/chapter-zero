# ⚡ TDD Workflow - Quick Reference

## 🔄 RED → GREEN → REFACTOR

### 🔴 RED: Write Failing Tests
- ✏️ **1 assertion per test** (SRP)
- 📝 **Name format**: `Subject_Verb_Expectation()`
- 🎯 **AAA pattern**: Arrange → Act → Assert

### 🟢 GREEN: Minimal Code
- 📦 **Only code tests require**
- 🚫 **No over-engineering**
- 📄 **Add XML docs** for public members

### OMEGA AGENT CHECK: Call [gdunit4_game_agent]/home/adam/Dev/omega-spiral/chapter-zero/.agents/gdunit4_game_agent.py
- Tell him what test has passed and in what file with a brief summary of what you believe to be true based on the passing test
- Ensure that the test has a screenshot created of the ui and our agent will evaluate the image and provide feedback
- Read the response from the agent into the chat log and then make changes according to the agents code review that is returned (it can take a minute or more)

### 🔵 REFACTOR: Improve Design
- 🧹 **Clean up** (tests still pass)
- 🎨 **Remove duplication**
- ⚡ **Optimize if needed**

---

## 📋 Test Template

```csharp
[TestCase]
[RequireGodotRuntime]
public void Subject_Verb_Expectation()
{
    // ARRANGE
    var menu = new StageSelectMenu();
    AutoFree(menu);

    // ACT
    var result = menu.GetStageStatus(1);

    // ASSERT (exactly ONE assertion)
    AssertBool(result.IsReady).IsTrue();
}
```

---

## 🔧 Common Assertions

| Code | What It Does |
|------|---|
| `AssertBool(x).IsTrue()` | Check boolean |
| `AssertString(x).IsEqual("val")` | Check string |
| `AssertObject(x).IsEqual(expected)` | Check enum/object |
| `AssertObject(x).IsNotNull()` | Check exists |
| `AssertInt(x).IsEqual(42)` | Check number |

---

## 🏃 Run Tests

```bash
# All tests
dotnet test

# Specific class
dotnet test --filter "FullyQualifiedName~StageSelectMenuTests"

# Specific test
dotnet test --filter "FullyQualifiedName~StageSelectMenuTests.Stage1_ReportsReadyStatus"

# Watch mode
dotnet watch test
```

---

## 📂 File Structure

| Type | Location | Example |
|------|----------|---------|
| Source | `source/scripts/ui/menus/` | `StageSelectMenu.cs` |
| Tests | `tests/ui/menus/` | `StageSelectMenuTests.cs` |

---

## 📖 See Also
- GdUnit4 API: `.github/instructions/gdUnit4Net-API.mdx`
- Test Adapter: `.github/instructions/gdUnit4Net-TestAdapter.md`
