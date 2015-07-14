using MediaFoundation;
using MediaFoundation.Misc;
using System;
using System.Diagnostics;
using System.Threading;

namespace Testv31
{
    class IMFTimedTextCueListTest : COMBase, IMFMediaEngineNotify
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
            TestS();
            TestM();
        }

        private void TestS()
        {
            HResult hr;

            IMFTimedTextTrack ttt;

            hr = m_tt.AddTrack("jjj", "en-us", MF_TIMED_TEXT_TRACK_KIND.Subtitles, out ttt);
            MFError.ThrowExceptionForHR(hr);

            IMFTimedTextCueList cuel;
            hr = ttt.GetCueList(out cuel);
            MFError.ThrowExceptionForHR(hr);

            int iLen;
            iLen = cuel.GetLength();

            IMFTimedTextCue cue1;
            hr = cuel.AddTextCue(0.0, 2.2, "cucuc", out cue1);
            MFError.ThrowExceptionForHR(hr);

            iLen = cuel.GetLength();

            Debug.Assert(iLen == 1);

            IMFTimedTextCue cue2;
            hr = cuel.GetCueByIndex(0, out cue2);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(cue1 == cue2);

            hr = cuel.GetCueById(1, out cue2);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(cue1 == cue2);

            string s;
            hr = cue1.GetOriginalId(out s);
            MFError.ThrowExceptionForHR(hr);

            hr = cuel.GetCueByOriginalId(s, out cue2);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(cue1 == cue2);

            hr = cuel.RemoveCue(cue1);
            MFError.ThrowExceptionForHR(hr);
        }
        private void TestM()
        {
            HResult hr;

            IMFTimedTextTrack ttt;

            hr = m_tt.AddTrack("jjj", "en-us", MF_TIMED_TEXT_TRACK_KIND.Metadata, out ttt);
            MFError.ThrowExceptionForHR(hr);

            IMFTimedTextCueList cuel;
            hr = ttt.GetCueList(out cuel);
            MFError.ThrowExceptionForHR(hr);

            int iLen;
            iLen = cuel.GetLength();

            IMFTimedTextCue cue1;
            hr = cuel.AddDataCue(2.3, 1.0, new byte[3], 3, out cue1);
            MFError.ThrowExceptionForHR(hr);
        }
        public HResult EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            if (eventid == MF_MEDIA_ENGINE_EVENT.LoadedData)
            {
                m_Done = true;
            }
            Debug.WriteLine(eventid);
            return 0;
        }

    }
}
