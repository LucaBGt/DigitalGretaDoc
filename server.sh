
cd ${WORKSPACE}

#${JENKINS_HOME}/2020.3.11f1/Editor/Unity -batchmode -manualLicenseFile ${JENKINS_HOME}/2020.3.11f1/Unity_lic.ulf

#${JENKINS_HOME}/2020.3.11f1/Editor/Unity -batchmode -nographics -quit -executeMethod JenkinsBuild.BuildLinuxServer "${WORKSPACE}\\Build_Server"

echo "Build Done, killing server"

kill $(ps aux | grep '/root/GretaServer/' | awk '{print $2}')

echo "Copying server"

cp -r ${WORKSPACE}/Build_Server/. /root/GretaServer/

echo "Restarting Server"

daemonize -E BUILD_ID=dontKillMe /root/GretaServer/run.sh

echo "Done"