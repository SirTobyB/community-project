#!/usr/bin/env bash

dotnet jb cleanupcode CommunityProject.sln --profile="Unity: Full Cleanup" --include="**/_Game/Scripts/**/*.cs" -eXtensions="JetBrains.Unity"