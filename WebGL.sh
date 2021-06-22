
echo "Starting WebGL.sh"
set -e
cd ${WORKSPACE}

UNITY_PATH=C:\Program Files\Unity\Hub\Editor\2020.3.5f1\Editor\Unity

#WEBGL STUFF 
$UNITY_PATH -batchmode -nographics -quit -executeMethod JenkinsBuild.BuildWebGL "${WORKSPACE}/Build_WebGL/"
echo "WebGL Build Done"

