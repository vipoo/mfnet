using MediaFoundation;
using MediaFoundation.Misc;
using System;
using System.Diagnostics;
using System.Threading;

namespace Testv31
{
    class IMFTimedTextTrackListTest: COMBase, IMFMediaEngineNotify
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
            int tid;

            hr = m_tt.AddDataSourceFromUrl(Program.Timed1,
                "asdf", 
                "en-us", 
                MF_TIMED_TEXT_TRACK_KIND.Captions, 
                true, 
                out tid);
            MFError.ThrowExceptionForHR(hr);

            while (!m_Done)
                Thread.Sleep(1);

            IMFTimedTextTrackList tttl;
            int iLen;

            hr = m_tt.GetTracks(out tttl);
            MFError.ThrowExceptionForHR(hr);

            iLen = tttl.GetLength();
            Debug.Assert(iLen > 0);

            IMFTimedTextTrack ttt1;
            hr = tttl.GetTrack(0, out ttt1);
            MFError.ThrowExceptionForHR(hr);

            IMFTimedTextTrack ttt2;
            hr = tttl.GetTrackById(tid, out ttt2);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(ttt1 == ttt2);
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
