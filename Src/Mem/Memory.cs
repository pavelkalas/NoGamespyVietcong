using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

/**
 * NoGamespyVietcong v1.0
 * by Pavel Kalaš 2025 (Floxen).
 * -----------------------------
 * https://github.com/pavelkalas/NoGamespyVietcong
 */

namespace NoGamespyVietcong.Src.Mem
{
    class Memory
    {
        // importování funkcí z user32.dll
        #region user32.dll imports
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)] private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)] private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")] private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)] private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);
        [DllImport("kernel32.dll", SetLastError = true)] [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)] [SuppressUnmanagedCodeSecurity] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        #endregion

        // konstanty přístupu
        private const uint PROCESS_VM_READ = 0x0010;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;
        private const uint PROCESS_ALL_ACCESS = 0x001F0FFF;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;

        /// <summary>
        /// Zapíše objekt na adresu v paměti v konkrétním modulu
        /// </summary>
        /// <param name="processId">ID procesu</param>
        /// <param name="moduleName">Jméno DLL modulu v paměti</param>
        /// <param name="offset">Adresa</param>
        /// <param name="value">Hodnota</param>
        /// <returns></returns>
        public static bool WriteObjectToAddress(int processId, string moduleName, int offset, object value)
        {
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processId);

            if (hProcess == IntPtr.Zero)
            {
                return false;
            }

            Process process = Process.GetProcessById(processId);
            IntPtr moduleBase = IntPtr.Zero;
            IntPtr address;

            if (!string.IsNullOrEmpty(moduleName))
            {
                foreach (ProcessModule module in process.Modules)
                {
                    if (module.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                    {
                        moduleBase = module.BaseAddress;
                        break;
                    }
                }

                if (moduleBase == IntPtr.Zero)
                {
                    CloseHandle(hProcess);
                    return false;
                }

                address = IntPtr.Add(moduleBase, offset);
            }
            else
            {
                address = IntPtr.Add(process.MainModule.BaseAddress, offset);
            }

            byte[] buffer;

            if (value is int i)
            {
                buffer = BitConverter.GetBytes(i);
            }
            else if (value is float f)
            {
                buffer = BitConverter.GetBytes(f);
            }
            else if (value is double d)
            {
                buffer = BitConverter.GetBytes(d);
            }
            else if (value is long l)
            {
                buffer = BitConverter.GetBytes(l);
            }
            else if (value is short s)
            {
                buffer = BitConverter.GetBytes(s);
            }
            else if (value is byte b)
            {
                buffer = new[] { b };
            }
            else if (value is string str)
            {
                buffer = Encoding.UTF8.GetBytes(str + '\0');
            }
            else
            {
                try
                {
                    int size = Marshal.SizeOf(value);
                    IntPtr ptr = Marshal.AllocHGlobal(size);
                    Marshal.StructureToPtr(value, ptr, false);
                    buffer = new byte[size];
                    Marshal.Copy(ptr, buffer, 0, size);
                    Marshal.FreeHGlobal(ptr);
                }
                catch
                {
                    CloseHandle(hProcess);
                    return false;
                }
            }

            if (!VirtualProtectEx(hProcess, address, (uint)buffer.Length, PAGE_EXECUTE_READWRITE, out uint oldProtect))
            {
                CloseHandle(hProcess);
                return false;
            }

            bool result = WriteProcessMemory(hProcess, address, buffer, (uint)buffer.Length, out _);
            VirtualProtectEx(hProcess, address, (uint)buffer.Length, oldProtect, out _);

            CloseHandle(hProcess);
            return result;
        }

        /// <summary>
        /// Injectne DLL soubor do cílového procesu.
        /// </summary>
        /// <param name="processHandle">Handle procesu, do kterého se má injektovat.</param>
        /// <param name="dllPath">Absolutní cesta k DLL souboru.</param>
        public static void InjectDllToProcess(Process processHandle, string dllPath)
        {
            if (processHandle != null)
            {
                IntPtr handle = OpenProcess(0x001F0FFF, false, processHandle.Id);
                IntPtr LibraryAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                IntPtr AllocatedMemory = VirtualAllocEx(handle, IntPtr.Zero, (uint)dllPath.Length + 1, 0x00001000, 4);
                WriteProcessMemory(handle, AllocatedMemory, Encoding.Default.GetBytes(dllPath), (uint)dllPath.Length + 1, out _);
                IntPtr threadHandle = CreateRemoteThread(handle, IntPtr.Zero, 0, LibraryAddress, AllocatedMemory, 0, IntPtr.Zero);
                WaitForSingleObject(handle, 500);
                CloseHandle(threadHandle);
                VirtualFreeEx(handle, AllocatedMemory, dllPath.Length + 1, 0x8000);
                CloseHandle(handle);
            }
        }
    }
}
