/* https://msdn.microsoft.com/en-us/library/windows/desktop/hh184779%28v=vs.85%29.aspx
 */
using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv21
{
    class IMFSampleGrabberSinkCallbackTest : COMBase, IMFSampleGrabberSinkCallback
    {
        int m_stat = 0;

        public void DoTests()
        {
            RunSampleGrabber(@"c:\sourceforge\mflib\test\media\AspectRatio4x3.wmv");
            Debug.Assert(m_stat == 0x7);
        }

        // Create a media source from a URL.
        private void CreateMediaSource(string pszURL, out IMFMediaSource ppSource)
        {
            IMFSourceResolver pSourceResolver = null;
            object pSource;

            // Create the source resolver.
            int hr;
            hr = MFExtern.MFCreateSourceResolver(out pSourceResolver);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType ObjectType;
            hr = pSourceResolver.CreateObjectFromURL(pszURL,
                MFResolution.MediaSource, null, out ObjectType, out pSource);
            MFError.ThrowExceptionForHR(hr);

            ppSource = (IMFMediaSource)pSource;

            //done:
            //SafeRelease(&pSourceResolver);
            //SafeRelease(&pSource);
            //return hr;
        }
        private void RunSampleGrabber(string pszFileName)
        {
            IMFMediaSession pSession = null;
            IMFMediaSource pSource = null;
            //SampleGrabberCB pCallback = null;
            IMFActivate pSinkActivate = null;
            IMFTopology pTopology = null;
            IMFMediaType pType = null;
            int hr;

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

            // Run the media session.
            RunSession(pSession, pTopology);

            //done:
            // Clean up.
            pSource.Shutdown();
            pSession.Shutdown();

            //SafeRelease(pSession);
            //SafeRelease(pSource);
            //SafeRelease(pCallback);
            //SafeRelease(pSinkActivate);
            //SafeRelease(pTopology);
            //SafeRelease(pType);
        }

        private void CreateTopology(IMFMediaSource pSource, IMFActivate pSinkActivate, IMFTopology ppTopo)
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
                    MFError.ThrowExceptionForHR(hr);

                    AddOutputNode(pTopology, pSinkActivate, 0, out pNode2);
                    MFError.ThrowExceptionForHR(hr);

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
            //(ppTopo).AddRef();

            //done:
            //SafeRelease(pTopology);
            //SafeRelease(pNode1);
            //SafeRelease(pNode2);
            //SafeRelease(pPD);
            //SafeRelease(pSD);
            //SafeRelease(pHandler);
        }

        private void RunSession(IMFMediaSession pSession, IMFTopology pTopology)
        {
            IMFMediaEvent pEvent = null;

            PropVariant var = new PropVariant();

            int hr = S_Ok;
            hr = pSession.SetTopology(0, pTopology);
            MFError.ThrowExceptionForHR(hr);

            hr = pSession.Start(Guid.Empty, var);
            MFError.ThrowExceptionForHR(hr);

            int x = 0;
            while (true)
            {
                int hrStatus = S_Ok;
                MediaEventType met;

#if false
                if (x == 3)
                {
                    hr = pSession.Pause();
                    System.Threading.Thread.Sleep(1000);
                    PropVariant pv = new PropVariant();
                    hr = MFExtern.MFCreateSequencerSegmentOffset(0, 0, pv);
                    hr = pSession.Start(Guid.Empty, new PropVariant());
                }
#endif

                hr = pSession.GetEvent(0, out pEvent);
                MFError.ThrowExceptionForHR(hr);

                hr = pEvent.GetStatus(out hrStatus);
                MFError.ThrowExceptionForHR(hr);

                hr = pEvent.GetType(out met);
                MFError.ThrowExceptionForHR(hr);

                if (met == MediaEventType.MESessionEnded)
                {
                    break;
                }
                //SafeRelease(pEvent);
                x++;
            }

            //done:
            //SafeRelease(pEvent);
            //return hr;
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

        public int OnSetPresentationClock(IMFPresentationClock pPresentationClock)
        {
            if (pPresentationClock != null)
            {
                long t;
                int hr;

                hr = pPresentationClock.GetTime(out t);
                MFError.ThrowExceptionForHR(hr);
            }

            m_stat |= 1;
            return S_Ok;
        }

        public int OnProcessSample(Guid guidMajorMediaType, int dwSampleFlags, long llSampleTime, long llSampleDuration, IntPtr pSampleBuffer, int dwSampleSize)
        {
            m_stat |= 2;
            return S_Ok;
        }

        public int OnShutdown()
        {
            m_stat |= 4;
            return S_Ok;
        }
    }
}
