# Bullet Girls Phantasia Trainer

A memory trainer for the game *Bullet Girls Phantasia* (PC version).

## Features
- Infinite Health
- Infinite Ammo
- One-Hit Kill
- Speed Hack

## Build & Run

```bash
dotnet build
dotnet run --project src/Trainer.CLI
```

## Requirements
- .NET 8.0 SDK
- Windows (uses Win32 API for memory manipulation)

## Project Structure
- `src/Trainer.Core/` - Core logic (memory reading/writing, pattern scanning)
- `src/Trainer.CLI/` - Console-based trainer interface
- `tests/Trainer.Core.Tests/` - Unit tests
