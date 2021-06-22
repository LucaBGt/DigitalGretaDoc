using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

public class ServerProfiler : MonoBehaviour
{

    private ProfilerRecorder recCS;

    private void PrintAllRecorderHandles()
    {
        List<ProfilerRecorderHandle> handles = new List<ProfilerRecorderHandle>();
        ProfilerRecorderHandle.GetAvailable(handles);

        foreach (var item in handles)
        {
            var desc = ProfilerRecorderHandle.GetDescription(item);
            Debug.Log(desc.Category + " : " + desc.Name);
        }
    }

    private void OnEnable()
    {
        recCS = ProfilerRecorder.StartNew(ProfilerCategory.Scripts, "Update.ScriptRunBehaviourUpdate", 15);

        StartCoroutine(ProfileRoutine());
    }

    private IEnumerator ProfileRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Debug.Log("RecCS: " + recCS.CurrentValue * 1e-6f + "s");
        }
    }


    private void OnDisable()
    {
        recCS.Dispose();
    }

}
