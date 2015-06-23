using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Testv30
{
    class IMFByteStreamTimeSeekTest
    {
#if false
        IMFByteStreamTimeSeek m_bsts;
#endif

        public void DoTests()
        {
#if false
            GetInterface();
            int hr;

            object o;
            IMFByteStream bs = m_bsts as IMFByteStream;
            hr = MFExtern.MFCreateStreamOnMFByteStreamEx(bs, typeof(IMFByteStream).GUID, out o);

            //IMFByteStream bs = m_bsts as IMFByteStream;
            //IntPtr ip = Marshal.AllocCoTaskMem(1000);
            //int i;
            //hr = bs.Read(ip, 1000, out i);
            //MFError.ThrowExceptionForHR(hr);

            bool b;
            hr = m_bsts.IsTimeSeekSupported(out b);
            MFError.ThrowExceptionForHR(hr);

            hr = m_bsts.TimeSeek(0);
            MFError.ThrowExceptionForHR(hr);
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

            m_bsts = pSource as IMFByteStreamTimeSeek;
#endif
        }
    }
}
