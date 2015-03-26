using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Testv22
{
    class IMFTopologyNodeAttributeEditorTest: COMBase, IMFSampleGrabberSinkCallback, IMFAsyncCallback
    {
        long m_Last = 0;

        public void DoTests()
        {
            // IMFTopologyNodeAttributeEditor requires a running topology.  After we build
            // the topology and call IMFMediaSession::Start, we call TestIt() which uses
            // IMFTopologyNodeAttributeEditor::UpdateNodeAttributes to reset the stop point.

            RunSampleGrabber(@"c:\sourceforge\mflib\test\media\AspectRatio4x3.wmv");

            // The total file is ~45,000,000.  We are sending 2 structs: the first says stop
            // after 5,000,000 and the second says stop after 25,000,000.  It is expected
            // that the second will overwrite the first, so we should stop at something
            // less than 25,000,000.
            // If you change the call to UpdateNodeAttributes to only process 1 of the
            // structs, m_Last should be 5,000,000.  If you comment out UpdateNodeAttributes,
            // this will be ~45,000,000.
            Debug.Assert(m_Last < 25000000);
        }

        void TestIt(IMFMediaSession pSession, IMFTopology top)
        {
            int hr;

            IMFGetService gs = (IMFGetService)pSession;
            object o;
            hr = gs.GetService(MFServices.MF_TOPONODE_ATTRIBUTE_EDITOR_SERVICE, typeof(IMFTopologyNodeAttributeEditor).GUID, out o);

            IMFTopologyNodeAttributeEditor tnae = (IMFTopologyNodeAttributeEditor)o;

            MFTopoNodeAttributeUpdate[] upd = new MFTopoNodeAttributeUpdate[2];
            upd[0] = new MFTopoNodeAttributeUpdate();
            upd[1] = new MFTopoNodeAttributeUpdate();

            upd[0].guidAttributeKey = MFAttributesClsid.MF_TOPONODE_MEDIASTOP;
            upd[0].attrType = MFAttributeType.Uint64;
            upd[0].NodeId = 0;
            upd[0].u64 = 5000000;

            upd[1].guidAttributeKey = MFAttributesClsid.MF_TOPONODE_MEDIASTOP;
            upd[1].attrType = MFAttributeType.Uint64;
            upd[1].NodeId = 0;
            upd[1].u64 = 25000000;

            long pID;
            hr = top.GetTopologyID(out pID);

            short si;
            hr = top.GetNodeCount(out si);
            MFError.ThrowExceptionForHR(hr);

            for (short x = 0; x < si; x++)
            {
                IMFTopologyNode n;
                hr = top.GetNode(x, out n);
                MFError.ThrowExceptionForHR(hr);

                long l;
                hr = n.GetTopoNodeID(out l);
                MFError.ThrowExceptionForHR(hr);

                upd[0].NodeId = l;
                upd[1].NodeId = l;

                hr = tnae.UpdateNodeAttributes(pID, 2, upd);
                MFError.ThrowExceptionForHR(hr);
            }
        }

        int depth = 0;
        void ShowNames(IMFAttributes ia)
        {
            int hr;

            int c;
            hr = ia.GetCount(out c);

            PropVariant pv = new PropVariant();
            Guid g;
            for (int y = 0; y < c; y++)
            {
                hr = ia.GetItemByIndex(y, out g, pv);

                for (int z = 0; z < depth; z++)
                    Console.Write("   ");

                if (pv.GetMFAttributeType() == MFAttributeType.IUnknown)
                {
                    Console.WriteLine(MFAttributesClsid.LookupName(g));

                    IMFAttributes ia2 = pv.GetIUnknown() as IMFAttributes;
                    if (ia2 != null)
                    {
                        depth++;
                        ShowNames(ia2);
                        depth--;
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("{0} ({1}): {2}", MFAttributesClsid.LookupName(g), pv.GetMFAttributeType(), pv.ToString()));
                }
            }
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

                hr = pSession.GetEvent(0, out pEvent);
                MFError.ThrowExceptionForHR(hr);

                hr = pEvent.GetStatus(out hrStatus);
                MFError.ThrowExceptionForHR(hr);

                hr = pEvent.GetType(out met);
                MFError.ThrowExceptionForHR(hr);

                if (met == MediaEventType.MESessionStarted)
                {
                    TestIt(pSession, pTopology);
                }

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

            return S_Ok;
        }

        public int OnProcessSample(Guid guidMajorMediaType, int dwSampleFlags, long llSampleTime, long llSampleDuration, IntPtr pSampleBuffer, int dwSampleSize)
        {
            m_Last = llSampleTime;
            Console.WriteLine(m_Last);
            return S_Ok;
        }

        public int OnShutdown()
        {
            return S_Ok;
        }

        public int GetClientCertificate(string pszUrl, out IntPtr ppbData, out int pcbData)
        {
            byte[] buff;
            int l;
            IntPtr t;
#if false
            //X509Certificate xc = new X509Certificate(@"c:\inetpub\client.p12.pfx", "a");
            X509Certificate xc = new X509Certificate(@"c:\inetpub\tempClientcert.crt", "");
            //X509Certificate xc = new X509Certificate(@"c:\inetpub\client.cer", "");
            buff = xc.GetRawCertData();
            l = buff.Length;

#else
            //BinaryReader br = new BinaryReader(new FileStream(@"c:\inetpub\client.cer", FileMode.Open));
            BinaryReader br = new BinaryReader(new FileStream(@"c:\inetpub\clientcertblob.txt", FileMode.Open));
            l = (int)br.BaseStream.Length;

            buff = new byte[l];
            br.Read(buff, 0, l);
#endif

            t = Marshal.AllocCoTaskMem(l);
            Marshal.Copy(buff, 0, t, l);

            pcbData = l;
            ppbData = t;

            return S_Ok;
        }

        public int BeginGetClientCertificate(string pszUrl, IMFAsyncCallback pCallback, object pState)
        {
            throw new NotImplementedException();
        }

        public int EndGetClientCertificate(IMFAsyncResult pResult, IntPtr ppbData, out int pcbData)
        {
            throw new NotImplementedException();
        }

        public int GetCertificatePolicy(string pszUrl, out bool pfOverrideAutomaticCheck, out bool pfClientCertificateAvailable)
        {
            pfOverrideAutomaticCheck = true;
            pfClientCertificateAvailable = true;
            return S_Ok;
        }

        public int OnServerCertificate(string pszUrl, IntPtr pbData, int cbData, out bool pfIsGood)
        {
            pfIsGood = true;
            return S_Ok;
        }

        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            throw new NotImplementedException();
        }

        public int Invoke(IMFAsyncResult pAsyncResult)
        {
            throw new NotImplementedException();
        }
    }
}
