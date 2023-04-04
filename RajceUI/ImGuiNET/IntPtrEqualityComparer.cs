using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RajceUI.ImGuiNET
{
    public class IntPtrEqualityComparer : IEqualityComparer<IntPtr>
    {
        public static IntPtrEqualityComparer Instance { get; } = new IntPtrEqualityComparer();

        IntPtrEqualityComparer() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(IntPtr p1, IntPtr p2) => p1 == p2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(IntPtr ptr) => ptr.GetHashCode();
    }
}
