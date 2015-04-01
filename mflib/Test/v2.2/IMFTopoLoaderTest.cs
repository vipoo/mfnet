using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv22
{
    class IMFTopoLoaderTest : COMBase
    {
        public void DoTests()
        {
            RunSampleGrabber(@"c:\sourceforge\mflib\test\media\AspectRatio4x3.wmv");
        }
        // Create a media source from a URL.
        private void CreateMediaSource(string pszURL, out IMFMediaSource ppSource)
        {
            IMFSourceResolver pSourceResolver = null;
            object pSource;

            IPropertyStore ps;

            int hr;
            hr = MFExtern.CreatePropertyStore(out ps);
            MFError.ThrowExceptionForHR(hr);

            PropVariant pv = new PropVariant(this);
            PropertyKey pk = new PropertyKey(MFProperties.MFNETSOURCE_SSLCERTIFICATE_MANAGER, 0);

            hr = ps.SetValue(pk, pv);
            MFError.ThrowExceptionForHR(hr);

            // Create the source resolver.
            hr = MFExtern.MFCreateSourceResolver(out pSourceResolver);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType ObjectType;
            hr = pSourceResolver.CreateObjectFromURL(
                pszURL,
                MFResolution.MediaSource,
                ps,
                out ObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr); // 0x80092003

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
            //hr = MFExtern.MFCreateSampleGrabberSinkActivate(pType, this, out pSinkActivate);
            //MFError.ThrowExceptionForHR(hr);

            // To run as fast as possible, set this attribute (requires Windows 7):
            //hr = pSinkActivate.SetUINT32(MFAttributesClsid.MF_SAMPLEGRABBERSINK_IGNORE_CLOCK, 1);
            //MFError.ThrowExceptionForHR(hr);

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
            int hr = S_Ok;
            hr = pSession.SetTopology(0, pTopology);
            MFError.ThrowExceptionForHR(hr);

            IMFTopoLoader tl;
            hr = MFExtern.MFCreateTopoLoader(out tl);
            MFError.ThrowExceptionForHR(hr);

            IMFTopology otopo, otopo2;
            hr = tl.Load(pTopology, out otopo, null);
            MFError.ThrowExceptionForHR(hr);

            hr = tl.Load(pTopology, out otopo2, otopo);
            MFError.ThrowExceptionForHR(hr);

            short sn;
            hr = otopo2.GetNodeCount(out sn);
            Debug.Assert(sn == 2);
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
    }
}
