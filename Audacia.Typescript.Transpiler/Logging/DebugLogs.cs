using System;
using static System.Console;

namespace Audacia.Typescript.Transpiler.Logging
{
	public class DebugLogs
	{
		public void IncludingType(Type type)
		{
			if (Log.Level > LogLevel.Debug) return;

			ForegroundColor = ConsoleColor.Magenta;
			Write("including: ");
			ResetColor();
			WriteLine(type.FullName);
		}

		public void InspectingClassAttributes(Type type)
		{
			if (Log.Level > LogLevel.Debug) return;

			ForegroundColor = ConsoleColor.Magenta;
			Write("inspecting class attributes: ");
			ResetColor();
			WriteLine(type.FullName);
		}

		public void InspectingPropertyAttributes(Type type)
		{
			if (Log.Level > LogLevel.Debug) return;

			ForegroundColor = ConsoleColor.Magenta;
			Write("inspecting property attributes: ");
			ResetColor();
			WriteLine(type.FullName);
		}
	}
}