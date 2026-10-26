import 'dotnet.justfile'

format: (format-solution "NorthSouthSystems.Opinions.slnx")

integrate: (coverage-summary "Release")

deploy: (publish "https://api.nuget.org/v3/index.json")
