using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Com3d2Mod
{
    public class NativeLoader
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetDllDirectory(string lpPathName);
    }
}