using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv22
{
    class IMFSaveJobTest : COMBase, IMFAsyncCallback
    {
        IMFSaveJob m_sj = null;

        public void DoTests()
        {
            int hr;

            IMFByteStream bs = GetInterface();

            IMFGetService gs = (IMFGetService)bs;

            object o;
            hr = gs.GetService(MFServices.MFNET_SAVEJOB_SERVICE, typeof(IMFSaveJob).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            m_sj = (IMFSaveJob)o;

            IMFByteStream bsout;
            hr = MFExtern.MFCreateFile(MFFileAccessMode.ReadWrite, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, "IMFSAVEJOB.wmv", out bsout);
            MFError.ThrowExceptionForHR(hr);

            hr = m_sj.BeginSave(bsout, this, null);
            MFError.ThrowExceptionForHR(hr);

            int pc;
            do
            {
                hr = m_sj.GetProgress(out pc);
                MFError.ThrowExceptionForHR(hr);
            } while (pc == 0);

            hr = m_sj.CancelSave();
            MFError.ThrowExceptionForHR(hr);

            // Wait for invoke
            while (m_sj != null)
                System.Threading.Thread.Sleep(100);

        }
        private IMFByteStream GetInterface()
        {
            int hr;

            IMFSourceResolver m_sr;

            hr = MFExtern.MFCreateSourceResolver(out m_sr);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType pObjectType;
            object pSource;

            hr = m_sr.CreateObjectFromURL(
                @"http://www.LimeGreenSocks.com/AspectRatio4x3.wmv",
                MFResolution.ByteStream,
                null,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            return pSource as IMFByteStream;

        }

        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            throw new NotImplementedException();
        }

        public int Invoke(IMFAsyncResult pAsyncResult)
        {
            int hr;
            hr = m_sj.EndSave(pAsyncResult);
            m_sj = null;

            return hr;
        }
    }
}
