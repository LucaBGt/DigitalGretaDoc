
echo "Starting WebGL.sh"
set -e
cd ${WORKSPACE}

UNITY_PATH=/root/Unity/Hub/Editor/2020.3.12f1/Editor/Unity
UNITY_LICENSE=/root/Unity/Unity_lic.ulf

$UNITY_PATH -batchmode -manualLicenseFile $UNITY_LICENSE || echo "Unity Licensing Failed (Caused when Already Setup)"

#WEBGL STUFF 
$UNITY_PATH -batchmode -nographics -quit -executeMethod JenkinsBuild.BuildWebGL "${WORKSPACE}/Build_WebGL"
echo "WebGL Build Done"

echo "Copying WebGL build to local site location"
cp -r ${WORKSPACE}/Build_WebGL/. /var/www/html/Gretaland/


echo "Done"