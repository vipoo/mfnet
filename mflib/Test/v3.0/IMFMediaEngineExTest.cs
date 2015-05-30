using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MediaFoundation.Transform;
using System.Threading;
using MediaFoundation.EVR;

namespace Testv30
{
    class IMFMediaEngineExTest : COMBase, IMFMediaEngineNotify
    {
        IMFMediaEngineEx m_mee;
        int m_Messages = 0;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetConsoleTitle(StringBuilder sb, int capacity);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        public void DoTests()
        {
            GetInterface();

            TestGetSet();
            TestA();
            TestB();
            TestC();
        }

        private void TestGetSet()
        {
            int hr;
            bool b;

            ///////////////////

            hr = m_mee.SetBalance(0.234);
            MFError.ThrowExceptionForHR(hr);

            double d = m_mee.GetBalance();

            Debug.Assert(d == 0.234);

            //////

            ERole er;
            hr = m_mee.GetAudioEndpointRole(out er);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(er != ERole.eConsole);

            hr = m_mee.SetAudioEndpointRole(ERole.eConsole);
            MFError.ThrowExceptionForHR(hr);

            hr = m_mee.GetAudioEndpointRole(out er);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(er == ERole.eConsole);

            //////

            hr = m_mee.GetRealTimeMode(out b);
            MFError.ThrowExceptionForHR(hr);

            hr = m_mee.SetRealTimeMode(!b);
            MFError.ThrowExceptionForHR(hr);

            bool b2;
            hr = m_mee.GetRealTimeMode(out b2);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(b != b2);
        }

        void TestA()
        {
            int hr;
            bool b;

            ///////////

            AUDIO_STREAM_CATEGORY asc;
            hr = m_mee.GetAudioStreamCategory(out asc);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(asc != AUDIO_STREAM_CATEGORY.Alerts);

            hr = m_mee.SetAudioStreamCategory(AUDIO_STREAM_CATEGORY.Alerts);
            MFError.ThrowExceptionForHR(hr);

            hr = m_mee.GetAudioStreamCategory(out asc);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(asc == AUDIO_STREAM_CATEGORY.Alerts);

            //////////////////

            hr = m_mee.SetSource(Program.File1);
            MFError.ThrowExceptionForHR(hr);

            while (m_Messages < 8)
                ;

            int iOld = m_Messages;
            hr = m_mee.EnableTimeUpdateTimer(false);
            MFError.ThrowExceptionForHR(hr);

            hr = m_mee.Play();
            MFError.ThrowExceptionForHR(hr);

            while (m_mee.IsPaused())
                ;

            Thread.Sleep(1000);
            hr = m_mee.Pause();
            MFError.ThrowExceptionForHR(hr);

            while (m_Messages < iOld + 7)
                ;

            Thread.Sleep(100);

            // Should be no TimeUpdate messages (or maybe just 1)
            Debug.Assert(m_Messages == iOld + 7);

            ///////////////////

            PropVariant pv = new PropVariant();
            Thread.Sleep(200);

            hr = m_mee.GetStatistics(MF_MEDIA_ENGINE_STATISTIC.FramesRendered, pv);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(pv.GetUInt() > 0);

            ///////////////

            hr = m_mee.SetCurrentTimeEx(1.1, MF_MEDIA_ENGINE_SEEK_MODE.Normal);
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(200);
            double t = m_mee.GetCurrentTime();

            Debug.Assert(t == 1.1);

            ////////////////

            MFVideoNormalizedRect nr = new MFVideoNormalizedRect(); nr.bottom = 1; nr.right = 1;
            MFRect mr = new MFRect(); mr.right = 800; mr.bottom = 600;
            MFARGB rgb = new MFARGB(); rgb.rgbGreen = 255;

            hr = m_mee.UpdateVideoStream(null, null, rgb);
            MFError.ThrowExceptionForHR(hr);

            hr = m_mee.EnableHorizontalMirrorMode(true);
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(1000);

            hr = m_mee.UpdateVideoStream(nr, mr, null);

            ///////////

            pv.Clear();
            hr = m_mee.GetPresentationAttribute(MFAttributesClsid.MF_PD_DURATION, pv);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(pv.GetULong() != 0);

            //////////////
            pv.Clear();
            hr = m_mee.GetStreamAttribute(0, MFAttributesClsid.MF_SD_LANGUAGE, pv);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(pv.GetString() == "en-us");

            //////////////
            bool b4;
            hr = m_mee.IsProtected(out b4);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(!b4);

            b4 = m_mee.IsStereo3D();
            Debug.Assert(!b4);

            ///////////////

            bool b3 = m_mee.IsPlaybackRateSupported(2.0);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(b3);

            ////////////////////

            double dpos = m_mee.GetCurrentTime();
            hr = m_mee.FrameStep(true);
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(100);
            double dpos2 = m_mee.GetCurrentTime();
            Debug.Assert(dpos2 > dpos);

            //////////////////////

            MF_MEDIA_ENGINE_S3D_PACKING_MODE packMode;
            hr = m_mee.GetStereo3DFramePackingMode(out packMode);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(packMode != MF_MEDIA_ENGINE_S3D_PACKING_MODE.TopBottom);

            hr = m_mee.SetStereo3DFramePackingMode(MF_MEDIA_ENGINE_S3D_PACKING_MODE.TopBottom);
            MFError.ThrowExceptionForHR(hr);

            hr = m_mee.GetStereo3DFramePackingMode(out packMode);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(packMode == MF_MEDIA_ENGINE_S3D_PACKING_MODE.TopBottom);

            /////

            int iStreamCount;
            hr = m_mee.GetNumberOfStreams(out iStreamCount);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(iStreamCount == 2);

            hr = m_mee.GetStreamSelection(0, out b);
            MFError.ThrowExceptionForHR(hr);

            hr = m_mee.SetStreamSelection(0, !b);
            MFError.ThrowExceptionForHR(hr);

            hr = m_mee.ApplyStreamSelections();
            MFError.ThrowExceptionForHR(hr);

            bool b2;
            hr = m_mee.GetStreamSelection(0, out b2);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(b != b2);

            hr = m_mee.SetStreamSelection(0, !b2);
            MFError.ThrowExceptionForHR(hr);
        }
        void TestB()
        {
            //hr = m_mee.InsertVideoEffect(); tested in meplayer
            //hr = m_mee.InsertAudioEffect(); tested in meplayer
            //hr = m_mee.SetTimelineMarkerTimer(); tested in meplayer
            //hr = m_mee.SetSourceFromByteStream(); tested in meplayer
            //hr = m_mee.RemoveAllEffects(); tested in meplayer
            //hr = m_mee.GetResourceCharacteristics(); tested in meplayer
            //hr = m_mee.CancelTimelineMarkerTimer(); tested in meplayer
        }
        void TestC()
        {
            int hr;
            IntPtr p;

            // None of these work right.  But the defs look right.

            // These two require passing an IDCompositionVisual to 
            // MF_MEDIA_ENGINE_PLAYBACK_VISUAL instead 
            // of an HWND to MF_MEDIA_ENGINE_PLAYBACK_HWND.

            hr = m_mee.EnableWindowlessSwapchainMode(true);
            hr = m_mee.GetVideoSwapchainHandle(out p);

            /////////////

            // I tried this with something that (claims to be) a 3d video, 
            // but changing the type never changes.  Possible because I 
            // don't have a 3d monitor?
            MF3DVideoOutputType vt;
            hr = m_mee.GetStereo3DRenderMode(out vt);
            hr = m_mee.SetStereo3DRenderMode(MF3DVideoOutputType.Stereo);
            hr = m_mee.GetStereo3DRenderMode(out vt);

        }

        private void GetInterface()
        {
            IMFMediaEngineClassFactory mecf = new MFMediaEngineClassFactory() as IMFMediaEngineClassFactory;

            int hr;
            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 5);
            MFError.ThrowExceptionForHR(hr);

            hr = ia.SetUnknown(MFAttributesClsid.MF_MEDIA_ENGINE_CALLBACK, this);
            MFError.ThrowExceptionForHR(hr);

            IntPtr p;
            StringBuilder sb = new StringBuilder(500);

            GetConsoleTitle(sb, sb.Capacity);
            p = FindWindow(null, sb.ToString());

            hr = ia.SetUINT64(MFAttributesClsid.MF_MEDIA_ENGINE_PLAYBACK_HWND, p.ToInt64());
            MFError.ThrowExceptionForHR(hr);

            IMFMediaEngine me;
            hr = mecf.CreateInstance(MF_MEDIA_ENGINE_CREATEFLAGS.None, ia, out me);
            MFError.ThrowExceptionForHR(hr);

            m_mee = me as IMFMediaEngineEx;
        }

        public int EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            m_Messages++;
            Debug.WriteLine(eventid);

            return S_Ok;
        }
    }
}
