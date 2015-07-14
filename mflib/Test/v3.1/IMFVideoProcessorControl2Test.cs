using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;

namespace Testv31
{
    // untested: EnableHardwareEffects, GetSupportedHardwareEffects
    class IMFVideoProcessorControl2Test
    {
        public void Dotests()
        {
            HResult hr;
            IMFVideoProcessorControl2 vpc = new VideoProcessorMFT() as IMFVideoProcessorControl2;

            hr = vpc.SetRotationOverride((int)MFVideoRotationFormat.R90);
            MFError.ThrowExceptionForHR(hr);

            hr = vpc.EnableHardwareEffects(true);
            //MFError.ThrowExceptionForHR(hr);

            int puiSupport;
            hr = vpc.GetSupportedHardwareEffects(out puiSupport);
            //MFError.ThrowExceptionForHR(hr);
        }
    }
}
