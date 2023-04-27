using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace MelonRajce
{
    internal static class Extensions
    {
        public static string Rep(this string s, int c)
        {
            string res = s;

            for (int i = 0; i < c; i++)
                res += s;

            return res;
        }

        public static bool IsNegative(this int i) => (i >> 31) == 1;
        public static bool IsNegative(this float f) => Math.Abs(f) != f;

        public static Vector2 ToV2(this Vector3 vec) => new Vector2(vec.x, vec.y);

        public static Rect Copy(this Rect rect) => new Rect(rect.position, rect.size);
        public static Rect FixWH(this Rect rect)
        {
            if (rect.width.IsNegative())
            {
                rect.width = Math.Abs(rect.width);
                rect.x -= rect.width;
            }
            if (rect.height.IsNegative())
            {
                rect.height = Math.Abs(rect.height);
                rect.y -= rect.height;
            }

            return rect;
        }

        public static GUIStyle Copy(this GUIStyle style) => new GUIStyle(style);
    }
}
