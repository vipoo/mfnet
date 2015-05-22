using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFMediaErrorTest
    {
        public void DoTests()
        {
            IMFMediaEngineClassFactory mecf = new MFMediaEngineClassFactory() as IMFMediaEngineClassFactory;
            Debug.Assert(mecf != null);

            int hr;
            IMFMediaError merr;

            hr = mecf.CreateError(out merr);
            MFError.ThrowExceptionForHR(hr);

            hr = merr.SetErrorCode(MF_MEDIA_ENGINE_ERR.Aborted);
            MFError.ThrowExceptionForHR(hr);

            MF_MEDIA_ENGINE_ERR m = merr.GetErrorCode();
            Debug.Assert(m == MF_MEDIA_ENGINE_ERR.Aborted);

            hr = merr.SetExtendedErrorCode(1011);
            MFError.ThrowExceptionForHR(hr);

            int q = merr.GetExtendedErrorCode();

            Debug.Assert(q == 1011);
        }
    }
}
