using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFImageSharingEngineTest
    {
        public void DoTests()
        {
            // returns 0x80040111 (Class not available)
            IMFImageSharingEngineClassFactory isecf = new MFMediaSharingEngineClassFactory() as IMFImageSharingEngineClassFactory;
            Debug.Assert(isecf != null);

            IMFImageSharingEngine ise;
            int hr = isecf.CreateInstanceFromUDN("asdf", out ise);
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
