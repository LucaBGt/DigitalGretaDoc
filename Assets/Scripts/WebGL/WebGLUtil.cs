using System.Runtime.InteropServices;

public static class WebGLUtil 
{
    [DllImport("__Internal")]
    public static extern void openWindow(string url);

}
