using System;
using static System.Console;

namespace Audacia.Typescript.Transpiler.Logging
{
	public class DebugLogs
	{
		public void IncludingType(Type @type)
		{
			if (Log.Level > LogLevel.Debug) return;
			
			ForegroundColor = ConsoleColor.Magenta;
			Write("including: ");
			ResetColor();
			WriteLine(@type.Name);
		}

		public static void Debug(string message)
		{
			if (Log.Level >= LogLevel.Debug) 
				WriteLine(message);
		}
		
		public static void Debug(Exception e)
		{
			if (Log.Level < LogLevel.Debug) return;

			ForegroundColor = ConsoleColor.Red;
			WriteLine(e);
			ResetColor();
		}

	}
}