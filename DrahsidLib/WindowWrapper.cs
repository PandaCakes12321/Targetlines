using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;

namespace DrahsidLib;

/// <summary>
/// A Simple disposable wrapper class for Window which allows me to set a minimum size and scale it correctly to the GlobalScale
/// </summary>
public class WindowWrapper : Window, IDisposable {
    private Vector2 MinSize = new Vector2(128, 128);
    private Vector2 AdjustedMinSize = new Vector2(128, 128);

    public WindowWrapper(string configWindowName, Vector2 minSize) : base(configWindowName) {
        AdjustedMinSize = MinSize = minSize;
    }

    public override void PreDraw() {
        AdjustedMinSize = MinSize * ImGuiHelpers.GlobalScale;
        ImGui.SetNextWindowSizeConstraints(AdjustedMinSize, new Vector2(float.MaxValue, float.MaxValue));
    }

    public override void Draw() {
    }

    public virtual void Dispose() {
    }
}

/// <summary>
/// A basic implementation test for WindowWrapper
/// </summary>
internal class TestWindow : WindowWrapper {
    TestWindow() : base("TestWindow", new Vector2(128, 128)) {
    }

    public override void Draw() {
    }

    public override void Dispose() {
    }
}

