using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;

namespace DrahsidLib;

public static unsafe class SceneCameraManagerExtensions
{
    public static unsafe Camera* GetCurrentCamera(this SceneCameraManager thisx)
    {
        return thisx.Cameras[thisx.CameraIndex].Value;
    }
}

