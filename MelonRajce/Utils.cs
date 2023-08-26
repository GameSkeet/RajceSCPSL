using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

namespace MelonRajce
{
    internal static class Utils
    {
        public enum ChamType
        {
            Flat,
            Shaded,
            BringToFront
        }

        private static Dictionary<string, AssetBundle> Bundles = new Dictionary<string, AssetBundle>();        

        public static byte[] ReadResource(string name)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            string foundResource = null;
            foreach (string resource in asm.GetManifestResourceNames())
            {
                if (resource.ToLower().EndsWith(name.ToLower()))
                {
                    foundResource = resource;
                    break;
                }
            }

            if (string.IsNullOrEmpty(foundResource))
            {
                Console.WriteLine("Cannot find '{0}'", name);
                return null;
            }

            MemoryStream ms = new MemoryStream();
            asm.GetManifestResourceStream(foundResource).CopyTo(ms);

            return ms.ToArray();
        }

        public static AssetBundle LoadBundle(string name)
        {
            if (Bundles.ContainsKey(name))
                return Bundles[name]; // Return the cached bundle

            AssetBundle bundle = AssetBundle.LoadFromMemory(ReadResource(name)); // Load the bundle from the resource

            return Bundles[name] = bundle; // Chache the bundle so we dont need to load it more than once
        }

        public static Rect SetSize(ref Rect rect, Vector2 vec)
        {
            rect.size = vec; // Set the rect size

            return rect;
        }

        public static bool IsTeamMate(Team me, Team player)
        {
            switch (me)
            {
                case Team.SCP:
                    return player == Team.SCP;
                case Team.MTF:
                    return player == Team.MTF || player == Team.RSC;
                case Team.CHI:
                    return player == Team.CHI || player == Team.CDP;
                case Team.RSC:
                    return player == Team.RSC || player == Team.MTF;
                case Team.CDP:
                    return player == Team.CHI;
            }

            return false;
        }
    }
}
