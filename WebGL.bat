
echo "Starting WebGL.bat"

cd %WORKSPACE%

set UNITY_PATH = C:\Program Files\Unity\Hub\Editor\2020.3.5f1\Editor\Unity

%UNITY_PATH% -batchmode -nographics -quit -executeMethod JenkinsBuild.BuildWebGL "%WORKSPACE%/Build_WebGL/"
echo "WebGL Build Done"

