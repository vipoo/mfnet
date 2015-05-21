using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Threading;

namespace Testv30
{
    class IMFCaptureEngineTest : COMBase, IMFCaptureEngineOnEventCallback
    {
        IMFCaptureEngine m_ce;
        int m_Status = 0;

        public void DoTests()
        {
            int hr;

            GetInterface();

            hr = m_ce.Initialize(this, null, null, null);
            MFError.ThrowExceptionForHR(hr);

            IMFCaptureSource cs;
            hr = m_ce.GetSource(out cs);
            MFError.ThrowExceptionForHR(hr);

            Program.IsA(cs, typeof(IMFCaptureSource).GUID);

            hr = m_ce.StartPreview();
            MFError.ThrowExceptionForHR(hr);

            hr = m_ce.StopPreview();
            MFError.ThrowExceptionForHR(hr);

            hr = m_ce.StartRecord();
            MFError.ThrowExceptionForHR(hr);

            hr = m_ce.StopRecord(true, true);
            MFError.ThrowExceptionForHR(hr);

            hr = m_ce.TakePhoto();
            MFError.ThrowExceptionForHR(hr);

            IMFCaptureSink csink;
            hr = m_ce.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.Photo, out csink);
            MFError.ThrowExceptionForHR(hr);

            Program.IsA(csink, typeof(IMFCaptureSink).GUID);

            while (Thread.VolatileRead(ref m_Status) != 63)
                ;

            Debug.WriteLine("Done!");
        }

        private void GetInterface()
        {
            IMFCaptureEngineClassFactory cecf = new MFCaptureEngineClassFactory() as IMFCaptureEngineClassFactory;
            Debug.Assert(cecf != null);

            object o;
            int hr = cecf.CreateInstance(CLSID.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(o != null);
            Program.IsA(o, typeof(IMFCaptureEngine).GUID);

            m_ce = o as IMFCaptureEngine;
        }

        public int OnEvent(IMFMediaEvent pEvent)
        {
            int hr;

            MediaEventType met;
            hr = pEvent.GetType(out met);
            MFError.ThrowExceptionForHR(hr);

            //Debug.WriteLine(met);

            if (met == MediaEventType.MEExtendedType)
            {
                Guid g;
                hr = pEvent.GetExtendedType(out g);
                MFError.ThrowExceptionForHR(hr);

                if (g == MF_CAPTURE_ENGINE.INITIALIZED)
                    m_Status |= 1;
                else if (g == MF_CAPTURE_ENGINE.PREVIEW_STARTED)
                    m_Status |= 2;
                else if (g == MF_CAPTURE_ENGINE.PREVIEW_STOPPED)
                    m_Status |= 4;
                else if (g == MF_CAPTURE_ENGINE.RECORD_STARTED)
                    m_Status |= 8;
                else if (g == MF_CAPTURE_ENGINE.RECORD_STOPPED)
                    m_Status |= 16;
                else if (g == MF_CAPTURE_ENGINE.PHOTO_TAKEN)
                    m_Status |= 32;

                //Debug.WriteLine(MF_CAPTURE_ENGINE.LookupName(g));
            }

            return hr;
        }
    }
}
