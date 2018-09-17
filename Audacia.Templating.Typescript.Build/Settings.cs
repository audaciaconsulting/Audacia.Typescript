using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Audacia.Templating.Typescript.Build 
{
	public class Settings
	{
		public const string Path = "config.json";
		
		public static IDictionary<string, Settings> Load()
		{
			if (!File.Exists(Path))
				throw new FileLoadException("Failed to read config file at: " + Path);
			
			var config = File.ReadAllText(Path);
			return JsonConvert.DeserializeObject<IDictionary<string, Settings>>(config);
		}

		public ICollection<string> Namespaces { get; } = new List<string>();

		public ICollection<string> Output { get; } = new List<string>();
	}
}