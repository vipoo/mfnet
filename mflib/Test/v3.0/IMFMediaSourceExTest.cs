using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFMediaSourceExTest
    {
        public void DoTests()
        {
#if false
            IMFMediaSource me;
            CreateMediaSource(@"C:\Windows\Performance\WinSAT\Clip_1080_5sec_10mbps_h264.mp4", out me);

            IMFMediaSourceEx mex;
            mex = me as IMFMediaSourceEx;
        }
        private void CreateMediaSource(string pszURL, out IMFMediaSource ppSource)
        {
            IMFSourceResolver pSourceResolver = null;
            object pSource;

            int hr;
            // Create the source resolver.
            hr = MFExtern.MFCreateSourceResolver(out pSourceResolver);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType ObjectType;
            hr = pSourceResolver.CreateObjectFromURL(
                pszURL,
                MFResolution.MediaSource,
                null,
                out ObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            ppSource = (IMFMediaSource)pSource;

            //done:
            //SafeRelease(&pSourceResolver);
            //SafeRelease(&pSource);
            //return hr;
#endif
        }
    }
}
