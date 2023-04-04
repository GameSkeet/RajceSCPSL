using System.Runtime.CompilerServices;

namespace RajceUI.ImGuiNET
{
    public unsafe partial struct ImFontConfigPtr
    {
        public ImFontConfigPtr(ref ImFontConfig fontConfig)
        {
            NativePtr = (ImFontConfig*)Unsafe.AsPointer(ref fontConfig);
        }
    }
}
