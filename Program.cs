using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TaskManager
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Priority { get; set; } = "Normal";
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class TaskService
    {
        private readonly string _filePath;
        private List<TaskItem> _tasks;
        private int _nextId;

        public TaskService(string filePath = "tasks.json")
        {
            _filePath = filePath;
            _tasks = LoadTasks();
            _nextId = _tasks.Count > 0 ? _tasks[^1].Id + 1 : 1;
        }

        public void AddTask(string title, string priority)
        {
            var task = new TaskItem { Id = _nextId++, Title = title, Priority = priority, IsCompleted = false, CreatedAt = DateTime.Now };
            _tasks.Add(task);
            SaveTasks();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  Task #{task.Id} added: \"{title}\" [{priority}]");
            Console.ResetColor();
        }

        public void ListTasks(string filter = "all")
        {
            var filtered = filter switch { "done" => _tasks.FindAll(t => t.IsCompleted), "pending" => _tasks.FindAll(t => !t.IsCompleted), _ => _tasks };
            if (filtered.Count == 0) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("\n  No tasks found."); Console.ResetColor(); return; }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  +----+----------------------------------+----------+--------+");
            Console.WriteLine("  | ID | Title                            | Priority | Status |");
            Console.WriteLine("  +----+----------------------------------+----------+--------+");
            Console.ResetColor();
            foreach (var task in filtered)
            {
                string status = task.IsCompleted ? "  Done " : "  Open ";
                string title = task.Title.Length > 32 ? task.Title[..29] + "..." : task.Title.PadRight(32);
                string priority = task.Priority.PadRight(8);
                Console.ForegroundColor = task.IsCompleted ? ConsoleColor.DarkGray : ConsoleColor.White;
                Console.Write($"  | {task.Id,2} | {title} | ");
                Console.ForegroundColor = task.Priority switch { "High" => ConsoleColor.Red, "Low" => ConsoleColor.DarkGray, _ => ConsoleColor.Yellow };
                Console.Write($"{priority}");
                Console.ForegroundColor = task.IsCompleted ? ConsoleColor.Green : ConsoleColor.White;
                Console.WriteLine($" |{status}|");
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  +----+----------------------------------+----------+--------+");
            Console.ResetColor();
            Console.WriteLine($"  Total: {filtered.Count}  |  Done: {_tasks.FindAll(t => t.IsCompleted).Count}  |  Pending: {_tasks.FindAll(t => !t.IsCompleted).Count}");
        }

        public void CompleteTask(int id)
        {
            var task = _tasks.Find(t => t.Id == id);
            if (task == null) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"\n  Task #{id} not found."); Console.ResetColor(); return; }
            task.IsCompleted = true; task.CompletedAt = DateTime.Now; SaveTasks();
            Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"\n  Task #{id} completed: \"{task.Title}\""); Console.ResetColor();
        }

        public void DeleteTask(int id)
        {
            var task = _tasks.Find(t => t.Id == id);
            if (task == null) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"\n  Task #{id} not found."); Console.ResetColor(); return; }
            _tasks.Remove(task); SaveTasks();
            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"\n  Task #{id} deleted: \"{task.Title}\""); Console.ResetColor();
        }

        public void UpdateTask(int id, string newTitle)
        {
            var task = _tasks.Find(t => t.Id == id);
            if (task == null) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine($"\n  Task #{id} not found."); Console.ResetColor(); return; }
            string old = task.Title; task.Title = newTitle; SaveTasks();
            Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine($"\n  Task #{id} updated: \"{old}\" -> \"{newTitle}\""); Console.ResetColor();
        }

        private List<TaskItem> LoadTasks()
        {
            if (!File.Exists(_filePath)) return new List<TaskItem>();
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
        }

        private void SaveTasks()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_tasks, options));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var service = new TaskService();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  === TaskManager CLI v1.0 ===");
            Console.WriteLine("  Type 'help' for commands.\n");
            Console.ResetColor();

            bool running = true;
            while (running)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("\n  taskmanager> ");
                Console.ResetColor();
                string input = Console.ReadLine()?.Trim() ?? "";
                string[] parts = input.Split(' ', 2);
                string cmd = parts[0].ToLower();

                switch (cmd)
                {
                    case "add":
                        Console.Write("  Title: "); string title = Console.ReadLine()?.Trim() ?? "";
                        if (string.IsNullOrEmpty(title)) { Console.WriteLine("  Title required."); break; }
                        Console.Write("  Priority (High/Normal/Low) [Normal]: ");
                        string pri = Console.ReadLine()?.Trim() ?? "Normal";
                        if (string.IsNullOrEmpty(pri)) pri = "Normal";
                        service.AddTask(title, pri); break;
                    case "list":
                        service.ListTasks(parts.Length > 1 ? parts[1].ToLower() : "all"); break;
                    case "done":
                        if (parts.Length > 1 && int.TryParse(parts[1], out int dId)) service.CompleteTask(dId);
                        else Console.WriteLine("  Usage: done <id>"); break;
                    case "delete":
                        if (parts.Length > 1 && int.TryParse(parts[1], out int delId)) service.DeleteTask(delId);
                        else Console.WriteLine("  Usage: delete <id>"); break;
                    case "update":
                        if (parts.Length > 1 && int.TryParse(parts[1], out int uId))
                        { Console.Write("  New title: "); string nt = Console.ReadLine()?.Trim() ?? ""; if (!string.IsNullOrEmpty(nt)) service.UpdateTask(uId, nt); }
                        else Console.WriteLine("  Usage: update <id>"); break;
                    case "help":
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\n  Commands: add, list, list pending, list done, done <id>, update <id>, delete <id>, help, exit");
                        Console.ResetColor(); break;
                    case "exit": case "quit": running = false; Console.WriteLine("\n  Goodbye!\n"); break;
                    default: if (!string.IsNullOrEmpty(cmd)) Console.WriteLine($"  Unknown: \"{cmd}\". Type 'help'."); break;
                }
            }
        }
    }
}
