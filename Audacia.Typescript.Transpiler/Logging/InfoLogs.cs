using System;
using static System.Console;

namespace Audacia.Typescript.Transpiler.Logging 
{
	public class InfoLogs
	{
		public void Class(Class @class)
		{
			if (Log.Level > LogLevel.Info) return;
			
			ForegroundColor = ConsoleColor.Green;
			Write("class ");
			ResetColor();
			WriteLine(@class.Name);
		}
		
		public void Enum(Enum @enum)
		{
			if (Log.Level > LogLevel.Info) return;
			
			ForegroundColor = ConsoleColor.DarkYellow;
			Write("enum ");
			ResetColor();
			WriteLine(@enum.Name);
		}
		
		public void Interface(Interface @interface)
		{
			if (Log.Level > LogLevel.Info) return;
			
			ForegroundColor = ConsoleColor.Magenta;
			Write("interface ");
			ResetColor();
			WriteLine(@interface.Name);
		}
		public void FileWritten(string path)
		{
			if (Log.Level > LogLevel.Info) return;
			
			ForegroundColor = ConsoleColor.Green;
			WriteLine($"Typescript file \"{System.IO.Path.GetFullPath(path)}\" written.");
			ResetColor();
		}
		public void JobComplete(float elapsed)
		{
			if (Log.Level > LogLevel.Info) return;
			
			ForegroundColor = ConsoleColor.Green;
			WriteLine($"Typescript transpile completed in {elapsed}ms.");
			ResetColor();
		}
	}
}