using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.ReadWrite;

namespace Testv22
{
    class IMFNetCredentialManagerTest : COMBase, IMFNetCredentialManager
    {
        IMFNetCredential m_imfNetCredential;
        int m_Stat = 0;

        // In order for this test to run, you need a web site that
        // prompts for credentials, and a valid set of credentials.
        // In addition, you need to send the credentials unencrypted.
        // These are not valid credentials, since I don't put those
        // into public code.

        // If you need to see this working, either use your own website,
        // or contact me for something that works on LGS.
        string URL = @"http://www.LimeGreenSocks.com/xxxx/AspectRatio4x3.wmv";
        string USERNAME = "asdf\0";
        string USERPASS = "fdsa\0";

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
            PropertyKey pk = new PropertyKey(MFProperties.MFNETSOURCE_CREDENTIAL_MANAGER, 0);

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

            // If everything got hit
            Debug.Assert(m_Stat == 15);
        }
        public int BeginGetCredentials(MFNetCredentialManagerGetParam pParam, IMFAsyncCallback pCallback, object pState)
        {
            int hr;
            IMFNetCredentialCache imfNetCredentialCache;

            m_Stat |= 1;

            hr = MFExtern.MFCreateCredentialCache(out imfNetCredentialCache); // new
            MFError.ThrowExceptionForHR(hr);

            // Eventually we need to return a imfNetCredential.  Create one here.
            MFNetCredentialRequirements req;
            hr = imfNetCredentialCache.GetCredential(pParam.pszUrl, pParam.pszRealm, MFNetAuthenticationFlags.ClearText, out m_imfNetCredential, out req);
            MFError.ThrowExceptionForHR(hr);

            // Normally you'd prompt the user for this (or something)
            string sUser = USERNAME;
            int iSize = Encoding.Unicode.GetByteCount(sUser);
            byte[] bIn = Encoding.Unicode.GetBytes(sUser);
            hr = m_imfNetCredential.SetUser(bIn, iSize, false);
            MFError.ThrowExceptionForHR(hr);

            // Intentionally use a bad password on the first try.
            string sPW;
            if (pParam.nRetries == 0)
                sPW = "x" + USERPASS;
            else
                sPW = USERPASS;
            int iSizep = Encoding.Unicode.GetByteCount(sPW);
            byte[] bInp = Encoding.Unicode.GetBytes(sPW);
            hr = m_imfNetCredential.SetPassword(bInp, iSizep, false);
            MFError.ThrowExceptionForHR(hr);

            // Let the caller continue
            IMFAsyncResult res;
            hr = MFExtern.MFCreateAsyncResult(this, pCallback, pState, out res);
            hr = pCallback.Invoke(res);
            return S_Ok;
        }

        public int EndGetCredentials(IMFAsyncResult pResult, out IMFNetCredential ppCred)
        {
            int hr;

            m_Stat |= 2;

            hr = pResult.SetStatus(0);
            MFError.ThrowExceptionForHR(hr);

            // Send back the credential
            ppCred = m_imfNetCredential;

            return hr;
        }

        // We get called to let us know if the credential worked.
        public int SetGood(IMFNetCredential pCred, bool fGood)
        {
            if (fGood == false)
                m_Stat |= 4;
            else
                m_Stat |= 8;

            return S_Ok;
        }
    }
}
