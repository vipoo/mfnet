using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace Testv30
{
    class IMFCapturePhotoSinkTest : COMBase, IMFCaptureEngineOnEventCallback, IMFCaptureEngineOnSampleCallback
    {
        IMFCapturePhotoSink m_pPhoto;
        int m_Init = 0;
        IMFCaptureEngine m_pEngine;
        int m_Frames = 0;
        int m_Done = 0;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetConsoleTitle(StringBuilder sb, int capacity);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        public void DoTests()
        {
            int hr;

            GetInterface2();

            Thread.Sleep(2000);

            GetInterface();

            FileInfo fi = new FileInfo("output1.jpg");
            if (fi.Exists)
            {
                fi.Delete();
            }

            hr = m_pPhoto.SetOutputFileName("output1.jpg");
            MFError.ThrowExceptionForHR(hr);

            Doit();

            fi = new FileInfo("output1.jpg");
            Debug.Assert(fi.Length > 1000);
            fi.Delete();

            ////////////////////

            fi = new FileInfo("output.jpg");
            if (fi.Exists)
            {
                fi.Delete();
            }

            IMFByteStream bs;
            hr = MFExtern.MFCreateFile(MFFileAccessMode.Write, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, "output.jpg", out bs);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pPhoto.SetOutputByteStream(bs);
            MFError.ThrowExceptionForHR(hr);

            Doit();

            fi = new FileInfo("output.jpg");
            Debug.Assert(fi.Length > 1000);
            fi.Delete();

            //////////////////////////////

            hr = m_pPhoto.SetSampleCallback(this);
            MFError.ThrowExceptionForHR(hr);

            Doit();

            Debug.Assert(m_Frames > 0);

            Debug.WriteLine("Done!");
        }

        private void Doit()
        {
            int hr;

            m_Frames = 0;
            m_Done = 0;

            Thread.Sleep(1000);

            hr = m_pEngine.TakePhoto();
            MFError.ThrowExceptionForHR(hr);

            while (Thread.VolatileRead(ref m_Done) == 0)
                ;
        }

        private void GetInterface()
        {
            int hr;

#if false
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
#endif

            //////////////////////////////////////

            // Get a pointer to the photo sink.
            IMFCaptureSink pSink;
            hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.Photo, out pSink);
            MFError.ThrowExceptionForHR(hr);

            m_pPhoto = pSink as IMFCapturePhotoSink;

            IMFCaptureSource pSource;
            hr = m_pEngine.GetSource(out pSource);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaType pMediaType, pMediaType2;
            hr = pSource.GetCurrentDeviceMediaType(-8, out pMediaType);
            MFError.ThrowExceptionForHR(hr);

            //Configure the photo format 
            CreatePhotoMediaType(pMediaType, out pMediaType2);

            hr = m_pPhoto.RemoveAllStreams();
            MFError.ThrowExceptionForHR(hr);

            int dwSinkStreamIndex;
            // Try to connect the first still image stream to the photo sink 
            hr = m_pPhoto.AddStream(-8, pMediaType2, null, out dwSinkStreamIndex);
            MFError.ThrowExceptionForHR(hr);

            hr = pSink.Prepare();
            MFError.ThrowExceptionForHR(hr);
        }

        private void GetInterface2()
        {
            IMFCaptureEngineClassFactory cecf = new MFCaptureEngineClassFactory() as IMFCaptureEngineClassFactory;
            Debug.Assert(cecf != null);

            object o;
            int hr = cecf.CreateInstance(CLSID.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            m_pEngine = o as IMFCaptureEngine;

#if false
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
#endif

            hr = m_pEngine.Initialize(this, null, null, null);
            MFError.ThrowExceptionForHR(hr);

            while (Thread.VolatileRead(ref m_Init) == 0)
                ;

            //////////////////////////////////////

            // Get a pointer to the photo sink.
            IMFCaptureSink pSink;
            hr = m_pEngine.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.Preview, out pSink);
            MFError.ThrowExceptionForHR(hr);

            IMFCapturePreviewSink pPreview = pSink as IMFCapturePreviewSink;

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
            hr = pPreview.AddStream(-6, pMediaType2, null, out dwSinkStreamIndex);
            MFError.ThrowExceptionForHR(hr);

            hr = pSink.Prepare();
            MFError.ThrowExceptionForHR(hr);

            IntPtr p;
            StringBuilder sb = new StringBuilder(500);

            GetConsoleTitle(sb, sb.Capacity);
            p = FindWindow(null, sb.ToString());

            hr = pPreview.SetRenderHandle(p);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pEngine.StartPreview();
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

        private void CopyAttribute(IMFAttributes pSrc, IMFAttributes pDest, Guid key)
        {
            PropVariant var = new PropVariant();
            int hr = pSrc.GetItem(key, var);
            MFError.ThrowExceptionForHR(hr);

            hr = pDest.SetItem(key, var);
            MFError.ThrowExceptionForHR(hr);

            var.Clear();
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
                else if (g == MF_CAPTURE_ENGINE.PREVIEW_STARTED)
                    m_Done = 1;
                else if (g == MF_CAPTURE_ENGINE.RECORD_STOPPED)
                    m_Done = 1;
                else if (g == MF_CAPTURE_ENGINE.PHOTO_TAKEN)
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
}
