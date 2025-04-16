# FolderSync

**FolderSync** is a C# console application for one-way synchronization of two folders.  
It synchronizes the contents of the `source` folder into the `replica` folder by deleting, adding, and updating files and folders.  
Supports periodic synchronization, logging to file and console, and hash caching for performance.

---

## 🚀 How to Run

```bash
dotnet run --project FolderSync -- <sourcePath> <replicaPath> <intervalSeconds> <logDirectoryPath>

Example: dotnet run --project FolderSync -- "C:\Source" "C:\Replica" 2 "C:\Logs"
