using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MediaFoundation.Transform;

namespace Testv22
{
    class IMFRealTimeClientTest : COMBase, IMFSampleGrabberSinkCallback, IMFAsyncCallback
    {
        int WORKQUEUE_ID;
        const string WORKQUEUE_MMCSS_CLASS = "audio";
        IMFWorkQueueServices m_workQueueServices;
        TestThing pTransformUnk = null;

        public void DoTests()
        {
            GetInterface(@"c:\sourceforge\mflib\test\media\AspectRatio4x3.wmv");

            int hr = m_workQueueServices.BeginRegisterTopologyWorkQueuesWithMMCSS(this, null);
            MFError.ThrowExceptionForHR(hr);

            hr = m_workQueueServices.BeginUnregisterTopologyWorkQueuesWithMMCSS(this, null);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(pTransformUnk.m_didit == 15);
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

                    ///////////////////
                    IMFTopologyNode pTransformNode = null;

                    // Create a transform node.
                    hr = MFExtern.MFCreateTopologyNode(MFTopologyType.TransformNode, out pTransformNode);
                    MFError.ThrowExceptionForHR(hr);

                    pTransformUnk = new TestThing(WORKQUEUE_MMCSS_CLASS);

                    hr = pTransformNode.SetObject(pTransformUnk);
                    MFError.ThrowExceptionForHR(hr);

                    // Add the transform node to the topology.
                    hr = pTopology.AddNode(pTransformNode);
                    MFError.ThrowExceptionForHR(hr);

                    // Connect the source node to the transform node.
                    hr = pNode1.ConnectOutput(0, pTransformNode, 0);
                    MFError.ThrowExceptionForHR(hr);

                    // Connect the transform node to the output node.
                    hr = pTransformNode.ConnectOutput(0, pNode2, 0);
                    MFError.ThrowExceptionForHR(hr);
                    ///////////////////

                    break;
                }
                else
                {
                    hr = pPD.DeselectStream(i);
                    MFError.ThrowExceptionForHR(hr);

                }
            }

            ppTopo = pTopology;
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

        #region IMFAsyncCallback

        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwFlags = MFASync.None;
            pdwQueue = MFAsyncCallbackQueue.Undefined;

            return S_Ok;
        }

        public int Invoke(IMFAsyncResult pAsyncResult)
        {
            return S_Ok;
        }

        #endregion

        #region IMFSampleGrabberSinkCallback

        public int OnClockStart(long hnsSystemTime, long llClockStartOffset)
        {
            throw new NotImplementedException();
        }

        public int OnClockStop(long hnsSystemTime)
        {
            throw new NotImplementedException();
        }

        public int OnClockPause(long hnsSystemTime)
        {
            throw new NotImplementedException();
        }

        public int OnClockRestart(long hnsSystemTime)
        {
            throw new NotImplementedException();
        }

        public int OnClockSetRate(long hnsSystemTime, float flRate)
        {
            throw new NotImplementedException();
        }

        public int OnSetPresentationClock(IMFPresentationClock pPresentationClock)
        {
            throw new NotImplementedException();
        }

        public int OnProcessSample(Guid guidMajorMediaType, int dwSampleFlags, long llSampleTime, long llSampleDuration, IntPtr pSampleBuffer, int dwSampleSize)
        {
            throw new NotImplementedException();
        }

        public int OnShutdown()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class TestThing : COMBase, IMFTransform, IDisposable, IMFRealTimeClient
    {
        #region Member variables

        IMFSample m_pSample;                    // Input sample.
        IMFMediaType m_pInputType;              // Input media type.
        IMFMediaType m_pOutputType;             // Output media type.

        string m_Class;
        int m_cbImageSize;
        public int m_didit;

        #endregion

        #region Registration methods

        [ComRegisterFunctionAttribute]
        static public void DllRegisterServer(Type t)
        {
            int hr = MFExtern.MFTRegister(
                typeof(TestThing).GUID,         // CLSID
                MFTransformCategory.MFT_CATEGORY_VIDEO_EFFECT,  // Category
                "Grayscale Video Effect .NET",  // Friendly name
                0,                          // Reserved, must be zero.
                0,
                null,
                0,
                null,
                null
                );
            MFError.ThrowExceptionForHR(hr);

        }

        [ComUnregisterFunctionAttribute]
        static public void DllUnregisterServer(Type t)
        {
            int hr = MFExtern.MFTUnregister(typeof(TestThing).GUID);
            MFError.ThrowExceptionForHR(hr);
        }

        #endregion

        public TestThing(string sClass)
        {
            TRACE("Constructor");

            m_pSample = null;
            m_pInputType = null;
            m_pOutputType = null;
            m_Class = sClass;
            m_cbImageSize = 100;
            m_didit = 0;
        }

        ~TestThing()
        {
            TRACE("Destructor");
            Dispose();
        }

        #region IMFTransform methods

        public int GetStreamLimits(
            MFInt pdwInputMinimum,
            MFInt pdwInputMaximum,
            MFInt pdwOutputMinimum,
            MFInt pdwOutputMaximum
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("GetStreamLimits");

                // Fixed stream limits.
                if (pdwInputMinimum != null)
                {
                    pdwInputMinimum.Assign(1);
                }
                if (pdwInputMaximum != null)
                {
                    pdwInputMaximum.Assign(1);
                }
                if (pdwOutputMinimum != null)
                {
                    pdwOutputMinimum.Assign(1);
                }
                if (pdwOutputMaximum != null)
                {
                    pdwOutputMaximum.Assign(1);
                }

                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public int GetStreamCount(
            MFInt pcInputStreams,
            MFInt pcOutputStreams
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("GetStreamCount");

                // Fixed stream count.
                if (pcInputStreams != null)
                {
                    pcInputStreams.Assign(1);
                }

                if (pcOutputStreams != null)
                {
                    pcOutputStreams.Assign(1);
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public int GetStreamIDs(
            int dwInputIDArraySize,
            int[] pdwInputIDs,
            int dwOutputIDArraySize,
            int[] pdwOutputIDs
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("GetStreamIDs");

                // Do not need to implement, because this MFT has a fixed number of 
                // streams and the stream IDs match the stream indexes.

                // However, I'm going to implement it anyway
                //throw new COMException("Fixed # of zero based streams", E_NotImplemented);

                pdwInputIDs[0] = 0;
                pdwOutputIDs[0] = 0;
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public int GetInputStreamInfo(
            int dwInputStreamID,
            out MFTInputStreamInfo pStreamInfo
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("GetInputStreamInfo");

                pStreamInfo = new MFTInputStreamInfo();

                lock (this)
                {
                    CheckValidInputStream(dwInputStreamID);

                    pStreamInfo.hnsMaxLatency = 0;
                    pStreamInfo.dwFlags = MFTInputStreamInfoFlags.WholeSamples | MFTInputStreamInfoFlags.SingleSamplePerBuffer;
                    pStreamInfo.cbSize = m_cbImageSize;
                    pStreamInfo.cbMaxLookahead = 0;
                    pStreamInfo.cbAlignment = 0;
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                pStreamInfo = new MFTInputStreamInfo();
                return Marshal.GetHRForException(e);
            }
        }

        public int GetOutputStreamInfo(
            int dwOutputStreamID,
            out MFTOutputStreamInfo pStreamInfo
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                int hr;
                TRACE("GetOutputStreamInfo");

                lock (this)
                {
                    CheckValidOutputStream(dwOutputStreamID);

                    if (m_pOutputType != null)
                    {

                        pStreamInfo.dwFlags = MFTOutputStreamInfoFlags.WholeSamples |
                             MFTOutputStreamInfoFlags.SingleSamplePerBuffer |
                             MFTOutputStreamInfoFlags.FixedSampleSize;
                        pStreamInfo.cbSize = m_cbImageSize;
                        pStreamInfo.cbAlignment = 0;

                        hr = S_Ok;
                    }
                    else
                    {
                        // No output type set
                        hr = MFError.MF_E_TRANSFORM_TYPE_NOT_SET;
                        pStreamInfo = new MFTOutputStreamInfo();
                    }
                }
                return hr;
            }
            catch (Exception e)
            {
                pStreamInfo = new MFTOutputStreamInfo();
                return Marshal.GetHRForException(e);
            }
        }

        public int GetAttributes(out IMFAttributes pAttributes)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("GetAttributes");

                pAttributes = null;

                // No attributes supported
                return E_NotImplemented;
            }
            catch (Exception e)
            {
                pAttributes = null;
                return Marshal.GetHRForException(e);
            }
        }

        public int GetInputStreamAttributes(
            int dwInputStreamID,
            out IMFAttributes ppAttributes
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("GetInputStreamAttributes");

                ppAttributes = null;

                // No input attributes supported
                return E_NotImplemented;
            }
            catch (Exception e)
            {
                ppAttributes = null;
                return Marshal.GetHRForException(e);
            }
        }

        public int GetOutputStreamAttributes(
            int dwOutputStreamID,
            out IMFAttributes ppAttributes
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("GetOutputStreamAttributes");

                ppAttributes = null;

                // No output attributes supported
                return E_NotImplemented;
            }
            catch (Exception e)
            {
                ppAttributes = null;
                return Marshal.GetHRForException(e);
            }
        }

        public int DeleteInputStream(int dwStreamID)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("DeleteInputStream");

                // Removing streams not supported
                return E_NotImplemented;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public int AddInputStreams(
            int cStreams,
            int[] adwStreamIDs
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("AddInputStreams");

                // Adding streams not supported
                return E_NotImplemented;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public int GetInputAvailableType(
            int dwInputStreamID,
            int dwTypeIndex, // 0-based
            out IMFMediaType ppType
        )
        {
            int hr = 0;

            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE(string.Format("GetInputAvailableType (stream = {0}, type index = {1})", dwInputStreamID, dwTypeIndex));

                lock (this)
                {
                    CheckValidInputStream(dwInputStreamID);

                    if (m_pOutputType != null)
                    {
                        ppType = m_pOutputType;
                    }
                    else
                    {
                        ppType = null;
                        hr = E_Fail;
                    }
                }
                return hr;
            }
            catch (Exception e)
            {
                ppType = null;
                return Marshal.GetHRForException(e);
            }
        }

        public int GetOutputAvailableType(
            int dwOutputStreamID,
            int dwTypeIndex, // 0-based
            out IMFMediaType ppType
        )
        {
            int hr;

            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE(string.Format("GetOutputAvailableType (stream = {0}, type index = {1})", dwOutputStreamID, dwTypeIndex));

                lock (this)
                {
                    CheckValidOutputStream(dwOutputStreamID);

                    if (dwTypeIndex == 0)
                    {

                        if (m_pInputType != null)
                        {
                            ppType = m_pInputType;
                            hr = 0;
                        }
                        else
                        {
                            ppType = null;
                            hr = E_Fail;
                        }
                    }
                    else
                    {
                        ppType = null;
                        hr = MFError.MF_E_NO_MORE_TYPES;
                    }
                }
                return hr;
            }
            catch (Exception e)
            {
                ppType = null;
                return Marshal.GetHRForException(e);
            }
        }

        public int SetInputType(
            int dwInputStreamID,
            IMFMediaType pType,
            MFTSetTypeFlags dwFlags
        )
        {
            TRACE("SetInputType");

            // Make sure we *never* leave this entry point with an exception
            try
            {
                lock (this)
                {
                    CheckValidInputStream(dwInputStreamID);

                    // Does the caller want us to set the type, or just test it?
                    bool bReallySet = ((dwFlags & MFTSetTypeFlags.TestOnly) == 0);

                    // If we have an input sample, the client cannot change the type now.
                    if (HasPendingOutput())
                    {
                        // Can't change type while samples are pending
                        return MFError.MF_E_INVALIDMEDIATYPE;
                    }

                    // Validate the type.
                    OnCheckInputType(pType);

                    // The type is OK. 
                    // Set the type, unless the caller was just testing.
                    if (bReallySet)
                    {
                        OnSetInputType(pType);
                    }
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public int SetOutputType(
            int dwOutputStreamID,
            IMFMediaType pType,
            MFTSetTypeFlags dwFlags
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("SetOutputType");

                lock (this)
                {
                    CheckValidOutputStream(dwOutputStreamID);

                    // Does the caller want us to set the type, or just test it?
                    bool bReallySet = ((dwFlags & MFTSetTypeFlags.TestOnly) == 0);

                    // If we have an input sample, the client cannot change the type now.
                    if (HasPendingOutput())
                    {
                        // Cannot change type while samples are pending
                        return MFError.MF_E_INVALIDMEDIATYPE;
                    }

                    // Validate the type.
                    OnCheckOutputType(pType);
                    if (bReallySet)
                    {
                        // The type is OK. 
                        // Set the type, unless the caller was just testing.
                        OnSetOutputType(pType);
                    }
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public int GetInputCurrentType(
            int dwInputStreamID,
            out IMFMediaType ppType
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                int hr;
                TRACE("GetInputCurrentType");

                lock (this)
                {
                    CheckValidInputStream(dwInputStreamID);

                    if (m_pInputType != null)
                    {
                        ppType = m_pInputType;
                        hr = S_Ok;
                    }
                    else
                    {
                        ppType = null;

                        // Type is not set
                        hr = MFError.MF_E_TRANSFORM_TYPE_NOT_SET;
                    }

                }
                return hr;
            }
            catch (Exception e)
            {
                ppType = null;
                return Marshal.GetHRForException(e);
            }
        }

        public int GetOutputCurrentType(
            int dwOutputStreamID,
            out IMFMediaType ppType
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                int hr;
                TRACE("GetOutputCurrentType");

                lock (this)
                {
                    CheckValidOutputStream(dwOutputStreamID);

                    if (m_pOutputType != null)
                    {
                        ppType = m_pOutputType;
                        hr = S_Ok;
                    }
                    else
                    {
                        ppType = null;

                        // No output type set
                        hr = MFError.MF_E_TRANSFORM_TYPE_NOT_SET;
                    }

                }
                return hr;
            }
            catch (Exception e)
            {
                ppType = null;
                return Marshal.GetHRForException(e);
            }
        }

        public int GetInputStatus(
            int dwInputStreamID,
            out MFTInputStatusFlags pdwFlags
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("GetInputStatus");

                lock (this)
                {
                    CheckValidInputStream(dwInputStreamID);

                    // If we already have an input sample, we don't accept
                    // another one until the client calls ProcessOutput or Flush.
                    if (m_pSample == null)
                    {
                        pdwFlags = MFTInputStatusFlags.AcceptData;
                    }
                    else
                    {
                        pdwFlags = MFTInputStatusFlags.None;
                    }
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                pdwFlags = 0;
                return Marshal.GetHRForException(e);
            }
        }

        public int GetOutputStatus(
            out MFTOutputStatusFlags pdwFlags)
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("GetOutputStatus");

                lock (this)
                {
                    // We can produce an output sample if (and only if)
                    // we have an input sample.
                    if (m_pSample != null)
                    {
                        pdwFlags = MFTOutputStatusFlags.SampleReady;
                    }
                    else
                    {
                        pdwFlags = MFTOutputStatusFlags.None;
                    }
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                pdwFlags = 0;
                return Marshal.GetHRForException(e);
            }
        }

        public int SetOutputBounds(
            long hnsLowerBound,
            long hnsUpperBound
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("SetOutputBounds");

                // Output bounds not supported
                return E_NotImplemented;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public int ProcessEvent(
            int dwInputStreamID,
            IMFMediaEvent pEvent
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("ProcessEvent");

                // Events not support
                return E_NotImplemented;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public int ProcessMessage(
            MFTMessageType eMessage,
            IntPtr ulParam
        )
        {
            // Make sure we *never* leave this entry point with an exception
            try
            {
                TRACE("ProcessMessage");

                lock (this)
                {
                    switch (eMessage)
                    {
                        case MFTMessageType.CommandFlush:
                            // Flush the MFT.
                            OnFlush();
                            break;

                        // The remaining messages do not require any action from this MFT.

                        case MFTMessageType.CommandDrain:
                            // Drain: Tells the MFT not to accept any more input until 
                            // all of the pending output has been processed. That is our 
                            // default behevior already, so there is nothing to do.

                            //MFTDrainType dt = (MFTDrainType)ulParam.ToInt32();
                            break;

                        case MFTMessageType.SetD3DManager:
                            //object o = Marshal.GetUniqueObjectForIUnknown(ulParam);
                            break;

                        case MFTMessageType.NotifyBeginStreaming:
                            break;

                        case MFTMessageType.NotifyEndStreaming:
                            break;

                        case MFTMessageType.NotifyEndOfStream:
                            //int i = ulParam.ToInt32();
                            break;

                        case MFTMessageType.NotifyStartOfStream:
                            break;
                    }
                }
                return S_Ok;
            }
            catch (Exception e)
            {
                return Marshal.GetHRForException(e);
            }
        }

        public int ProcessInput(
            int dwInputStreamID,
            IMFSample pSample,
            int dwFlags
        )
        {
            TRACE("ProcessInput");

            return S_Ok;
        }

        public int ProcessOutput(
            MFTProcessOutputFlags dwFlags,
            int cOutputBufferCount,
            MFTOutputDataBuffer[] pOutputSamples, // one per stream
            out ProcessOutputStatus pdwStatus
        )
        {
            pdwStatus = 0;

            TRACE("ProcessOutput");

            return S_Ok;
        }

        #endregion

        #region Private Methods

        //-------------------------------------------------------------------
        // Name: OnGetPartialType
        // Description: Returns a partial media type from our list.
        //
        // dwTypeIndex: Index into the list of peferred media types.
        // ppmt: Receives a pointer to the media type.
        //-------------------------------------------------------------------

        //-------------------------------------------------------------------
        // Name: OnCheckInputType
        // Description: Validate an input media type.
        //-------------------------------------------------------------------

        private void OnCheckInputType(IMFMediaType pmt)
        {
            TRACE("OnCheckInputType");

            // If the output type is set, see if they match.
            if (m_pOutputType != null)
            {
                MFMediaEqual flags;
                int hr = pmt.IsEqual(m_pOutputType, out flags);

                // IsEqual can return S_FALSE. Treat this as failure.
                if (hr != S_Ok)
                {
                    throw new COMException("Output type != input type", MFError.MF_E_INVALIDTYPE);
                }
            }
            else
            {
                // Output type is not set. Just check this type.
                OnCheckMediaType(pmt);
            }
        }

        //-------------------------------------------------------------------
        // Name: OnCheckOutputType
        // Description: Validate an output media type.
        //-------------------------------------------------------------------

        private void OnCheckOutputType(IMFMediaType pmt)
        {
            TRACE("OnCheckOutputType");

            // If the input type is set, see if they match.
            if (m_pInputType != null)
            {
                MFMediaEqual flags;
                int hr = pmt.IsEqual(m_pInputType, out flags);

                // IsEqual can return S_FALSE. Treat this as failure.
                if (hr != S_Ok)
                {
                    throw new COMException("Output type != input type", MFError.MF_E_INVALIDTYPE);
                }
            }
            else
            {
                // Input type is not set. Just check this type.
                OnCheckMediaType(pmt);
            }
        }

        //-------------------------------------------------------------------
        // Name: OnCheckMediaType
        // Description: Validates a media type for this transform.
        //-------------------------------------------------------------------

        private void OnCheckMediaType(IMFMediaType pmt)
        {
            Guid major_type;
            Guid subtype;

            // Major type must be video.
            int hr = pmt.GetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, out major_type);
            MFError.ThrowExceptionForHR(hr);

            // Subtype must be one of the subtypes in our global list.
            // Get the subtype GUID.
            hr = pmt.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out subtype);
            MFError.ThrowExceptionForHR(hr);

            TraceSubtype(subtype);
        }

        //-------------------------------------------------------------------
        // Name: OnSetInputType
        // Description: Sets the input media type.
        //
        // Prerequisite:
        // The input type has already been validated.
        //-------------------------------------------------------------------

        private void OnSetInputType(IMFMediaType pmt)
        {
            TRACE("OnSetInputType");

            // Release the old type
            SafeRelease(m_pInputType);

            // Set the type.
            m_pInputType = pmt;

            // Update the format information.
            UpdateFormatInfo();
        }

        //-------------------------------------------------------------------
        // Name: OnSetOutputType
        // Description: Sets the output media type.
        //
        // Prerequisite:
        // The output type has already been validated.
        //-------------------------------------------------------------------

        private void OnSetOutputType(IMFMediaType pmt)
        {
            TRACE("OnSetOutputType");

            // Release the old type
            SafeRelease(m_pOutputType);

            // Set the type.
            m_pOutputType = pmt;
        }

        //-------------------------------------------------------------------
        // Name: OnFlush
        // Description: Flush the MFT.
        //-------------------------------------------------------------------

        private void OnFlush()
        {
            // For this MFT, flushing just means releasing the input sample.
            SafeRelease(m_pSample);
            m_pSample = null;
        }

        //-------------------------------------------------------------------
        // Name: UpdateFormatInfo
        // Description: After the input type is set, update our format 
        //              information.
        //-------------------------------------------------------------------

        private void UpdateFormatInfo()
        {
        }

        //-------------------------------------------------------------------
        // Name: GetImageSize
        // Description: 
        // Calculates the buffer size needed, based on the video format.
        //-------------------------------------------------------------------

        private void CheckValidInputStream(int dwInputStreamID)
        {
            if (dwInputStreamID != 0)
            {
                throw new COMException("Invalid input stream ID", MFError.MF_E_INVALIDSTREAMNUMBER);
            }
        }

        private void CheckValidOutputStream(int dwOutputStreamID)
        {
            if (dwOutputStreamID != 0)
            {
                throw new COMException("Invalid output stream ID", MFError.MF_E_INVALIDSTREAMNUMBER);
            }
        }

        // HasPendingOutput: Returns TRUE if the MFT is holding an input sample.
        private bool HasPendingOutput() { return m_pSample != null; }

        private void TraceSubtype(Guid g)
        {
#if DEBUG
            FourCC fc = new FourCC(g);
            TRACE(string.Format("Subtype: {0}", fc.ToString()));
#endif
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            TRACE("Dispose");

            SafeRelease(m_pSample);
            m_pSample = null;

            if (m_pInputType == m_pOutputType)
            {
                SafeRelease(m_pInputType);
            }
            else
            {
                SafeRelease(m_pInputType);
                SafeRelease(m_pOutputType);
            }

            m_pInputType = null;
            m_pOutputType = null;

            GC.SuppressFinalize(this);
        }

        #endregion

        public int RegisterThreads(int dwTaskIndex, string wszClass)
        {
            Debug.Assert(wszClass == this.m_Class);
            m_didit |= 1;

            return S_Ok;
        }

        public int UnregisterThreads()
        {
            m_didit |= 2;
            return S_Ok;
        }

        public int SetWorkQueue(MFAsyncCallbackQueue dwWorkQueueId)
        {
            if (dwWorkQueueId != MFAsyncCallbackQueue.Undefined)
            {
                m_didit |= 4;
            }
            else
            {
                m_didit |= 8;
            }
            return S_Ok;
        }
    }
}

