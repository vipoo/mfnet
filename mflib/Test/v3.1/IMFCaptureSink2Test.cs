using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using MediaFoundation.Transform;
using MediaFoundation.OPM;

namespace Testv31
{
    class IMFCaptureSink2Test : COMBase, IMFCaptureEngineOnSampleCallback2, IMFCaptureEngineOnEventCallback
    {
        public static readonly Guid gOutputType = new Guid("{C141A119-DB9E-4FC4-9AD9-FEC2C65F6C68}");
        IMFCaptureSink2 m_csink2;

        IMFCaptureEngine m_ce;
        int m_videostream;
        int m_Init = 0;
        IMFMediaType m_mt;
        int m_Count = 0;

        public void DoTests()
        {
            int hr = 0;
            GetInterface();

            int i;
            hr = m_csink2.AddStream(m_videostream, m_mt, null, out i);
            MFError.ThrowExceptionForHR(hr);

            IMFCapturePreviewSink cps = m_csink2 as IMFCapturePreviewSink;
            if (cps != null)
            {
                hr = cps.SetSampleCallback(m_videostream, this);
                MFError.ThrowExceptionForHR(hr);
            }

            IMFCaptureRecordSink crs = m_csink2 as IMFCaptureRecordSink;
            if (crs != null)
            {
                hr = crs.SetSampleCallback(i, this);
                MFError.ThrowExceptionForHR(hr);
            }

            hr = m_mt.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, gOutputType);
            MFError.ThrowExceptionForHR(hr);

            hr = m_csink2.SetOutputMediaType(i, m_mt, null);
            MFError.ThrowExceptionForHR(hr);

            while (Thread.VolatileRead(ref m_Init) != 3)
                ;
        }
        private void GetInterface()
        {
            IMFCaptureEngineClassFactory cecf = new MFCaptureEngineClassFactory() as IMFCaptureEngineClassFactory;
            Debug.Assert(cecf != null);

            object o;
            int hr = cecf.CreateInstance(CLSID.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            m_ce = o as IMFCaptureEngine;

            hr = m_ce.Initialize(this, null, null, null);
            MFError.ThrowExceptionForHR(hr);

            while (Thread.VolatileRead(ref m_Init) == 0)
                ;

            IMFCaptureSource source;
            hr = m_ce.GetSource(out source);
            MFError.ThrowExceptionForHR(hr);

            int c;
            hr = source.GetDeviceStreamCount(out c);
            MFError.ThrowExceptionForHR(hr);

            //m_videostream = 0;
            //const int FIRST_SOURCE_VIDEO_STREAM = -4; broken
            const int PREFERRED_SOURCE_STREAM_FOR_AUDIO = -9;
            //const int PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW = -6;
            hr = source.GetStreamIndexFromFriendlyName(PREFERRED_SOURCE_STREAM_FOR_AUDIO, out m_videostream);
            MFError.ThrowExceptionForHR(hr);

            hr = source.GetAvailableDeviceMediaType(m_videostream, 0, out m_mt);
            MFError.ThrowExceptionForHR(hr);

            Debug.WriteLine(MFDump.DumpAttribs(m_mt));

            IMFCaptureSink m_csink;
            hr = m_ce.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.Record, out m_csink);
            MFError.ThrowExceptionForHR(hr);

            m_csink2 = m_csink as IMFCaptureSink2;
        }

        public int OnEvent(IMFMediaEvent pEvent)
        {
            int hr;

            MediaEventType met = 0;
            hr = pEvent.GetType(out met);
            MFError.ThrowExceptionForHR(hr);

            int hr2;
            hr = pEvent.GetStatus(out hr2);

            switch (met)
            {
                case MediaEventType.MEExtendedType:
                    {
                        Guid g;
                        hr = pEvent.GetExtendedType(out g);
                        MFError.ThrowExceptionForHR(hr);

                        if (g == MF_CAPTURE_ENGINE.INITIALIZED)
                            m_Init = 1;

                        else if (g == MF_CAPTURE_ENGINE.SINK_PREPARED)
                            m_Init = 2;

                        else if (g == MF_CAPTURE_ENGINE.RECORD_STOPPED)
                            ;
                        //m_Done = 1;

                        else if (g == MF_CAPTURE_ENGINE.ERROR)
                        {
                        }

                        else if (g == MF_CAPTURE_ENGINE.OUTPUT_MEDIA_TYPE_SET)
                        {
                            m_Init = 3;
                        }
                        Debug.WriteLine(string.Format("{0} 0x{1:x}", MFDump.LookupName(typeof(MF_CAPTURE_ENGINE), g), hr2));

                        break;
                    }
                default:
                    {
                        Debug.WriteLine(met);
                        break;
                    }
            }

            return hr;
        }

        public int OnSample(IMFSample pSample)
        {
            m_Count++;
            return S_Ok;
        }

        public int OnSynchronizedEvent(IMFMediaEvent pEvent)
        {
            Debug.WriteLine("WE'RE HERE!!!");
            throw new NotImplementedException();
        }

    }
}
