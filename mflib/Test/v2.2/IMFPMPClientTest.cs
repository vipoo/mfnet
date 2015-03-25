using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Testv22
{
    class IMFPMPClientTest : COMBase, IMFPMPHost
    {
        string FILENAME1 = @"file://c:/sourceforge/mflib/test/media/AspectRatio4x3.wmv";
        int m_state = 0;

        public void DoTests()
        {
            int hr;

            IMFMediaSource ms = GetMediaSource();
            IMFPMPClient pc = (IMFPMPClient)ms;

            // We aren't testing IMFPMPHost here, just IMFPMPClient
            hr = pc.SetPMPHost(this);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(m_state == 1);
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


        public int LockProcess()
        {
            m_state = 1;
            return S_Ok;
        }

        public int UnlockProcess()
        {
            throw new NotImplementedException();
        }

        public int CreateObjectByCLSID(Guid clsid, System.Runtime.InteropServices.ComTypes.IStream pStream, Guid riid, out object ppv)
        {
            throw new NotImplementedException();
        }
    }
}
