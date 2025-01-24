using System;
using Godot;

public class FileSystem
{
	public FileSystem() {}

	public void ForFilesInDirectory(string path, Action<string, string> fileAction, bool includeSubDirectories = false)  {
		using DirAccess dir = DirAccess.Open(path);
		dir.ListDirBegin();
		
		string fileName;
		do {
			fileName = dir.GetNext();

			if (string.IsNullOrEmpty(fileName))
				break;

			if (!dir.CurrentIsDir())
				fileAction(fileName, $"{path}/{fileName}");
			
			if (dir.CurrentIsDir() && includeSubDirectories)
				ForFilesInDirectory($"{path}/{fileName}", fileAction, true);
			
		} while (!string.IsNullOrEmpty(fileName));
		dir.ListDirEnd();
	}
}
