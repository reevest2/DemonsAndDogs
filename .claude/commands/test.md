Run the test suite and provide a clear summary.

1. Run `dotnet test tests/ --verbosity normal` to execute all tests.

2. Parse the output and provide a summary:

   ## Test Results
   - **Passed**: [count]
   - **Failed**: [count]
   - **Skipped**: [count]
   - **Total**: [count]
   - **Duration**: [time]

3. If any tests failed, for each failure:
   - **Test**: full test name
   - **Error**: the assertion or exception message
   - **Suggestion**: brief analysis of what likely went wrong

4. If all tests passed, confirm with a brief summary.

If $ARGUMENTS is provided, pass it as a filter to dotnet test (e.g., `/test UserService` runs `dotnet test tests/ --filter "UserService"`).
