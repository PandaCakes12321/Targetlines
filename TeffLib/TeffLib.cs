using Dalamud.Plugin;

namespace TeffLib; 

public static class TeffLib {
    public static void Initialize(IDalamudPluginInterface pluginInterface, DrawToolTipDelegate? DrawToolTipFn = null) {
        Service.Initialize(pluginInterface);
        if (DrawToolTipFn != null) {
            WindowDrawHelpers.DrawTooltip = DrawToolTipFn;
        }
    }
}
