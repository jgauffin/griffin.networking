using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    public static class SecureStringExtensions
    {
        public static string ToClearText(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}