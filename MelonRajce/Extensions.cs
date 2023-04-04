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

        public static Vector2 ToV2(this Vector3 vec) => new Vector2(vec.x, vec.y);

        public static Rect Copy(this Rect rect) => new Rect(rect.position, rect.size);

        public static GUIStyle Copy(this GUIStyle style) => new GUIStyle(style);
    }
}
