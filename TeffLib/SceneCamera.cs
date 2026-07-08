using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;

namespace TeffLib;

public static unsafe class SceneCameraManagerExtensions
{
    public static unsafe Camera* GetCurrentCamera(this SceneCameraManager thisx)
    {
        return thisx.Cameras[thisx.CameraIndex].Value;
    }
}

