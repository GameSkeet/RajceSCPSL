using System;

namespace RajceUI.ImGuiNET
{
    [Flags]
    public enum ImGuiSelectableFlags
    {
        None = 0,
        DontClosePopups = 1,
        SpanAllColumns = 2,
        AllowDoubleClick = 4,
        Disabled = 8,
        AllowItemOverlap = 16,
    }
}
