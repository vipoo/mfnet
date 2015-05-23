using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.Transform;
using System.Threading;
using MediaFoundation.ReadWrite;

namespace Testv30
{
    class IMFCaptureSourceTest : COMBase, IMFCaptureEngineOnEventCallback
    {
        IMFCaptureEngine m_pEngine;
        IMFCaptureSource m_Source;

        public void DoTests()
        {
            GetInterface();

            int hr = S_Ok;

            //////////////////////

            IMFMediaSource ms;
            hr = m_Source.GetCaptureDeviceSource(MF_CAPTURE_ENGINE_DEVICE_TYPE.Video, out ms);
            MFError.ThrowExceptionForHR(hr);

            //////////////////////

            IMFActivate ia;
            hr = m_Source.GetCaptureDeviceActivate(MF_CAPTURE_ENGINE_DEVICE_TYPE.Video, out ia);
            MFError.ThrowExceptionForHR(hr);

            IMFTransform t = new CColorConvertDMO() as IMFTransform;

            hr = m_Source.AddEffect(0, t);
            MFError.ThrowExceptionForHR(hr);

            // Wait for effect to get added.
            Thread.Sleep(2000);

            hr = m_Source.RemoveEffect(0, t);
            MFError.ThrowExceptionForHR(hr);

            hr = m_Source.RemoveAllEffects(0);
            MFError.ThrowExceptionForHR(hr);

            //////////////////////

            IMFMediaType mt;
            hr = m_Source.GetAvailableDeviceMediaType(0, 0, out mt);
            MFError.ThrowExceptionForHR(hr);

            Debug.WriteLine(MFDump.DumpAttribs(mt));

            hr = m_Source.SetCurrentDeviceMediaType(0, mt);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaType mt2;
            hr = m_Source.GetCurrentDeviceMediaType(0, out mt2);
            MFError.ThrowExceptionForHR(hr);

            MFMediaEqual me;
            hr = mt.IsEqual(mt2, out me);
            Debug.Assert(hr == 0);

            //////////////////////

            int iCount;
            hr = m_Source.GetDeviceStreamCount(out iCount);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(iCount > 0);

            for (int x = 0; x < iCount; x++)
            {
                MF_CAPTURE_ENGINE_STREAM_CATEGORY sc;
                hr = m_Source.GetDeviceStreamCategory(x, out sc);
                MFError.ThrowExceptionForHR(hr);

                Debug.WriteLine(sc);
            }

            //////////////////////


            //const int MF_SOURCE_READER_FIRST_AUDIO_STREAM = unchecked((int)0xFFFFFFFD);
            //const int MF_SOURCE_READER_FIRST_VIDEO_STREAM = unchecked((int)0xFFFFFFFC);

            const int MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW = unchecked((int)0xFFFFFFFA);
            const int MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_RECORD = unchecked((int)0xFFFFFFF9);
            const int MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_PHOTO = unchecked((int)0xFFFFFFF8);
            const int MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_AUDIO = unchecked((int)0xFFFFFFF7);

            int ind;
            //hr = m_Source.GetStreamIndexFromFriendlyName(MF_SOURCE_READER_FIRST_AUDIO_STREAM, out ind);
            MFError.ThrowExceptionForHR(hr);

            //hr = m_Source.GetStreamIndexFromFriendlyName(MF_SOURCE_READER_FIRST_VIDEO_STREAM, out ind);
            MFError.ThrowExceptionForHR(hr);

            //hr = m_Source.GetStreamIndexFromFriendlyName(0, out ind);
            MFError.ThrowExceptionForHR(hr);

            hr = m_Source.GetStreamIndexFromFriendlyName(MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW, out ind);
            MFError.ThrowExceptionForHR(hr);

            hr = m_Source.GetStreamIndexFromFriendlyName(MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_VIDEO_RECORD, out ind);
            MFError.ThrowExceptionForHR(hr);

            hr = m_Source.GetStreamIndexFromFriendlyName(MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_PHOTO, out ind);
            MFError.ThrowExceptionForHR(hr);

            int aud;
            hr = m_Source.GetStreamIndexFromFriendlyName(MF_CAPTURE_ENGINE_PREFERRED_SOURCE_STREAM_FOR_AUDIO, out aud);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(aud != ind);

            //////////////////////////

            object o;
            hr = m_Source.GetService(Guid.Empty, typeof(IMFSourceReader).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            Program.IsA(o, typeof(IMFSourceReader).GUID);

            bool b, b2;
            hr = m_Source.GetMirrorState(0, out b);

            // Apparently my device doesn't support mirroring.
            if (hr != E_NotImplemented)
            {
                MFError.ThrowExceptionForHR(hr);

                hr = m_Source.SetMirrorState(0, !b);
                MFError.ThrowExceptionForHR(hr);

                hr = m_Source.GetMirrorState(0, out b2);
                MFError.ThrowExceptionForHR(hr);

                Debug.Assert(b != b2);
            }
        }
        private void GetInterface()
        {
            IMFCaptureEngineClassFactory cecf = new MFCaptureEngineClassFactory() as IMFCaptureEngineClassFactory;
            Debug.Assert(cecf != null);

            object o;
            int hr = cecf.CreateInstance(CLSID.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            m_pEngine = o as IMFCaptureEngine;

            int c;
            IMFActivate[] ia;
            IMFAttributes it;
            hr = MFExtern.MFCreateAttributes(out it, 3);
            hr = it.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_VIDCAP_GUID);
            MFError.ThrowExceptionForHR(hr);

            string szFriendlyName;
            int cchName;

            hr = MFExtern.MFEnumDeviceSources(it, out ia, out c);
            MFError.ThrowExceptionForHR(hr);

            hr = ia[0].GetAllocatedString(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME, out szFriendlyName, out cchName);
            MFError.ThrowExceptionForHR(hr);

            Debug.WriteLine(szFriendlyName);

            hr = m_pEngine.Initialize(this, null, null, ia[0]);
            MFError.ThrowExceptionForHR(hr);

            //////////////////////////////////////

            // Get a pointer to the photo sink.
            IMFCaptureSink pSink;
            hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.Photo, out pSink);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pEngine.GetSource(out m_Source);
            MFError.ThrowExceptionForHR(hr);
        }
        public int OnEvent(IMFMediaEvent pEvent)
        {
            int hr;

            MediaEventType met;
            hr = pEvent.GetType(out met);
            MFError.ThrowExceptionForHR(hr);

            Debug.WriteLine(met);

            return 0;
        }

    }
}
