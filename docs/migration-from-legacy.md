# Migration from the legacy application

The original application targeted .NET Framework 4.5.2 and combined console
input, validation, random assignment, and presentation in one Windows-oriented
project. The modernization preserves the personal fantasy-football purpose and
the complete Git history while replacing that architecture with .NET 10 Core,
CLI, and test projects.

## Current-tree retirement

After the modern solution built and its full test suite passed independently,
the following superseded paths were removed from the current tree:

- `HyphyOregonConferences.sln`
- `HyphyOregonConferences/App.config`
- `HyphyOregonConferences/HyphyOregonConferences.csproj`
- `HyphyOregonConferences/Program.cs`
- `HyphyOregonConferences/Properties/AssemblyInfo.cs`
- `HyphyOregonConferences/favicon.ico`

No modern project, test command, CI workflow, documentation command, or
packaging step uses those files. They remain recoverable from repository
history. The annotated local tag `legacy-dotnet-framework-4.5.2` identifies the
exact baseline commit without rewriting any historical commit.

The legacy executable was neither migrated into the modern tree nor used by
the modern build and packaging process.
