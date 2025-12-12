# Tetris Game (WPF, .NET 8)

Simple Tetris implementation for Windows (WPF, .NET 8).

## Requirements
- .NET SDK 8.0+
- Windows 10/11

## Run
```powershell
dotnet build
dotnet run --project Tetris_Game.csproj
```

## Controls
- Left/Right: Move
- Up: Rotate
- Down: Soft drop
- Space: Hard drop
- P: Pause

## Structure
- `models/`: grid, blocks, queue, game state
- `views/`: WPF views and code-behind
- `resources/`: images, audio
- `DatabaseManager.cs`, `DBTetris.sql`: highscore


