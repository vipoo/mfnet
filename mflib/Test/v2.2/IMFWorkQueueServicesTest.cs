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
    public class IMFWorkQueueServicesTest : IMFAsyncCallback
    {
        const int WORKQUEUE_ID = 0x5a5a5a5a;
        const string WORKQUEUE_MMCSS_CLASS = "Playback"; // See HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Task

        IMFMediaSession m_mediaSession = null;
        IMFWorkQueueServices m_workQueueServices;

        int m_platformWorkQueueTaskId = -1;

        ManualResetEvent m_topologyReady = new ManualResetEvent(false);
        ManualResetEvent m_registerPlatformWorkQueue = new ManualResetEvent(false);
        ManualResetEvent m_unregisterPlatformWorkQueue = new ManualResetEvent(false);
        ManualResetEvent m_registerTopologyWorkQueues = new ManualResetEvent(false);
        ManualResetEvent m_unregisterTopologyWorkQueues = new ManualResetEvent(false);


        public void DoTests()
        {
            GetInterface(@"..\..\..\Media\AspectRatio4x3.wmv");

            //TestPlatformWorkQueueWithMMCSS();
            TestTopologyWorkQueuesWithMMCSS();
        }

        private void TestPlatformWorkQueueWithMMCSS()
        {
            int hr = 0;
            string platformWorkQueueClassName = "MFLibTest1ClassName";

            // ***** Register Platform WorkQueue
            hr = m_workQueueServices.BeginRegisterPlatformWorkQueueWithMMCSS(MFAsyncCallbackQueue.Standard, platformWorkQueueClassName, 0, this, new AsyncState("IMFWorkQueueServices.BeginRegisterPlatformWorkQueueWithMMCSS"));
            MFError.ThrowExceptionForHR(hr);

            // Wait for the end of the Async code (see Invoke method)
            bool registerDone = m_registerPlatformWorkQueue.WaitOne(TimeSpan.FromSeconds(5.0));
            Debug.Assert(registerDone, "RegisterPlatformWorkQueueWithMMCSS not done !");

            // ***** Get Class Name
            int classNameSize = 0;
            hr = m_workQueueServices.GetPlaftormWorkQueueMMCSSClass(MFAsyncCallbackQueue.Standard, null, ref classNameSize);
            if (hr == MFError.MF_E_BUFFERTOOSMALL || hr == COMBase.E_Pointer)
            {
                // GetPlaftormWorkQueueMMCSSClass is documented to accept NULL pointer for the pwszClass parameter but that's not true.
                if (classNameSize == 0) classNameSize = 255;

                StringBuilder classNameBuilder = new StringBuilder(classNameSize);
                hr = m_workQueueServices.GetPlaftormWorkQueueMMCSSClass(MFAsyncCallbackQueue.Standard, classNameBuilder, ref classNameSize);
                MFError.ThrowExceptionForHR(hr);

                Debug.Assert(platformWorkQueueClassName.Equals(classNameBuilder.ToString()));
            }

            // ***** Get TaskID
            int taskID;
            hr = m_workQueueServices.GetPlatformWorkQueueMMCSSTaskId(MFAsyncCallbackQueue.Standard, out taskID);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(m_platformWorkQueueTaskId == taskID);

            // ***** Unregister Platform WorkQueue
            hr = m_workQueueServices.BeginUnregisterPlatformWorkQueueWithMMCSS(MFAsyncCallbackQueue.Standard, this, new AsyncState("IMFWorkQueueServices.BeginUnregisterPlatformWorkQueueWithMMCSS"));
            MFError.ThrowExceptionForHR(hr);

            // Wait for the end of the Async code (see Invoke method)
            bool unregisterDone = m_unregisterPlatformWorkQueue.WaitOne(TimeSpan.FromSeconds(5.0));
            Debug.Assert(unregisterDone, "UnregisterPlatformWorkQueueWithMMCSS not done !");
        }

        private void TestTopologyWorkQueuesWithMMCSS()
        {
            int hr = 0;

            hr = m_workQueueServices.BeginRegisterTopologyWorkQueuesWithMMCSS(this, new AsyncState("IMFWorkQueueServices.BeginRegisterTopologyWorkQueuesWithMMCSS"));
            MFError.ThrowExceptionForHR(hr);

            // Wait for the end of the Async code (see Invoke method)
            bool registerDone = m_registerTopologyWorkQueues.WaitOne(TimeSpan.FromSeconds(5.0));
            Debug.Assert(registerDone, "RegisterTopologyWorkQueuesWithMMCSS not done !");

            hr = m_workQueueServices.BeginUnregisterTopologyWorkQueuesWithMMCSS(this, new AsyncState("IMFWorkQueueServices.BeginUnregisterTopologyWorkQueuesWithMMCSS"));
            MFError.ThrowExceptionForHR(hr);

            // Wait for the end of the Async code (see Invoke method)
            bool unregisterDone = m_registerTopologyWorkQueues.WaitOne(TimeSpan.FromSeconds(5.0));
            Debug.Assert(unregisterDone, "UnregisterTopologyWorkQueuesWithMMCSS not done !");
        }


        private void GetInterface(string fileName)
        {
            IMFSourceResolver sourceResolver = null;
            IMFMediaSource mediaSource = null;
            IMFPresentationDescriptor pd = null; 
            IMFTopology topology = null;

            int hr;

            // Create the Media Session.
            hr = MFExtern.MFCreateMediaSession(null, out m_mediaSession);
            MFError.ThrowExceptionForHR(hr);

            // Create the Media Source from the Source Resolver
            hr = MFExtern.MFCreateSourceResolver(out sourceResolver);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType objectType = MFObjectType.Invalid;
            object tmp;

            hr = sourceResolver.CreateObjectFromURL(fileName, MFResolution.MediaSource, null, out objectType, out tmp);
            MFError.ThrowExceptionForHR(hr);

            mediaSource = (IMFMediaSource)tmp;

            hr = mediaSource.CreatePresentationDescriptor(out pd);
            MFError.ThrowExceptionForHR(hr);

            // Create the Topography from the Media Source
            hr = MFExtern.MFCreateTopology(out topology);
            MFError.ThrowExceptionForHR(hr);

            int descriptorCount;

            hr = pd.GetStreamDescriptorCount(out descriptorCount);
            MFError.ThrowExceptionForHR(hr);

            for (int i = 0; i < descriptorCount; i++)
            {
                bool selected;
                IMFStreamDescriptor sd;

                hr = pd.GetStreamDescriptorByIndex(i, out selected, out sd);
                MFError.ThrowExceptionForHR(hr);

                if (selected)
                {
                    hr = AddSourceNode(topology, mediaSource, pd, sd);
                    MFError.ThrowExceptionForHR(hr);

                    hr = AddOutputNode(topology, sd);
                    MFError.ThrowExceptionForHR(hr);
                }
            }

            hr = m_mediaSession.BeginGetEvent(this, new AsyncState("IMFMediaSession.BeginGetEvent"));
            MFError.ThrowExceptionForHR(hr);

            hr = m_mediaSession.SetTopology(MFSessionSetTopologyFlags.Immediate, topology);
            MFError.ThrowExceptionForHR(hr);

            // Wait the arrival of the MF_TOPOSTATUS_READY status (see the Invoke method)
            //m_topologyReady.WaitOne();

            object service;

            hr = (m_mediaSession as IMFGetService).GetService(MFServices.MF_WORKQUEUE_SERVICES, typeof(IMFWorkQueueServices).GUID, out service);
            MFError.ThrowExceptionForHR(hr);

            m_workQueueServices = (IMFWorkQueueServices)service;
        }

        private int AddSourceNode(IMFTopology topology, IMFMediaSource mediaSource, IMFPresentationDescriptor pd, IMFStreamDescriptor sd)
        {
            int hr = 0;
            IMFTopologyNode topoNode = null;

            do
            {
                hr = MFExtern.MFCreateTopologyNode(MFTopologyType.SourcestreamNode, out topoNode);
                if (hr < 0) break;

                hr = topoNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_SOURCE, mediaSource);
                if (hr < 0) break;

                hr = topoNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_PRESENTATION_DESCRIPTOR, pd);
                if (hr < 0) break;

                hr = topoNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_STREAM_DESCRIPTOR, sd);
                if (hr < 0) break;

                // Need to test IMFWorkQueueServices
                hr = topoNode.SetUINT32(MFAttributesClsid.MF_TOPONODE_WORKQUEUE_ID, WORKQUEUE_ID);
                if (hr < 0) break;

                hr = topoNode.SetString(MFAttributesClsid.MF_TOPONODE_WORKQUEUE_MMCSS_CLASS, WORKQUEUE_MMCSS_CLASS);
                if (hr < 0) break;

                hr = topology.AddNode(topoNode);
                if (hr < 0) break;
            }
            while (false);

            if (topoNode != null) Marshal.ReleaseComObject(topoNode);

            return hr;
        }

        private int AddOutputNode(IMFTopology topology, IMFStreamDescriptor sd)
        {
            int hr = 0;
            Guid majorType;
            IMFMediaTypeHandler mediaTypeHandler = null;
            IMFMediaSink mediaSink = null;
            IMFTopologyNode topoNode = null;

            do
            {
                hr = sd.GetMediaTypeHandler(out mediaTypeHandler);
                if (hr < 0) break;

                hr = mediaTypeHandler.GetMajorType(out majorType);
                if (hr < 0) break;

                if (majorType == MFMediaType.Audio)
                {
                    hr = MFExtern.MFCreateAudioRenderer(null, out mediaSink);
                    if (hr < 0) break;
                }
                else if (majorType == MFMediaType.Video)
                {
                    object tmp;

                    hr = MFExtern.MFCreateVideoRenderer(typeof(IMFMediaSink).GUID, out tmp);
                    if (hr < 0) break;

                    mediaSink = (IMFMediaSink)tmp;
                }
                else
                {
                    hr = COMBase.E_Fail;
                    break;
                }

                hr = MFExtern.MFCreateTopologyNode(MFTopologyType.OutputNode, out topoNode);
                if (hr < 0) break;

                hr = topoNode.SetObject(mediaSink);
                if (hr < 0) break;

                hr = topology.AddNode(topoNode);
                if (hr < 0) break;
            }
            while (false);

            if (mediaTypeHandler != null) Marshal.ReleaseComObject(mediaTypeHandler);
            if (mediaSink != null) Marshal.ReleaseComObject(mediaSink);
            if (topoNode != null) Marshal.ReleaseComObject(topoNode);

            return hr;
        }

        #region IMFAsyncCallback Members

        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwFlags = default(MFASync);
            pdwQueue = default(MFAsyncCallbackQueue);
            return COMBase.E_NotImplemented;
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
                    IMFMediaEvent mediaEvent;

                    hr = m_mediaSession.EndGetEvent(pAsyncResult, out mediaEvent);
                    if (hr < 0) break;

                    int status;

                    hr = mediaEvent.GetUINT32(MFAttributesClsid.MF_EVENT_TOPOLOGY_STATUS, out status);
                    if (hr == 0)
                    {
                        if (((MFTopoStatus)status) == MFTopoStatus.Ready)
                        {
                            Marshal.ReleaseComObject(mediaEvent);

                            m_topologyReady.Set();
                            hr = 0;
                            break;
                        }
                    }
                    else
                    {
                        Marshal.ReleaseComObject(mediaEvent);

                        hr = m_mediaSession.BeginGetEvent(this, asyncState);
                        if (hr < 0) break;
                    }
                }
                #endregion

                #region Callback code for IMFWorkQueueServices.BeginRegisterPlatformWorkQueueWithMMCSS
                else if (asyncState.Token == "IMFWorkQueueServices.BeginRegisterPlatformWorkQueueWithMMCSS")
                {
                    hr = m_workQueueServices.EndRegisterPlatformWorkQueueWithMMCSS(pAsyncResult, out m_platformWorkQueueTaskId);
                    m_registerPlatformWorkQueue.Set();
                }
                #endregion

                #region Callback code for IMFWorkQueueServices.BeginUnregisterPlatformWorkQueueWithMMCSS
                else if (asyncState.Token == "IMFWorkQueueServices.BeginUnregisterPlatformWorkQueueWithMMCSS")
                {
                    hr = m_workQueueServices.EndUnregisterPlatformWorkQueueWithMMCSS(pAsyncResult);
                    m_unregisterPlatformWorkQueue.Set();
                }
                #endregion

                #region Callback code for IMFWorkQueueServices.BeginRegisterTopologyWorkQueuesWithMMCSS
                else if (asyncState.Token == "IMFWorkQueueServices.BeginRegisterTopologyWorkQueuesWithMMCSS")
                {
                    hr = m_workQueueServices.EndRegisterTopologyWorkQueuesWithMMCSS(pAsyncResult);
                    m_registerTopologyWorkQueues.Set();
                }
                #endregion

                #region Callback code for IMFWorkQueueServices.BeginUnregisterTopologyWorkQueuesWithMMCSS
                else if (asyncState.Token == "IMFWorkQueueServices.BeginUnregisterTopologyWorkQueuesWithMMCSS")
                {
                    hr = m_workQueueServices.EndUnregisterTopologyWorkQueuesWithMMCSS(pAsyncResult);
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
    public class AsyncCallback : IMFAsyncCallback
    {
        private Func<IMFAsyncResult, int> m_invokeFunction;
        private ManualResetEvent m_syncEvent = new ManualResetEvent(false);
        private int m_hr;

        public AsyncCallback(Func<IMFAsyncResult, int> invokeFunction)
        {
            m_invokeFunction = invokeFunction;
        }

        /// <summary>
        /// Use to wait the finalization of the Invoke method.
        /// </summary>
        /// <returns>the HRESULT of the Invoke method.</returns>
        public int WaitForFinalization()
        {
            return WaitForFinalization(TimeSpan.FromMilliseconds(int.MaxValue));
        }

        /// <summary>
        /// Use to wait the finalization of the Invoke method.
        /// </summary>
        /// <param name="timeout">A maximum wait time before returning even if the Invoke method don't complete.</param>
        /// <returns>the HRESULT if the Invoke method or -1 if the timeout has been reached.</returns>
        public int WaitForFinalization(TimeSpan timeout)
        {
            if (m_syncEvent.WaitOne(timeout) == true)
                return m_hr;
            else
                return -1;
        }

        #region IMFAsyncCallback Members

        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            Debug.WriteLine("AsyncCallback.GetParameters");
            pdwFlags = default(MFASync);
            pdwQueue = default(MFAsyncCallbackQueue);
            return COMBase.E_NotImplemented;
        }

        public int Invoke(IMFAsyncResult pAsyncResult)
        {
            Debug.WriteLine("Entering AsyncCallback.Invoke");

            m_hr = m_invokeFunction(pAsyncResult);
            m_syncEvent.Set();

            Debug.WriteLine(string.Format("Exiting AsyncCallback.Invoke (hr = 0x{0:X8})", m_hr));
            return m_hr;
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
