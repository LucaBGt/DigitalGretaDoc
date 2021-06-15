#!/bin/bash

cd ${WORKSPACE}

${JENKINS_HOME}/2020.3.11f1/Editor/Unity -batchmode -manualLicenseFile ${JENKINS_HOME}/2020.3.11f1/Unity_lic.ulf

${JENKINS_HOME}/2020.3.11f1/Editor/Unity -batchmode -nographics -quit -executeMethod JenkinsBuild.BuildLinuxServer "${WORKSPACE}\\Build_Server"
