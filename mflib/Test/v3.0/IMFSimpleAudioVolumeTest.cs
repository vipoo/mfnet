using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFSimpleAudioVolumeTest
    {
        public void DoTests()
        {
            int hr;

            IMFMediaSink ppSink;
            hr = MFExtern.MFCreateAudioRenderer(null, out ppSink);
            MFError.ThrowExceptionForHR(hr);

            IMFGetService gs;
            gs = (IMFGetService)ppSink;

            object o;
            hr = gs.GetService(MFServices.MR_POLICY_VOLUME_SERVICE, typeof(IMFSimpleAudioVolume).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            IMFSimpleAudioVolume sav = (IMFSimpleAudioVolume)o;

            float start;
            hr = sav.GetMasterVolume(out start);
            MFError.ThrowExceptionForHR(hr);

            hr = sav.SetMasterVolume(.61f);
            MFError.ThrowExceptionForHR(hr);

            float end;
            hr = sav.GetMasterVolume(out end);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(end == .61f);

            hr = sav.SetMasterVolume(start);
            MFError.ThrowExceptionForHR(hr);

            bool m;
            hr = sav.GetMute(out m);
            MFError.ThrowExceptionForHR(hr);

            hr = sav.SetMute(!m);
            MFError.ThrowExceptionForHR(hr);

            bool m2;
            hr = sav.GetMute(out m2);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(m != m2);
            hr = sav.SetMute(m);
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
