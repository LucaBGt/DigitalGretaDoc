
echo "Starting server.sh"
set -e
cd ${WORKSPACE}

${JENKINS_HOME}/2020.3.11f1/Editor/Unity -batchmode -manualLicenseFile ${JENKINS_HOME}/2020.3.11f1/Unity_lic.ulf || echo "Unity Licensing Failed (Caused when Already Setup)"

${JENKINS_HOME}/2020.3.11f1/Editor/Unity -batchmode -nographics -quit -executeMethod JenkinsBuild.BuildLinuxServer "${WORKSPACE}/Build_Server"
echo "Server Build Done"

sleep 3 

${JENKINS_HOME}/2020.3.11f1/Editor/Unity -batchmode -nographics -quit -executeMethod JenkinsBuild.BuildWebGL "${WORKSPACE}/Build_WebGL"
echo "WebGL Build Done"

echo "Copying WebGL build to local site location"
cp -r ${WORKSPACE}/Build_WebGL/. /var/www/html/Gretaland/

#replace and restart server
echo "killing server"

pkill Server.x86_64

sleep 3

echo "Copying server"

cp -r ${WORKSPACE}/Build_Server/. /root/GretaServer/ 

echo "Restarting Server"

daemonize -E BUILD_ID=dontKillMe /root/GretaServer/run.sh

echo "Done"