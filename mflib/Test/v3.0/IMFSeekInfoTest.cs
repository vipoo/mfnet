using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Testv30
{
    class IMFSeekInfoTest
    {
        // UNTESTABLE - Due to MF not support QI.
        IMFSeekInfo m_si;

        public void DoTests()
        {
            Init();

            int hr;

            PropVariant pvSeekTime = new PropVariant(1000000);
            PropVariant pvarPreviousKeyFrame = new PropVariant();
            PropVariant pvarNextKeyFrame = new PropVariant();

            hr = m_si.GetNearestKeyFrames(Guid.Empty, pvSeekTime, pvarPreviousKeyFrame, pvarNextKeyFrame);
            MFError.ThrowExceptionForHR(hr);
        }

        private void Init()
        {
            IMFMediaSource pSource;
            IMFSourceResolver sr;
            MFObjectType pObjectType;
            object o;

            int hr = MFExtern.MFCreateSourceResolver(out sr);
            MFError.ThrowExceptionForHR(hr);

            hr = sr.CreateObjectFromURL(
                @"C:\Windows\Performance\WinSAT\Clip_1080_5sec_10mbps_h264.mp4",
                MFResolution.MediaSource,
                null,
                out pObjectType,
                out o);
            MFError.ThrowExceptionForHR(hr);

            pSource = o as IMFMediaSource;

            IMFGetService gs = o as IMFGetService;

            object o2;
            hr = gs.GetService(MFServices.MF_SCRUBBING_SERVICE, gUnk, out o2);
            MFError.ThrowExceptionForHR(hr);

            m_si = o2 as IMFSeekInfo;

            // The object returned from GetService DOESN'T SUPPORT QI CORRECTLY!!!!!
            // This interface figures that passing back a pointer that it
            // says is an IMFSeekInfo is close enough, but .Net REQUIRES that QI
            // for the interface work correctly.

            IntPtr p = Marshal.GetIUnknownForObject(o2);

            Guid g = typeof(IMFSeekInfo).GUID;
            IntPtr p2;
            hr = Marshal.QueryInterface(p, ref g, out p2);
        }
    }
}
