using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.EVR;

namespace Testv21
{
    class IMFVideoMixerControl2Test
    {
        public void DoTests()
        {
            IMFGetService gs = (IMFGetService)new EnhancedVideoRenderer();
            object o;
            int hr;
            hr = gs.GetService(MFServices.MR_VIDEO_MIXER_SERVICE, typeof(IMFVideoMixerControl2).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            IMFVideoMixerControl2 vmc = (IMFVideoMixerControl2)o;
            hr = vmc.SetMixingPrefs(MFVideoMixPrefs.AllowDropToBob | MFVideoMixPrefs.ForceBob);
            MFError.ThrowExceptionForHR(hr);

            MFVideoMixPrefs mp;
            hr = vmc.GetMixingPrefs(out mp);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(mp == (MFVideoMixPrefs.AllowDropToBob | MFVideoMixPrefs.ForceBob));
        }
    }
}
