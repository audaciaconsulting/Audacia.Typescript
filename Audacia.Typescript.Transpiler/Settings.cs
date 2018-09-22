using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Audacia.Typescript.Transpiler
{
	public class Settings
	{
		[XmlElement("Assembly")]
		public AssemblySettings[] Assemblies { get; set; }
		
		public IEnumerable<string> Namespaces => Assemblies.SelectMany(x => x.Namespaces).Select(n => n.Name);

		private static readonly XmlSerializer Xml = new XmlSerializer(typeof(Settings));

		public static Settings Load(string path)
		{
			if (!File.Exists(path))
			{
				using (TextWriter writer = new StreamWriter(path))
					Xml.Serialize(writer, Default);
				
				throw new InvalidDataException("Failed to find a config file at: " + path + ". A template config file has been automatically generated for you");
			}

			using (var myFileStream = new FileStream(path, FileMode.Open))
			{
				return (Settings) Xml.Deserialize(myFileStream);
			}
		}
		
		public static Settings Default => new Settings
		{
			Assemblies = new[]
			{
				new AssemblySettings
				{
					Assembly = "../../path/to/example/assembly.dll",
					Namespaces = new[] { "example", "namespace", "specifications " }.Select(x => new NamespaceSettings(x)).ToArray(),
					Outputs = new[] { "../../example/output/file.ts" }.Select(x => new OutputSettings(x)).ToArray()
				},
				new AssemblySettings
				{
					Assembly = "../../another/example/assembly.dll",
					Namespaces = new[] { "example", "namespace", "specifications " }.Select(x => new NamespaceSettings(x)).ToArray(),
					Outputs = new[] { "../../example/output/file.ts" }.Select(x => new OutputSettings(x)).ToArray()
				}
			}
		};
	}
}