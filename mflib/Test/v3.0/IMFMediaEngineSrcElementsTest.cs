using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Threading;

namespace Testv30
{
    class IMFMediaEngineSrcElementsTest : COMBase, IMFMediaEngineSrcElements, IMFMediaEngineNotify
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

            hr = spAttributes.SetUINT64(MFAttributesClsid.MF_MEDIA_ENGINE_PLAYBACK_HWND, IntPtr.Zero.ToInt64());
            MFError.ThrowExceptionForHR(hr);

            // Create the Media Engine.
            hr = spFactory.CreateInstance(MF_MEDIA_ENGINE_CREATEFLAGS.AudioOnly, spAttributes, out m_spMediaEngine);
            MFError.ThrowExceptionForHR(hr);

            hr = m_spMediaEngine.SetSourceElements(this);
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(2000);

            // IMFMediaEngine doesn't seem to call AddElement or RemoveAllElements.  But why would it?
            // Also, GetMedia isn't called.  It's not clear to me what this does and when (or if)
            // IMFMediaEngine might ever need it.
            //Debug.Assert(m_Status == 511);

            // These seem like the important ones, and they work.
            Debug.Assert(m_Status == 31);
        }

        public int GetLength()
        {
            m_Status |= 1;
            return 2;
        }

        public int GetURL(int index, out string pURL)
        {
            switch (index)
            {
                case 0:
                    m_Status |= 2;
                    pURL = @"hm://asdf.gooba";
                    break;
                case 1:
                    m_Status |= 4;
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
                    m_Status |= 8;
                    pType = null;
                    break;
                case 1:
                    m_Status |= 16;
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
                    m_Status |= 32;
                    pMedia = "maba";
                    break;
                case 1:
                    m_Status |= 64;
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
            m_Status |= 128;
            throw new NotImplementedException();
        }

        public int RemoveAllElements()
        {
            m_Status |= 256;
            throw new NotImplementedException();
        }

        public int EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            Debug.WriteLine(eventid);
            return 0;
        }
    }
}
