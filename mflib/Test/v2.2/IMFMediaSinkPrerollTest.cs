using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv22
{
    class IMFMediaSinkPrerollTest
    {
        IMFMediaSinkPreroll m_msp;

        public void DoTests()
        {
            int hr;

            GetInterface();

            hr = m_msp.NotifyPreroll(100);
            MFError.ThrowExceptionForHR(hr);
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

            m_msp = (IMFMediaSinkPreroll)pAudioSink;
        }
    }
}
