#General vars
CONFIG?=Debug
ARGS:=/p:Configuration="${CONFIG}" $(ARGS)

all:
	# nuget restoring
	if [ ! -f src/.nuget/nuget.exe ]; then \
		mkdir -p src/.nuget ; \
	    echo "nuget.exe not found! downloading latest version" ; \
	    curl -O https://dist.nuget.org/win-x86-commandline/latest/nuget.exe ; \
	    mv nuget.exe src/.nuget/ ; \
	fi

	msbuild ShowInGithub.sln $(ARGS)

clean:
	msbuild ShowInGithub.sln $(ARGS) /t:Clean

pack: all
	msbuild ShowInGithub.sln $(ARGS) /p:CreatePackage=true

install: all
	msbuild ShowInGithub.sln $(ARGS) /p:InstallAddin=true

.PHONY: all clean pack install
