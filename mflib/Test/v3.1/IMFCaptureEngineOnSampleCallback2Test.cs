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
    class IMFCaptureEngineOnSampleCallback2Test : COMBase, IMFCaptureEngineOnEventCallback, IMFCaptureEngineOnSampleCallback2//, ICustomQueryInterface
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
            RegisterMFT();
#if false
            MediaFoundation.Alt.IMFTopologyServiceLookupClientAlt a1 = null;
            MediaFoundation.EVR.IEVRFilterConfig a2 = null;
            IAdvancedMediaCapture a3 = null;
            MediaFoundation.MFPlayer.IMFPMediaItem a4 = null;
            INamedPropertyStore a5 = null;
            IOPMVideoOutput a6 = null;
            MediaFoundation.ReadWrite.IMFReadWriteClassFactory a7 = null;
            IMFLocalMFTRegistration a8 = null;

            a1.InitServicePointers();
            a2.GetNumberOfStreams();
            a3.GetAdvancedMediaCaptureSettings();
            a4.GetNumberOfStreams();
            a5.GetNameAt();
            a6.COPPCompatibleGetInformation();
            a7.CreateInstanceFromObject();
            a8.RegisterMFTs();
#endif

            HResult hr = 0;
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
#if false
            hr = m_mt.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, g1);
            MFError.ThrowExceptionForHR(hr);

            hr = m_csink2.SetOutputMediaType(i, m_mt, null);
            MFError.ThrowExceptionForHR(hr);

#endif
            hr = m_mt.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, gOutputType);
            MFError.ThrowExceptionForHR(hr);

            hr = m_csink2.SetOutputMediaType(i, m_mt, null);
            MFError.ThrowExceptionForHR(hr);
            Thread.Sleep(1000);
#if true
            hr = m_csink2.Prepare();
            MFError.ThrowExceptionForHR(hr);

            while (Thread.VolatileRead(ref m_Init) != 2)
                ;

            hr = m_ce.StartRecord();
            MFError.ThrowExceptionForHR(hr);

            while (m_Count < 1000)
                Thread.Sleep(10);

            hr = m_ce.StopRecord(true, true);
            MFError.ThrowExceptionForHR(hr);
            Thread.Sleep(500);
#endif

        }
        public void RegisterMFT()
        {
            HResult hr;

            MFTRegisterTypeInfo[] ti = new MFTRegisterTypeInfo[1];
            MFTRegisterTypeInfo[] to = new MFTRegisterTypeInfo[1];
            ti[0] = new MFTRegisterTypeInfo();
            to[0] = new MFTRegisterTypeInfo();

            to[0].guidSubtype = IMFCaptureSink2Test.gOutputType;

#if true
#if AUDIO
            ti[0].guidMajorType = MFMediaType.Audio;
            to[0].guidMajorType = MFMediaType.Audio;
            ti[0].guidSubtype = MFMediaType.Float;
            //hr = MFExtern.MFTRegisterLocalByCLSID(typeof(AudMft.Foo).GUID,
            hr = MFExtern.MFTRegister(typeof(AudMft.Foo).GUID,
            MFTransformCategory.MFT_CATEGORY_AUDIO_ENCODER, "Daddy", MFT_EnumFlag.SyncMFT, ti.Length, ti, to.Length, to, null);
#else
            ti[0].guidMajorType = MFMediaType.Video;
            to[0].guidMajorType = MFMediaType.Video;
            ti[0].guidSubtype = MFMediaType.YUY2;
            hr = MFExtern.MFTRegister(Guid.Empty, //typeof(AudMft.Foo).GUID,
                                      MFTransformCategory.MFT_CATEGORY_VIDEO_ENCODER,
                                      "Daddy",
                                      MFT_EnumFlag.SyncMFT,
                                      ti.Length,
                                      ti,
                                      to.Length,
                                      to,
                                      null);
#endif
#else
            //hr = MFExtern.MFTRegisterLocal(new cf(), MFTransformCategory.MFT_CATEGORY_AUDIO_DECODER, "Daddy", MFT_EnumFlag.LocalMFT, ti.Length, ti, to.Length, to);
            hr = MFExtern.MFTRegisterLocal(new cf(), MFTransformCategory.MFT_CATEGORY_AUDIO_DECODER, "Daddy", 0, 0, ti, 0, to);
            MFTransformCategory.MFT_CATEGORY_AUDIO_DECODER, "Daddy", MFT_EnumFlag.LocalMFT, ti.Length, ti, to.Length, to);
#endif
            MFError.ThrowExceptionForHR(hr);
        }
        private void GetInterface()
        {
            IMFCaptureEngineClassFactory cecf = new MFCaptureEngineClassFactory() as IMFCaptureEngineClassFactory;
            Debug.Assert(cecf != null);

            object o;
            HResult hr = cecf.CreateInstance(CLSID.CLSID_MFCaptureEngine, typeof(IMFCaptureEngine).GUID, out o);
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
            //const int PREFERRED_SOURCE_STREAM_FOR_AUDIO = -9;
            const int PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW = -6;
            hr = source.GetStreamIndexFromFriendlyName(PREFERRED_SOURCE_STREAM_FOR_VIDEO_PREVIEW, out m_videostream);
            MFError.ThrowExceptionForHR(hr);

            hr = source.GetAvailableDeviceMediaType(m_videostream, 0, out m_mt);
            MFError.ThrowExceptionForHR(hr);

            Debug.WriteLine(MFDump.DumpAttribs(m_mt));

            IMFCaptureSink m_csink;
            hr = m_ce.GetSink(MF_CAPTURE_ENGINE_SINK_TYPE.Record, out m_csink);
            MFError.ThrowExceptionForHR(hr);

            m_csink2 = m_csink as IMFCaptureSink2;
        }

        public HResult OnEvent(IMFMediaEvent pEvent)
        {
            HResult hr;

            MediaEventType met = 0;
            hr = pEvent.GetType(out met);
            MFError.ThrowExceptionForHR(hr);

            HResult hr2;
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
                            m_Init = 99;
                        //m_Done = 1;

                        else if (g == MF_CAPTURE_ENGINE.ERROR)
                        {
                        }

                        else if (g == MF_CAPTURE_ENGINE.OUTPUT_MEDIA_TYPE_SET)
                        {
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

        public HResult OnSample(IMFSample pSample)
        {
            m_Count++;
            return HResult.S_OK;
        }

        public HResult OnSynchronizedEvent(IMFMediaEvent pEvent)
        {
            Debug.WriteLine("WE'RE HERE!!!");
            throw new NotImplementedException();
        }

        public CustomQueryInterfaceResult GetInterface(ref Guid iid, out IntPtr ppv)
        {
            ppv = IntPtr.Zero;
            if (
                iid != typeof(IMFMediaEventGenerator).GUID &&
                iid != typeof(IMFShutdown).GUID)
            {
                string sGuidString = iid.ToString("B");
                string sKeyName = string.Format("HKEY_CLASSES_ROOT\\Interface\\{0}", sGuidString);
                string sName = (string)Microsoft.Win32.Registry.GetValue(sKeyName, null, sGuidString);

                if (sName == null || sName.Trim().Length == 0)
                    sName = sGuidString.ToString();

                Debug.WriteLine(string.Format("Unhandled interface requested: {0}", sName));
            }
            return CustomQueryInterfaceResult.NotHandled;
        }
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("00000001-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IClassFactory
    {
        [PreserveSig]
        int CreateInstance(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);

        [PreserveSig]
        int LockServer(
            [In, MarshalAs(UnmanagedType.Bool)] bool fLock);
    }

    class cf : COMBase, IClassFactory
    {
        public int CreateInstance(
            [In, MarshalAs(UnmanagedType.IUnknown)]object pUnkOuter,
            [MarshalAs(UnmanagedType.LPStruct)]Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)]out object ppvObject)
        {
            ppvObject = null; // new AudMft.Foo();
            return 0;
        }

        public int LockServer([In, MarshalAs(UnmanagedType.Bool)]bool fLock)
        {
            return 0;
        }
    }
}
