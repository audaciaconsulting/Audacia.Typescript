using System;
using System.Reflection;

namespace Audacia.Typescript.Transpiler.Logging {
	public class WarningLogs
	{
		public void FailedToReadProperty(Exception exception, PropertyInfo source)
		{
			if (Log.Level >= LogLevel.Warn) return;

			var sourceType = source.DeclaringType;
			var nameSpace = sourceType?.Namespace ?? string.Empty;
			var className = sourceType?.Name ?? string.Empty;
			var propertyName = source.Name;
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("warn: ");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write(exception.GetType().Name);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(" encountered inspecting ");
			Console.ResetColor();
			Console.Write(nameSpace);
			Console.Write(".");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write(className);
			Console.ResetColor();
			Console.Write(".");
			Console.Write(propertyName);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine();
			Console.Write(exception);
			Console.WriteLine();
		}

		public void FailedToInstantiateType(Exception exception, Type sourceType)
		{
			if (Log.Level >= LogLevel.Warn) return;
			
			var nameSpace = sourceType.Namespace;
			var className = sourceType.Name;
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("warn: ");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write(exception.GetType().Name);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(" encountered instantiating ");
			Console.ResetColor();
			Console.Write(nameSpace);
			Console.Write(".");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write(className);
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine();
			Console.Write(exception);
			Console.WriteLine();
			Console.WriteLine();
		}
	}
}