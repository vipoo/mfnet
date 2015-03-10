using System;
using MediaFoundation;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;

using System.Runtime.InteropServices.ComTypes;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MediaFoundation.Transform;
using System.Threading;
using System.Text;
using MediaFoundation.EVR;

namespace Testv21
{
    class TestExterns
    {
        string FILENAME = @"c:/sourceforge/mflib/test/media/AspectRatio4x3.wmv";

        public void DoTests()
        {
            Test1();
            Test2();
            Test3();

            foo t1 = new foo();
            t1.DoTests();
        }

        private IMFByteStream GetByteStream(IStream isw)
        {
            int hr;
            IMFByteStream bs;

            hr = MFExtern.MFCreateMFByteStreamOnStream(isw, out bs);
            MFError.ThrowExceptionForHR(hr);

            return bs;
        }

        private void Test1()
        {
            int hr;
            IStreamWrapper isw = new IStreamWrapper(FILENAME);

            using (isw)
            {
                IMFByteStream bs = GetByteStream(isw);

                IMFSourceReader sr;
                hr = MFExtern.MFCreateSourceReaderFromByteStream(bs, null, out sr);
                MFError.ThrowExceptionForHR(hr);

                Debug.Assert(sr != null);
            }
        }
        private void Test2()
        {
            int hr;
            IStreamWrapper isw = new IStreamWrapper(FILENAME);

            using (isw)
            {
                IMFByteStream bs = GetByteStream(isw);

                IMFAttributes attr;
                hr = MFExtern.MFCreateAttributes(out attr, 1);
                MFError.ThrowExceptionForHR(hr);

                hr = attr.SetUINT32(MFAttributesClsid.MF_LOW_LATENCY, 1);
                MFError.ThrowExceptionForHR(hr);

                IMFSourceReader sr;
                hr = MFExtern.MFCreateSourceReaderFromByteStream(bs, attr, out sr);
                MFError.ThrowExceptionForHR(hr);

                Debug.Assert(sr != null);
            }
        }
        private void Test3()
        {
            int aai;
            int hr;
            IMFActivate[] ma;

            hr = MFExtern.MFTEnumEx(
                MFTransformCategory.MFT_CATEGORY_VIDEO_DECODER,
                MFT_EnumFlag.SyncMFT,
                null,
                null,
                out ma,
                out aai);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(aai > 1);
            Debug.Assert(aai == ma.Length);

            // .Net doesn't always do the qi unless you call things
            for (int x = 0; x < aai; x++)
            {
                hr = ma[x].ShutdownObject();
                MFError.ThrowExceptionForHR(hr);
            }
        }
    }

    class foo : COMBase, IMFAsyncCallback, IMFActivate
    {
        string FILENAME1 = @"file://c:/sourceforge/mflib/test/media/AspectRatio4x3.wmv";
        string FILENAME2 = @"c:\sourceforge\mflib\test\media\AspectRatio4x3.wmv";

        public void DoTests()
        {
            TestA();
            TestB();
            TestC();
            TestD();
            TestE();
            TestF();
            TestG();
            TestH();
            TestI();
            TestJ();
            TestK();
            TestL();
            TestM();
            TestN();
            TestO();
            TestP();
            TestQ();
            TestR();
            TestS();
            TestT();
            TestU();
            TestW(); // remote desktop
        }

        private void TestA()
        {
            int hr;

            // The interface is still untested
            //IMFQualityManager imfQualityManager;
            //hr = MFExtern.MFCreateStandardQualityManager(out imfQualityManager); // new
            //MFError.ThrowExceptionForHR(hr);

            //IsA(imfQualityManager, typeof(IMFQualityManager).GUID);

#if false
            IMFTopology imfTopology = null;
            IMFPresentationClock imfPresentationClock=null;
            IMFTopologyNode imfTopologyNode = null;
            IMFSample imfSample = null;
            IMFMediaEvent imfMediaEvent = null;
            object o = null;

            hr = MFExtern.MFCreatePresentationClock(out imfPresentationClock);

            hr = imfQualityManager.NotifyTopology(imfTopology);
            hr = imfQualityManager.NotifyPresentationClock(imfPresentationClock);
            hr = imfQualityManager.NotifyProcessInput(imfTopologyNode, 0, imfSample);
            hr = imfQualityManager.NotifyProcessOutput(imfTopologyNode, 0, imfSample );
            hr = imfQualityManager.NotifyQualityEvent(o, imfMediaEvent);
            hr = imfQualityManager.Shutdown();
#endif

            IMFPMPServer imfPMPServer;
            hr = MFExtern.MFCreatePMPServer(MFPMPSessionCreationFlags.None, out imfPMPServer); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfPMPServer, typeof(IMFPMPServer).GUID);

            hr = imfPMPServer.LockProcess();
            MFError.ThrowExceptionForHR(hr);

            object o2;
            Guid g = new Guid("00000000-0000-0000-C000-000000000046"); // IUnknown

            // Since this object is in another process, .net requires the interface to be registered,
            // which most IMF interfaces haven't done.
            hr = imfPMPServer.CreateObjectByCLSID(typeof(CColorConvertDMO).GUID, g, out o2);
            MFError.ThrowExceptionForHR(hr);

            hr = imfPMPServer.UnlockProcess();
            MFError.ThrowExceptionForHR(hr);

            // The interface is still untested
            //IMFASFMultiplexer imfASFMultiplexer;
            //hr = MFExtern.MFCreateASFMultiplexer(out imfASFMultiplexer); // new
            //MFError.ThrowExceptionForHR(hr);

            //IsA(imfASFMultiplexer, typeof(IMFASFMultiplexer).GUID);
        }

        private void TestB()
        {
            int hr;

            IMFAsyncResult imfAsyncResult;
            object objState = null;

            hr = MFExtern.MFCreateAsyncResult(objState, null, objState, out imfAsyncResult);
            MFError.ThrowExceptionForHR(hr);

            IMFAsyncCallback imfAsyncCallback = (IMFAsyncCallback)this;
            hr = MFExtern.MFPutWorkItem((int)MFAsyncCallbackQueue.Standard, imfAsyncCallback, objState);
            MFError.ThrowExceptionForHR(hr);

            bool b = are.WaitOne();
            Debug.Assert(b && m_Hits == 1);

            hr = MFExtern.MFCreateAsyncResult(null, this, null, out imfAsyncResult);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFPutWorkItemEx((int)MFAsyncCallbackQueue.Standard, imfAsyncResult); // new
            MFError.ThrowExceptionForHR(hr);

            b = are.WaitOne();
            Debug.Assert(b && m_Hits == 2);

            hr = imfAsyncResult.GetStatus();
            Debug.Assert(hr == 7);

            long l;

            hr = MFExtern.MFScheduleWorkItem(imfAsyncCallback, objState, 4, out l); // new
            MFError.ThrowExceptionForHR(hr);

            b = are.WaitOne();
            Debug.Assert(b && m_Hits == 3);

            hr = MFExtern.MFScheduleWorkItemEx(imfAsyncResult, -1234, out l); // new
            MFError.ThrowExceptionForHR(hr);

            b = are.WaitOne();
            Debug.Assert(b && m_Hits == 4);

            hr = MFExtern.MFScheduleWorkItemEx(imfAsyncResult, -1000, out l); // new
            MFError.ThrowExceptionForHR(hr);

            b = are.WaitOne(100);
            Debug.Assert(!b);

            hr = MFExtern.MFCancelWorkItem(l); // new
            MFError.ThrowExceptionForHR(hr);

            b = are.WaitOne(1000);
            Debug.Assert(!b);

            int dwKey;
            hr = MFExtern.MFAddPeriodicCallback(new MFExtern.MFPERIODICCALLBACK(MyMethod), objState, out dwKey); // new
            MFError.ThrowExceptionForHR(hr);

            b = are2.WaitOne(16);
            Debug.Assert(b);

            hr = MFExtern.MFRemovePeriodicCallback(dwKey); // new
            MFError.ThrowExceptionForHR(hr);

            int iWait;
            hr = MFExtern.MFGetTimerPeriodicity(out iWait);
            MFError.ThrowExceptionForHR(hr);

            b = are2.WaitOne(iWait * 2);
            Debug.Assert(!b && m_Hits2 == 1);

            hr = MFExtern.MFLockWorkQueue((int)MFAsyncCallbackQueue.Standard); // new
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFUnlockWorkQueue((int)MFAsyncCallbackQueue.Standard);
            MFError.ThrowExceptionForHR(hr);

            int wq;
            hr = MFExtern.MFAllocateWorkQueue(out wq);
            MFError.ThrowExceptionForHR(hr);

            m_testno = 1;

            hr = MFExtern.MFBeginRegisterWorkQueueWithMMCSS(wq, "audio", 0, imfAsyncCallback, objState); // new
            MFError.ThrowExceptionForHR(hr);

            b = are.WaitOne();
            Debug.Assert(b && m_Hits == 5);

            StringBuilder szClass = new StringBuilder(100);
            MFInt mf = new MFInt(100);

            hr = MFExtern.MFGetWorkQueueMMCSSClass(wq, szClass, mf); // new
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(szClass.ToString() == "audio");

            hr = MFExtern.MFGetWorkQueueMMCSSTaskId(wq, out m_dwTaskId); // new
            MFError.ThrowExceptionForHR(hr);

            m_testno = 2;
            hr = MFExtern.MFBeginUnregisterWorkQueueWithMMCSS(wq, imfAsyncCallback, objState); // new
            MFError.ThrowExceptionForHR(hr);

            b = are.WaitOne();
            Debug.Assert(b && m_Hits == 6);

            ///////////////
            int iq;
            hr = MFExtern.MFAllocateWorkQueueEx(MFASYNC_WORKQUEUE_TYPE.WindowWorkqueue, out iq); // new
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFPutWorkItem(iq, imfAsyncCallback, objState);
            MFError.ThrowExceptionForHR(hr);

            b = are.WaitOne();
            Debug.Assert(b && m_Hits == 7);
        }

        private void TestC()
        {
            int hr;
            IMFNetCredentialCache imfNetCredentialCache;

            hr = MFExtern.MFCreateCredentialCache(out imfNetCredentialCache); // new
            MFError.ThrowExceptionForHR(hr);

            IMFNetCredential imfNetCredential;
            MFNetCredentialRequirements req;
            hr = imfNetCredentialCache.GetCredential("http://foo.bar", "here", MFNetAuthenticationFlags.ClearText, out imfNetCredential, out req);
            MFError.ThrowExceptionForHR(hr);

            string sUser = "asdf\0";
            int iSize = Encoding.Unicode.GetByteCount(sUser);
            byte[] bIn = Encoding.Unicode.GetBytes(sUser);
            hr = imfNetCredential.SetUser(bIn, iSize, false);
            MFError.ThrowExceptionForHR(hr);

            byte[] b = null;
            MFInt mf = new MFInt(0);
            hr = imfNetCredential.GetUser(b, mf, false);
            MFError.ThrowExceptionForHR(hr);

            b = new byte[mf];

            hr = imfNetCredential.GetUser(b, mf, false);
            MFError.ThrowExceptionForHR(hr);

            string s = Encoding.Unicode.GetString(b);

            Debug.Assert(s.CompareTo(sUser) == 0);

            string sPW = "jkl;\0";
            int iSizep = Encoding.Unicode.GetByteCount(sPW);
            byte[] bInp = Encoding.Unicode.GetBytes(sPW);
            hr = imfNetCredential.SetPassword(bInp, iSizep, false);
            MFError.ThrowExceptionForHR(hr);

            byte[] bp = null;
            MFInt mfp = new MFInt(0);
            hr = imfNetCredential.GetPassword(bp, mfp, false);
            MFError.ThrowExceptionForHR(hr);

            bp = new byte[mfp];

            hr = imfNetCredential.GetPassword(bp, mfp, false);
            MFError.ThrowExceptionForHR(hr);

            string sp = Encoding.Unicode.GetString(bp);

            Debug.Assert(sp.CompareTo(sPW) == 0);

            bool lo;
            hr = imfNetCredential.LoggedOnUser(out lo);
            Marshal.ThrowExceptionForHR(hr);

            Debug.Assert(lo == false);

            hr = imfNetCredentialCache.SetGood(imfNetCredential, true);
            Marshal.ThrowExceptionForHR(hr);

            hr = imfNetCredentialCache.SetUserOptions(imfNetCredential, MFNetCredentialOptions.AllowClearText);
            Marshal.ThrowExceptionForHR(hr);
        }

        private void TestD()
        {
            int hr;
            IMFVideoMediaType imfVideoMediaType;
            VideoInfoHeader2 vih2 = new VideoInfoHeader2();

            hr = MFExtern.MFCreateVideoMediaTypeFromVideoInfoHeader2(vih2, Marshal.SizeOf(vih2), MFVideoFlags.AnalogProtected, Guid.Empty, out imfVideoMediaType); // new
            MFError.ThrowExceptionForHR(hr);

#pragma warning disable 618 // obsolete
            MFVideoFormat vf = imfVideoMediaType.GetVideoFormat();

            Debug.Assert(vf.videoInfo.VideoFlags == MFVideoFlags.AnalogProtected);

            IntPtr ip;
            hr = imfVideoMediaType.GetVideoRepresentation(MFRepresentation.AMMediaType, out ip, 240);
            MFError.ThrowExceptionForHR(hr);
#pragma warning restore 618 // obsolete

            AMMediaType mt;
            mt = (AMMediaType)Marshal.PtrToStructure(ip, typeof(AMMediaType));

            Debug.Assert(mt.formatType == MFRepresentation.VideoInfo2);

            hr = imfVideoMediaType.FreeRepresentation(MFRepresentation.AMMediaType, ip);
            MFError.ThrowExceptionForHR(hr);
        }

        private void TestE()
        {
            int hr;
            //IMFTopoLoader imfTopoLoader;

            // The interface is still untested
            //hr = MFExtern.MFCreateTopoLoader(out imfTopoLoader); // new
            //MFError.ThrowExceptionForHR(hr);

            //IMFTopology tp;
            //hr = imfTopoLoader.Load(null, out tp, null);

            //IsA(imfTopoLoader, typeof(IMFTopoLoader).GUID);

            BitmapInfoHeader bmh = new BitmapInfoHeader();
            FillBMI(bmh);
            int i;
            bool b;
            int j = Marshal.SizeOf(bmh);
            hr = MFExtern.MFCalculateBitmapImageSize(bmh, j, out i, out b);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(b);

            bmh.Compression = 222;
            hr = MFExtern.MFCalculateBitmapImageSize(bmh, j, out i, out b);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(!b);
        }

        private void TestF()
        {
            int hr;
            VideoInfoHeader vih = new VideoInfoHeader();
            IMFVideoMediaType imfVideoMediaType;

            hr = MFExtern.MFCreateVideoMediaTypeFromVideoInfoHeader(vih, Marshal.SizeOf(vih), 1, 1, MFVideoInterlaceMode.Progressive, MFVideoFlags.DigitallyProtected, MFMediaType.RGB32, out imfVideoMediaType); // new
            MFError.ThrowExceptionForHR(hr);

#pragma warning disable 618 // obsolete
            MFVideoFormat vf = imfVideoMediaType.GetVideoFormat();
#pragma warning restore 618 // obsolete

            Debug.Assert(vf.videoInfo.VideoFlags == MFVideoFlags.DigitallyProtected);
        }

        private void TestG()
        {
            int hr;
#pragma warning disable 618 // obsolete
            IMFAudioMediaType imfAudioMediaType;
            WaveFormatEx wfe = new WaveFormatEx();
            wfe.nChannels = 3;

            hr = MFExtern.MFCreateAudioMediaType(wfe, out imfAudioMediaType); // new
            MFError.ThrowExceptionForHR(hr);

            IntPtr ip = imfAudioMediaType.GetAudioFormat();
            WaveFormatEx wfe2 = (WaveFormatEx)Marshal.PtrToStructure(ip, typeof(WaveFormatEx));
#pragma warning restore 618

            Debug.Assert(wfe2.nChannels == 3);
        }

        private void TestH()
        {
            int hr;
            IMFAttributes imfAttributes = null;
            IMFMediaSink imfMediaSink;
            IMFSourceReader imfSourceReader;
            IMFSinkWriter imfSinkWriter;
            IMFByteStream imfByteStream;

            hr = MFExtern.MFCreateFile(MFFileAccessMode.Write, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, @"externx.asm", out imfByteStream);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateASFMediaSink(imfByteStream, out imfMediaSink); // new
            MFError.ThrowExceptionForHR(hr);

            IMFActivate imfActivate;
            IMFASFContentInfo imfASFContentInfo = (IMFASFContentInfo)imfMediaSink;
            hr = MFExtern.MFCreateASFMediaSinkActivate(FILENAME2, imfASFContentInfo, out imfActivate); // new
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateSinkWriterFromMediaSink(imfMediaSink, imfAttributes, out imfSinkWriter); // new
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateSourceReaderFromURL("http://www.LimeGreenSocks.com/AspectRatio4x3.wmv", imfAttributes, out imfSourceReader); // new
            MFError.ThrowExceptionForHR(hr);
        }

        private void TestI()
        {
            FourCC fc1 = new FourCC("YUY2");
            FourCC fc2 = new FourCC("WMV1");

            bool b1 = MFExtern.MFIsFormatYUV(fc1.ToInt32()); // new
            bool b2 = MFExtern.MFIsFormatYUV(fc2.ToInt32()); // new

            Debug.Assert(b1);
            Debug.Assert(!b2);

            int i;
            int hr = MFExtern.MFGetPlaneSize(fc1.ToInt32(), 320, 240, out i); // new
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(i == 153600);

            IMFVideoMediaType imfVideoMediaType;
            hr = MFExtern.MFCreateVideoMediaTypeFromSubtype(MFMediaType.RGB32, out imfVideoMediaType); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfVideoMediaType, typeof(IMFVideoMediaType).GUID);

            float[] f = new float[12];
            short[] s = new short[12];
            short[] s2 = new short[12];

            for (short x = 0; x < 12; x++)
                s[x] = x;

            hr = MFExtern.MFConvertFromFP16Array(f, s, 12); // new
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFConvertToFP16Array(s2, f, 12); // new
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(s2[4] == s[4]);
        }

        private void TestJ()
        {
            int hr;

            object o;
            hr = MFExtern.MFCreateVideoSampleAllocator(typeof(IMFVideoSampleAllocator).GUID, out o); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(o, typeof(IMFVideoSampleAllocator).GUID);

            long l = MFExtern.MFllMulDiv(1111, 2222, 3333, 4444); // new
            Debug.Assert(l == 742);
        }

        private void TestK()
        {
            int hr;
            IMFPresentationDescriptor imfPresentationDescriptor;
            IMFStreamDescriptor[] d = new IMFStreamDescriptor[2];
            /////////////////
            IMFMediaType[] pmt = new IMFMediaType[1];
            hr = MFExtern.MFCreateMediaType(out pmt[0]);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateStreamDescriptor(333, 1, pmt, out d[0]);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateStreamDescriptor(334, 1, pmt, out d[1]);
            MFError.ThrowExceptionForHR(hr);
            ////////////////
            hr = MFExtern.MFCreatePresentationDescriptor(0, d, out imfPresentationDescriptor);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFRequireProtectedEnvironment(imfPresentationDescriptor); // new
            Debug.Assert(hr == S_False);

            IMFPresentationClock imfPresentationClock;
            hr = MFExtern.MFCreatePresentationClock(out imfPresentationClock);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFShutdownObject(imfPresentationClock); // new
            MFError.ThrowExceptionForHR(hr);

            IMFNetProxyLocator imfNetProxyLocator;
            hr = MFExtern.MFCreateProxyLocator("http", null, out imfNetProxyLocator); // new
            MFError.ThrowExceptionForHR(hr);

            hr = imfNetProxyLocator.FindFirstProxy("http", "http://localhost", false);
            MFError.ThrowExceptionForHR(hr);

            StringBuilder sb = null;
            MFInt sbs = new MFInt(0);
            hr = imfNetProxyLocator.GetCurrentProxy(sb, sbs);
            MFError.ThrowExceptionForHR(hr);

            sb = new StringBuilder(sbs);
            sb.Append("asdf");
            hr = imfNetProxyLocator.GetCurrentProxy(sb, sbs);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(sb.ToString() == ""); // I have no proxy

            hr = imfNetProxyLocator.FindNextProxy();
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(hr == S_False); // no more proxies

            IMFNetProxyLocator imfNetProxyLocator2;
            hr = imfNetProxyLocator.Clone(out imfNetProxyLocator2);
            MFError.ThrowExceptionForHR(hr);

            hr = imfNetProxyLocator2.RegisterProxyResult(-1);
            MFError.ThrowExceptionForHR(hr);

            object o;
            hr = MFExtern.MFCreateNetSchemePlugin(typeof(IMFSchemeHandler).GUID, out o); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(o, typeof(IMFSchemeHandler).GUID);

#if false // CancelObjectCreation and EndCreateObject cause .net to crash, presumably a bug in MF
            // MS describes IMFSchemeHandler as "Applications do not use this interface."
            m_imfSchemeHandler = (IMFSchemeHandler)o;

            m_testno = 3;
            object objState = null;
            IntPtr cancelCookie;
            IMFAsyncCallback cb = (IMFAsyncCallback)this;
            hr = m_imfSchemeHandler.BeginCreateObject("http://www.LimeGreenSocks.com", MFResolution.ByteStream, null, out cancelCookie, cb, null);
            MFError.ThrowExceptionForHR(hr);
            hr = m_imfSchemeHandler.CancelObjectCreation(cancelCookie);
            MFError.ThrowExceptionForHR(hr);
            Thread.Sleep(5000);

            hr = m_imfSchemeHandler.BeginCreateObject("http://www.LimeGreenSocks.com", MFResolution.ByteStream, null, out cancelCookie, cb, null);
            MFError.ThrowExceptionForHR(hr);
            are.WaitOne();
#endif

            IMFASFProfile imfASFProfile;
            imfPresentationDescriptor = GetInterface();
            hr = MFExtern.MFCreateASFProfileFromPresentationDescriptor(imfPresentationDescriptor, out imfASFProfile); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfASFProfile, typeof(IMFASFProfile).GUID);
        }

        private void TestL()
        {
            int hr;

            IMFByteStream imfByteStream, imfByteStream2;

            hr = MFExtern.MFCreateFile(MFFileAccessMode.Read, MFFileOpenMode.FailIfNotExist, MFFileFlags.None, FILENAME2, out imfByteStream);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateASFIndexerByteStream(imfByteStream, 0, out imfByteStream2); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfByteStream2, typeof(IMFByteStream).GUID);

            IMFMediaSource ms = GetMediaSource();

            IMFSourceReader rdr;
            hr = MFExtern.MFCreateSourceReaderFromMediaSource(ms, null, out rdr);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaType imfMediaType;
            hr = rdr.GetCurrentMediaType(0, out imfMediaType);
            MFError.ThrowExceptionForHR(hr);

            IMFActivate imfActivate;
            IPropertyStore ps;

            IMFASFContentInfo imfASFContentInfo;
            hr = MFExtern.MFCreateASFContentInfo(out imfASFContentInfo);
            MFError.ThrowExceptionForHR(hr);

            IMFASFProfile imfASFProfile;
            hr = MFExtern.MFCreateASFProfileFromPresentationDescriptor(GetInterface(), out imfASFProfile); // new
            MFError.ThrowExceptionForHR(hr);

            hr = imfASFContentInfo.SetProfile(imfASFProfile);
            MFError.ThrowExceptionForHR(hr);

            IMFPresentationDescriptor imfPresentationDescriptor;
            hr = MFExtern.MFCreatePresentationDescriptorFromASFProfile(imfASFProfile, out imfPresentationDescriptor); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfPresentationDescriptor, typeof(IMFPresentationDescriptor).GUID);

            hr = imfASFContentInfo.GetEncodingConfigurationPropertyStore(0, out ps);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateWMAEncoderActivate(imfMediaType, ps, out imfActivate); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfActivate, typeof(IMFActivate).GUID);

            hr = rdr.GetCurrentMediaType(1, out imfMediaType);
            MFError.ThrowExceptionForHR(hr);

            hr = imfASFContentInfo.GetEncodingConfigurationPropertyStore(1, out ps);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateWMVEncoderActivate(imfMediaType, ps, out imfActivate); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfActivate, typeof(IMFActivate).GUID);

            IMFMediaSink imfMediaSink;
            IMFByteStream imfByteStream3;
            hr = MFExtern.MFCreateFile(MFFileAccessMode.Write, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, @"MFCreateASFStreamingMediaSink.wmv", out imfByteStream3);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateASFStreamingMediaSink(imfByteStream3, out imfMediaSink); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfMediaSink, typeof(IMFMediaSink).GUID);

            hr = MFExtern.MFCreateASFStreamingMediaSinkActivate(this, imfASFContentInfo, out imfActivate); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfActivate, typeof(IMFActivate).GUID);
        }

        private void TestM()
        {
            int hr;

            IMFAttributes imfAttributes;
            hr = MFExtern.MFCreateAttributes(out imfAttributes, 5);
            MFError.ThrowExceptionForHR(hr);

            hr = imfAttributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaSource imfMediaSource;
            hr = MFExtern.MFCreateDeviceSource(imfAttributes, out imfMediaSource); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfMediaSource, typeof(IMFMediaSource).GUID);

            IMFActivate imfActivate;
            hr = MFExtern.MFCreateDeviceSourceActivate(imfAttributes, out imfActivate); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfActivate, typeof(IMFActivate).GUID);

            IMFTransform mft;
            hr = MFExtern.MFCreateSampleCopierMFT(out mft); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(mft, typeof(IMFTransform).GUID);

            IMFTopology imfTopology;
            IMFTranscodeProfile imfTranscodeProfile;
            hr = MFExtern.MFCreateTranscodeProfile(out imfTranscodeProfile);
            MFError.ThrowExceptionForHR(hr);

            IMFAttributes imfAttributes2;
            hr = MFExtern.MFCreateAttributes(out imfAttributes2, 5);
            MFError.ThrowExceptionForHR(hr);

            hr = imfAttributes2.SetGUID(MFAttributesClsid.MF_TRANSCODE_CONTAINERTYPE, MFTranscodeContainerType.ASF);
            MFError.ThrowExceptionForHR(hr);

            hr = imfTranscodeProfile.SetContainerAttributes(imfAttributes2);
            MFError.ThrowExceptionForHR(hr);

            IMFAttributes imfAttributes3;
            hr = MFExtern.MFCreateAttributes(out imfAttributes3, 5);
            MFError.ThrowExceptionForHR(hr);

            hr = imfAttributes3.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.WMAudioV9);
            MFError.ThrowExceptionForHR(hr);

            hr = imfTranscodeProfile.SetAudioAttributes(imfAttributes3);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateTranscodeTopology(GetMediaSource(), "MFCreateTranscodeTopology.wmv", imfTranscodeProfile, out imfTopology); // new
            MFError.ThrowExceptionForHR(hr);

            IMFTopologyNode imfTopologyNode;
            hr = imfTopology.GetNode(0, out imfTopologyNode);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaType imfMediaType;
            hr = MFExtern.MFGetTopoNodeCurrentType(imfTopologyNode, 0, true, out imfMediaType); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfMediaType, typeof(IMFMediaType).GUID);
        }

        private void TestN()
        {
            int hr;
            IMFAttributes imfAttributes;
            hr = MFExtern.MFCreateAttributes(out imfAttributes, 5);
            MFError.ThrowExceptionForHR(hr);

            IMFCollection imfCollection;
            hr = MFExtern.MFCreateCollection(out imfCollection);
            MFError.ThrowExceptionForHR(hr);

            hr = imfCollection.AddElement(GetMediaSource());
            MFError.ThrowExceptionForHR(hr);

            hr = imfCollection.AddElement(GetMediaSource());
            MFError.ThrowExceptionForHR(hr);

            IMFMediaSource imfMediaSource;
            hr = MFExtern.MFCreateAggregateSource(imfCollection, out imfMediaSource); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfMediaSource, typeof(IMFMediaSource).GUID);

            hr = MFExtern.MFTranscodeGetAudioOutputAvailableTypes(MFMediaType.WMAudioV9, MFT_EnumFlag.AsyncMFT | MFT_EnumFlag.SyncMFT, imfAttributes, out imfCollection); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfCollection, typeof(IMFCollection).GUID);
        }

        private void TestO()
        {
            int hr;

            IMFByteStream imfByteStream;

            hr = MFExtern.MFCreateFile(MFFileAccessMode.Read, MFFileOpenMode.FailIfNotExist, MFFileFlags.None, FILENAME2, out imfByteStream);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaType imfMediaType;

            hr = MFExtern.MFCreateMediaType(out imfMediaType);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateFile(MFFileAccessMode.ReadWrite, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, @"MFCreateMPEG4MediaSink.mp4", out imfByteStream);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaSink imfMediaSink;

            hr = imfMediaType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            MFError.ThrowExceptionForHR(hr);

            hr = imfMediaType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.MFMPEG4Format);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(imfMediaType, MFAttributesClsid.MF_MT_FRAME_RATE, 25, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = imfMediaType.SetUINT32(MFAttributesClsid.MF_MT_AVG_BITRATE, 100000);
            MFError.ThrowExceptionForHR(hr);

            hr = imfMediaType.SetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, (int)MFVideoInterlaceMode.Progressive);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(imfMediaType, MFAttributesClsid.MF_MT_FRAME_SIZE, 640, 480);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateMPEG4MediaSink(imfByteStream, imfMediaType, null, out imfMediaSink); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfMediaSink, typeof(IMFMediaSink).GUID);

            hr = MFExtern.MFCreate3GPMediaSink(imfByteStream, imfMediaType, null, out imfMediaSink); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfMediaSink, typeof(IMFMediaSink).GUID);
        }

        private void TestP()
        {
            int hr;

            Guid IDirect3DDevice9_IID = new Guid("D0223B96-BF7A-43fd-92BD-A43B0D82B9EB");
            object o;
            hr = MFExtern.MFCreateVideoPresenter(null, IDirect3DDevice9_IID, typeof(IMFVideoPresenter).GUID, out o); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(o, typeof(IMFVideoPresenter).GUID);

            hr = MFExtern.MFCreateVideoMixer(null, IDirect3DDevice9_IID, typeof(IMFTransform).GUID, out o); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(o, typeof(IMFTransform).GUID);

            object o2;
            hr = MFExtern.MFCreateVideoMixerAndPresenter(null, null, typeof(IMFTransform).GUID, out o, typeof(IMFVideoPresenter).GUID, out o2); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(o2, typeof(IMFVideoPresenter).GUID);
            IsA(o, typeof(IMFTransform).GUID);
        }

        private void TestQ()
        {
            int hr;
            VideoInfoHeader2 vih2 = new VideoInfoHeader2();
            vih2.BmiHeader = new BitmapInfoHeader();
            vih2.BmiHeader.Size = Marshal.SizeOf(vih2.BmiHeader);

            IntPtr p = Marshal.AllocCoTaskMem(Marshal.SizeOf(vih2));
            Marshal.StructureToPtr(vih2, p, false);

            hr = MFExtern.MFValidateMediaTypeSize(MFRepresentation.VideoInfo2, p, Marshal.SizeOf(vih2)); // new
            MFError.ThrowExceptionForHR(hr);

            BitmapInfoHeader bmi = new BitmapInfoHeader();
            FillBMI(bmi);

            IMFVideoMediaType vmt;
            hr = MFExtern.MFCreateVideoMediaTypeFromBitMapInfoHeaderEx(bmi, Marshal.SizeOf(bmi), 2, 1, MFVideoInterlaceMode.Progressive, MFVideoFlags.ProgressiveContent, 25, 1, 150000, out vmt); // new
            MFError.ThrowExceptionForHR(hr);
            IsA(vmt, typeof(IMFVideoMediaType).GUID);

            IMFMediaType imfMediaType;
            hr = MFExtern.MFCreateMediaType(out imfMediaType);
            MFError.ThrowExceptionForHR(hr);

            AMMediaType mt = new AMMediaType();
            IntPtr pmt = Marshal.AllocCoTaskMem(Marshal.SizeOf(mt));
            Marshal.StructureToPtr(mt, pmt, false);
            hr = MFExtern.MFCreateMediaTypeFromRepresentation(MFRepresentation.AMMediaType, pmt, out imfMediaType); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfMediaType, typeof(IMFMediaType).GUID);

            bool b = MFExtern.MFCompareFullToPartialMediaType(imfMediaType, imfMediaType); // new
            Debug.Assert(b);

            IMFMediaType pWrap;
            hr = MFExtern.MFWrapMediaType(imfMediaType, MFMediaType.Video, MFMediaType.RGB32, out pWrap); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(pWrap, typeof(IMFMediaType).GUID);

            Mpeg1VideoInfo v1 = new Mpeg1VideoInfo();
            v1.hdr = new VideoInfoHeader();
            vih2.SrcRect = new MFRect();
            vih2.TargetRect = new MFRect();
            v1.hdr.BmiHeader = new BitmapInfoHeader();
            FillBMI(v1.hdr.BmiHeader);
            int i = Marshal.SizeOf(v1);
            v1.hdr.BmiHeader.Compression = new FourCC("MPEG").ToInt32();
            hr = MFExtern.MFInitMediaTypeFromMPEG1VideoInfo(imfMediaType, v1, i, MFMediaType.MPEG); // new
            MFError.ThrowExceptionForHR(hr);

            Mpeg2VideoInfo v2 = new Mpeg2VideoInfo();
            hr = MFExtern.MFInitMediaTypeFromMPEG2VideoInfo(imfMediaType, v2, Marshal.SizeOf(v2), MFMediaType.MPEG2); // new
            MFError.ThrowExceptionForHR(hr);
        }

        private void TestR()
        {
            int hr;
            int i;

            hr = MFExtern.MFCalculateImageSize(MFMediaType.RGB32, 240, 320, out i); // new
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(i == 307200);

            int j;
            hr = MFExtern.MFAverageTimePerFrameToFrameRate(123456543, out i, out j); // new
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(i == 1 && j == 10);
        }

        private void TestS()
        {
            int hr;

            IMFASFProfile imfASFProfile;
            hr = MFExtern.MFCreateASFProfileFromPresentationDescriptor(GetInterface(), out imfASFProfile); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfASFProfile, typeof(IMFASFProfile).GUID);

            IMFPresentationDescriptor imfPresentationDescriptor;
            hr = MFExtern.MFCreatePresentationDescriptorFromASFProfile(imfASFProfile, out imfPresentationDescriptor); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(imfPresentationDescriptor, typeof(IMFPresentationDescriptor).GUID);

            int i;
            IntPtr p;
            hr = MFExtern.MFSerializePresentationDescriptor(imfPresentationDescriptor, out i, out p); // new
            MFError.ThrowExceptionForHR(hr);

            IMFPresentationDescriptor pd;
            hr = MFExtern.MFDeserializePresentationDescriptor(i, p, out pd); // new
            MFError.ThrowExceptionForHR(hr);

            IsA(pd, typeof(IMFPresentationDescriptor).GUID);
        }

        private void TestT()
        {
            int hr;

            Guid g = typeof(CColorConvertDMO).GUID;
            Guid g2 = typeof(IClassFactory).GUID;
            IClassFactory cf;
            hr = CoGetClassObject(g, 1, IntPtr.Zero, g2, out cf);

            hr = MFExtern.MFTRegisterLocal(cf, MFTransformCategory.MFT_CATEGORY_VIDEO_EFFECT, "Mommy", MFT_EnumFlag.AsyncMFT | MFT_EnumFlag.LocalMFT, 0, null, 0, null);  // new
            MFError.ThrowExceptionForHR(hr);

            int i;
            IMFActivate[] ia;
            hr = MFExtern.MFTEnumEx(MFTransformCategory.MFT_CATEGORY_VIDEO_EFFECT, MFT_EnumFlag.AsyncMFT | MFT_EnumFlag.LocalMFT, null, null, out ia, out i);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(i == 1);

            hr = MFExtern.MFTUnregisterLocal(cf); // new
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFTEnumEx(MFTransformCategory.MFT_CATEGORY_VIDEO_EFFECT, MFT_EnumFlag.AsyncMFT | MFT_EnumFlag.LocalMFT, null, null, out ia, out i);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(i == 0);

            hr = MFExtern.MFTRegisterLocalByCLSID(g, MFTransformCategory.MFT_CATEGORY_VIDEO_EFFECT, "Daddy", MFT_EnumFlag.LocalMFT | MFT_EnumFlag.AsyncMFT, 0, null, 0, null); // new
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFTEnumEx(MFTransformCategory.MFT_CATEGORY_VIDEO_EFFECT, MFT_EnumFlag.AsyncMFT | MFT_EnumFlag.LocalMFT, null, null, out ia, out i);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(i == 1);

            hr = MFExtern.MFTUnregisterLocalByCLSID(g); // new
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFTEnumEx(MFTransformCategory.MFT_CATEGORY_VIDEO_EFFECT, MFT_EnumFlag.AsyncMFT | MFT_EnumFlag.LocalMFT, null, null, out ia, out i);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(i == 0);
        }

        private void TestU()
        {
            int hr;
            IMFAttributes imfAttributes;

            hr = MFExtern.MFCreateAttributes(out imfAttributes, 5);
            MFError.ThrowExceptionForHR(hr);

            Guid gNotFound = Guid.NewGuid();
            Guid gFound = Guid.NewGuid();

            int ia = 1020;
            int ib = 2424;

            long la = 1020;
            long lb = 2424;

            double da = 12.34;
            double db = 56.78;

            Double dRes = MFExtern.MFGetAttributeDouble(imfAttributes, gNotFound, db); // new
            Debug.Assert(dRes == db);

            hr = imfAttributes.SetDouble(gFound, da);
            MFError.ThrowExceptionForHR(hr);

            dRes = MFExtern.MFGetAttributeDouble(imfAttributes, gFound, db);
            Debug.Assert(dRes == da);

            hr = MFExtern.MFSetAttributeRatio(imfAttributes, gFound, ia, ib); // new
            MFError.ThrowExceptionForHR(hr);

            int iResa, iResb;
            hr = MFExtern.MFGetAttributeRatio(imfAttributes, gFound, out iResa, out iResb); // new
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(iResa == ia && iResb == ib);

            hr = MFExtern.MFSetAttributeSize(imfAttributes, gFound, ib, ia); // new
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFGetAttributeSize(imfAttributes, gFound, out iResa, out iResb); // new
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(iResa == ib && iResb == ia);

            hr = imfAttributes.DeleteAllItems();
            MFError.ThrowExceptionForHR(hr);

            iResa = MFExtern.MFGetAttributeUINT32(imfAttributes, gNotFound, ib); // new
            Debug.Assert(iResa == ib);

            hr = imfAttributes.SetUINT32(gFound, ia);
            MFError.ThrowExceptionForHR(hr);

            iResa = MFExtern.MFGetAttributeUINT32(imfAttributes, gFound, ib); // new
            Debug.Assert(iResa == ia);

            hr = imfAttributes.DeleteAllItems();
            MFError.ThrowExceptionForHR(hr);

            long lres = MFExtern.MFGetAttributeUINT64(imfAttributes, gNotFound, lb);
            Debug.Assert(lres == lb);

            hr = imfAttributes.SetUINT64(gFound, la);
            MFError.ThrowExceptionForHR(hr);

            lres = MFExtern.MFGetAttributeUINT64(imfAttributes, gFound, lb);
            Debug.Assert(lres == la);

            long x = MFExtern.Pack2UINT32AsUINT64(-1, -1); // new
            MFExtern.Unpack2UINT32AsUINT64(x, out iResa, out iResb); // new
            Debug.Assert(iResa == -1 && iResb == -1);

            x = MFExtern.PackSize(-1, -1); // new
            Debug.Assert(x == -1);

            MFExtern.UnpackSize(-1, out iResa, out iResb); // new
            Debug.Assert(iResa == -1 && iResb == -1);

            x = MFExtern.PackRatio(-1, -1); // new
            Debug.Assert(x == -1);

            MFExtern.UnpackRatio(-1, out iResa, out iResb); // new
            Debug.Assert(iResa == -1 && iResb == -1);

            hr = imfAttributes.SetString(gFound, "we the people");
            MFError.ThrowExceptionForHR(hr);

            string s;
            hr = MFExtern.MFGetAttributeString(imfAttributes, gFound, out s); // new
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFGetAttributeString(imfAttributes, gNotFound, out s); // new
            Debug.Assert(hr == MFError.MF_E_ATTRIBUTENOTFOUND);
        }

        private void TestW()
        {
            int hr;
            IMFRemoteDesktopPlugin imfRemoteDesktopPlugin;

            Console.WriteLine("top");

            hr = MFExtern.MFCreateRemoteDesktopPlugin(out imfRemoteDesktopPlugin); // new
            if (hr == (unchecked((int)0x80070005)))
                throw new Exception("Must be running under remote desktop");
            MFError.ThrowExceptionForHR(hr);

            Console.WriteLine(hr);

            IMFTopology imfTopology = null;
            hr = imfRemoteDesktopPlugin.UpdateTopology(imfTopology);
            MFError.ThrowExceptionForHR(hr);
            Console.WriteLine(hr);
        }

        AutoResetEvent are = new AutoResetEvent(false);
        AutoResetEvent are2 = new AutoResetEvent(false);
        int m_Hits = 0;
        int m_Hits2 = 0;
        int m_dwTaskId;
        int m_testno = 0;
        IMFSchemeHandler m_imfSchemeHandler;

        #region Non-MF externs

        [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = true)]
        public static extern int CoGetClassObject(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid clsid,
            int dwContext,
            IntPtr serverInfo,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid xclsid,
            out IClassFactory cf);

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

        [StructLayout(LayoutKind.Sequential)]
        public class AMMediaType
        {
            public Guid majorType;
            public Guid subType;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fixedSizeSamples;
            [MarshalAs(UnmanagedType.Bool)]
            public bool temporalCompression;
            public int sampleSize;
            public Guid formatType;
            public IntPtr unkPtr; // IUnknown Pointer
            public int formatSize;
            public IntPtr formatPtr; // Pointer to a buff determined by formatType
        }

        #endregion

        #region Helpers

        private void FillBMI(BitmapInfoHeader w1)
        {
            w1.Size = Marshal.SizeOf(typeof(BitmapInfoHeader));
            w1.ClrUsed = 0;
            w1.BitCount = 32;
            w1.Compression = 0;

            w1.ClrImportant = 1;
            w1.Height = 2;
            w1.Planes = (short)(3);
            w1.ImageSize = 4;
            w1.Width = 5;
            w1.XPelsPerMeter = 6;
            w1.YPelsPerMeter = 7;
        }

        private void IsA(object a, Guid g)
        {
            IntPtr ppv;
            IntPtr p = Marshal.GetIUnknownForObject(a);
            int hr = Marshal.QueryInterface(p, ref g, out ppv);
            MFError.ThrowExceptionForHR(hr);
            Marshal.Release(p);
            Marshal.Release(p);
        }

        private IMFMediaSource GetMediaSource()
        {
            IMFMediaSource pSource;
            IMFSourceResolver sr;
            MFObjectType pObjectType;
            object o;

            int hr = MFExtern.MFCreateSourceResolver(out sr);
            MFError.ThrowExceptionForHR(hr);

            hr = sr.CreateObjectFromURL(
                FILENAME1,
                MFResolution.MediaSource,
                null,
                out pObjectType,
                out o);
            MFError.ThrowExceptionForHR(hr);

            pSource = o as IMFMediaSource;

            return pSource;
        }

        private IMFPresentationDescriptor GetInterface()
        {
            IMFMediaSource pSource = GetMediaSource();

            IMFPresentationDescriptor pd;
            int hr;

            hr = pSource.CreatePresentationDescriptor(out pd);
            MFError.ThrowExceptionForHR(hr);

            return pd;
        }

        private void MyMethod([MarshalAs(UnmanagedType.IUnknown)] object asdf)
        {
            m_Hits2++;
            are2.Set();
        }

        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwQueue = MFAsyncCallbackQueue.Undefined;
            pdwFlags = MFASync.None;

            return E_NotImplemented;
        }

        public int Invoke(IMFAsyncResult pAsyncResult)
        {
            int hr;

            hr = pAsyncResult.SetStatus(7);
            MFError.ThrowExceptionForHR(hr);

            m_Hits++;

            switch (m_testno)
            {
                case 1:
                    hr = MFExtern.MFEndRegisterWorkQueueWithMMCSS(pAsyncResult, out m_dwTaskId); // new
                    MFError.ThrowExceptionForHR(hr);
                    break;
                case 2:
                    hr = MFExtern.MFEndUnregisterWorkQueueWithMMCSS(pAsyncResult); // new
                    MFError.ThrowExceptionForHR(hr);
                    break;
                case 3:
                    IMFAsyncResult imfAsyncResult;
                    object objState = null;

                    hr = MFExtern.MFCreateAsyncResult(objState, null, objState, out imfAsyncResult);
                    MFError.ThrowExceptionForHR(hr);

                    MFObjectType mf;
                    object o;
                    hr = m_imfSchemeHandler.EndCreateObject(imfAsyncResult, out mf, out o);
                    break;
            }

            bool b = are.Set();
            Debug.Assert(b);

            return hr;
        }

        #endregion

        public int GetItem(Guid guidKey, PropVariant pValue)
        {
            throw new NotImplementedException();
        }

        public int GetItemType(Guid guidKey, out MFAttributeType pType)
        {
            throw new NotImplementedException();
        }

        public int CompareItem(Guid guidKey, ConstPropVariant Value, out bool pbResult)
        {
            throw new NotImplementedException();
        }

        public int Compare(IMFAttributes pTheirs, MFAttributesMatchType MatchType, out bool pbResult)
        {
            throw new NotImplementedException();
        }

        public int GetUINT32(Guid guidKey, out int punValue)
        {
            throw new NotImplementedException();
        }

        public int GetUINT64(Guid guidKey, out long punValue)
        {
            throw new NotImplementedException();
        }

        public int GetDouble(Guid guidKey, out double pfValue)
        {
            throw new NotImplementedException();
        }

        public int GetGUID(Guid guidKey, out Guid pguidValue)
        {
            throw new NotImplementedException();
        }

        public int GetStringLength(Guid guidKey, out int pcchLength)
        {
            throw new NotImplementedException();
        }

        public int GetString(Guid guidKey, StringBuilder pwszValue, int cchBufSize, out int pcchLength)
        {
            throw new NotImplementedException();
        }

        public int GetAllocatedString(Guid guidKey, out string ppwszValue, out int pcchLength)
        {
            throw new NotImplementedException();
        }

        public int GetBlobSize(Guid guidKey, out int pcbBlobSize)
        {
            throw new NotImplementedException();
        }

        public int GetBlob(Guid guidKey, byte[] pBuf, int cbBufSize, out int pcbBlobSize)
        {
            throw new NotImplementedException();
        }

        public int GetAllocatedBlob(Guid guidKey, out IntPtr ip, out int pcbSize)
        {
            throw new NotImplementedException();
        }

        public int GetUnknown(Guid guidKey, Guid riid, out object ppv)
        {
            throw new NotImplementedException();
        }

        public int SetItem(Guid guidKey, ConstPropVariant Value)
        {
            throw new NotImplementedException();
        }

        public int DeleteItem(Guid guidKey)
        {
            throw new NotImplementedException();
        }

        public int DeleteAllItems()
        {
            throw new NotImplementedException();
        }

        public int SetUINT32(Guid guidKey, int unValue)
        {
            throw new NotImplementedException();
        }

        public int SetUINT64(Guid guidKey, long unValue)
        {
            throw new NotImplementedException();
        }

        public int SetDouble(Guid guidKey, double fValue)
        {
            throw new NotImplementedException();
        }

        public int SetGUID(Guid guidKey, Guid guidValue)
        {
            throw new NotImplementedException();
        }

        public int SetString(Guid guidKey, string wszValue)
        {
            throw new NotImplementedException();
        }

        public int SetBlob(Guid guidKey, byte[] pBuf, int cbBufSize)
        {
            throw new NotImplementedException();
        }

        public int SetUnknown(Guid guidKey, object pUnknown)
        {
            throw new NotImplementedException();
        }

        public int LockStore()
        {
            throw new NotImplementedException();
        }

        public int UnlockStore()
        {
            throw new NotImplementedException();
        }

        public int GetCount(out int pcItems)
        {
            throw new NotImplementedException();
        }

        public int GetItemByIndex(int unIndex, out Guid pguidKey, PropVariant pValue)
        {
            throw new NotImplementedException();
        }

        public int CopyAllItems(IMFAttributes pDest)
        {
            throw new NotImplementedException();
        }

        public int ActivateObject(Guid riid, out object ppv)
        {
            throw new NotImplementedException();
        }

        public int ShutdownObject()
        {
            throw new NotImplementedException();
        }

        public int DetachObject()
        {
            throw new NotImplementedException();
        }
    }

    public class IStreamWrapper : IStream, IDisposable
    {
        public IStreamWrapper(string file)
        {
            if (file == null)
                throw new ArgumentNullException("stream", "Can't wrap null stream.");
            this.stream = new FileStream(file, FileMode.Open);
        }

        public IStreamWrapper(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream", "Can't wrap null stream.");
            this.stream = stream;
        }

        Stream stream;

        public void Clone(out System.Runtime.InteropServices.ComTypes.IStream ppstm)
        {
            throw new Exception("not implemented");
        }

        public void Commit(int grfCommitFlags) 
        {
            throw new Exception("not implemented");
        }

        public void CopyTo(System.Runtime.InteropServices.ComTypes.IStream pstm,
          long cb, System.IntPtr pcbRead, System.IntPtr pcbWritten)
        {
            throw new Exception("not implemented");
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new Exception("not implemented");
        }

        public void Read(byte[] pv, int cb, System.IntPtr pcbRead)
        {
            if (pcbRead != IntPtr.Zero)
                Marshal.WriteInt64(pcbRead, (Int64)stream.Read(pv, 0, cb));
            else
                stream.Read(pv, 0, cb);
        }

        public void Revert()
        {
            throw new Exception("not implemented");
        }

        public void Seek(long dlibMove, int dwOrigin, System.IntPtr plibNewPosition)
        {
            if (plibNewPosition != IntPtr.Zero)
                Marshal.WriteInt64(plibNewPosition, stream.Seek(dlibMove, (SeekOrigin)dwOrigin));
            else
                stream.Seek(dlibMove, (SeekOrigin)dwOrigin);
        }

        public void SetSize(long libNewSize)
        {
            throw new Exception("not implemented");
        }

        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG();

            pstatstg.cbSize = stream.Length;
        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new Exception("not implemented");
        }

        public void Write(byte[] pv, int cb, System.IntPtr pcbWritten)
        {
            throw new Exception("not implemented");
        }


        public void Dispose()
        {
            stream.Close();
            stream = null;
        }
    }
}
