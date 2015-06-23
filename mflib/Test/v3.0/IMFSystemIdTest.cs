using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFSystemIdTest
    {
        public void DoTests()
        {
#if false
            int hr;

            IMFSystemId id;
            hr = MFExtern.MFGetSystemId(out id);
            MFError.ThrowExceptionForHR(hr);

            //hr = id.Setup();

            int i;
            IntPtr ip;
            hr = id.GetData(out i, out ip);
            MFError.ThrowExceptionForHR(hr);
#endif
        }
    }
}
