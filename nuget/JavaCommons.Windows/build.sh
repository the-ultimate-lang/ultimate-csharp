set -uvx
set -e
cwd=`pwd`
ts=`date "+%Y.%m%d.%H%M.%S"`
sed -i -e "s/<Version>.*<\/Version>/<Version>${ts}<\/Version>/g" JavaCommons.Windows.csproj
rm -rf obj bin nupkg
rm -rf ./*.nupkg
dotnet pack -c Release -o . JavaCommons.Windows.csproj
