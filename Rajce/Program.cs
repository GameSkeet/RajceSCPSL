using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Rajce.SMI;

namespace Rajce
{
    public static class Program
    {
        //private static MainWindow form = null;
        private static Injector injector = null;
        private static IntPtr injectedAsm = IntPtr.Zero;

        private static IntPtr loadlibraryA = IntPtr.Zero;

        private static byte[] RajceInternalBytes = null;

        private static byte[] readEmbed(string name)
        {
            MemoryStream ms = new MemoryStream();
            Assembly.GetExecutingAssembly().GetManifestResourceStream($"Rajce.Resources.{name.Replace("/", ".")}").CopyTo(ms);

            return ms.ToArray();
        }
        private static bool InjectDll(string dll, IntPtr process)
        {
            IntPtr alloc = Native.VirtualAllocEx(process, IntPtr.Zero, 260, AllocationType.MEM_RESERVE | AllocationType.MEM_COMMIT, MemoryProtection.PAGE_EXECUTE_READWRITE);

            if (alloc == IntPtr.Zero)
            {
                Console.WriteLine("Neslo udelat misto v Laboratoři aby se tam mohlo vyskytnout rajce");
                Console.ReadLine();

                return false;
            }

            dll += "\0";
            Native.WriteProcessMemory(process, alloc, Encoding.UTF8.GetBytes(dll), 260);

            if (loadlibraryA == IntPtr.Zero)
            {
                IntPtr kernel = Native.LoadLibraryA("kernel32.dll");
                loadlibraryA = Native.GetProcAddress(kernel, "LoadLibraryA");
            }

            IntPtr ret = Native.CreateRemoteThread(process, IntPtr.Zero, 0, loadlibraryA, alloc, 0, out _);

            Native.VirtualFreeEx(process, alloc, 0, MemoryFreeType.MEM_RELEASE);

            Native.CloseHandle(ret);

            return true;
        }

        private static void WaitForSCP()
        {
            Process[] processes;
            while ((processes = Process.GetProcessesByName("SCPSL")).Length == 0)
            {
                Console.WriteLine("Cekam na SCP ty magore...");
                Thread.Sleep(2500);
            }

            Console.Clear();

            if (processes.Length > 1)
            {
                Console.WriteLine("Active SCPs:");
                foreach (Process process in processes)
                    Console.WriteLine("Process: {0}", process.MainWindowTitle);
            }

            Console.WriteLine("Našla se SCP Skrytá Laboratoř");

            /*IntPtr process = Native.OpenProcess(ProcessAccessRights.PROCESS_ALL_ACCESS, false, processes[0].Id);

            string path1 = Path.GetFullPath("cimgui.dll");
            string path2 = Path.GetFullPath("cimgui-freetype.dll");

            File.WriteAllBytes(path1, readEmbed("cimgui.dll"));
            File.WriteAllBytes(path2, readEmbed("cimgui-freetype.dll"));

            InjectDll(path1, process);
            InjectDll(path2, process);*/

            Console.WriteLine("Moc lehká věc nato aby šlá bypassnout");

            //Native.CloseHandle(process);

            while (true)
            {
                try
                {
                    injector = new Injector(processes[0].Id);
                    injectedAsm = injector.Inject(RajceInternalBytes, "RajceInternal", "Main", "Inject");
                    break;
                }
                catch {}

                Thread.Sleep(1000);
            }

            Console.WriteLine("Nenašla protože je skrytá");
            Console.WriteLine("Ale taky už je Rajcatko v Laboratoři");
        }

        [STAThread]
        public static void Main()
        {
            if (!File.Exists("RajceInternal.dll") && RajceInternalBytes == null)
            {
                Console.WriteLine("Ty hajzle kde máš ten Internal!!!");
                Console.ReadLine();
                return;
            }
            else Console.WriteLine("Nasel jsem RajceInternal takze to mas dobry ty magore");

            if (RajceInternalBytes == null)
                RajceInternalBytes = File.ReadAllBytes("RajceInternal.dll");

            WaitForSCP();

            Thread.Sleep(1000);
            Console.Clear();

            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(form = new MainWindow());*/
        }

        public static void Exit()
        {
            /*if (form == null)
                return;

            form.BeforeClose();*/

            try
            {
                injector.Eject(injectedAsm, "RajceInternal", "Main", "Eject");
                injector.Dispose();
            } catch {}

            injectedAsm = IntPtr.Zero;

            /*form.Close();
            form.Dispose();

            form = null;*/

            Main();
        }
    }
}
