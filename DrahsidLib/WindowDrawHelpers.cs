using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Common.Math;
using Dalamud.Bindings.ImGui;

namespace DrahsidLib;

public delegate void DrawToolTipDelegate(string label);

/// <summary>
/// Some helper functions for ImGui.
/// </summary>
public static class WindowDrawHelpers {
    /// <summary>
    /// Lazily evaluated size of F character
    /// </summary>
    private static Vector2? _TextSizeF = null;
    internal static Vector2 TextSizeF {
        get {
            return _TextSizeF ??= ImGui.CalcTextSize("F");
        }
        set {
            _TextSizeF = value;
        }
    }
    internal static float TextSizeFWidthx10 {
        get {
            return TextSizeF.X * 10.0f;
        }
    }


    /// <summary>
    /// Draw a tooltip if the most recent item is hovered.
    /// </summary>
    /// <param name="label">Tooltip text.</param>
    public static void DrawTooltipDefaultImpl(string label) {
        if (ImGui.IsItemHovered()) {
            ImGui.SetTooltip(label);
        }
    }

    /// <summary>
    /// Allow the ability to override the default DrawTooltip function (for using hide tooltips)
    /// </summary>
    public static DrawToolTipDelegate DrawTooltip = DrawTooltipDefaultImpl;

    /// <summary>
    /// Draws both a float slider, and an input. Supports using a tooltip.
    /// </summary>
    /// <param name="label">Text used for the `label` parameter.</param>
    /// <param name="cvar">Value to output to.</param>
    /// <param name="tooltip">Tooltip to display for the float.</param>
    /// <param name="min">Minimum float for the slider.</param>
    /// <param name="max">Maximum float for the slider.</param>
    /// <returns>True if cvar changed.</returns>
    public static bool DrawFloatInputTooltip(string label, ref float cvar, string tooltip, float min = 0, float max = 1) {
        bool result;
        bool drawTooltip = tooltip.IsNullOrEmpty() == false;

        ImGui.SetNextItemWidth(TextSizeFWidthx10 * 2);
        result = ImGui.SliderFloat(label, ref cvar, min, max);
        if (drawTooltip) {
            DrawTooltip(tooltip);
        }

        ImGui.SameLine();
        ImGui.SetNextItemWidth(TextSizeFWidthx10);
        result |= ImGui.InputFloat($"##{label}sl", ref cvar);
        if (drawTooltip) {
            DrawTooltip(tooltip);
        }
        return result;
    }

    /// <summary>
    /// Draws both a float slider, and an input.
    /// </summary>
    /// <param name="label">Text used for the `label` parameter.</param>
    /// <param name="cvar">Value to output to.</param>
    /// <param name="min">Minimum float for the slider.</param>
    /// <param name="max">Maximum float for the slider.</param>
    /// <returns>True if cvar changed.</returns>
    public static bool DrawFloatInput(string label, ref float cvar, float min = 0, float max = 1) {
        bool result;

        ImGui.SetNextItemWidth(TextSizeFWidthx10 * 2);
        result = ImGui.SliderFloat(label, ref cvar, min, max);

        ImGui.SameLine();
        ImGui.SetNextItemWidth(TextSizeFWidthx10);
        result |= ImGui.InputFloat($"##{label}sl", ref cvar);
        return result;
    }

    /// <summary>
    /// Draw a checkbox with a tooltip.
    /// </summary>
    /// <param name="label">Text used for the `label` parameter.</param>
    /// <param name="cvar">Value to output to.</param>
    /// <param name="tooltip">Tooltip to display for the float.</param>
    /// <returns>True if the checkbox was toggled.</returns>
    public static bool DrawCheckboxTooltip(string label, ref bool cvar, string tooltip) {
        bool ret = ImGui.Checkbox(label, ref cvar);

        if (tooltip.IsNullOrEmpty() == false) {
            DrawTooltip(tooltip);
        }

        return ret;
    }

    /// <summary>
    /// Draw a button with a tooltip.
    /// </summary>
    /// <param name="label">Text used for the `label` parameter.</param>
    /// <param name="tooltip">Tooltip to display for the float.</param>
    /// <returns></returns>
    public static bool DrawButtonTooltip(string label, string tooltip) {
        bool ret = ImGui.Button(label);

        if (tooltip.IsNullOrEmpty() == false) {
            DrawTooltip(tooltip);
        }

        return ret;
    }

    /// <summary>
    /// Draw input text with a tooltip.
    /// </summary>
    /// <param name="label">Text used for the `label` parameter.</param>
    /// <param name="cvar">Value to output to.</param>
    /// <param name="tooltip">Tooltip to display for the float.</param>
    /// <returns></returns>
    public static bool DrawInputTextTooltip(string label, ref string cvar, string tooltip, uint maxlength = 32) {
        bool ret = ImGuiDL.InputText(label, ref cvar, maxlength);

        if (tooltip.IsNullOrEmpty() == false) {
            DrawTooltip(tooltip);
        }

        return ret;
    }

    public static uint ColorToUint(Vector4 color) {
        return (uint)(((byte)(color.W * 255) << 24) |
                     ((byte)(color.X * 255) << 16)  |
                     ((byte)(color.Y * 255) << 8)   |
                     (byte)(color.Z * 255));
    }
}
