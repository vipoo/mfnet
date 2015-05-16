using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFPluginControl2Test
    {
        public void DoTests()
        {
            int hr;

            IMFPluginControl pc;
            hr = MFExtern.MFGetPluginControl(out pc);
            MFError.ThrowExceptionForHR(hr);

            IMFPluginControl2 pc2;
            pc2 = (IMFPluginControl2)pc;

            hr = pc2.SetPolicy(MF_PLUGIN_CONTROL_POLICY.UseAllPlugins);
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
