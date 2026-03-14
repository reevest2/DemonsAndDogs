#!/bin/bash
# PostToolUse hook: auto-format C# files after Edit/Write
# Reads tool input from stdin, extracts file path, runs dotnet format if it's a .cs file

INPUT=$(cat)
FILE_PATH=$(echo "$INPUT" | python -c "import sys,json; d=json.load(sys.stdin); print(d.get('tool_input',{}).get('file_path',d.get('tool_input',{}).get('path','')))" 2>/dev/null)

if [[ "$FILE_PATH" == *.cs ]]; then
  dotnet format "$CLAUDE_PROJECT_DIR/reCurse.slnx" --include "$FILE_PATH" --verbosity quiet 2>/dev/null
fi

exit 0
