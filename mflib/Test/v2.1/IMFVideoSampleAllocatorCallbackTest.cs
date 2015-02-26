using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

using MediaFoundation.EVR;
using System.Runtime.InteropServices;

namespace Testv21
{
    class IMFVideoSampleAllocatorCallbackTest : COMBase, IMFAsyncCallback, IMFVideoSampleAllocatorNotify
    {
        public void DoTests()
        {
            IMFTopology pTopology = null;

            // Create the media session.
            CreateSession();

            // Create the media source.
            CreateMediaSource(@"c:\SourceForge\mflib\Test\Media\AspectRatio4x3.wmv");

            // Create a partial topology.
            CreateTopologyFromSource(out pTopology);

            // Set the topology on the media session.
            int hr = m_pSession.SetTopology(0, pTopology);
            MFError.ThrowExceptionForHR(hr);

            /////////////////
            // Something is happening in the background.  Wait for it to finish.
            System.Threading.Thread.Sleep(1000);

            // Find the EVR node
            IMFGetService gs = null;
            IMFTopologyNode p;
            IMFVideoSampleAllocatorCallback sac2 = null;
            object o;
            short s;

            hr = pTopology.GetNodeCount(out s);
            MFError.ThrowExceptionForHR(hr);

            for (short x = 0; x < s; x++)
            {
                hr = pTopology.GetNode(x, out p);
                MFError.ThrowExceptionForHR(hr);

                hr = p.GetObject(out o);

                if (hr >= 0)
                {
                    IMFStreamSink ss = o as IMFStreamSink;
                    if (ss != null)
                    {
                        gs = (IMFGetService)ss;
                        hr = gs.GetService(MFServices.MR_VIDEO_ACCELERATION_SERVICE, typeof(IMFVideoSampleAllocator).GUID, out o);
                        if (hr >= 0)
                        {
                            sac2 = (IMFVideoSampleAllocatorCallback)o;
                        }
                    }
                }
            }

            hr = sac2.SetCallback(this);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pSession.Start(Guid.Empty, new PropVariant());
            MFError.ThrowExceptionForHR(hr);
            System.Threading.Thread.Sleep(1000);

            // Must be running to get non-zero value
            int l;
            hr = sac2.GetFreeSampleCount(out l);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(l != 0);

            // Make sure NotifyRelease is getting called
            Debug.Assert(m_Done);
        }

        protected void CreateSession()
        {
            // Create the media session.
            int hr = MFExtern.MFCreateMediaSession(null, out m_pSession);
            MFError.ThrowExceptionForHR(hr);

            // Start pulling events from the media session
            hr = m_pSession.BeginGetEvent(this, null);
            MFError.ThrowExceptionForHR(hr);
        }

        protected void CreateMediaSource(string sURL)
        {
            TRACE("CPlayer::CreateMediaSource");

            IMFSourceResolver pSourceResolver;
            object pSource;

            // Create the source resolver.
            int hr = MFExtern.MFCreateSourceResolver(out pSourceResolver);
            MFError.ThrowExceptionForHR(hr);

            try
            {
                // Use the source resolver to create the media source.
                MFObjectType ObjectType = MFObjectType.Invalid;

                hr = pSourceResolver.CreateObjectFromURL(
                        sURL,                       // URL of the source.
                        MFResolution.MediaSource,   // Create a source object.
                        null,                       // Optional property store.
                        out ObjectType,             // Receives the created object type.
                        out pSource                 // Receives a pointer to the media source.
                    );
                MFError.ThrowExceptionForHR(hr);

                // Get the IMFMediaSource interface from the media source.
                m_pSource = (IMFMediaSource)pSource;
            }
            finally
            {
                // Clean up
                Marshal.ReleaseComObject(pSourceResolver);
            }
        }

        protected void CreateTopologyFromSource(out IMFTopology ppTopology)
        {
            TRACE("CPlayer::CreateTopologyFromSource");

            Debug.Assert(m_pSession != null);
            Debug.Assert(m_pSource != null);

            IMFTopology pTopology = null;
            IMFPresentationDescriptor pSourcePD = null;
            int cSourceStreams = 0;

            int hr;

            try
            {
                // Create a new topology.
                hr = MFExtern.MFCreateTopology(out pTopology);
                MFError.ThrowExceptionForHR(hr);

                hr = pTopology.SetUINT32(MFAttributesClsid.MF_TOPOLOGY_DXVA_MODE, (int)MFTOPOLOGY_DXVA_MODE.None);
                MFError.ThrowExceptionForHR(hr);

                // Create the presentation descriptor for the media source.
                hr = m_pSource.CreatePresentationDescriptor(out pSourcePD);
                MFError.ThrowExceptionForHR(hr);

                // Get the number of streams in the media source.
                hr = pSourcePD.GetStreamDescriptorCount(out cSourceStreams);
                MFError.ThrowExceptionForHR(hr);

                TRACE(string.Format("Stream count: {0}", cSourceStreams));

                // For each stream, create the topology nodes and add them to the topology.
                for (int i = 0; i < cSourceStreams; i++)
                {
                    AddBranchToPartialTopology(pTopology, pSourcePD, i);
                }

                // Return the IMFTopology pointer to the caller.
                ppTopology = pTopology;
            }
            catch
            {
                // If we failed, release the topology
                SafeRelease(pTopology);
                throw;
            }
            finally
            {
                SafeRelease(pSourcePD);
            }
        }

        protected void AddBranchToPartialTopology(
            IMFTopology pTopology,
            IMFPresentationDescriptor pSourcePD,
            int iStream
            )
        {
            int hr;

            TRACE("CPlayer::AddBranchToPartialTopology");

            Debug.Assert(pTopology != null);

            IMFStreamDescriptor pSourceSD = null;
            IMFTopologyNode pSourceNode = null;
            IMFTopologyNode pOutputNode = null;
            bool fSelected = false;

            try
            {
                // Get the stream descriptor for this stream.
                hr = pSourcePD.GetStreamDescriptorByIndex(iStream, out fSelected, out pSourceSD);
                MFError.ThrowExceptionForHR(hr);

                // Create the topology branch only if the stream is selected.
                // Otherwise, do nothing.
                if (fSelected)
                {
                    // Create a source node for this stream.
                    CreateSourceStreamNode(pSourcePD, pSourceSD, out pSourceNode);

                    // Create the output node for the renderer.
                    CreateOutputNode(pSourceSD, out pOutputNode);

                    // Add both nodes to the topology.
                    hr = pTopology.AddNode(pSourceNode);
                    MFError.ThrowExceptionForHR(hr);
                    hr = pTopology.AddNode(pOutputNode);
                    MFError.ThrowExceptionForHR(hr);

                    // Connect the source node to the output node.
                    hr = pSourceNode.ConnectOutput(0, pOutputNode, 0);
                    MFError.ThrowExceptionForHR(hr);
                }
            }
            finally
            {
                // Clean up.
                SafeRelease(pSourceSD);
                SafeRelease(pSourceNode);
                SafeRelease(pOutputNode);
            }
        }

        protected void CreateSourceStreamNode(
            IMFPresentationDescriptor pSourcePD,
            IMFStreamDescriptor pSourceSD,
            out IMFTopologyNode ppNode
            )
        {
            Debug.Assert(m_pSource != null);

            int hr;
            IMFTopologyNode pNode = null;

            try
            {
                // Create the source-stream node.
                hr = MFExtern.MFCreateTopologyNode(MFTopologyType.SourcestreamNode, out pNode);
                MFError.ThrowExceptionForHR(hr);

                // Set attribute: Pointer to the media source.
                hr = pNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_SOURCE, m_pSource);
                MFError.ThrowExceptionForHR(hr);

                // Set attribute: Pointer to the presentation descriptor.
                hr = pNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_PRESENTATION_DESCRIPTOR, pSourcePD);
                MFError.ThrowExceptionForHR(hr);

                // Set attribute: Pointer to the stream descriptor.
                hr = pNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_STREAM_DESCRIPTOR, pSourceSD);
                MFError.ThrowExceptionForHR(hr);

                // Return the IMFTopologyNode pointer to the caller.
                ppNode = pNode;
            }
            catch
            {
                // If we failed, release the pnode
                SafeRelease(pNode);
                throw;
            }
        }

        protected void CreateOutputNode(
            IMFStreamDescriptor pSourceSD,
            out IMFTopologyNode ppNode
            )
        {
            IMFTopologyNode pNode = null;
            IMFMediaTypeHandler pHandler = null;
            IMFActivate pRendererActivate = null;

            Guid guidMajorType = Guid.Empty;
            int hr = S_Ok;

            // Get the stream ID.
            int streamID = 0;

            try
            {
                try
                {
                    hr = pSourceSD.GetStreamIdentifier(out streamID); // Just for debugging, ignore any failures.
                    MFError.ThrowExceptionForHR(hr);
                }
                catch
                {
                    TRACE("IMFStreamDescriptor::GetStreamIdentifier" + hr.ToString());
                }

                // Get the media type handler for the stream.
                hr = pSourceSD.GetMediaTypeHandler(out pHandler);
                MFError.ThrowExceptionForHR(hr);

                // Get the major media type.
                hr = pHandler.GetMajorType(out guidMajorType);
                MFError.ThrowExceptionForHR(hr);

                // Create a downstream node.
                hr = MFExtern.MFCreateTopologyNode(MFTopologyType.OutputNode, out pNode);
                MFError.ThrowExceptionForHR(hr);

                // Create an IMFActivate object for the renderer, based on the media type.
                if (MFMediaType.Audio == guidMajorType)
                {
                    // Create the audio renderer.
                    TRACE(string.Format("Stream {0}: audio stream", streamID));
                    hr = MFExtern.MFCreateAudioRendererActivate(out pRendererActivate);
                    MFError.ThrowExceptionForHR(hr);
                }
                else if (MFMediaType.Video == guidMajorType)
                {
                    // Create the video renderer.
                    TRACE(string.Format("Stream {0}: video stream", streamID));
                    hr = MFExtern.MFCreateVideoRendererActivate(IntPtr.Zero, out pRendererActivate);
                    MFError.ThrowExceptionForHR(hr);
                }
                else
                {
                    TRACE(string.Format("Stream {0}: Unknown format", streamID));
                    throw new COMException("Unknown format", E_Fail);
                }

                // Set the IActivate object on the output node.
                hr = pNode.SetObject(pRendererActivate);
                MFError.ThrowExceptionForHR(hr);

                // Return the IMFTopologyNode pointer to the caller.
                ppNode = pNode;
            }
            catch
            {
                // If we failed, release the pNode
                SafeRelease(pNode);
                throw;
            }
            finally
            {
                // Clean up.
                SafeRelease(pHandler);
                SafeRelease(pRendererActivate);
            }
        }

        #region IMFAsyncCallback Members

        int IMFAsyncCallback.GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwFlags = MFASync.FastIOProcessingCallback;
            pdwQueue = MFAsyncCallbackQueue.Standard;
            //throw new COMException("IMFAsyncCallback.GetParameters not implemented in Player", E_NotImplemented);

            return S_Ok;
        }

        int IMFAsyncCallback.Invoke(IMFAsyncResult pResult)
        {
            int hr;
            IMFMediaEvent pEvent = null;
            MediaEventType meType = MediaEventType.MEUnknown;  // Event type
            int hrStatus = 0;           // Event status

            try
            {
                // Get the event from the event queue.
                hr = m_pSession.EndGetEvent(pResult, out pEvent);
                MFError.ThrowExceptionForHR(hr);

                // Get the event type.
                hr = pEvent.GetType(out meType);
                MFError.ThrowExceptionForHR(hr);

                // Get the event status. If the operation that triggered the event did
                // not succeed, the status is a failure code.
                hr = pEvent.GetStatus(out hrStatus);
                MFError.ThrowExceptionForHR(hr);

#if false
                TRACE(string.Format("Media event: " + meType.ToString()));

                // Check if the async operation succeeded.
                if (Succeeded(hrStatus))
                {
                    // Switch on the event type. Update the internal state of the CPlayer as needed.
                    switch (meType)
                    {
                        case MediaEventType.MESessionTopologyStatus:
                            // Get the status code.
                            int i;
                            hr = pEvent.GetUINT32(MFAttributesClsid.MF_EVENT_TOPOLOGY_STATUS, out i);
                            MFError.ThrowExceptionForHR(hr);
                            TopoStatus = (MFTopoStatus)i;
                            switch (TopoStatus)
                            {
                                case MFTopoStatus.Ready:
                                    OnTopologyReady(pEvent);
                                    break;
                                default:
                                    // Nothing to do.
                                    break;
                            }
                            break;

                        case MediaEventType.MESessionStarted:
                            OnSessionStarted(pEvent);
                            break;

                        case MediaEventType.MESessionPaused:
                            OnSessionPaused(pEvent);
                            break;

                        case MediaEventType.MESessionClosed:
                            OnSessionClosed(pEvent);
                            break;

                        case MediaEventType.MEEndOfPresentation:
                            OnPresentationEnded(pEvent);
                            break;
                    }
                }
                else
                {
                    // The async operation failed. Notify the application
                    NotifyError(hrStatus);
                }
#endif
            }
            finally
            {
                // Request another event.
                if (meType != MediaEventType.MESessionClosed)
                {
                    hr = m_pSession.BeginGetEvent(this, null);
                    MFError.ThrowExceptionForHR(hr);
                }

                SafeRelease(pEvent);
            }

            return S_Ok;
        }

        #endregion

        #region Member Variables

        protected IMFMediaSession m_pSession;
        protected IMFMediaSource m_pSource;
        protected bool m_Done = false;

        #endregion

        public int NotifyRelease()
        {
            m_Done = true;
            return 0;
        }
    }
}
