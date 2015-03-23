using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Testv22
{
    class IMFAudioStreamVolumeTest
    {
        IMFAudioStreamVolume m_asv;

        public void DoTests()
        {
            int hr;

            GetInterface();

            float f;
            hr = m_asv.SetChannelVolume(0, 0.5f);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asv.GetChannelVolume(0, out f);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(f == 0.5);

            int i;
            hr = m_asv.GetChannelCount(out i);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(i == 2);

            float[] fa1 = new float[i];
            float[] fa2 = new float[i];

            float f3 = 0;
            for (int x = 0; x < i; x++)
            {
                fa1[x] = f3;
                f3 += 0.5f;
            }

            hr = m_asv.SetAllVolumes(i, fa1);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asv.GetAllVolumes(i, fa2);
            MFError.ThrowExceptionForHR(hr);

            for (int x = 0; x < i; x++)
            {
                Debug.Assert(fa1[x] == fa2[x]);
            }
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
            hr = gs.GetService(MFServices.MR_STREAM_VOLUME_SERVICE, typeof(IMFAudioStreamVolume).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            m_asv = (IMFAudioStreamVolume)o;
        }
    }
}
