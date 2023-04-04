using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

namespace RajceInternal
{
    public class Main
    {
        [DllImport("kernel32")]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32")]
        private static extern void SetStdHandle(int nStdHandle, IntPtr handle);
        [DllImport("kernel32")]
        private static extern bool SetConsoleMode(IntPtr hHandle, int mode);
        [DllImport("kernel32")]
        private static extern bool GetConsoleMode(IntPtr hHandle, out int mode);
        [DllImport("kernel32")]
        private static extern bool AllocConsole();

        private static Dictionary<string, Assembly> AsmDict = new Dictionary<string, Assembly>();

        private static GameObject gameObject;
        private static Menu menu;

        internal static CoroutineThing _Coroutine = null;

        public static Dictionary<string, Shader> LoadedShaders = new Dictionary<string, Shader>();
        public static Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>();

        private static void LoadAssembly(string assemblyName, string filename)
        {
            byte[] assemblyBytes = null;
            Assembly asm = null;
            Assembly currAsm = Assembly.GetExecutingAssembly();

            using (Stream s = currAsm.GetManifestResourceStream(assemblyName))
            {
                if (s == null)
                    throw new Exception(assemblyName + " is not found in Resources");

                assemblyBytes = new byte[s.Length];
                s.Read(assemblyBytes, 0, (int)s.Length);
                
                try
                {
                    asm = Assembly.Load(assemblyBytes);
                    AsmDict.Add(asm.FullName, asm);

                    return;
                } catch {}
            }
        }
        private static Assembly Get(string asmFullName)
        {
            if (AsmDict.Count == 0)
                return null;

            if (AsmDict.TryGetValue(asmFullName, out Assembly asm))
                return asm;

            return null;
        }

        private static void LoadData()
        {
            Console.WriteLine("Loading data");

            var ba = AssetBundle.LoadFromMemoryAsync(Utils.ReadResource("Data.assets"));
            ba.completed += (op) =>
            {
                Console.WriteLine("Finished loading Data.assets");

                AssetBundle bundle = ba.assetBundle;
                if (bundle == null)
                {
                    Console.WriteLine("Loaded data is invalid");
                    return;
                }

                foreach (Shader shader in bundle.LoadAllAssets<Shader>())
                {
                    Console.WriteLine("Adding shader: {0}", shader.name);
                    LoadedShaders[shader.name.ToLower()] = shader;
                }
                Console.WriteLine("Loaded all shaders");

                foreach (AudioClip audio in bundle.LoadAllAssets<AudioClip>())
                {
                    Console.WriteLine("Adding audio: {0}", audio.name);
                    Sounds[audio.name.ToLower()] = audio;
                }
                Console.WriteLine("Loaded all sounds");
            };
        }

        public static void Inject()
        {
            AllocConsole();

            TextWriter writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(writer);

            IntPtr stdout = GetStdHandle(-11);
            GetConsoleMode(stdout, out int mode);
            SetConsoleMode(stdout, mode | 0x4);

            Console.WriteLine("Attached Console successfully");

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                ConsoleColor old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(args.ExceptionObject);
                Console.ForegroundColor = old;
            };

            //LoadAssembly("RajceInternal.Res.RajceUI.dll", "RajceUI.dll");

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                return Get(args.Name);
            };

            LoadData();

            gameObject = new GameObject();
            GameObject.DontDestroyOnLoad(gameObject);
            _Coroutine = gameObject.AddComponent<CoroutineThing>();
            menu = gameObject.AddComponent<Menu>();

            Console.WriteLine("RajceInternal fully loaded");
        }

        public static void Eject()
        {
            Console.WriteLine("Unloading RajceInternal");

            Utils.ClearCaches();
            menu.DoCleaning();
            GameObject.Destroy(gameObject);
        }
    }
}
