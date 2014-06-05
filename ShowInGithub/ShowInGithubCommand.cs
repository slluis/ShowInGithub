using System.IO;
using System.Linq;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Core;

namespace ShowInGithub
{
	public enum Commands
	{
		ShowInGithub,
		CopyGithubLink
	}

	public class ShowInGithubCommand: CommandHandler
	{
		protected override void Update (CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			info.Visible = doc != null && doc.Editor != null && GetGitDir (doc.FileName) != null;
		}

		internal static string GetGitDir (string subdir)
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
			var url = GetUrl ();
			if (url != null)
				DesktopService.ShowUrl (url);
		}

		internal static string GetUrl ()
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
				return null;

			var url = rref.GetValue ("url");
			if (url.EndsWith (".git"))
				url = url.Substring (0, url.Length - 4);
			var i = url.IndexOf ("github.com");
			if (i == -1)
				return null;

			string subdir = doc.FileName.ToRelative (new FilePath (dir).ParentDirectory);
			subdir = subdir.Replace ('\\','/');
			var line1 = doc.Editor.OffsetToLineNumber (doc.Editor.SelectionRange.Offset);
			var line2 = doc.Editor.OffsetToLineNumber (doc.Editor.SelectionRange.EndOffset);
			var tline = line1.ToString ();
			if (line1 != line2)
				tline += "-" + line2;
			return "https://github.com/" + url.Substring (i + 11) + "/blob/" + branch + "/" + subdir + "#L" + tline;
		}
	}

	public class CopyGithubLinkCommand: CommandHandler
	{
		protected override void Update (CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			info.Visible = doc != null && doc.Editor != null && ShowInGithubCommand.GetGitDir (doc.FileName) != null;
		}

		protected override void Run ()
		{
			var url = ShowInGithubCommand.GetUrl ();
			if (url != null) {
				Xwt.Clipboard.SetText (url);
				IdeApp.Workbench.StatusBar.ShowMessage ("GitHub url successfully copied to clipboard");
			} else {
				Xwt.Clipboard.SetText ("");
				IdeApp.Workbench.StatusBar.ShowError ("GitHub url could not be copied to clipboard");
			}
		}
	}
}
