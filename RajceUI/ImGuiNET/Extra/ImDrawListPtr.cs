using System;
using System.Runtime.CompilerServices;

namespace RajceUI.ImGuiNET
{
    public unsafe partial struct ImDrawListPtr : IEquatable<ImDrawListPtr>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ImDrawListPtr other) => NativePtr == other.NativePtr;
    }
}
