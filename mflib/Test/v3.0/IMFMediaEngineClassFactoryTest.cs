using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFMediaEngineClassFactoryTest : COMBase, IMFMediaEngineNotify
    {
        public void DoTests()
        {
            IMFMediaEngineClassFactory mecf = new MFMediaEngineClassFactory() as IMFMediaEngineClassFactory;
            Debug.Assert(mecf != null);

            int hr;
            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 5);
            MFError.ThrowExceptionForHR(hr);

            hr = ia.SetUnknown(MFAttributesClsid.MF_MEDIA_ENGINE_CALLBACK, this);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaEngine me;
            hr = mecf.CreateInstance(MF_MEDIA_ENGINE_CREATEFLAGS.AudioOnly, ia, out me);
            MFError.ThrowExceptionForHR(hr);

            Program.IsA(me, typeof(IMFMediaEngine).GUID);

            ///////////////////////

            IMFMediaTimeRange tr;
            hr = mecf.CreateTimeRange(out tr);
            MFError.ThrowExceptionForHR(hr);

            Program.IsA(tr, typeof(IMFMediaTimeRange).GUID);

            ///////////////////////

            IMFMediaError merr;
            hr = mecf.CreateError(out merr);
            MFError.ThrowExceptionForHR(hr);

            Program.IsA(merr, typeof(IMFMediaError).GUID);
        }

        public int EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            throw new NotImplementedException();
        }
    }
}
