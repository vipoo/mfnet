using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Testv30
{
    [ComImport, Guid("4B0B6227-8B08-4b45-8BA9-02944B25DDD9")]
    public class Hack
    {
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("9F7AF24D-C1F0-4b88-8444-AB695F4A29A2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IHack
    {
        void Set(IntPtr lpInterface,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.Bool)] bool bAddRef
            );
    }
    class IMFSeekInfoTest
    {
        // UNTESTABLE - Due to MF not support QI.
#if false
        IMFSeekInfo m_si;
#endif

        public void DoTests()
        {
#if false
            Init();

            int hr;

            PropVariant pvSeekTime = new PropVariant(0);
            PropVariant pvarPreviousKeyFrame = new PropVariant();
            PropVariant pvarNextKeyFrame = new PropVariant();

            hr = m_si.GetNearestKeyFrames(Guid.Empty, pvSeekTime, pvarPreviousKeyFrame, pvarNextKeyFrame);
            MFError.ThrowExceptionForHR(hr);
#endif
        }

        private void Init()
        {
#if false
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
            hr = gs.GetService(MFServices.MF_SCRUBBING_SERVICE, typeof(IMFSeekInfo).GUID, out o2);
            MFError.ThrowExceptionForHR(hr);

            //m_si = o2 as IMFSeekInfo;

            // The object returned from GetService DOESN'T SUPPORT QI CORRECTLY!!!!!
            // This interface figures that passing back a pointer that it
            // says is an IMFSeekInfo is close enough, but .Net REQUIRES that QI
            // for the interface work correctly.

            IntPtr p = Marshal.GetIUnknownForObject(o2);

            Guid g = typeof(IMFSeekInfo).GUID;
            IntPtr p2;
            hr = Marshal.QueryInterface(p, ref g, out p2);

            IHack h1 = (IHack)new Hack();

            h1.Set(p, typeof(IMFSeekInfo).GUID, true);
            m_si = h1 as IMFSeekInfo;
#endif
        }
    }
}
