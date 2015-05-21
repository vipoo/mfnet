using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace Testv30
{
    class IMFCaptureRecordSinkTest : COMBase, IMFCaptureEngineOnEventCallback, IMFCaptureEngineOnSampleCallback, IMFMediaSink
    {
        IMFCaptureSink m_csink;
        int m_Init = 0;
        IMFMediaType m_mt;
        IMFCaptureEngine m_ce;
        int m_videostream;
        int m_Frames = 0;
        int m_Done = 0;
        int m_DidCustom = 0;

        public void DoTests()
        {
            int hr;

            GetInterface();

            IMFCaptureRecordSink rs = m_csink as IMFCaptureRecordSink;

            ///////////////////
            int i;
            hr = m_csink.AddStream(m_videostream, m_mt, null, out i);
            MFError.ThrowExceptionForHR(hr);

            hr = rs.SetCustomSink(this);
            MFError.ThrowExceptionForHR(hr);

            hr = m_csink.Prepare();
            MFError.ThrowExceptionForHR(hr);

            while (Thread.VolatileRead(ref m_Init) == 1)
                ;

            ///////////////////

            hr = rs.SetRotation(i, 180);
            MFError.ThrowExceptionForHR(hr);

            int r;
            hr = rs.GetRotation(i, out r);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(r == 180);

            ////////////////////////

            FileInfo fi = new FileInfo("output.wma");
            if (fi.Exists)
            {
                fi.Delete();
            }

            hr = rs.SetOutputFileName("output.wma");
            MFError.ThrowExceptionForHR(hr);

            Doit();

            fi = new FileInfo("output.wma");
            Debug.Assert(fi.Length > 1000);
            fi.Delete();

            ////////////////////

            fi = new FileInfo("output.pcm");
            if (fi.Exists)
            {
                fi.Delete();
            }

            IMFByteStream bs;
            hr = MFExtern.MFCreateFile(MFFileAccessMode.Write, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, "output.pcm", out bs);
            MFError.ThrowExceptionForHR(hr);

            hr = rs.SetOutputByteStream(bs, MFTranscodeContainerType.ASF);
            MFError.ThrowExceptionForHR(hr);

            Doit();

            fi = new FileInfo("output.pcm");
            Debug.Assert(fi.Length > 1000);
            fi.Delete();

            //////////////////////////////

            hr = rs.SetSampleCallback(i, this);
            MFError.ThrowExceptionForHR(hr);

            Doit();

            Debug.Assert(m_Frames > 100);

            hr = rs.SetCustomSink(this);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(m_DidCustom == 1);

            Debug.WriteLine("Done!");
        }

        private void Doit()
        {
            int hr;

            m_Frames = 0;
            m_Done = 0;

            hr = m_ce.StartRecord();
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(3000);

            hr = m_ce.StopRecord(true, false);
            MFError.ThrowExceptionForHR(hr);

            while (Thread.VolatileRead(ref m_Done) == 0)
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

            hr = source.GetStreamIndexFromFriendlyName(-9, out m_videostream);
            MFError.ThrowExceptionForHR(hr);

            hr = source.GetAvailableDeviceMediaType(m_videostream, 0, out m_mt);
            MFError.ThrowExceptionForHR(hr);

            hr = m_ce.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.Record, out m_csink);
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

                else if (g == MF_CAPTURE_ENGINE.SINK_PREPARED)
                    m_Init = 2;

                else if (g == MF_CAPTURE_ENGINE.RECORD_STOPPED)
                    m_Done = 1;

                else if (g == MF_CAPTURE_ENGINE.ERROR)
                {
                    int hr2;
                    hr = pEvent.GetStatus(out hr2);
                    Debug.WriteLine(string.Format("0x{0:x}", hr2));
                }

                Debug.WriteLine(MF_CAPTURE_ENGINE.LookupName(g));
            }

            return hr;
        }

        public int OnSample(IMFSample pSample)
        {
            m_Frames++;
            return S_Ok;
        }

        #region IMFMediaSink

        public int GetCharacteristics(out MFMediaSinkCharacteristics pdwCharacteristics)
        {
            pdwCharacteristics = MFMediaSinkCharacteristics.Rateless;
            return S_Ok;
        }

        public int AddStreamSink(int dwStreamSinkIdentifier, IMFMediaType pMediaType, out IMFStreamSink ppStreamSink)
        {
            throw new NotImplementedException();
        }

        public int RemoveStreamSink(int dwStreamSinkIdentifier)
        {
            throw new NotImplementedException();
        }

        public int GetStreamSinkCount(out int pcStreamSinkCount)
        {
            pcStreamSinkCount = 1;
            return S_Ok;
        }

        public int GetStreamSinkByIndex(int dwIndex, out IMFStreamSink ppStreamSink)
        {
            m_DidCustom = 1;
            ppStreamSink = null;
            return -22;
        }

        public int GetStreamSinkById(int dwStreamSinkIdentifier, out IMFStreamSink ppStreamSink)
        {
            throw new NotImplementedException();
        }

        public int SetPresentationClock(IMFPresentationClock pPresentationClock)
        {
            throw new NotImplementedException();
        }

        public int GetPresentationClock(out IMFPresentationClock ppPresentationClock)
        {
            throw new NotImplementedException();
        }

        public int Shutdown()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
