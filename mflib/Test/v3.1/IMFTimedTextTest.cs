using MediaFoundation;
using MediaFoundation.Misc;
using System;
using System.Diagnostics;
using System.Threading;

namespace Testv31
{
    class IMFTimedTextTest : COMBase, IMFMediaEngineNotify, IMFTimedTextNotify
    {
        IMFTimedText m_tt;
        IMFMediaEngine m_me;
        bool m_Done = false;

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

            HResult hr;
            hr = m_tt.RegisterNotifications(this);
            MFError.ThrowExceptionForHR(hr);

            int tid;
            hr = m_tt.AddDataSourceFromUrl(Program.Timed1,
                "asdf", 
                "en-us", 
                MF_TIMED_TEXT_TRACK_KIND.Captions, 
                true, 
                out tid);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(tid != 0);

            IMFByteStream bs;
            hr = MFExtern.MFCreateFile(MFFileAccessMode.Read, MFFileOpenMode.FailIfNotExist, MFFileFlags.None, Program.Timed1, out bs);
            MFError.ThrowExceptionForHR(hr);

            int tid2;
            hr = m_tt.AddDataSource(bs, "fdsa", "en-us", MF_TIMED_TEXT_TRACK_KIND.Captions, false, out tid2);
            MFError.ThrowExceptionForHR(hr);

            hr = m_tt.SelectTrack(tid2, true);
            MFError.ThrowExceptionForHR(hr);

            IMFTimedTextTrack ttt;
            hr = m_tt.AddTrack("jjj", "en-us", MF_TIMED_TEXT_TRACK_KIND.Subtitles, out ttt);
            MFError.ThrowExceptionForHR(hr);

            bool b;
            hr = m_tt.SetInBandEnabled(false);
            MFError.ThrowExceptionForHR(hr);

            b = m_tt.IsInBandEnabled();
            Debug.Assert(!b);

            hr = m_tt.SetInBandEnabled(true);
            MFError.ThrowExceptionForHR(hr);

            b = m_tt.IsInBandEnabled();
            Debug.Assert(b);

            hr = m_tt.SetCueTimeOffset(1.0);
            MFError.ThrowExceptionForHR(hr);

            double d;
            hr = m_tt.GetCueTimeOffset(out d);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(d > .99 && d < 1.1);

            IMFTimedTextTrackList tttl;
            int iLen;

            hr = m_tt.GetActiveTracks(out tttl);
            MFError.ThrowExceptionForHR(hr);
            iLen = tttl.GetLength();

            hr = m_tt.GetTextTracks(out tttl);
            MFError.ThrowExceptionForHR(hr);
            iLen = tttl.GetLength();

            hr = m_tt.GetMetadataTracks(out tttl);
            MFError.ThrowExceptionForHR(hr);
            iLen = tttl.GetLength();

            hr = m_tt.GetTracks(out tttl);
            MFError.ThrowExceptionForHR(hr);
            iLen = tttl.GetLength();

            hr = m_me.Play();
            MFError.ThrowExceptionForHR(hr);

            while (!m_Done)
                Thread.Sleep(1);

            for (int x=0; x < iLen; x++)
            {
                hr = tttl.GetTrack(x, out ttt);
                if (ttt != null)
                {
                    hr = m_tt.RemoveTrack(ttt);
                    MFError.ThrowExceptionForHR(hr);
                }
            }
        }
        public HResult EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            if (eventid == MF_MEDIA_ENGINE_EVENT.Ended)
            {
                m_Done = true;
            }
            Debug.WriteLine(eventid);
            return 0;
        }

        void IMFTimedTextNotify.Cue(MF_TIMED_TEXT_CUE_EVENT cueEvent, double currentTime, IMFTimedTextCue cue)
        {
            throw new NotImplementedException();
        }

        void IMFTimedTextNotify.Error(MF_TIMED_TEXT_ERROR_CODE errorCode, HResult extendedErrorCode, int sourceTrackId)
        {
            throw new NotImplementedException();
        }

        void IMFTimedTextNotify.Reset()
        {
        }

        void IMFTimedTextNotify.TrackAdded(int trackId)
        {
        }

        void IMFTimedTextNotify.TrackReadyStateChanged(int trackId)
        {
        }

        void IMFTimedTextNotify.TrackRemoved(int trackId)
        {
        }

        void IMFTimedTextNotify.TrackSelected(int trackId, bool selected)
        {
        }
    }
}
