using System.Runtime.CompilerServices;

namespace RajceUI.ImGuiNET
{
    public unsafe partial struct ImGuiTextFilterPtr
    {
        public ImGuiTextFilterPtr(ref ImGuiTextFilter filter)
        {
            NativePtr = (ImGuiTextFilter*)Unsafe.AsPointer(ref filter);
        }
    }
}
