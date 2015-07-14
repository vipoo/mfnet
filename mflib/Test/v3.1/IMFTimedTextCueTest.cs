using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MediaFoundation;
using System.Diagnostics;
using MediaFoundation.Misc;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace Testv31
{
    // not tested: GetRegion, GetStyle
    class IMFTimedTextCueTest : COMBase, IMFMediaEngineNotify
    {
        IMFTimedText m_tt;
        IMFTimedTextCueList m_cuesM;
        IMFTimedTextCueList m_cuesS;
        int iDid = 0;

        public void Dotests()
        {
            GetInterface();
            GetInterface2();

            CueS();
#if true
            CueM(Program.Timed1);
#else
            foreach (string s in Directory.GetFiles(@"C:\Users\david\Documents\Visual Studio 2015\Projects\TimedText", "*.xml")) CueM(s);
            foreach (string s in Directory.GetFiles(@"C:\Users\david\Documents\Visual Studio 2015\Projects\TimedText\Animation", "*.xml")) CueM(s);
            foreach (string s in Directory.GetFiles(@"C:\Users\david\Documents\Visual Studio 2015\Projects\TimedText\Content", "*.xml")) CueM(s);
            foreach (string s in Directory.GetFiles(@"C:\Users\david\Documents\Visual Studio 2015\Projects\TimedText\Metadata", "*.xml")) CueM(s);
            foreach (string s in Directory.GetFiles(@"C:\Users\david\Documents\Visual Studio 2015\Projects\TimedText\Parameters", "*.xml")) CueM(s);
            foreach (string s in Directory.GetFiles(@"C:\Users\david\Documents\Visual Studio 2015\Projects\TimedText\Styling", "*.xml")) CueM(s);
            foreach (string s in Directory.GetFiles(@"C:\Users\david\Documents\Visual Studio 2015\Projects\TimedText\Timing", "*.xml")) CueM(s);
#endif
        }
        private void CueM(string sFileName)
        {
            HResult hr;

            System.IO.StreamReader myFile =
               new System.IO.StreamReader(sFileName);
            string myString = myFile.ReadToEnd();

            byte[] ba = Encoding.ASCII.GetBytes(myString);
            IMFTimedTextCue cueM;
            hr = m_cuesM.AddDataCue(0.1, 1.1, ba, ba.Length, out cueM);
            MFError.ThrowExceptionForHR(hr);

            IMFTimedTextBinary ttb;
            hr = cueM.GetData(out ttb);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(ttb != null);

            int bal;
            byte[] ba2 = null;
            IntPtr ip;
            hr = ttb.GetData(out ip, out bal);
            MFError.ThrowExceptionForHR(hr);

            ba2 = new byte[bal];
            Marshal.Copy(ip, ba2, 0, bal);

            for (int x = 0; x < bal; x++)
                Debug.Assert(ba[x] == ba2[x]);

            IMFTimedTextRegion ttr;
            hr = cueM.GetRegion(out ttr);
            MFError.ThrowExceptionForHR(hr);
            if (ttr != null)
                ttr = null;

            IMFTimedTextStyle tts;
            hr = cueM.GetStyle(out tts);
            MFError.ThrowExceptionForHR(hr);

            if (tts != null)
                tts = null;

            m_cuesM.RemoveCue(cueM);
            iDid++;
        }
        private void CueS()
        {
            HResult hr;
            string s;

            IMFTimedTextCue cueS;
            hr = m_cuesS.AddTextCue(0.1, 1.1, "asdf", out cueS);
            MFError.ThrowExceptionForHR(hr);

            int tic = cueS.GetId();
            Debug.Assert(tic == 1);

            hr = cueS.GetOriginalId(out s);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(s != null);

            MF_TIMED_TEXT_TRACK_KIND tttk = cueS.GetCueKind();
            Debug.Assert(tttk == MF_TIMED_TEXT_TRACK_KIND.Subtitles);

            double st = cueS.GetStartTime();
            Debug.Assert(st > 0 && st < 0.2);

            double dur = cueS.GetDuration();
            Debug.Assert(dur > 1 && dur < 1.2);

            int tid = cueS.GetTrackId();

            Debug.Assert(tid > 0);

            int iCount = cueS.GetLineCount();
            Debug.Assert(iCount > 0);

            IMFTimedTextFormattedText ttft;
            hr = cueS.GetLine(0, out ttft);
            MFError.ThrowExceptionForHR(hr);

            TestFormat(ttft);

            Debug.Assert(ttft != null);
        }
        private void TestFormat(IMFTimedTextFormattedText ttft)
        {
            HResult hr;
            string s;

            hr = ttft.GetText(out s);

            int c = ttft.GetSubformattingCount();

            for (int x = 0; x < c; x++)
            {
                int a;
                int l;
                IMFTimedTextStyle st;
                hr = ttft.GetSubformatting(x, out a, out l, out st);
            }
        }
        private void GetInterface2()
        {
            HResult hr;

            int tid;
            hr = m_tt.AddDataSourceFromUrl(Program.Timed1,
                "asdf",
                "en-us",
                MF_TIMED_TEXT_TRACK_KIND.Captions,
                true,
                out tid);
            MFError.ThrowExceptionForHR(hr);

            hr = m_tt.AddDataSourceFromUrl(Program.Timed1,
                "asdf",
                "en-us",
                 MF_TIMED_TEXT_TRACK_KIND.Captions,
                true,
                out tid);
            MFError.ThrowExceptionForHR(hr);
            Thread.Sleep(1000);

            IMFTimedTextTrack tttS;
            IMFTimedTextTrack tttM;
            hr = m_tt.AddTrack("jjj", "en-us", MF_TIMED_TEXT_TRACK_KIND.Subtitles, out tttS);
            MFError.ThrowExceptionForHR(hr);

            hr = m_tt.AddTrack("jjj", "en-us", MF_TIMED_TEXT_TRACK_KIND.Metadata, out tttM);
            MFError.ThrowExceptionForHR(hr);

            tttS.GetCueList(out m_cuesS);
            tttM.GetCueList(out m_cuesM);
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
