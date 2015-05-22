using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Threading;

namespace Testv30
{
    class IMFMediaEngineNotifyTest : COMBase, IMFMediaEngineNotify
    {
        int m_Events = 0;

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

            hr = m_spMediaEngine.Pause();
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(1000);

            Debug.Assert(m_Events > 0);
        }

        public int EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            Debug.WriteLine(eventid);
            m_Events++;

            return 0;
        }
    }
}
