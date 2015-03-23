using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Testv22
{
    class IMFByteStreamBufferingTest
    {
        IMFByteStreamBuffering m_bsb;

        public void DoTests()
        {
            int hr;

            GetInterface();

            hr = m_bsb.EnableBuffering(true);
            MFError.ThrowExceptionForHR(hr);

            hr = m_bsb.StopBuffering();
            Debug.Assert(hr == 1); // Not buffering

            MFByteStreamBufferingParams bp = new MFByteStreamBufferingParams();
            bp.cBuckets = 5;
            bp.prgBuckets = new MF_LeakyBucketPair[bp.cBuckets];

            hr = m_bsb.SetBufferingParams(bp);
            MFError.ThrowExceptionForHR(hr);
        }

        private void GetInterface()
        {
            int hr;

            IMFSourceResolver m_sr;

            hr = MFExtern.MFCreateSourceResolver(out m_sr);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType pObjectType;
            object pSource;

            hr = m_sr.CreateObjectFromURL(
                @"http://www.LimeGreenSocks.com/AspectRatio4x3.wmv",
                MFResolution.ByteStream,
                null,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            m_bsb = (IMFByteStreamBuffering)pSource;

        }
    }
}
