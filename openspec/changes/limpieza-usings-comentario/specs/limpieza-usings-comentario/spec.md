# Specification: Clean Unused Usings and Add Author Comment

## Capability: `code-hygiene`

### Scenario 1: Remove unused using directives
```gherkin
Given a .cs file with unused using directives
When the cleanup runs
Then only unused using directives are removed
And all used using directives remain
And the file still compiles
```

### Scenario 2: Add author comment
```gherkin
Given a .cs file without the comment "// walterg20"
When the cleanup runs
Then the comment "// walterg20" is added at the top of the file
And existing content is preserved
```

### Scenario 3: Idempotency
```gherkin
Given a .cs file already containing "// walterg20"
When the cleanup runs again
Then no duplicate comment is added
```

### Scenario 4: Build verification
```gherkin
Given all .cs files have been processed
When building the solution
Then `dotnet build "TP_ControlVehicular.slnx"` completes with 0 errors
```
