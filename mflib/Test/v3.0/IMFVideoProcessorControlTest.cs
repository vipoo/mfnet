using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Testv30
{
    class IMFVideoProcessorControlTest
    {
        public void DoTests()
        {
            IMFVideoProcessorControl vpc = new VideoProcessorMFT() as IMFVideoProcessorControl;

            int hr;

            MFARGB rgb = new MFARGB();
            hr = vpc.SetBorderColor(rgb);
            MFError.ThrowExceptionForHR(hr);

            MFRect r, r2;

            r = new MFRect();
            r2 = new MFRect();

            hr = vpc.SetSourceRectangle(r);
            MFError.ThrowExceptionForHR(hr);

            hr = vpc.SetDestinationRectangle(r2);
            MFError.ThrowExceptionForHR(hr);

            hr = vpc.SetMirror(MF_VIDEO_PROCESSOR_MIRROR.Horizontal);
            MFError.ThrowExceptionForHR(hr);

            MFSize s = new MFSize();

            hr = vpc.SetConstrictionSize(s);
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
