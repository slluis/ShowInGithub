using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"ShowInGithub", 
	Namespace = "ShowInGithub",
	Version = "1.0"
)]

[assembly:AddinName ("ShowInGithub")]
[assembly:AddinCategory ("ShowInGithub")]
[assembly:AddinDescription ("ShowInGithub")]
[assembly:AddinAuthor ("Lluis Sanchez Gual")]

[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.VersionControl", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.SourceEditor2", MonoDevelop.BuildInfo.Version)]
