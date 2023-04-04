﻿using System;

namespace RajceUI.ImGuiNET
{
    [Flags]
    public enum ImGuiViewportFlags
    {
        None = 0,
        IsPlatformWindow = 1,
        IsPlatformMonitor = 2,
        OwnedByApp = 4,
        NoDecoration = 8,
        NoTaskBarIcon = 16,
        NoFocusOnAppearing = 32,
        NoFocusOnClick = 64,
        NoInputs = 128,
        NoRendererClear = 256,
        TopMost = 512,
        Minimized = 1024,
        NoAutoMerge = 2048,
        CanHostOtherWindows = 4096,
    }
}
