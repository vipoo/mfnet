using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv22
{
    class IMFRemoteProxyTest
    {
        public void DoTests()
        {
#if false
            int hr;

            IMFActivate ea;
            IMFMediaSession ms;
            hr = MFExtern.MFCreatePMPMediaSession(MFPMPSessionCreationFlags.None, null, out ms, out ea);
            MFError.ThrowExceptionForHR(hr);

            IMFGetService gs = (IMFGetService)ms;

            object o;
            hr = gs.GetService(MFServices.MF_PMP_SERVICE, typeof(IMFPMPHost).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            ///////////////////////////////////////////////////////
            IMFPMPHost ph = (IMFPMPHost)o;
            hr = ph.LockProcess();
            MFError.ThrowExceptionForHR(hr);

            hr = ph.CreateObjectByCLSID(CLSID.CLSID_MFSourceResolver, null, typeof(IMFSourceResolver).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            IMFGetService gs2 = (IMFGetService)o;
            object o2;
            hr = gs2.GetService(MFServices.MF_REMOTE_PROXY, typeof(IMFRemoteProxy).GUID, out o2);
            IMFRemoteProxy rp2 = (IMFRemoteProxy)o2;

            hr = ph.UnlockProcess();
            MFError.ThrowExceptionForHR(hr);
#endif
        }
    }
}
