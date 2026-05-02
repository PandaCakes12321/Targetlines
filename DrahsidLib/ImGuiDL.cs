
using System;
using System.Text;
using Dalamud.Bindings.ImGui;

namespace DrahsidLib;

public static class ImGuiDL
{
    public static bool InputText(string label, ref string text, uint maxLength, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None) {
        ImU8String imLabel = new ImU8String(label);
        byte[] buffer = new byte[maxLength];
        if (!string.IsNullOrEmpty(text))
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            int copyLength = Math.Min(textBytes.Length, (int)maxLength - 1);
            Array.Copy(textBytes, buffer, copyLength);
        }

        Span<byte> bufferSpan = buffer.AsSpan();
        bool result = ImGui.InputText(imLabel, bufferSpan, flags);
        if (result) {
            int nullIndex = Array.IndexOf(buffer, (byte)0);
            if (nullIndex == -1) nullIndex = buffer.Length;
            text = Encoding.UTF8.GetString(buffer, 0, nullIndex);
        }

        return result;
    }
}
