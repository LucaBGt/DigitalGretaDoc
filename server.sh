
echo "Starting server.sh"
set -e
cd ${WORKSPACE}

UNITY_PATH=/root/Unity/Hub/Editor/2020.3.12f1/Editor/Unity
UNITY_LICENSE=/root/Unity/Unity_lic.ulf

$UNITY_PATH -batchmode -manualLicenseFile $UNITY_LICENSE || echo "Unity Licensing Failed (Caused when Already Setup)"

$UNITY_PATH -batchmode -nographics -quit -executeMethod JenkinsBuild.BuildLinuxServer "${WORKSPACE}/Build_Server/"
echo "Server Build Done"

#replace and restart server
echo "killing server"

pkill Server.x86_64 || echo "server not running"

sleep 3

echo "Copying server"

cp -r ${WORKSPACE}/Build_Server/. /root/GretaServer/ 

echo "Restarting Server"

daemonize -E BUILD_ID=dontKillMe /root/GretaServer/run.sh

echo "Done"