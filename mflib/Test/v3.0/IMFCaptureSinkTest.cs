using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Threading;
using MediaFoundation.ReadWrite;

namespace Testv30
{
    class IMFCaptureSinkTest : COMBase, IMFCaptureEngineOnEventCallback
    {
        IMFCaptureSink m_csink;
        int m_Init = 0;
        IMFMediaType m_mt;

        public void DoTests()
        {
            int hr;

            GetInterface();

            IMFCaptureRecordSink rs = m_csink as IMFCaptureRecordSink;
            hr = rs.SetOutputFileName("output.wmv");
            MFError.ThrowExceptionForHR(hr);

            int i;
            hr = m_csink.AddStream(0, m_mt, null, out i);
            MFError.ThrowExceptionForHR(hr);

            hr = m_csink.Prepare();
            MFError.ThrowExceptionForHR(hr);

            while (Thread.VolatileRead(ref m_Init) == 1)
                ;

            IMFMediaType mt;
            hr = m_csink.GetOutputMediaType(i, out mt);
            MFError.ThrowExceptionForHR(hr);

            MFMediaEqual mf;
            hr = mt.IsEqual(m_mt, out mf);
            Debug.Assert(hr == S_Ok);

            object o;
            hr = m_csink.GetService(0, Guid.Empty, typeof(IMFSinkWriter).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            Program.IsA(o, typeof(IMFSinkWriter).GUID);

            hr = m_csink.RemoveAllStreams();
            MFError.ThrowExceptionForHR(hr);

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

            IMFCaptureEngine ce = o as IMFCaptureEngine;

            hr = ce.Initialize(this, null, null, null);
            MFError.ThrowExceptionForHR(hr);

            while (Thread.VolatileRead(ref m_Init) == 0)
                ;

            hr = ce.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.Record, out m_csink);
            MFError.ThrowExceptionForHR(hr);

            IMFCaptureSource source;
            hr = ce.GetSource(out source);
            MFError.ThrowExceptionForHR(hr);

            int j = 0;

            hr = source.GetAvailableDeviceMediaType(j, 0, out m_mt);
            MFError.ThrowExceptionForHR(hr);
        }

        public int OnEvent(IMFMediaEvent pEvent)
        {
            int hr;

            MediaEventType met;
            hr = pEvent.GetType(out met);
            MFError.ThrowExceptionForHR(hr);

            Debug.WriteLine(met);

            if (met == MediaEventType.MEExtendedType)
            {
                Guid g;
                hr = pEvent.GetExtendedType(out g);
                MFError.ThrowExceptionForHR(hr);

                if (g == MF_CAPTURE_ENGINE.INITIALIZED)
                    m_Init = 1;

                if (g == MF_CAPTURE_ENGINE.SINK_PREPARED)
                    m_Init = 2;

                Debug.WriteLine(MF_CAPTURE_ENGINE.LookupName(g));
            }

            return hr;
        }
    }
}
