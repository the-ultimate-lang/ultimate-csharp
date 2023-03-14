set -uvx
set -e

cwd=`pwd`

cd $cwd
rm -rf obj bin
#dotnet publish -c Release -r win-x64 -f net6.0-windows --self-contained false
#rm -rf $HOME/cmd/jcs
#cp -rp bin/Release/net6.0-windows/win-x64/publish $HOME/cmd/jcs
dotnet build -c Release
rm -rf $HOME/cmd/jcs
cp -rp bin/Release/net472 $HOME/cmd/jcs
