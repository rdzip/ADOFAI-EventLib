dotnet "C:\Program Files\dotnet\sdk\7.0.305\MSBuild.dll" /p:Configuration=Release
mkdir Release
cd ./Release
mkdir EventLib
cp ../EventLib/bin/Release/EventLib.dll ./EventLib/EventLib.dll

cp ../Info.json ./EventLib/Info.json
tar -acf EventLib-Release.zip EventLib
mv EventLib-Release.zip ../
cd ../
rm -rf Release
pause
