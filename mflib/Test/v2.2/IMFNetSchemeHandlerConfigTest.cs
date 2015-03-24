using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv22
{
    class IMFNetSchemeHandlerConfigTest
    {
        public void DoTests()
        {
            object o;
            int hr;

            hr = MFExtern.MFCreateNetSchemePlugin(typeof(IMFSchemeHandler).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            IMFNetSchemeHandlerConfig nsc = (IMFNetSchemeHandlerConfig)o;

            int i;
            hr = nsc.GetNumberOfSupportedProtocols(out i);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(i != 0);

            for (int x = 0; x < i; x++)
            {
                MFNetSourceProtocolType pt;
                hr = nsc.GetSupportedProtocolType(x, out pt);
                MFError.ThrowExceptionForHR(hr);
            }

            hr = nsc.ResetProtocolRolloverSettings();
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
