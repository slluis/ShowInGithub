using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"ShowInGithub", 
	Namespace = "ShowInGithub",
	Version = "1.0.3"
)]

[assembly:AddinName ("Show in Github")]
[assembly:AddinCategory ("Version Control")]
[assembly:AddinDescription ("Adds a command to the text editor context menu which will jump to the current file and line in Github")]
[assembly:AddinAuthor ("Lluis Sanchez Gual")]
