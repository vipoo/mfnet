using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFMediaSharingEngineTest : COMBase, IMFMediaEngineNotify
    {
        public void DoTests()
        {
#if false
            IMFMediaEngineClassFactory mecf = new MFMediaEngineClassFactory() as IMFMediaEngineClassFactory;
            Debug.Assert(mecf != null);

            int hr;
            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 5);
            MFError.ThrowExceptionForHR(hr);

            hr = ia.SetUnknown(MFAttributesClsid.MF_MEDIA_ENGINE_CALLBACK, this);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaEngine me;
            hr = mecf.CreateInstance(MF_MEDIA_ENGINE_CREATEFLAGS.None, ia, out me);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaSharingEngine mse = me as IMFMediaSharingEngine;
#endif
        }

        public int EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            throw new NotImplementedException();
        }
    }
}
