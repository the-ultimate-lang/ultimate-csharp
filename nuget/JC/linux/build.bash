set -uvx
set -e
dotnet build -c Release
ilmerge -wildcards -out:`cygpath -wm ~/cmd/linux.exe` ./bin/Release/net472/linux.exe ./bin/Release/net472/*.dll