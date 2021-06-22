
echo "Starting Android.bat"

cd %WORKSPACE%

set UNITY_PATH=C:\"Program Files"\Unity\Hub\Editor\2020.3.5f1\Editor\Unity

%UNITY_PATH% -batchmode -nographics -quit -executeMethod JenkinsBuild.BuildAndroid "%WORKSPACE%/Build_Android/Gretaland.apk"
echo "Android Build Done"

echo "Done"