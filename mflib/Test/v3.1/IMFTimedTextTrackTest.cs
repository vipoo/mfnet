using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MediaFoundation;
using System.Diagnostics;
using MediaFoundation.Misc;
using System.Threading;

namespace Testv31
{
    class IMFTimedTextTrackTest : COMBase, IMFMediaEngineNotify
    {
        IMFTimedText m_tt;
        IMFTimedTextTrack m_ttt;
        public void Dotests()
        {
            HResult hr;

            GetInterface();

            string sLabel = "asdf";
            MF_TIMED_TEXT_TRACK_KIND tttk = MF_TIMED_TEXT_TRACK_KIND.Captions;
            int tid;

            hr = m_tt.SetInBandEnabled(true);
            MFError.ThrowExceptionForHR(hr);

            hr = m_tt.AddDataSourceFromUrl(Program.Timed1,
                sLabel,
                "en-us",
                tttk,
                true,
                out tid);
            MFError.ThrowExceptionForHR(hr);
            Thread.Sleep(1000);

            IMFTimedTextTrackList tttl;
            hr = m_tt.GetTracks(out tttl);
            MFError.ThrowExceptionForHR(hr);

            hr = tttl.GetTrackById(tid, out m_ttt);
            MFError.ThrowExceptionForHR(hr);

            int tid2 = m_ttt.GetId();
            Debug.Assert(tid == tid2);

            string s;
            hr = m_ttt.GetLabel(out s);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(sLabel == s);

            string sLabel2 = "fdsa";
            hr = m_ttt.SetLabel(sLabel2);
            MFError.ThrowExceptionForHR(hr);

            hr = m_ttt.GetLabel(out s);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(sLabel2 == s);

            hr = m_ttt.GetLanguage(out s);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(s == "en-us");

            MF_TIMED_TEXT_TRACK_KIND tk = m_ttt.GetTrackKind();
            Debug.Assert(tttk == tk);

            bool b = m_ttt.IsInBand();

            s = null;

            hr = m_ttt.GetInBandMetadataTrackDispatchType(out s);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(s != null);

            bool b2 = m_ttt.IsActive();

            MF_TIMED_TEXT_ERROR_CODE ttec = m_ttt.GetErrorCode();

            HResult geec = m_ttt.GetExtendedErrorCode();

            Guid g1 = Guid.NewGuid();
            Guid g2 = g1;
            hr = m_ttt.GetDataFormat(out g1);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(g1 != g2 && g1 == Guid.Empty);

            MF_TIMED_TEXT_TRACK_READY_STATE tttrs = m_ttt.GetReadyState();
            Debug.Assert(tttrs == MF_TIMED_TEXT_TRACK_READY_STATE.Loaded);

            IMFTimedTextCueList cues;
            hr = m_ttt.GetCueList(out cues);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(cues != null);
        }

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

            IMFMediaEngine me;
            hr = mecf.CreateInstance(MF_MEDIA_ENGINE_CREATEFLAGS.AudioOnly, ia, out me);
            MFError.ThrowExceptionForHR(hr);

            hr = me.SetSource(Program.File1);

            IMFGetService gs = me as IMFGetService;
            object o;
            hr = gs.GetService(MFAttributesClsid.MF_MEDIA_ENGINE_TIMEDTEXT, typeof(IMFTimedText).GUID, out o);
            m_tt = o as IMFTimedText;
        }
        public HResult EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            Debug.WriteLine(eventid);
            return 0;
        }
    }
}
