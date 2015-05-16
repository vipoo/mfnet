using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFSharingEngineClassFactoryTest
    {
        public void DoTests()
        {
            // returns 0x80040111 (Class not available)
            IMFSharingEngineClassFactory isecf = new MFMediaSharingEngineClassFactory() as IMFSharingEngineClassFactory;
            Debug.Assert(isecf != null);
        }
    }
}
