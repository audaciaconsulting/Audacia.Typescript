using System;
using static System.Console;

namespace Audacia.Typescript.Transpiler.Logging
{
	public static class Log
	{
		public static LogLevel Level { get; set; }

		public static DebugLogs Debug { get; } = new DebugLogs();
		public static InfoLogs Info { get; } = new InfoLogs();
		
		
		public static WarningLogs Warn { get; } = new WarningLogs();
		
		public static void Error(string message)
		{
			if (Level <= LogLevel.Error) return;
				
			ForegroundColor = ConsoleColor.Red;
			WriteLine(message);
			ResetColor();
		}
	}
}