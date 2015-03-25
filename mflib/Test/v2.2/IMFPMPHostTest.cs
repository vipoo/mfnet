using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv22
{
    class IMFPMPHostTest
    {
        public void DoTests()
        {
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

            // For cross-process, must be registered interface (like IUnknown)
            Guid iunk = new Guid("00000000-0000-0000-C000-000000000046"); 
            hr = ph.CreateObjectByCLSID(typeof(MFSourceResolver).GUID, null, iunk, out o);
            MFError.ThrowExceptionForHR(hr);

            hr = ph.UnlockProcess();
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
