using System;
using System.Reflection;
using static System.Console;

namespace Audacia.Typescript.Transpiler
{
	public enum LogLevel
	{
		Debug,
		Info,
		Warn,
		Error
	}
	
	public static class Log
	{
		public static LogLevel Level { get; set; }

		public static void Debug(string message)
		{
			if (Level >= LogLevel.Debug) 
				WriteLine(message);
		}
		
		public static void Info(string message)
		{
			if (Level >= LogLevel.Info) 
				WriteLine(message);
		}

		public static void Error(string message)
		{
			if (Level <= LogLevel.Error) return;
				
			ForegroundColor = ConsoleColor.Red;
			WriteLine(message);
			ResetColor();
		}

		public static void Warn(Exception exception, PropertyInfo source, Property target)
		{
			if (Level <= LogLevel.Warn) return;

			var sourceType = source.DeclaringType;
			var nameSpace = sourceType?.Namespace ?? string.Empty;
			var className = sourceType?.Name ?? string.Empty;
			var propertyName = source.Name;
			
			ForegroundColor = ConsoleColor.Red;
			Write("warn: ");
			ResetColor();
			ForegroundColor = ConsoleColor.Blue;
			WriteLine(exception);
			Write(" encountered inspecting ");
			Write(nameSpace);
			Write(".");
			ForegroundColor = ConsoleColor.Blue;
			Write(className);
			Write(".");
			Write(propertyName);
			ForegroundColor = ConsoleColor.Red;
			WriteLine(exception.Message);
			Console.WriteLine();
			Write(exception.StackTrace);
		}

		public static void Warn(Exception exception, Type sourceType)
		{
			var nameSpace = sourceType.Namespace;
			var className = sourceType.Name;
			
			ForegroundColor = ConsoleColor.Red;
			Write("warn: ");
			ResetColor();
			ForegroundColor = ConsoleColor.Blue;
			Write(exception);
			Write(" encountered instantiating ");
			Write(nameSpace);
			Write(".");
			ForegroundColor = ConsoleColor.Blue;
			Write(className);
			Console.WriteLine();
			ForegroundColor = ConsoleColor.Red;
			WriteLine("exception:", exception.Message);
			Console.WriteLine();
			Write(exception.StackTrace);
			Console.WriteLine();
		}
		
		public static void Class(Class @class)
		{
			if (Level > LogLevel.Info) return;
			
			ForegroundColor = ConsoleColor.Green;
			Write("class ");
			ResetColor();
			Console.WriteLine(@class.Name);
		}
		
		public static void Enum(Enum @enum)
		{
			if (Level > LogLevel.Info) return;
			
			ForegroundColor = ConsoleColor.DarkYellow;
			Write("enum ");
			ResetColor();
			Console.WriteLine(@enum.Name);
		}
		
		public static void Interface(Interface @interface)
		{
			if (Level > LogLevel.Info) return;
			
			ForegroundColor = ConsoleColor.Magenta;
			Write("interface ");
			ResetColor();
			Console.WriteLine(@interface.Name);
		}
	}
}