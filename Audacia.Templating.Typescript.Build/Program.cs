using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Audacia.Templating.Typescript.Build
{
	public class Program
	{
		private static IDictionary<string, Settings> Settings { get; set; }

		public static void Main()
		{
			Settings = Build.Settings.Load();

			foreach (var setting in Settings)
			{
				var assembly = Assembly.LoadFrom(setting.Key);
				var elements = Reflect(assembly, setting.Value);

				var fileContents = string.Join(Environment.NewLine, elements);

				foreach (var output in setting.Value.Output)
				{
					File.WriteAllText(output, fileContents);

					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"Typescript file \"{Path.GetFullPath(output)}\" written for assembly \"{assembly.GetName().Name}\"");
					Console.ForegroundColor = ConsoleColor.White;
				}
				
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine();
				Console.WriteLine($"Transformation complete.");
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.White;
			}
		}

		private static IEnumerable<Element> Reflect(Assembly assembly, Settings settings)
		{
			var types = assembly.GetTypes()
				.Where(t => settings.Namespaces.Contains(t.Namespace))
				.Where(t => t.IsClass || t.IsInterface || t.IsEnum)
				.Where(t => t.IsPublic && !t.IsNested);

			foreach (var type in types)
			{
				if (type.IsClass) yield return Class(type);
				else if (type.IsInterface) yield return Interface(type);
				else if (type.IsEnum) yield return Enum(type);
			}
		}

		private static Element Class(Type type)
		{
			var @class = new Class(type.Name) { Modifiers = { Modifier.Export } };

			var interfaces = type.GetInterfaces().Select(x => x.Name);
			var members = Members(type);
			
			foreach(var @interface in interfaces)
				@class.Implements.Add(@interface);
			
			foreach(var member in members)
				@class.Members.Add(member);
			
			Console.WriteLine($"Class \"{@class.Name}\" generated.");
			return @class;
		}

		private static Element Interface(Type type)
		{
			var  @interface = new Interface(type.Name) { Modifiers = { Modifier.Export } };
			
			var interfaces = type.GetInterfaces().Select(x => x.Name);
			var members = Members(type);
			
			foreach(var @base in interfaces)
				@interface.Extends.Add(@base);
			
			foreach(var member in members)
				@interface.Members.Add(member);
			
			Console.WriteLine($"Interface \"{@interface.Name}\" generated.");
			return @interface;
		}

		private static Element Enum(Type t) // Enums<>, since Enum<> is not allowed.
		{
			var @enum = new Enum<string>(t.Name);
			var values = (int[]) System.Enum.GetValues(t);
			
			foreach (var val in values)
			{
				var name = System.Enum.GetName(t, val);
				
				var attribute = t.GetMember(name)
					.Single()
					.GetCustomAttributes()
					.FirstOrDefault(a => a.GetType().Name == "EnumMemberAttribute");
			
				var label = attribute?.GetType()
					.GetProperty("Value")
					.GetValue(attribute)
					.ToString();
				
				@enum.Members.Add(name, label ?? name);				
			}
			
			Console.WriteLine($"Enum \"{@enum.Name}\" generated.");
			return @enum;
		}

		private static IEnumerable<Property> Members(Type type)
		{
			return type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
				.Where(mi => mi.MemberType == MemberTypes.Property)
				.Select(m => new Property(ToCamelCase(m.Name)));
		}

		private static string ToCamelCase(string s)
		{
			if (string.IsNullOrEmpty(s)) return s;
			if (s.Length < 2) return s.ToLowerInvariant();
			return char.ToLowerInvariant(s[0]) + s.Substring(1);
		}

		private static string GetTypeName(MemberInfo mi)
		{
			var t = mi is PropertyInfo info ? info.PropertyType : ((FieldInfo) mi).FieldType;
			return GetTypeName(t);
		}

		private static string GetTypeName(Type t)
		{
			if (t.IsPrimitive)
			{
				if (t == typeof(bool)) return "boolean";
				if (t == typeof(char)) return "string";
				return "number";
			}

			if (t == typeof(decimal)) return "number";
			if (t == typeof(string)) return "string";
			if (t == typeof(Guid)) return "string";
			if (t.IsArray)
			{
				var at = t.GetElementType();
				return GetTypeName(at) + "[]";
			}

			if (typeof(System.Collections.IEnumerable).IsAssignableFrom(t))
			{
				var collectionType = t.GetGenericArguments()[0]; // all my enumerables are typed, so there is a generic argument
				return GetTypeName(collectionType) + "[]";
			}

			if (Nullable.GetUnderlyingType(t) != null)
			{
				return GetTypeName(Nullable.GetUnderlyingType(t));
			}

			if (!t.Namespace.Contains("Common"))
				return "Core." + t.Name;

			if (t.IsEnum) return t.Name;

			return t.Name;
		}
	}
}