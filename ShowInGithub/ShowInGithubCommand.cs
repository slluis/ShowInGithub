using System.IO;
using System.Linq;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Core;

namespace ShowInGithub
{
	public enum Commands
	{
		ShowInGithub
	}

	public class ShowInGithubCommand: CommandHandler
	{
		protected override void Update (CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			info.Visible = doc != null && doc.Editor != null && GetGitDir (doc.FileName) != null;
		}

		string GetGitDir (string subdir)
		{
			var r = Path.GetPathRoot (subdir);
			while (!string.IsNullOrEmpty (subdir) && subdir != r) {
				var gd = Path.Combine (subdir, ".git");
				if (Directory.Exists (gd))
					return gd;
				subdir = Path.GetDirectoryName (subdir);
			}
			return null;
		}

		protected override void Run ()
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			var dir = GetGitDir (doc.FileName);
			var file = new GitConfigFile ();
			file.LoadFile (Path.Combine (dir, "config"));

			string head = File.ReadAllText (Path.Combine (dir, "HEAD")).Trim ();
			string branch;
			string remote = null;
			if (head.StartsWith ("ref: refs/heads/")) {
				branch = head.Substring (16);
				var sec = file.Sections.FirstOrDefault (s => s.Type == "branch" && s.Name == branch);
				if (sec != null)
					remote = sec.GetValue ("remote");
			} else {
				branch = head;
			}
			if (string.IsNullOrEmpty (remote))
				remote = "origin";
			var rref = file.Sections.FirstOrDefault (s => s.Type == "remote" && s.Name == remote);
			if (rref == null)
				return;

			var url = rref.GetValue ("url");
			if (url.EndsWith (".git"))
				url = url.Substring (0, url.Length - 4);
			var i = url.IndexOf ("github.com");
			if (i == -1)
				return;

			string subdir = doc.FileName.ToRelative (new FilePath (dir).ParentDirectory);
			subdir = subdir.Replace ('\\','/');
			url = "https://github.com/" + url.Substring (i + 11) + "/blob/" + branch + "/" + subdir + "#L" + doc.Editor.Caret.Line;
			DesktopService.ShowUrl (url);

		}
	}
}

