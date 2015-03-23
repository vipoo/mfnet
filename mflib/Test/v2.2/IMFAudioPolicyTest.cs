using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.ReadWrite;

namespace Testv22
{
    class IMFAudioPolicyTest
    {
        IMFAudioPolicy m_ap;

        public void DoTests()
        {
            int hr;
            string s;

            GetInterface();

            hr = m_ap.SetDisplayName("asdf");
            MFError.ThrowExceptionForHR(hr);

            hr = m_ap.GetDisplayName(out s);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(s == "asdf");

            hr = m_ap.SetIconPath(@"%windir%\system32\shell32.dll,4");
            MFError.ThrowExceptionForHR(hr);

            hr = m_ap.GetIconPath(out s);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(s == @"%windir%\system32\shell32.dll,4");

            Guid g = new Guid();
            hr = m_ap.SetGroupingParam(g);
            MFError.ThrowExceptionForHR(hr);

            Guid g2;
            hr = m_ap.GetGroupingParam(out g2);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(g == g2);
        }

        private void GetInterface()
        {
            int hr;

            IMFMediaSink pAudioSink;
            hr = MFExtern.MFCreateAudioRenderer(null, out pAudioSink);
            MFError.ThrowExceptionForHR(hr);

            IMFStreamSink pStreamSink;
            hr = pAudioSink.GetStreamSinkByIndex(0, out pStreamSink);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaTypeHandler pMediaTypeHandler;
            hr = pStreamSink.GetMediaTypeHandler(out pMediaTypeHandler);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaType pSinkMediaType;
            hr = pMediaTypeHandler.GetMediaTypeByIndex(2, out pSinkMediaType);
            MFError.ThrowExceptionForHR(hr);

            hr = pMediaTypeHandler.SetCurrentMediaType(pSinkMediaType);
            MFError.ThrowExceptionForHR(hr);

            IMFGetService gs = (IMFGetService)pAudioSink;

            object o;
            hr = gs.GetService(MFServices.MR_AUDIO_POLICY_SERVICE, typeof(IMFAudioPolicy).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            m_ap = (IMFAudioPolicy)o;
        }
    }
}
