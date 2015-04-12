using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv22
{
    [ComVisible(true)]
    public class IMFWorkQueueServicesTest : COMBase, IMFSampleGrabberSinkCallback, IMFAsyncCallback
    {
        int m_didit = 0;
        int WORKQUEUE_ID;
        const string WORKQUEUE_MMCSS_CLASS = "audio"; // See HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Task
        IMFWorkQueueServices m_workQueueServices;
        int m_platformWorkQueueTaskId = -1;
        ManualResetEvent m_registerPlatformWorkQueue = new ManualResetEvent(false);
        ManualResetEvent m_unregisterPlatformWorkQueue = new ManualResetEvent(false);
        ManualResetEvent m_registerTopologyWorkQueues = new ManualResetEvent(false);
        ManualResetEvent m_unregisterTopologyWorkQueues = new ManualResetEvent(false);

        public void DoTests()
        {
            GetInterface(@"c:\sourceforge\mflib\test\media\AspectRatio4x3.wmv");

            TestPlatformWorkQueueWithMMCSS();
            TestTopologyWorkQueuesWithMMCSS();

            Debug.Assert(m_didit == 4095);
        }

        // Create a media source from a URL.
        private void CreateMediaSource(string pszURL, out IMFMediaSource ppSource)
        {
            IMFSourceResolver pSourceResolver = null;
            object pSource;

            int hr;
            // Create the source resolver.
            hr = MFExtern.MFCreateSourceResolver(out pSourceResolver);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType ObjectType;
            hr = pSourceResolver.CreateObjectFromURL(
                pszURL,
                MFResolution.MediaSource,
                null,
                out ObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            ppSource = (IMFMediaSource)pSource;

            //done:
            //SafeRelease(&pSourceResolver);
            //SafeRelease(&pSource);
            //return hr;
        }
        private void GetInterface(string pszFileName)
        {
            IMFMediaSession pSession = null;
            IMFMediaSource pSource = null;
            IMFActivate pSinkActivate = null;
            IMFTopology pTopology = null;
            IMFMediaType pType = null;
            int hr;

            hr = MFExtern.MFAllocateWorkQueueEx(MFASYNC_WORKQUEUE_TYPE.WindowWorkqueue, out WORKQUEUE_ID);
            MFError.ThrowExceptionForHR(hr);

            // Configure the media type that the Sample Grabber will receive.
            // Setting the major and subtype is usually enough for the topology loader
            // to resolve the topology.

            hr = MFExtern.MFCreateMediaType(out pType);
            MFError.ThrowExceptionForHR(hr);
            hr = pType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Audio);
            MFError.ThrowExceptionForHR(hr);
            hr = pType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.PCM);
            MFError.ThrowExceptionForHR(hr);

            // Create the sample grabber sink.
            //pCallback = this;
            hr = MFExtern.MFCreateSampleGrabberSinkActivate(pType, this, out pSinkActivate);
            MFError.ThrowExceptionForHR(hr);

            // To run as fast as possible, set this attribute (requires Windows 7):
            hr = pSinkActivate.SetUINT32(MFAttributesClsid.MF_SAMPLEGRABBERSINK_IGNORE_CLOCK, 1);
            MFError.ThrowExceptionForHR(hr);

            // Create the Media Session.
            hr = MFExtern.MFCreateMediaSession(null, out pSession);
            MFError.ThrowExceptionForHR(hr);

            // Create the media source.
            CreateMediaSource(pszFileName, out pSource);
            MFError.ThrowExceptionForHR(hr);

            // Create the topology.
            CreateTopology(pSource, pSinkActivate, out pTopology);

            hr = pSession.SetTopology(0, pTopology);
            MFError.ThrowExceptionForHR(hr);

            object service;

            hr = (pSession as IMFGetService).GetService(MFServices.MF_WORKQUEUE_SERVICES, typeof(IMFWorkQueueServices).GUID, out service);
            MFError.ThrowExceptionForHR(hr);

            m_workQueueServices = (IMFWorkQueueServices)service;
        }

        private void TestPlatformWorkQueueWithMMCSS()
        {
            int hr = 0;

            // ***** Register Platform WorkQueue
            hr = m_workQueueServices.BeginRegisterPlatformWorkQueueWithMMCSS((MFAsyncCallbackQueue)WORKQUEUE_ID, WORKQUEUE_MMCSS_CLASS, 0, this, new AsyncState("IMFWorkQueueServices.BeginRegisterPlatformWorkQueueWithMMCSS"));
            MFError.ThrowExceptionForHR(hr);
            m_didit |= 1;

            // Wait for the end of the Async code (see Invoke method)
            bool registerDone = m_registerPlatformWorkQueue.WaitOne(TimeSpan.FromSeconds(5.0));
            Debug.Assert(registerDone, "RegisterPlatformWorkQueueWithMMCSS not done !");

            // ***** Get Class Name
            int classNameSize = 0;
            hr = m_workQueueServices.GetPlaftormWorkQueueMMCSSClass((MFAsyncCallbackQueue)WORKQUEUE_ID, null, ref classNameSize);
            if (hr == MFError.MF_E_BUFFERTOOSMALL || hr == COMBase.E_Pointer)
            {
                // GetPlaftormWorkQueueMMCSSClass is documented to accept NULL pointer for the pwszClass parameter but that's not true.
                if (classNameSize == 0) classNameSize = 255;

                StringBuilder classNameBuilder = new StringBuilder(classNameSize);
                hr = m_workQueueServices.GetPlaftormWorkQueueMMCSSClass((MFAsyncCallbackQueue)WORKQUEUE_ID, classNameBuilder, ref classNameSize);
                MFError.ThrowExceptionForHR(hr);

                m_didit |= 2;
                Debug.Assert(WORKQUEUE_MMCSS_CLASS.Equals(classNameBuilder.ToString()));
            }

            // ***** Get TaskID
            int taskID;
            hr = m_workQueueServices.GetPlatformWorkQueueMMCSSTaskId((MFAsyncCallbackQueue)WORKQUEUE_ID, out taskID);
            MFError.ThrowExceptionForHR(hr);
            m_didit |= 4;

            Debug.Assert(m_platformWorkQueueTaskId == taskID);

            // ***** Unregister Platform WorkQueue
            hr = m_workQueueServices.BeginUnregisterPlatformWorkQueueWithMMCSS((MFAsyncCallbackQueue)WORKQUEUE_ID, this, new AsyncState("IMFWorkQueueServices.BeginUnregisterPlatformWorkQueueWithMMCSS"));
            MFError.ThrowExceptionForHR(hr);

            m_didit |= 8;

            // Wait for the end of the Async code (see Invoke method)
            bool unregisterDone = m_unregisterPlatformWorkQueue.WaitOne(TimeSpan.FromSeconds(5.0));
            Debug.Assert(unregisterDone, "UnregisterPlatformWorkQueueWithMMCSS not done !");
        }

        private void TestTopologyWorkQueuesWithMMCSS()
        {
            int hr = 0;

            hr = m_workQueueServices.BeginRegisterTopologyWorkQueuesWithMMCSS(this, new AsyncState("IMFWorkQueueServices.BeginRegisterTopologyWorkQueuesWithMMCSS"));
            MFError.ThrowExceptionForHR(hr);

            m_didit |= 16;

            // Wait for the end of the Async code (see Invoke method)
            bool registerDone = m_registerTopologyWorkQueues.WaitOne(TimeSpan.FromSeconds(5.0));
            Debug.Assert(registerDone, "RegisterTopologyWorkQueuesWithMMCSS not done !");

            // ***** Get Class Name
            int classNameSize = 0;
            hr = m_workQueueServices.GetTopologyWorkQueueMMCSSClass((MFAsyncCallbackQueue)WORKQUEUE_ID, null, ref classNameSize);
            if (hr == MFError.MF_E_BUFFERTOOSMALL || hr == COMBase.E_Pointer)
            {
                // GetPlaftormWorkQueueMMCSSClass is documented to accept NULL pointer for the pwszClass parameter but that's not true.
                if (classNameSize == 0) classNameSize = 255;

                StringBuilder classNameBuilder = new StringBuilder(classNameSize);
                hr = m_workQueueServices.GetPlaftormWorkQueueMMCSSClass((MFAsyncCallbackQueue)WORKQUEUE_ID, classNameBuilder, ref classNameSize);
                MFError.ThrowExceptionForHR(hr);

                m_didit |= 32;

                Debug.Assert(WORKQUEUE_MMCSS_CLASS.Equals(classNameBuilder.ToString()));
            }

            // ***** Get TaskID
            int taskID;
            hr = m_workQueueServices.GetTopologyWorkQueueMMCSSTaskId((MFAsyncCallbackQueue)WORKQUEUE_ID, out taskID);
            MFError.ThrowExceptionForHR(hr);

            m_didit |= 64;

            Debug.Assert(m_platformWorkQueueTaskId + 1 == taskID);

            hr = m_workQueueServices.BeginUnregisterTopologyWorkQueuesWithMMCSS(this, new AsyncState("IMFWorkQueueServices.BeginUnregisterTopologyWorkQueuesWithMMCSS"));
            MFError.ThrowExceptionForHR(hr);

            m_didit |= 128;

            // Wait for the end of the Async code (see Invoke method)
            bool unregisterDone = m_registerTopologyWorkQueues.WaitOne(TimeSpan.FromSeconds(5.0));
            Debug.Assert(unregisterDone, "UnregisterTopologyWorkQueuesWithMMCSS not done !");
        }
        private void AddSourceNode(
            IMFTopology pTopology,           // Topology.
            IMFMediaSource pSource,          // Media source.
            IMFPresentationDescriptor pPD,   // Presentation descriptor.
            IMFStreamDescriptor pSD,         // Stream descriptor.
            out IMFTopologyNode ppNode)         // Receives the node pointer.
        {
            IMFTopologyNode pNode = null;

            int hr = S_Ok;
            hr = MFExtern.MFCreateTopologyNode(MFTopologyType.SourcestreamNode, out pNode);
            MFError.ThrowExceptionForHR(hr);

            hr = pNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_SOURCE, pSource);
            MFError.ThrowExceptionForHR(hr);

            hr = pNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_PRESENTATION_DESCRIPTOR, pPD);
            MFError.ThrowExceptionForHR(hr);

            hr = pNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_STREAM_DESCRIPTOR, pSD);
            MFError.ThrowExceptionForHR(hr);

            // Need to test IMFWorkQueueServices
            hr = pNode.SetUINT32(MFAttributesClsid.MF_TOPONODE_WORKQUEUE_ID, WORKQUEUE_ID);
            MFError.ThrowExceptionForHR(hr);

            hr = pNode.SetString(MFAttributesClsid.MF_TOPONODE_WORKQUEUE_MMCSS_CLASS, WORKQUEUE_MMCSS_CLASS);
            MFError.ThrowExceptionForHR(hr);

            hr = pTopology.AddNode(pNode);
            MFError.ThrowExceptionForHR(hr);

            // Return the pointer to the caller.
            ppNode = pNode;
            //(ppNode).AddRef();

            //done:
            //SafeRelease(pNode);
        }

        // Add an output node to a topology.
        void AddOutputNode(
            IMFTopology pTopology,     // Topology.
            IMFActivate pActivate,     // Media sink activation object.
            int dwId,                 // Identifier of the stream sink.
            out IMFTopologyNode ppNode)   // Receives the node pointer.
        {
            IMFTopologyNode pNode = null;

            int hr = S_Ok;
            hr = MFExtern.MFCreateTopologyNode(MFTopologyType.OutputNode, out pNode);
            MFError.ThrowExceptionForHR(hr);

            hr = pNode.SetObject(pActivate);
            MFError.ThrowExceptionForHR(hr);

            hr = pNode.SetUINT32(MFAttributesClsid.MF_TOPONODE_STREAMID, dwId);
            MFError.ThrowExceptionForHR(hr);

            hr = pNode.SetUINT32(MFAttributesClsid.MF_TOPONODE_NOSHUTDOWN_ON_REMOVE, 0);
            MFError.ThrowExceptionForHR(hr);

            hr = pTopology.AddNode(pNode);
            MFError.ThrowExceptionForHR(hr);

            // Return the pointer to the caller.
            ppNode = pNode;
            //(ppNode).AddRef();

            //done:
            //SafeRelease(pNode);
        }
        private void CreateTopology(IMFMediaSource pSource, IMFActivate pSinkActivate, out IMFTopology ppTopo)
        {
            IMFTopology pTopology = null;
            IMFPresentationDescriptor pPD = null;
            IMFStreamDescriptor pSD = null;
            IMFMediaTypeHandler pHandler = null;
            IMFTopologyNode pNode1 = null;
            IMFTopologyNode pNode2 = null;

            int hr = S_Ok;
            int cStreams = 0;

            hr = MFExtern.MFCreateTopology(out pTopology);
            MFError.ThrowExceptionForHR(hr);

            hr = pSource.CreatePresentationDescriptor(out pPD);
            MFError.ThrowExceptionForHR(hr);

            hr = pPD.GetStreamDescriptorCount(out cStreams);
            MFError.ThrowExceptionForHR(hr);

            for (int i = 0; i < cStreams; i++)
            {
                // In this example, we look for audio streams and connect them to the sink.

                bool fSelected = false;
                Guid majorType;

                hr = pPD.GetStreamDescriptorByIndex(i, out fSelected, out pSD);
                MFError.ThrowExceptionForHR(hr);

                hr = pSD.GetMediaTypeHandler(out pHandler);
                MFError.ThrowExceptionForHR(hr);

                hr = pHandler.GetMajorType(out majorType);
                MFError.ThrowExceptionForHR(hr);


                if (majorType == MFMediaType.Audio && fSelected)
                {
                    AddSourceNode(pTopology, pSource, pPD, pSD, out pNode1);
                    AddOutputNode(pTopology, pSinkActivate, 0, out pNode2);

                    hr = pNode1.ConnectOutput(0, pNode2, 0);
                    MFError.ThrowExceptionForHR(hr);

                    break;
                }
                else
                {
                    hr = pPD.DeselectStream(i);
                    MFError.ThrowExceptionForHR(hr);

                }
                //SafeRelease(pSD);
                //SafeRelease(pHandler);
            }

            ppTopo = pTopology;

            //done:
            ;
            //SafeRelease(pTopology);
            //SafeRelease(pNode1);
            //SafeRelease(pNode2);
            //SafeRelease(pPD);
            //SafeRelease(pSD);
            //SafeRelease(pHandler);
            //return hr;
        }

        #region IMFClockStateSink methods

        public int OnClockStart(long hnsSystemTime, long llClockStartOffset)
        {
            return S_Ok;
        }

        public int OnClockStop(long hnsSystemTime)
        {
            return S_Ok;
        }

        public int OnClockPause(long hnsSystemTime)
        {
            return S_Ok;
        }

        public int OnClockRestart(long hnsSystemTime)
        {
            return S_Ok;
        }

        public int OnClockSetRate(long hnsSystemTime, float flRate)
        {
            return S_Ok;
        }

        #endregion

        #region IMFSampleGrabberSinkCallback

        public int OnSetPresentationClock(IMFPresentationClock pPresentationClock)
        {
            if (pPresentationClock != null)
            {
                long t;
                int hr;

                hr = pPresentationClock.GetTime(out t);
                MFError.ThrowExceptionForHR(hr);
            }

            return S_Ok;
        }

        public int OnProcessSample(Guid guidMajorMediaType, int dwSampleFlags, long llSampleTime, long llSampleDuration, IntPtr pSampleBuffer, int dwSampleSize)
        {
            return S_Ok;
        }

        public int OnShutdown()
        {
            return S_Ok;
        }

        #endregion

        #region IMFAsyncCallback

        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwFlags = MFASync.None;
            pdwQueue = MFAsyncCallbackQueue.Undefined;

            return E_NotImplemented;
        }

        public int Invoke(IMFAsyncResult pAsyncResult)
        {
            int hr = 0;
            object tmp;

            do
            {
                hr = pAsyncResult.GetState(out tmp);
                if (hr < 0) break;

                AsyncState asyncState = (AsyncState)tmp;

                #region Code piece used by GetInterface()
                if (asyncState.Token == "IMFMediaSession.BeginGetEvent")
                {
                }
                #endregion

                #region Callback code for IMFWorkQueueServices.BeginRegisterPlatformWorkQueueWithMMCSS
                else if (asyncState.Token == "IMFWorkQueueServices.BeginRegisterPlatformWorkQueueWithMMCSS")
                {
                    hr = m_workQueueServices.EndRegisterPlatformWorkQueueWithMMCSS(pAsyncResult, out m_platformWorkQueueTaskId);
                    if (hr == 0)
                        m_didit |= 256;

                    m_registerPlatformWorkQueue.Set();
                }
                #endregion

                #region Callback code for IMFWorkQueueServices.BeginUnregisterPlatformWorkQueueWithMMCSS
                else if (asyncState.Token == "IMFWorkQueueServices.BeginUnregisterPlatformWorkQueueWithMMCSS")
                {
                    hr = m_workQueueServices.EndUnregisterPlatformWorkQueueWithMMCSS(pAsyncResult);
                    if (hr == 0)
                        m_didit |= 512;
                    m_unregisterPlatformWorkQueue.Set();
                }
                #endregion

                #region Callback code for IMFWorkQueueServices.BeginRegisterTopologyWorkQueuesWithMMCSS
                else if (asyncState.Token == "IMFWorkQueueServices.BeginRegisterTopologyWorkQueuesWithMMCSS")
                {
                    hr = m_workQueueServices.EndRegisterTopologyWorkQueuesWithMMCSS(pAsyncResult);
                    if (hr == 0)
                        m_didit |= 1024;
                    m_registerTopologyWorkQueues.Set();
                }
                #endregion

                #region Callback code for IMFWorkQueueServices.BeginUnregisterTopologyWorkQueuesWithMMCSS
                else if (asyncState.Token == "IMFWorkQueueServices.BeginUnregisterTopologyWorkQueuesWithMMCSS")
                {
                    hr = m_workQueueServices.EndUnregisterTopologyWorkQueuesWithMMCSS(pAsyncResult);
                    if (hr == 0)
                        m_didit |= 2048;
                    m_registerTopologyWorkQueues.Set();
                }
                #endregion
                else
                {
                    hr = COMBase.E_Fail;
                    break;
                }

            }
            while (false);

            Marshal.ReleaseComObject(pAsyncResult);

            return hr;
        }

        #endregion
    }



    [ComVisible(true)]
    public class AsyncState
    {
        private string m_token;

        public string Token
        {
            get { return m_token; }
            set { m_token = value; }
        }

        public AsyncState(string state)
        {
            m_token = state;
        }
    }
}
