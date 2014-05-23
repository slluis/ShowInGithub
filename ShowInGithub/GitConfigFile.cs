using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace ShowInGithub
{
	class GitConfigFile
	{
		List<GitConfigSection> sections = new List<GitConfigSection> ();

		public IEnumerable<GitConfigSection> Sections {
			get { return sections; }
		}

		public void LoadFile (string file)
		{
			LoadContent (File.ReadAllText (file));
		}

		public void LoadContent (string content)
		{
			var sr = new StringReader (content);
			GitConfigSection section = null;
			string line;
			while ((line = sr.ReadLine ()) != null) {
				if (line.StartsWith ("[")) {
					section = new GitConfigSection ();
					int i = line.IndexOfAny (new [] { ' ', ']' }, 1);
					if (i == -1)
						continue;
					section.Type = line.Substring (1, i - 1);
					i = line.IndexOf ('"', i);
					if (i != -1) {
						int j = line.LastIndexOf ('"');
						if (j != -1 && j > i)
							section.Name = line.Substring (i + 1, j - i - 1);
					}
					sections.Add (section);
				} else if (line.StartsWith ("\t") && section != null) {
					int i = line.IndexOf ('=');
					var key = line.Substring (0, i).Trim ();
					var value = line.Substring (i + 1).Trim ();
					section.SetValue (key, value);
				}
			};
		}

		public void SaveFile (string file)
		{
			File.WriteAllText (file, GetContent ());
		}

		public string GetContent ()
		{
			using (StringWriter sw = new StringWriter ()) {
				foreach (var s in sections) {
					sw.Write ("[" + s.Type);
					if (!string.IsNullOrEmpty (s.Name))
						sw.Write (" \"" + s.Name + "\"");
					sw.WriteLine ("]");
					foreach (var k in s.Keys) {
						sw.WriteLine ("\t" + k + " = " + s.GetValue (k));
					}
				}
				return sw.ToString ();
			}
		}

		public bool Modified {
			get { return sections.Any (s => s.Modified); }
		}
	}

	class GitConfigSection
	{
		public string Type { get; set; }
		public string Name { get; set; }

		internal bool Modified { get; set; }

		List<Tuple<string,string>> properties = new List<Tuple<string,string>> ();

		public IEnumerable<string> Keys {
			get { return properties.Select (p => p.Item1); }
		}

		public string GetValue (string key)
		{
			return properties.Where (p => p.Item1 == key).Select (p => p.Item2).FirstOrDefault ();
		}

		public void SetValue (string key, string value)
		{
			var i = properties.FindIndex (t => t.Item1 == key);
			if (i == -1) {
				var p = new Tuple<string, string> (key, value);
				properties.Add (p);
				Modified = true;
			} else {
				if (properties [i].Item2 != value) {
					properties [i] = new Tuple<string, string> (key, value);
					Modified = true;
				}
			}
		}

		public void RemoveValue (string key)
		{
			var i = properties.FindIndex (t => t.Item1 == key);
			if (i != -1) {
				properties.RemoveAt (i);
				Modified = true;
			}
		}
	}
}

