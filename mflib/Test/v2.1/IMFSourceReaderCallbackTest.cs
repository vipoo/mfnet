using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.ReadWrite;
using System.Runtime.InteropServices;
using MediaFoundation.Alt;

namespace Testv21
{
    class IMFSourceReaderCallbackTest : COMBase, IMFSourceReaderCallback
    {
        int m_Status = 0;

        public void DoTests()
        {
            int hr;

            IMFSourceReader rdr;
            IMFMediaSource ms;
            object pSource;
            IMFSourceResolver sr;
            MFObjectType pObjectType;

            hr = MFExtern.MFCreateSourceResolver(out sr);
            MFError.ThrowExceptionForHR(hr);

            hr = sr.CreateObjectFromURL(
                //@"c:/sourceforge/mflib/test/media/AspectRatio4x3.wmv", // Won't do OnEvent
                @"http://www.LimeGreenSocks.com/AspectRatio4x3.wmv",
                MFResolution.MediaSource,
                null,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            ms = pSource as IMFMediaSource;

            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = ia.SetUnknown(MFAttributesClsid.MF_SOURCE_READER_ASYNC_CALLBACK, this);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateSourceReaderFromMediaSource(ms, ia, out rdr);
            MFError.ThrowExceptionForHR(hr);

            IMFSourceReaderAsync sr2 = (IMFSourceReaderAsync)rdr;

            hr = sr2.ReadSample((int)MF_SOURCE_READER.FirstVideoStream, MF_SOURCE_READER_CONTROL_FLAG.None, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            MFError.ThrowExceptionForHR(hr);

            hr = sr2.Flush(1);
            MFError.ThrowExceptionForHR(hr);

            for (int x = 0; x < 30; x++)
            {
                if (m_Status != 15)
                    System.Threading.Thread.Sleep(1000);
                else
                    break;
            }

            Debug.Assert(m_Status == 15);
        }

        public int OnReadSample(int hrStatus, int dwStreamIndex, MF_SOURCE_READER_FLAG dwStreamFlags, long llTimestamp, IMFSample pSample)
        {
            m_Status |= 1;
            return hrStatus;
        }

        public int OnFlush(int dwStreamIndex)
        {
            m_Status |= 2;

            return S_Ok;
        }

        public int OnEvent(int dwStreamIndex, IMFMediaEvent pEvent)
        {
            MediaEventType pmet;
            int hr = pEvent.GetType(out pmet);

            if (hr >= 0)
            {
                switch (pmet)
                {
                    case MediaEventType.MEBufferingStarted:
                        m_Status |= 4;
                        break;
                    case MediaEventType.MEBufferingStopped:
                        m_Status |= 8;
                        break;
                }
            }

            return hr;
        }
    }
}
