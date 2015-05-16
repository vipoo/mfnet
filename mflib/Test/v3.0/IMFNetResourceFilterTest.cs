using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFNetResourceFilterTest : COMBase, IMFNetResourceFilter
    {
        int status = 0;

        public void DoTests()
        {
            GetInterface();

            Debug.Assert(status == 3);
        }
        private void GetInterface()
        {
            int hr;

            IMFSourceResolver m_sr;

            IPropertyStore ps;

            hr = MFExtern.CreatePropertyStore(out ps);
            MFError.ThrowExceptionForHR(hr);

            PropVariant pv = new PropVariant(this);
            PropertyKey pk = new PropertyKey(MFProperties.MFNETSOURCE_RESOURCE_FILTER, 0);

            hr = ps.SetValue(pk, pv);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateSourceResolver(out m_sr);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType pObjectType;
            object pSource;

            hr = m_sr.CreateObjectFromURL(
                Program.File2,
                MFResolution.ByteStream,
                ps,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            hr = m_sr.CreateObjectFromURL(
                @"http://mfnet.sf.net",
                MFResolution.ByteStream,
                ps,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);
        }

        public int OnRedirect(string pszUrl, out bool pvbCancel)
        {
            status |= 2;
            pvbCancel = true;

            return 0;
        }

        public int OnSendingRequest(string pszUrl)
        {
            status |= 1;

            return 0;
        }
    }
}
