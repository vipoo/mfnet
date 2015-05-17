using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFByteStreamCacheControl2Test
    {
        IMFByteStreamCacheControl2 m_bscc2;

        public void DoTests()
        {
            GetInterface();
            int hr;

            const int iBuffSize = 1024 * 20000;

            hr = m_bscc2.SetCacheLimit(iBuffSize);
            MFError.ThrowExceptionForHR(hr);

            int iCount = 0;

            bool b;
            do
            {
                hr = m_bscc2.IsBackgroundTransferActive(out b);
                MFError.ThrowExceptionForHR(hr);
                iCount++;
            } while (b);

            Debug.Assert(iCount > 1);

            int i;
            MF_BYTE_STREAM_CACHE_RANGE[] pr = new MF_BYTE_STREAM_CACHE_RANGE[4];
            hr = m_bscc2.GetByteRanges(out i, out pr);

            Debug.Assert(i == 1 && pr[0].qwStartOffset == 0 && pr[0].qwEndOffset == iBuffSize);
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
                Program.File2,
                MFResolution.ByteStream,
                null,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            m_bscc2 = pSource as IMFByteStreamCacheControl2;
        }
    }
}
