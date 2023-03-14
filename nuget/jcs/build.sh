cwd=`pwd`
ts=`date "+%Y.%m%d.%H%M.%S"`
version="${ts}-beta"
echo -n $version > version.txt
sed -i -e "s/<Version>.*<\/Version>/<Version>${version}<\/Version>/g" jcs.csproj
rm -rf obj bin
dotnet build -c Release
rm -rf *.nupkg
cp -rp bin/Release/*.nupkg .
