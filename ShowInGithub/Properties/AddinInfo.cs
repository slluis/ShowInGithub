using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"ShowInGithub", 
	Namespace = "ShowInGithub",
	Version = "1.0.1"
)]

[assembly:AddinName ("Show in Github")]
[assembly:AddinCategory ("Version Control")]
[assembly:AddinDescription ("Adds a command to the text editor context menu which will jump to the current file and line in Github")]
[assembly:AddinAuthor ("Lluis Sanchez Gual")]

[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.VersionControl", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.SourceEditor2", MonoDevelop.BuildInfo.Version)]
