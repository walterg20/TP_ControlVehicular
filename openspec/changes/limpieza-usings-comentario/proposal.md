# Proposal: Clean Unused Usings and Add Author Comment

## Why
The codebase has accumulated unused `using` directives across many `.cs` files, increasing noise and compilation warnings. Additionally, there is no consistent author marker in source files.

## What
1. Remove only unused `using` directives from every `.cs` file (keep all used ones).
2. Add a single comment `// walterg20` at the top of every `.cs` file if not already present.
3. Do not alter any other code or behavior.

## Impact
- All `.cs` files under the solution.
- Build must remain green: `dotnet build "TP_ControlVehicular.slnx"`.
