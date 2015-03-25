using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv22
{
    class IMFSAMIStyleTest
    {
        public void DoTests()
        {
            int hr;
            IMFSAMIStyle ss = GetInterface() as IMFSAMIStyle;

            int i;
            hr = ss.GetStyleCount(out i);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(i == 2);

            string s;
            hr = ss.GetSelectedStyle(out s);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(s == "Standard");

            PropVariant pv = new PropVariant();
            hr = ss.GetStyles(pv);
            MFError.ThrowExceptionForHR(hr);

            string []sa = pv.GetStringArray();
            Debug.Assert(sa.Length == i);

            hr = ss.SetSelectedStyle(sa[1]);
            MFError.ThrowExceptionForHR(hr);

            hr = ss.GetSelectedStyle(out s);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(s == "Youth");
        }

        private object GetInterface()
        {
            int hr;

            IMFSourceResolver m_sr;

            hr = MFExtern.MFCreateSourceResolver(out m_sr);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType pObjectType;
            object pSource;

            hr = m_sr.CreateObjectFromURL(
                @"C:\SourceForge\mflib\Test\Media\test.sami",
                MFResolution.MediaSource,
                null,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            IMFGetService gs = (IMFGetService)pSource;

            object o;
            hr = gs.GetService(MFServices.MF_SAMI_SERVICE, typeof(IMFSAMIStyle).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            return o;
        }
    }
}
