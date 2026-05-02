using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Common.Math;
using System;

namespace DrahsidLib;

public unsafe static class GameObjectExtensions {
    const int CursorHeightOffset = 0x124;
    const float HeadHeightOffset = -0.2f;

    public unsafe static float GetCursorHeight(this IGameObject thisx)
    {
        return Marshal.PtrToStructure<float>(thisx.Address + CursorHeightOffset);
    }

    public unsafe static bool TargetIsTargetable(this IGameObject thisx) {
        if (thisx.TargetObject == null) {
            return false;
        }

        CSGameObject* targetobj = (CSGameObject*)thisx.TargetObject.Address;
        return targetobj->GetIsTargetable();
    }

    public unsafe static Vector3 GetHeadPosition(this IGameObject thisx)
    {
        Vector3 pos = thisx.Position;
        pos.Y += thisx.GetCursorHeight() - HeadHeightOffset;
        return pos;
    }
}

