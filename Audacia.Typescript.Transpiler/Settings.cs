using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler
{
	public class AssemblySettings
	{
		public string Assembly { get; set; }
		
		[XmlArrayItem("Namespace")]
		public string[] Namespaces { get; set; }

		public string Output { get; set; }
	}
	
	public class Settings
	{
		[XmlArrayItem("Assembly")]
		public AssemblySettings[] Assemblies { get; set; }
		
		public IEnumerable<string> Namespaces => Assemblies.SelectMany(x => x.Namespaces);

		private static readonly XmlSerializer Xml = new XmlSerializer(typeof(Settings));

		public static Settings Load(string path)
		{
			if (!File.Exists(path))
			{
				var @default = new Settings
				{
					Assemblies = new[]
					{
						new AssemblySettings
						{
							Assembly = "../../path/to/example/assembly.dll",
							Namespaces = new[] { "example", "namespace", "specifications " },
							Output = "../../example/output/file.ts"
						},
						new AssemblySettings
						{
							Assembly = "../../another/example/assembly.dll",
							Namespaces = new[] { "example", "namespace", "specifications " },
							Output = "../../example/output/file.ts"
						}
					}
				};

				  
				using (TextWriter writer = new StreamWriter(path))
					Xml.Serialize(writer, @default);
				
				throw new InvalidDataException("Failed to find a config file at: " + path + ". A template config file has been automatically generated for you");
			}

			using (var myFileStream = new FileStream(path, FileMode.Open))
			{
				return (Settings) Xml.Deserialize(myFileStream);
			}
		}
	}
}