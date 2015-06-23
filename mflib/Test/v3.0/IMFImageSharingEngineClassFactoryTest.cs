using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFImageSharingEngineClassFactoryTest
    {
        public void DoTests()
        {
#if false
            // returns 0x80040111 (Class not available)
            IMFImageSharingEngineClassFactory isecf = new MFMediaSharingEngineClassFactory() as IMFImageSharingEngineClassFactory;
            Debug.Assert(isecf != null);
#endif
        }
    }
}
