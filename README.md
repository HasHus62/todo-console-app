# ✅ TaskManager CLI

A clean, modern C# console application for managing to-do tasks. Features full CRUD operations, JSON file persistence, priority levels, and a color-coded terminal UI.

## ✨ Features

- **CRUD Operations** — Create, Read, Update, Delete tasks
- **Priority Levels** — High, Normal, Low (color-coded)
- **Status Tracking** — Mark tasks as done/pending
- **JSON Persistence** — Tasks saved to `tasks.json`
- **Color UI** — Terminal output with color-coded priorities and status
- **Filters** — List all, pending, or completed tasks
- **Clean Architecture** — OOP design with separate service layer

## 🛠 Tech Stack

- C# (.NET 8.0)
- System.Text.Json for serialization
- Console color formatting

## 📋 Commands

| Command | Description |
|---------|-------------|
| `add` | Add a new task (prompts for title and priority) |
| `list` | Show all tasks |
| `list pending` | Show only pending tasks |
| `list done` | Show only completed tasks |
| `done <id>` | Mark task as completed |
| `update <id>` | Update task title |
| `delete <id>` | Delete a task |
| `help` | Show all commands |
| `exit` | Quit the application |

## 🚀 Getting Started

```bash
# Clone the repository
git clone https://github.com/HasHus62/todo-console-app.git
cd todo-console-app

# Build and run
dotnet run
```

## 📂 Project Structure

- `Program.cs` — Main entry point and CLI interface
- `TaskItem` — Task data model
- `TaskService` — Business logic and persistence
- `TaskManager.csproj` — .NET project file
- `tasks.json` — Auto-generated storage file (gitignored)

## 📄 License

MIT License
