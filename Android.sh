
echo "Starting Android.sh"
set -e
cd ${WORKSPACE}

UNITY_PATH=/root/Unity/Hub/Editor/2020.3.12f1/Editor/Unity
UNITY_LICENSE=/root/Unity/Unity_lic.ulf

$UNITY_PATH -batchmode -manualLicenseFile $UNITY_LICENSE || echo "Unity Licensing Failed (Caused when Already Setup)"

$UNITY_PATH -batchmode -nographics -quit -executeMethod JenkinsBuild.BuildAndroid "${WORKSPACE}/Build_Android"
echo "Android Build Done"


echo "Done"