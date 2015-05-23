using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Threading;

namespace Testv30
{
    class IMFMediaEngineExtensionTest : COMBase, IMFMediaEngineExtension, IMFMediaEngineSrcElements, IMFMediaEngineNotify
    {
        int m_Status = 0;

        public void DoTests()
        {
            int hr;
            IMFMediaEngine m_spMediaEngine;

            // Create the class factory for the Media Engine.
            IMFMediaEngineClassFactory spFactory = new MFMediaEngineClassFactory() as IMFMediaEngineClassFactory;

            // Set configuration attribiutes.
            IMFAttributes spAttributes;
            hr = MFExtern.MFCreateAttributes(out spAttributes, 2);
            MFError.ThrowExceptionForHR(hr);

            hr = spAttributes.SetUnknown(MFAttributesClsid.MF_MEDIA_ENGINE_CALLBACK, this);
            MFError.ThrowExceptionForHR(hr);

            hr = spAttributes.SetUnknown(MFAttributesClsid.MF_MEDIA_ENGINE_EXTENSION, this);
            MFError.ThrowExceptionForHR(hr);

            hr = spAttributes.SetUINT64(MFAttributesClsid.MF_MEDIA_ENGINE_PLAYBACK_HWND, IntPtr.Zero.ToInt64());
            MFError.ThrowExceptionForHR(hr);

            // Create the Media Engine.
            hr = spFactory.CreateInstance(MF_MEDIA_ENGINE_CREATEFLAGS.AudioOnly, spAttributes, out m_spMediaEngine);
            MFError.ThrowExceptionForHR(hr);

            hr = m_spMediaEngine.SetSourceElements(this);
            MFError.ThrowExceptionForHR(hr);

            while ((m_Status & 16) == 0)
                ;

            // While I would like to get to 31, I can't think of a good way 
            // to get IMFMediaEngine to cancel.  I would expect it to call
            // CanPlay (since it logs an event by that name), but it doesn't.
            //Debug.Assert(m_Status == 31);

            // Best I can do.
            Debug.Assert(m_Status == 26);
        }

        #region IMFMediaEngineExtension

        public int CanPlayType(bool AudioOnly, string MimeType, out MF_MEDIA_ENGINE_CANPLAY pAnswer)
        {
            m_Status |= 1;
            throw new NotImplementedException();
        }

        public int BeginCreateObject(string bstrURL, IMFByteStream pByteStream, MFObjectType type, out object ppIUnknownCancelCookie, IMFAsyncCallback pCallback, object punkState)
        {
            m_Status |= 2;

            int hr = S_Ok;
            Debug.WriteLine(string.Format("{0} {1} {2}", bstrURL, type, pByteStream == null));
            ppIUnknownCancelCookie = null;

            if (type == MFObjectType.ByteStream)
            {
                IMFAsyncResult res;
                hr = MFExtern.MFCreateAsyncResult(this, pCallback, punkState, out res);
                if (Succeeded(hr))
                {
                    hr = pCallback.Invoke(res);
                }
            }
            else
            {
                hr = -1;
            }

            return hr;
        }

        public int CancelObjectCreation(object pIUnknownCancelCookie)
        {
            m_Status |= 4;

            throw new NotImplementedException();
        }

        public int EndCreateObject(IMFAsyncResult pResult, out object ppObject)
        {
            m_Status |= 8;

            Debug.WriteLine("END");

            IMFByteStream bs;
            int hr = MFExtern.MFCreateFile(MFFileAccessMode.Read, MFFileOpenMode.FailIfNotExist, MFFileFlags.None, Program.File1, out bs);
            ppObject = bs;
            return hr;
        }

        #endregion

        #region IMFMediaEngineSrcElements

        public int GetLength()
        {
            return 1;
        }

        public int GetURL(int index, out string pURL)
        {
            switch (index)
            {
                case 0:
                    pURL = @"moo://asdf.gooba";
                    break;
                case 1:
                    pURL = Program.File2;
                    break;
                default:
                    pURL = null;
                    return -1;
            }
            return 0;
        }

        public int GetType(int index, out string pType)
        {
            switch (index)
            {
                case 0:
                    pType = null;
                    break;
                case 1:
                    pType = "audio/mp4";
                    break;
                default:
                    pType = null;
                   return -1;
            }
            return 0;
        }

        public int GetMedia(int index, out string pMedia)
        {
            switch (index)
            {
                case 0:
                    pMedia = "maba";
                    break;
                case 1:
                    pMedia = "mbab";
                    break;
                default:
                    pMedia = null;
                    return -1;
            }
            return 0;
        }

        public int AddElement(string pURL, string pType, string pMedia)
        {
            throw new NotImplementedException();
        }

        public int RemoveAllElements()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMFMediaEngineNotify

        public int EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            if (eventid == MF_MEDIA_ENGINE_EVENT.Error)
            {
                Debug.WriteLine(string.Format("Error {0} 0x{1:x} ({2})", (MF_MEDIA_ENGINE_ERR)param1.ToInt32(), param2, MFError.GetErrorText(param2)));
            }
            else if (eventid == MF_MEDIA_ENGINE_EVENT.CanPlayThrough)
            {
                Debug.WriteLine(eventid);
                m_Status |= 16;
            }
            else
            {
                Debug.WriteLine(eventid);
            }

            return 0;
        }

        #endregion
    }
}
