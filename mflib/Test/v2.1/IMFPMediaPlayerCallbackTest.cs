using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.MFPlayer;

namespace Testv21
{
    class IMFPMediaPlayerCallbackTest : COMBase, IMFPMediaPlayerCallback
    {
        bool m_Done = false;
        IMFPMediaPlayer pp;

        public void DoTests()
        {
            int hr;

            hr = MFExtern.MFPCreateMediaPlayer(
                @"c:/sourceforge/mflib/test/media/AspectRatio4x3.wmv",
                false, 
                MediaFoundation.MFPlayer.MFP_CREATION_OPTIONS.FreeThreadedCallback, 
                this, 
                IntPtr.Zero, 
                out pp);
            MFError.ThrowExceptionForHR(hr);
            System.Threading.Thread.Sleep(5000);

            Debug.Assert(m_Done);
        }

        public int OnMediaPlayerEvent(MFP_EVENT_HEADER pEventHeader)
        {
            Debug.WriteLine(string.Format("{0} {1} {2}", pEventHeader.eEventType, pEventHeader.eState, pEventHeader.hrEvent));
            if (pp == pEventHeader.pMediaPlayer)
                m_Done = true;
            return S_Ok;
        }
    }
}
