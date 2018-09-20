using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Audacia.Templating.Typescript.Build 
{
	public class Settings
	{
		public static IDictionary<string, Settings> Load(string path)
		{
			if (!File.Exists(path))
			{
				var @default = new Dictionary<string, Settings>
				{
					{
						"../../path/to/example/assembly.dll", new Settings
						{
							Namespaces = {"example", "namespace", "specifications "},
							Output = "../../example/output/file.ts"
						}
					},
					{
						"../../another/example/assembly.dll", new Settings
						{
							Namespaces = {"example", "namespace", "specifications "},
							Output = "../../example/output/file.ts"
						}
					}
				};

				var json = JsonConvert.SerializeObject(@default);
				File.WriteAllText(path, json);
				
				throw new InvalidDataException("Failed to find a config file at: " + path + ". A template config file has been automatically generated for you");
			}
				//throw new FileLoadException("Failed to read config file at: " + path);
			
			var config = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<IDictionary<string, Settings>>(config);
		}

		public ICollection<string> Namespaces { get; } = new List<string>();

		public string Output { get; set; }
	}
}