using System.Runtime.CompilerServices;

namespace RajceUI.ImGuiNET
{
    public unsafe partial struct ImGuiSizeCallbackDataPtr
    {
        public ImGuiSizeCallbackDataPtr(ref ImGuiSizeCallbackData data)
        {
            NativePtr = (ImGuiSizeCallbackData*)Unsafe.AsPointer(ref data);
        }
    }
}
