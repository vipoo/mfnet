using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using MediaFoundation.EVR;

namespace Testv30
{
    class IMFCapturePreviewSinkTest : COMBase, IMFCaptureEngineOnEventCallback, IMFCaptureEngineOnSampleCallback
    {
        IMFCapturePreviewSink m_pPreview;
        int m_Init = 0;
        IMFCaptureEngine m_pEngine;
        int m_Frames = 0;
        int m_Done = 0;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetConsoleTitle(StringBuilder sb, int capacity);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        [DllImport("user32.dll")]
        public static extern int GetClientRect(IntPtr hWnd, MFRect mr);

        public void DoTests()
        {
            GetInterface();

            TestHandle();

            TestSample();

            TestRotate();

            TestMirror();

            TestSet();

            //////////////////////////////

            Debug.WriteLine("Done!");
        }

        private void TestSet()
        {
            int hr;

            hr = m_pPreview.SetCustomSink(new mooSink());
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(2000);

            // mooSink has a fake implementation of IMFMediaSink & IDCompositionVisual
            hr = m_pPreview.SetRenderSurface(new mooSink());
            MFError.ThrowExceptionForHR(hr);
        }

        private void TestMirror()
        {
            int hr;

            bool b, b2;

            hr = m_pPreview.GetMirrorState(out b);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pPreview.SetMirrorState(!b);
            if (hr != MFError.MF_E_CAPTURE_SINK_MIRROR_ERROR)
            {
                MFError.ThrowExceptionForHR(hr);

                hr = m_pPreview.GetMirrorState(out b2);
                MFError.ThrowExceptionForHR(hr);

                Debug.Assert(b != b2);
            }
        }

        private void TestRotate()
        {
            int hr;

            int r, r2;
            hr = m_pPreview.GetRotation(0, out r);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pPreview.SetRotation(0, 180);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pPreview.GetRotation(0, out r2);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(r2 == 180);
        }

        private void TestSample()
        {
            int hr;

            hr = m_pPreview.SetSampleCallback(0, this);
            MFError.ThrowExceptionForHR(hr);

            Doit();

            Debug.Assert(m_Frames > 0);
        }

        private void TestHandle()
        {
            IntPtr p;
            StringBuilder sb = new StringBuilder(500);

            GetConsoleTitle(sb, sb.Capacity);
            p = FindWindow(null, sb.ToString());

            int hr;

            hr = m_pPreview.SetRenderHandle(p);
            MFError.ThrowExceptionForHR(hr);

            MFRect mr = new MFRect();
            GetClientRect(p, mr);

            MFARGB g = new MFARGB();
            g.rgbBlue = 255;

            hr = m_pEngine.StartPreview();
            MFError.ThrowExceptionForHR(hr);

            // 3 seconds with default - blue

            Thread.Sleep(3000);

            MFInt mf = new MFInt(g.ToInt32());

            mr.right /= 2;

            g.rgbBlue = 0;
            g.rgbGreen = 255;
            mf.Assign(g.ToInt32());

            hr = m_pPreview.UpdateVideo(null, mr, mf);
            MFError.ThrowExceptionForHR(hr);

            // 3 seconds with right / 2 - green

            Thread.Sleep(3000);

            mr.right *= 2;

            g.rgbBlue = 255;
            g.rgbGreen = 0;
            mf.Assign(g.ToInt32());

            hr = m_pPreview.UpdateVideo(null, mr, mf);
            MFError.ThrowExceptionForHR(hr);

            // 3 seconds with default - blue
            Thread.Sleep(3000);

            MFVideoNormalizedRect mn = new MFVideoNormalizedRect();
            mn.bottom = 0.5f;
            mn.right = 1.0f;

            g.rgbBlue = 0;
            g.rgbRed = 255;
            mf.Assign(g.ToInt32());

            hr = m_pPreview.UpdateVideo(mn, mr, mf);
            MFError.ThrowExceptionForHR(hr);

            // 3 seconds with bottom / 2 - red
            Thread.Sleep(3000);
            
            hr = m_pEngine.StopPreview();
            MFError.ThrowExceptionForHR(hr);

            while (Thread.VolatileRead(ref m_Done) == 0)
                ;

        }

        private void Doit()
        {
            int hr;

            m_Frames = 0;
            m_Done = 0;

            Thread.Sleep(100);

            hr = m_pEngine.StartPreview();
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(3000);
            Thread.Sleep(3000);

            hr = m_pEngine.StopPreview();
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

            while (Thread.VolatileRead(ref m_Init) == 0)
                ;

            //////////////////////////////////////

            // Get a pointer to the photo sink.
            IMFCaptureSink pSink;
            hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.Preview, out pSink);
            MFError.ThrowExceptionForHR(hr);

            m_pPreview = pSink as IMFCapturePreviewSink;

            IMFCaptureSource pSource;
            hr = m_pEngine.GetSource(out pSource);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaType pMediaType, pMediaType2;
            hr = pSource.GetCurrentDeviceMediaType(-6, out pMediaType);
            MFError.ThrowExceptionForHR(hr);

            CloneVideoMediaType(pMediaType, MFMediaType.RGB32, out pMediaType2);
            hr = pMediaType2.SetUINT32(MFAttributesClsid.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);

            int dwSinkStreamIndex;
            // Try to connect the first still image stream to the photo sink 
            hr = m_pPreview.AddStream(-6, pMediaType2, null, out dwSinkStreamIndex);
            MFError.ThrowExceptionForHR(hr);

            hr = pSink.Prepare();
            MFError.ThrowExceptionForHR(hr);
        }

        private void CreatePhotoMediaType(IMFMediaType pSrcMediaType, out IMFMediaType pPhotoMediaType)
        {
            int hr = MFExtern.MFCreateMediaType(out pPhotoMediaType);
            MFError.ThrowExceptionForHR(hr);

            hr = pPhotoMediaType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Image);
            MFError.ThrowExceptionForHR(hr);

            hr = pPhotoMediaType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFImageFormat.JPEG);
            MFError.ThrowExceptionForHR(hr);

            CopyAttribute(pSrcMediaType, pPhotoMediaType, MFAttributesClsid.MF_MT_FRAME_SIZE);
        }

        private void CopyAttribute(IMFAttributes pSrc, IMFAttributes pDest, Guid key)
        {
            PropVariant var = new PropVariant();
            int hr = pSrc.GetItem(key, var);
            MFError.ThrowExceptionForHR(hr);

            hr = pDest.SetItem(key, var);
            MFError.ThrowExceptionForHR(hr);

            var.Clear();
        }

        private void CloneVideoMediaType(IMFMediaType pSrcMediaType, Guid guidSubType, out IMFMediaType pNewMediaType)
        {
            int hr = MFExtern.MFCreateMediaType(out pNewMediaType);
            MFError.ThrowExceptionForHR(hr);

            hr = pNewMediaType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            MFError.ThrowExceptionForHR(hr);

            hr = pNewMediaType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, guidSubType);
            MFError.ThrowExceptionForHR(hr);

            CopyAttribute(pSrcMediaType, pNewMediaType, MFAttributesClsid.MF_MT_FRAME_SIZE);

            CopyAttribute(pSrcMediaType, pNewMediaType, MFAttributesClsid.MF_MT_FRAME_RATE);

            CopyAttribute(pSrcMediaType, pNewMediaType, MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO);

            CopyAttribute(pSrcMediaType, pNewMediaType, MFAttributesClsid.MF_MT_INTERLACE_MODE);
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
                else if (g == MF_CAPTURE_ENGINE.PHOTO_TAKEN)
                    m_Done = 1;
                else if (g == MF_CAPTURE_ENGINE.PREVIEW_STOPPED)
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
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("4d93059d-097b-4651-9a60-f0f25116e2f3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]    
    public interface iasdf
    {
    }

    class mooSink : COMBase, IMFMediaSink, iasdf
    {
        public int GetCharacteristics(out MFMediaSinkCharacteristics pdwCharacteristics)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public int GetStreamSinkByIndex(int dwIndex, out IMFStreamSink ppStreamSink)
        {
            throw new NotImplementedException();
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
    }
}
