using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler
{
	public class AssemblySettings
	{
		public string Assembly { get; set; }
		
		[XmlArrayItem("Assembly")]
		public ICollection<string> Namespaces { get; } = new List<string>();

		public string Output { get; set; }
	}
	
	public class Settings
	{
		[XmlArrayItem("Assembly")]
		public AssemblySettings[] Assemblies { get; private set; }
		
		public IEnumerable<string> Namespaces => Assemblies.SelectMany(x => x.Namespaces);

		private static readonly XmlSerializer Xml = new XmlSerializer(typeof(Settings));
		private IDictionary<string, AssemblySettings> _lookup;

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
							Namespaces = { "example", "namespace", "specifications " },
							Output = "../../example/output/file.ts"
						},
						new AssemblySettings
						{
							Assembly = "../../another/example/assembly.dll",
							Namespaces = { "example", "namespace", "specifications " },
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
				var settings = (Settings) Xml.Deserialize(myFileStream);
				settings._lookup = settings.Assemblies.ToDictionary(x => x.Assembly, x => x);
				return settings;
			}
		}
	}
}