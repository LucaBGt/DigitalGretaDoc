using UnityEngine;
using UnityEngine.Rendering;
using Mirror;

public class DetectHeadless : MonoBehaviour
{
    void Awake()
    {
        if (IsHeadless())
        {
            print("headless mode detected");
            //StartServer();
        }
    }
    bool IsHeadless()
    {
        return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }
}
