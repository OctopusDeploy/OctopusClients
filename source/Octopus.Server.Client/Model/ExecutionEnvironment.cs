﻿#if NET462
using System;
using System.Runtime.InteropServices;

namespace Octopus.Client.Model
{
    internal static class ExecutionEnvironment
    {
        /// <summary>
        /// Based on some internal methods used my mono itself
        /// https://github.com/mono/mono/blob/master/mcs/class/corlib/System/Environment.cs
        /// </summary>
        public static bool IsRunningOnNix => (Environment.OSVersion.Platform == PlatformID.Unix) && !IsRunningOnMac;

        //from https://github.com/jpobst/Pinta/blob/master/Pinta.Core/Managers/SystemManager.cs#L162
        //(MIT license)
        [DllImport("libc")]//From Managed.Windows.Forms/XplatUI
        static extern int uname(IntPtr buf);

        public static bool IsRunningOnMac
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                    return true;
                if (Environment.OSVersion.Platform != PlatformID.Unix)
                    return false;

                var buf = IntPtr.Zero;
                try
                {
                    buf = Marshal.AllocHGlobal(8192);
                    // Get sysname from uname ()
                    if (uname(buf) == 0)
                    {
                        var os = Marshal.PtrToStringAnsi(buf);
                        if (os == "Darwin")
                            return true;
                    }
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    if (buf != IntPtr.Zero)
                        Marshal.FreeHGlobal(buf);
                }
                return false;
            }
        }
    }
}
#endif