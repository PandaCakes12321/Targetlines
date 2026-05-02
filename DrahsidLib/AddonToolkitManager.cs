using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Dalamud.Bindings.ImGui;
using System;

namespace DrahsidLib;

public enum NodePositionAnchor {
    TOP_LEFT, TOP_CENTER, TOP_RIGHT,
    CENTER_LEFT, CENTER_CENTER, CENTER_RIGHT,
    BOTTOM_LEFT, BOTTOM_CENTER, BOTTOM_RIGHT,
};

/// <summary>
/// Helper class for ATK related things
/// </summary>
public static unsafe class AddonToolkitHelper {
    public enum UnitListLayer {
        DepthLayer1,
        DepthLayer2,
        DepthLayer3,
        DepthLayer4,
        DepthLayer5,
        DepthLayer6,
        DepthLayer7,
        DepthLayer8,
        DepthLayer9,
        DepthLayer10,
        DepthLayer11,
        DepthLayer12,
        DepthLayer13,
        LoadedUnits,
        FocusedUnits,
        Units16,
        Units17,
        Units18,
        Count
    };

    public static readonly string[] UnitListLayerName = new string[(int)UnitListLayer.Count] {
        "Depth Layer 1",
        "Depth Layer 2",
        "Depth Layer 3",
        "Depth Layer 4",
        "Depth Layer 5",
        "Depth Layer 6",
        "Depth Layer 7",
        "Depth Layer 8",
        "Depth Layer 9",
        "Depth Layer 10",
        "Depth Layer 11",
        "Depth Layer 12",
        "Depth Layer 13",
        "Loaded Units",
        "Focused Units",
        "Units 16",
        "Units 17",
        "Units 18",
    };

    /// <summary>
    /// Get the position (in screen space) of the node. This function traverses upwards through it's parents to get the correct offset.
    /// </summary>
    /// <param name="node">Pointer to the node whose position to get.</param>
    /// <returns>Absolute (screen space) position of the node.</returns>
    public static Vector2 GetNodeAbsolutePosition(AtkResNode* node) {
        if (node == null) {
            Service.Logger.Warning("[GetNodeAbsolutePosition] Node is null!");
            return Vector2.Zero;
        }

        AtkResNode* parent = node->ParentNode;
        Vector2 pos = new Vector2(node->X, node->Y);

        while (parent != null) {
            pos.X *= parent->ScaleX;
            pos.Y *= parent->ScaleY;
            pos.X += parent->X;
            pos.Y += parent->Y;
            parent = parent->ParentNode;
        }

        return pos;
    }

    /// <summary>
    /// Get the total scale of the node. This function traverses upwards through it's parents to get the scalar.
    /// </summary>
    /// <param name="node">Pointer to the node whose scale to get.</param>
    /// <returns>Total scale of the node.</returns>
    public static Vector2 GetNodeScale(AtkResNode* node) {
        if (node == null) {
            Service.Logger.Warning("[GetNodeScale] Node is null!");
            return Vector2.One;
        }

        Vector2 scale = new Vector2(node->ScaleX, node->ScaleY);
        while (node->ParentNode != null) {
            node = node->ParentNode;
            scale.X *= node->ScaleX;
            scale.Y *= node->ScaleY;
        }

        return scale;
    }

    /// <summary>
    /// Get the size of the node after scaling. This function gets the scaled size by multiplying the base size by the result of GetNodeScale.
    /// </summary>
    /// <param name="node">Pointer to the node whose size to get.</param>
    /// <returns>Scaled size of the node.</returns>
    public static Vector2 GetNodeScaledSize(AtkResNode* node) {
        if (node == null) {
            Service.Logger.Warning("[GetNodeScaledSize] Node is null!");
            return Vector2.One;
        }

        Vector2 scale = GetNodeScale(node);
        return new Vector2(node->Width * scale.X, node->Height * scale.Y);
    }

    /// <summary>
    /// Get the visibility of the node. This function checks the visibility of the parent nodes to see if it is actually visible.
    /// </summary>
    /// <param name="node">Pointer to the node whose visibility to get.</param>
    /// <returns>True if the node is visible.</returns>
    public static bool GetNodeVisible(AtkResNode* node) {
        if (node == null) {
            Service.Logger.Warning("[GetNodeVisible] Node is null!");
            return false;
        }

        do {
            if ((node->NodeFlags & NodeFlags.Visible) != NodeFlags.Visible) {
                return false;
            }
            node = node->ParentNode;
        } while (node != null);

        return true;
    }

    /// <summary>
    /// Given an anchor, return the offset from the real position of the node.
    /// </summary>
    /// <param name="node">Pointer to the node whose offset to get.</param>
    /// <param name="anchor">Offset anchor within the node's bounds to get.</param>
    /// <returns>The offset of the anchor.</returns>
    public static Vector2 GetNodePositionAnchorOffset(AtkResNode* node, NodePositionAnchor anchor) {
        if (node == null) {
            Service.Logger.Warning("[GetNodePositionAnchorOffset] Node is null!");
            return Vector2.Zero;
        }

        Vector2 size = GetNodeScaledSize(node);
        Vector2 offset = Vector2.Zero;
        switch (anchor) {
            case NodePositionAnchor.TOP_LEFT:
                break;
            case NodePositionAnchor.TOP_CENTER:
                offset.X = size.X / 2;
                break;
            case NodePositionAnchor.TOP_RIGHT:
                offset.X = size.X;
                break;
            case NodePositionAnchor.CENTER_LEFT:
                offset.Y = size.Y / 2;
                break;
            case NodePositionAnchor.CENTER_CENTER:
                offset.X = size.X / 2;
                offset.Y = size.Y / 2;
                break;
            case NodePositionAnchor.CENTER_RIGHT:
                offset.X = size.X;
                offset.Y = size.Y / 2;
                break;
            case NodePositionAnchor.BOTTOM_LEFT:
                offset.Y = size.Y;
                break;
            case NodePositionAnchor.BOTTOM_CENTER:
                offset.X = size.X / 2;
                offset.Y = size.Y;
                break;
            case NodePositionAnchor.BOTTOM_RIGHT:
                offset.X = size.X;
                offset.Y = size.Y;
                break;
        }

        return offset;
    }

    /// <summary>
    /// Given an anchor, get the absolute position of a node.
    /// </summary>
    /// <param name="node">Pointer to the node whose absolute position to get, given an anchor to offset with.</param>
    /// <param name="anchor">Offset anchor to use.</param>
    /// <returns></returns>
    public static Vector2 GetNodeAbsolutePositionForAnchor(AtkResNode* node, NodePositionAnchor anchor) {
        if (node == null) {
            Service.Logger.Warning("[GetNodeAbsolutePositionForAnchor] Node is null!");
            return Vector2.Zero;
        }

        return GetNodeAbsolutePosition(node) + GetNodePositionAnchorOffset(node, anchor);
    }

    /// <summary>
    /// Check if a point intersects a rect, given it's position and size.
    /// </summary>
    /// <param name="point">The point used for the intersection check.</param>
    /// <param name="rectPos">The position in which the rect begins.</param>
    /// <param name="rectSize">The size of the rect.</param>
    /// <returns>True if the point is within the rect.</returns>
    public static bool GetPointIntersectsRect(Vector2 point, Vector2 rectPos, Vector2 rectSize) {
        return point.X >= rectPos.X &&
                   point.Y >= rectPos.Y &&
                   point.X <= rectPos.X + rectSize.X &&
                   point.Y <= rectPos.Y + rectSize.Y;
    }

    /// <summary>
    /// Check if a point intersects a node.
    /// </summary>
    /// <param name="node">Pointer to the node used for the intersection check.</param>
    /// <param name="point">The point used for the intersection check.</param>
    /// <returns>True if the point is within the node.</returns>
    public static bool GetPointIntersectsNode(AtkResNode* node, Vector2 point) {
        if (node == null) {
            Service.Logger.Warning("[GetPointIntersectsNode] Node is null!");
            return false;
        }

        return GetPointIntersectsRect(
            point,
            GetNodeAbsolutePosition(node),
            GetNodeScaledSize(node)
        );
    }

    /// <summary>
    /// Draw an outline around the node.
    /// </summary>
    /// <param name="node">Pointer to the node to draw an outline around.</param>
    /// <param name="name">Text to draw on top of the outline.</param>
    /// <param name="BackgroundColor">Optional background color of outline.</param>
    /// <param name="VisibleColor">Optional color of the outline for when the node is visible.</param>
    /// <param name="InvisibleColor">Optional color of the outline for when the node is invisible.</param>
    public static void DrawOutline(AtkResNode* node, string name, uint BackgroundColor = 0x08000000, uint VisibleColor = 0xFF00FF00, uint InvisibleColor = 0xFF0000FF) {
        if (node == null) {
            Service.Logger.Warning("[DrawOutline] Node is null!");
            return;
        }

        ImDrawListPtr drawList = ImGui.GetForegroundDrawList(ImGuiHelpers.MainViewport);
        Vector2 position = GetNodeAbsolutePosition(node) + (Vector2)ImGuiHelpers.MainViewport.Pos;
        Vector2 size = GetNodeScaledSize(node);
        bool nodeVisible = GetNodeVisible(node);
        uint outlineColor = nodeVisible ? VisibleColor : InvisibleColor;
        
        drawList.AddRectFilled(position, position + size, BackgroundColor);
        drawList.AddRect(position, position + size, outlineColor);
        drawList.AddText(position, outlineColor, name);
    }
}

public unsafe class AddonNodeHelper {
    public AtkResNode* Node = (AtkResNode*)IntPtr.Zero;

    /// <summary>
    /// Get the anchored position of this node.
    /// </summary>
    /// <param name="anchor">Anchor to use.</param>
    /// <returns>The anchored position of this node. This is not absolute.</returns>
    public Vector2 GetPositionAnchored(NodePositionAnchor anchor) {
        Vector2 offset = AddonToolkitHelper.GetNodePositionAnchorOffset(Node, anchor);
        offset.X += Node->X;
        offset.Y += Node->Y;
        return offset;
    }

    /// <summary>
    /// Get the absolute (screen-space) anchored position of this node.
    /// </summary>
    /// <param name="anchor">Anchor to use.</param>
    public Vector2 GetAbsolutePositionAnchored(NodePositionAnchor anchor) {
        return AddonToolkitHelper.GetNodeAbsolutePositionForAnchor(Node, anchor);
    }

    /// <summary>
    /// Set the anchored position of this node.
    /// </summary>
    /// <param name="anchor">Anchor to use.</param>
    /// <param name="pos">Position to set the node to.</param>
    public void SetPositionAnchored(NodePositionAnchor anchor, Vector2 pos) {
        Vector2 offset = AddonToolkitHelper.GetNodePositionAnchorOffset(Node, anchor);
        Node->X = pos.X - offset.X;
        Node->Y = pos.Y - offset.Y;
    }

    /// <summary>
    /// Set the absolute (screen-space) anchored position of this node.
    /// </summary>
    /// <param name="anchor">Anchor to use.</param>
    /// <param name="pos">Position to set the node to.</param>
    /// <returns>The anchored position of this node. This is absolute.</returns>
    public void SetAbsolutePositionAnchored(NodePositionAnchor anchor, Vector2 pos) {
        Vector2 absolutePosition = AddonToolkitHelper.GetNodeAbsolutePositionForAnchor(Node, anchor);
        Vector2 offset = AddonToolkitHelper.GetNodePositionAnchorOffset(Node, anchor);
        Node->X = pos.X - (absolutePosition.X - offset.X);
        Node->Y = pos.Y - (absolutePosition.Y - offset.Y);
    }


    /// <summary>
    /// Get the real (TOP_LEFT) position of this node.
    /// </summary>
    /// <returns>The real (TOP_LEFT) position of this node.</returns>
    public Vector2 Position {
        get { return new Vector2(Node->X, Node->Y); }
        set { Node->X = value.X; Node->Y = value.Y; }
    }

    /// <summary>
    /// The size of this node.
    /// </summary>
    public Vector2 Size {
        get {
            return new Vector2(Node->Width, Node->Height);
        }
        set {
            Node->Width = (ushort)value.X;
            Node->Height = (ushort)value.Y;
        }
    }

    /// <summary>
    /// The total (scaled) size of this node. This takes the node's parents into consideration.
    /// </summary>
    public Vector2 SizeScaled {
        get {
            return AddonToolkitHelper.GetNodeScaledSize(Node);
        }
        set {
            Vector2 scale = AddonToolkitHelper.GetNodeScale(Node);
            if (scale.X != 0) {
                Node->Width = (ushort)(value.X / scale.X);
            }
            else {
                Service.Logger.Warning("[AddonNodeHelper SizeScaled Setter] Invalid scale X value.");
            }

            if (scale.Y != 0) {
                Node->Height = (ushort)(value.Y / scale.Y);
            }
            else {
                Service.Logger.Warning("[AddonNodeHelper SizeScaled Setter] Invalid scale Y value.");
            }
        }
    }

    /// <summary>
    /// The unscaled scale of this node.
    /// </summary>
    public Vector2 Scale {
        get {
            return new Vector2(Node->ScaleX, Node->ScaleY);
        }
        set {
            Node->ScaleX = value.X;
            Node->ScaleY = value.Y;
        }
    }

    /// <summary>
    /// The total (scaled) scale of this node. This takes the node's parents into consideration.
    /// </summary>
    public Vector2 ScaleScaled {
        get {
            return AddonToolkitHelper.GetNodeScale(Node);
        }
        set {
            Vector2 scale = AddonToolkitHelper.GetNodeScale(Node);
            if (scale.X != 0) {
                Node->ScaleX = (ushort)(value.X / scale.X);
            }
            else {
                Service.Logger.Warning("[AddonNodeHelper ScaleScaled Setter] Invalid scale X value.");
            }

            if (scale.Y != 0) {
                Node->ScaleY = (ushort)(value.Y / scale.Y);
            }
            else {
                Service.Logger.Warning("[AddonNodeHelper ScaleScaled Setter] Invalid scale Y value.");
            }
        }
    }

    /// <summary>
    /// The opacity/alpha of this node. This does note take into account the AtkUnitBase.
    /// </summary>
    public byte Opacity {
        get {
            return Node->Alpha_2;
        }
        set {
            Node->Alpha_2 = value;
        }
    }

    public void DrawOutline(string label) {
        AddonToolkitHelper.DrawOutline(Node, label);
    }


    AddonNodeHelper(AtkResNode* node) {
        Node = node;
    }
}

