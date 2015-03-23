using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Threading;

namespace Testv22
{
    class IMFSchemeHandlerTest : COMBase, IMFAsyncCallback
    {
        IMFSchemeHandler m_imfSchemeHandler;
        int m_hr = 0;
        AutoResetEvent are = new AutoResetEvent(false);

        public void DoTests()
        {
            object o;
            int hr;

            hr = MFExtern.MFCreateNetSchemePlugin(typeof(IMFSchemeHandler).GUID, out o); // new
            MFError.ThrowExceptionForHR(hr);

            Program.IsA(o, typeof(IMFSchemeHandler).GUID);

            m_imfSchemeHandler = (IMFSchemeHandler)o;

            m_hr = 0;
            object cancelCookie;
            IMFAsyncCallback cb = (IMFAsyncCallback)this;
            hr = m_imfSchemeHandler.BeginCreateObject("http://www.LimeGreenSocks.com", MFResolution.ByteStream, null, out cancelCookie, cb, null);
            MFError.ThrowExceptionForHR(hr);

            // Immediately cancel it
            hr = m_imfSchemeHandler.CancelObjectCreation(cancelCookie);
            MFError.ThrowExceptionForHR(hr);

            // Wait for the Invoke to get called
            are.WaitOne();
            Debug.Assert(m_hr == E_Abort); // Should be cancelled

            // Try again
            hr = m_imfSchemeHandler.BeginCreateObject("http://www.LimeGreenSocks.com", MFResolution.ByteStream, null, out cancelCookie, cb, null);
            MFError.ThrowExceptionForHR(hr);

            // Wait for the Invoke to get called
            are.WaitOne();
            Debug.Assert(m_hr == 7); // Set by SetStatus in Invoke (below)
        }

        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwQueue = MFAsyncCallbackQueue.Undefined;
            pdwFlags = MFASync.None;

            return E_NotImplemented;
        }

        public int Invoke(IMFAsyncResult pAsyncResult)
        {
            int hr;

            hr = pAsyncResult.SetStatus(7);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType mf;
            object o;
            m_hr = m_imfSchemeHandler.EndCreateObject(pAsyncResult, out mf, out o);
            if (m_hr >= 0)
            {
                Debug.Assert(mf == MFObjectType.ByteStream);
                Program.IsA(o, typeof(IMFByteStream).GUID);
            }

            bool b = are.Set();
            Debug.Assert(b);

            return hr;
        }
    }
}
