using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.EVR;

namespace Testv21
{
    class IMFQualityAdviseLimitsTest
    {
        IMFQualityAdviseLimits qal;

        public void DoTests()
        {
            int hr;
            object o;

            IMFGetService gs = (IMFGetService)new EnhancedVideoRenderer();

            hr = gs.GetService(MFServices.MR_VIDEO_RENDER_SERVICE, typeof(IMFQualityAdviseLimits).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            qal = (IMFQualityAdviseLimits)o;

            MFQualityDropMode dm;
            hr = qal.GetMaximumDropMode(out dm);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(dm == MFQualityDropMode.None);

            MFQualityLevel ql;
            hr = qal.GetMinimumQualityLevel(out ql);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(ql == MFQualityLevel.Normal);

            /* In theory, you can use this to set other limits, but EVR doesn't
             * seem to support anything else. */
            IMFQualityAdvise qa = (IMFQualityAdvise)qal;

            hr = qa.SetDropMode(MFQualityDropMode.Mode1);
            //MFError.ThrowExceptionForHR(hr);

            hr = qa.SetQualityLevel(MFQualityLevel.NormalMinus1);
            //MFError.ThrowExceptionForHR(hr);
        }
    }
}
