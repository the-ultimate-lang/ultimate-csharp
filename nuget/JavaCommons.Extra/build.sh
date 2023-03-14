cwd=`pwd`
ts=`date "+%Y.%m%d.%H%M.%S"`
sed -i -e "s/<Version>.*<\/Version>/<Version>${ts}<\/Version>/g" JavaCommons.Extra.csproj
rm -rf obj bin
rm -rf *.nupkg
dotnet pack -c Release -o .
