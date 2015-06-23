using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Testv30
{
    class IMFProtectedEnvironmentAccessTest
    {
        public void DoTests()
        {
#if false
            int hr;

            IMFProtectedEnvironmentAccess pea;
            hr = MFExtern.MFCreateProtectedEnvironmentAccess(out pea);
            MFError.ThrowExceptionForHR(hr);

            Program.IsA(pea, typeof(IMFProtectedEnvironmentAccess).GUID);

            IntPtr ip;
            int i;
            hr = pea.ReadGRL(out i, out ip);
            MFError.ThrowExceptionForHR(hr);

            IntPtr ipout = Marshal.AllocCoTaskMem(1000);
            hr = pea.Call(0, IntPtr.Zero, 1000, ipout);
            MFError.ThrowExceptionForHR(hr);
#endif
        }
    }
}
