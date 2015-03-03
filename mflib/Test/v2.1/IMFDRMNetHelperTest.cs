/* https://msdn.microsoft.com/en-us/library/windows/desktop/dd368789%28v=vs.85%29.aspx */

using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace Testv21
{
    // Untestable: No docs for format of DRM objects
    class IMFDRMNetHelperTest : COMBase
    {
        public void DoTests()
        {
#if false
            int hr;

            IMFMediaSink ms = null;
            IMFDRMNetHelper dh;

            IMFByteStream bs;
            hr = MFExtern.MFCreateFile(MFFileAccessMode.Write, MFFileOpenMode.DeleteIfExist, MFFileFlags.NoBuffering, "IMFDRMNetHelperTest.asf", out bs);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateASFStreamingMediaSink(bs, out ms);
            MFError.ThrowExceptionForHR(hr);

            IMFASFContentInfo ci = (IMFASFContentInfo)ms;
            IPropertyStore ps;
            hr = ci.GetEncodingConfigurationPropertyStore(0, out ps);
            MFError.ThrowExceptionForHR(hr);

            PropVariant pv = new PropVariant((uint)MFSinkWMDRMAction.Transcode);
            hr = ps.SetValue(MFPKEY_ASFMEDIASINK.MFPKEY_ASFMEDIASINK_DRMACTION, pv);
            MFError.ThrowExceptionForHR(hr);

            dh = (IMFDRMNetHelper)ms;

            int i;
            IntPtr p = IntPtr.Zero;
            hr = dh.GetChainedLicenseResponse(out p, out i);
            Debug.Assert(hr == E_NotImplemented); // Per docs: Not implemented in this release.

            IntPtr a = Marshal.AllocCoTaskMem(100);
            IntPtr b = Marshal.AllocCoTaskMem(100);
            IntPtr c = Marshal.AllocCoTaskMem(100);
            IntPtr d = Marshal.AllocCoTaskMem(100);
            string k;
#if true
            hr = dh.ProcessLicenseRequest(a, 100, out b, out i, out k);
#else

#if false
            hr = dh.ProcessLicenseRequest(a, 100, out b, out i, out k);
            MFError.ThrowExceptionForHR(hr);
#else
            hr = dh.ProcessLicenseRequest(a, 100, b, c, d);
#endif
#endif
#endif
        }
    }
}
