using System;
using System.IO;
using System.Reflection;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Object = UnityEngine.Object;

namespace RajceInternal
{
    internal static class Utils
    {
        private static Dictionary<string, Texture> m_dTextureCache = new Dictionary<string, Texture>();

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
        public static Texture LoadTextureFromResource(Vector2 size, string name)
        {
            string texID = size.x + name + size.y;
            if (m_dTextureCache.TryGetValue(texID, out Texture tex))
                return tex;

            byte[] data = ReadResource(name);
            if (data == null)
                return null;

            Texture2D tex0 = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGBA32, false, false);
            tex0.LoadRawTextureData(data);
            m_dTextureCache[texID] = tex0;

            return tex0;
        }

        public static void ClearCaches()
        {
            foreach (KeyValuePair<string, Texture> kvp in m_dTextureCache)
                Object.Destroy(kvp.Value);

            m_dTextureCache.Clear();
        }
    }
}
