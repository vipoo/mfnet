using MediaFoundation;
using MediaFoundation.Misc;
using System;
using System.Diagnostics;
using System.Threading;

namespace Testv31
{
    // Untested: Error
    class IMFTimedTextNotifyTest : COMBase, IMFMediaEngineNotify, IMFTimedTextNotify
    {
        IMFTimedText m_tt;
        IMFMediaEngine m_me;
        int m_bits = 0;
        bool m_Done = false;
        bool m_Loaded = false;

        private void GetInterface()
        {
            IMFMediaEngineClassFactory mecf = new MFMediaEngineClassFactory() as IMFMediaEngineClassFactory;
            Debug.Assert(mecf != null);

            HResult hr;
            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 5);
            MFError.ThrowExceptionForHR(hr);

            hr = ia.SetUnknown(MFAttributesClsid.MF_MEDIA_ENGINE_CALLBACK, this);
            MFError.ThrowExceptionForHR(hr);

            hr = mecf.CreateInstance(MF_MEDIA_ENGINE_CREATEFLAGS.AudioOnly, ia, out m_me);
            MFError.ThrowExceptionForHR(hr);

            hr = m_me.SetSource(Program.File1);

            IMFGetService gs = m_me as IMFGetService;
            object o;
            hr = gs.GetService(MFAttributesClsid.MF_MEDIA_ENGINE_TIMEDTEXT, typeof(IMFTimedText).GUID, out o);
            m_tt = o as IMFTimedText;
        }

        public void Dotests()
        {
            GetInterface();

            while (!m_Loaded)
                Thread.Sleep(1);

            HResult hr;
            hr = m_tt.RegisterNotifications(this);

            int tid;
            hr = m_tt.AddDataSourceFromUrl(
                Program.Timed2,
                "asdf",
                "en-us",
                MF_TIMED_TEXT_TRACK_KIND.Captions,
                true,
                out tid);
            MFError.ThrowExceptionForHR(hr);

            IMFTimedTextTrack ttt;
            hr = m_tt.AddTrack("jjj", "en-us", MF_TIMED_TEXT_TRACK_KIND.Subtitles, out ttt);
            MFError.ThrowExceptionForHR(hr);

            int tid2 = ttt.GetId();

            hr = m_tt.SelectTrack(tid2, true);
            MFError.ThrowExceptionForHR(hr);

            IMFTimedTextCueList cuel;
            hr = ttt.GetCueList(out cuel);
            MFError.ThrowExceptionForHR(hr);

            IMFTimedTextCue cue;
            hr = cuel.AddTextCue(0.0, 2.2, "cucuc", out cue);
            MFError.ThrowExceptionForHR(hr);

            hr = m_tt.RemoveTrack(ttt);
            MFError.ThrowExceptionForHR(hr);

            hr = m_me.Shutdown();
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(500);

            Debug.Assert(m_bits == 127);
        }
        public HResult EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            if (eventid == MF_MEDIA_ENGINE_EVENT.Ended)
            {
                m_Done = true;
            }
            if (eventid == MF_MEDIA_ENGINE_EVENT.LoadedData)
            {
                m_Loaded = true;
            }
            Debug.WriteLine(eventid);
            return 0;
        }

        void IMFTimedTextNotify.Cue(MF_TIMED_TEXT_CUE_EVENT cueEvent, double currentTime, IMFTimedTextCue cue)
        {
            m_bits |= 1;
        }

        void IMFTimedTextNotify.Error(MF_TIMED_TEXT_ERROR_CODE errorCode, HResult extendedErrorCode, int sourceTrackId)
        {
            m_bits |= 2;
        }

        void IMFTimedTextNotify.Reset()
        {
            m_bits |= 4;
        }

        void IMFTimedTextNotify.TrackAdded(int trackId)
        {
            m_bits |= 8;
        }

        void IMFTimedTextNotify.TrackReadyStateChanged(int trackId)
        {
            m_bits |= 16;
        }

        void IMFTimedTextNotify.TrackRemoved(int trackId)
        {
            m_bits |= 32;
        }

        void IMFTimedTextNotify.TrackSelected(int trackId, bool selected)
        {
            m_bits |= 64;
        }
    }
}
