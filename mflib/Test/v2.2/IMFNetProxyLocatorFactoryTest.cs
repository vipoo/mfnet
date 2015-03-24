using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv22
{
    class IMFNetProxyLocatorFactoryTest : COMBase, IMFNetProxyLocatorFactory 
    {
        string URL = @"http://www.LimeGreenSocks.com/AspectRatio4x3.wmv";
        int state = 0;

        public void DoTests()
        {
            GetInterface();
        }
        private void GetInterface()
        {
            int hr;

            IPropertyStore ps;

            hr = MFExtern.CreatePropertyStore(out ps);
            MFError.ThrowExceptionForHR(hr);

            PropVariant pv = new PropVariant(this);
            PropertyKey pk = new PropertyKey(MFProperties.MFNETSOURCE_PROXYLOCATORFACTORY, 0);

            hr = ps.SetValue(pk, pv);
            MFError.ThrowExceptionForHR(hr);

            IMFSourceResolver m_sr;

            hr = MFExtern.MFCreateSourceResolver(out m_sr);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType pObjectType;
            object pSource;

            hr = m_sr.CreateObjectFromURL(
                URL,
                MFResolution.MediaSource,
                ps,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(state == 1);
        }

        public int CreateProxyLocator(string pszProtocol, out IMFNetProxyLocator ppProxyLocator)
        {
            state = 1;
            ppProxyLocator = new foo();
            return E_Unexpected;
        }
    }
    class foo : COMBase, IMFNetProxyLocator
    {
        int iProxyIndex;
        string[] proxylist;

        public foo()
        {
            proxylist = new string[2];
            proxylist[0] = "moo";
            proxylist[1] = "";

            iProxyIndex = -1;
        }

        public int FindFirstProxy(string pszHost, string pszUrl, bool fReserved)
        {
            iProxyIndex = -1;
            return S_Ok;
        }

        public int FindNextProxy()
        {
            iProxyIndex++;
            return S_Ok;
        }

        public int RegisterProxyResult(int hrOp)
        {
            // "moo" is (probably) not a valid proxy. This gets called to
            // let us know that.
            return S_Ok;
        }

        public int GetCurrentProxy(StringBuilder pszStr, MFInt pcchStr)
        {
            // Normally we are called twice.  Once to get the length, the 
            // other to read the string.
            if (pszStr != null)
            {
                pszStr.Append(proxylist[iProxyIndex]);
            }
            pcchStr.Assign(proxylist[iProxyIndex].Length + 1);
            return S_Ok;
        }

        public int Clone(out IMFNetProxyLocator ppProxyLocator)
        {
            throw new NotImplementedException();
        }
    }
}
