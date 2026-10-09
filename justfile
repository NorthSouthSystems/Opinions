import 'dotnet.justfile'

format: (format-solution "NorthSouthSystems.Opinions.slnx")

deploy: tools (publish "https://api.nuget.org/v3/index.json")
