using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFSeekInfoTest
    {
        public void DoTests()
        {
            IMFMediaSource pSource;
            IMFSourceResolver sr;
            MFObjectType pObjectType;
            object o;

            int hr = MFExtern.MFCreateSourceResolver(out sr);
            MFError.ThrowExceptionForHR(hr);

            hr = sr.CreateObjectFromURL(
                //Program.File1,
                Program.File2,
                MFResolution.MediaSource,
                null,
                out pObjectType,
                out o);
            MFError.ThrowExceptionForHR(hr);

            pSource = o as IMFMediaSource;

            IMFGetService gs = o as IMFGetService;

            object o2;
            hr = gs.GetService(MFServices.MF_SCRUBBING_SERVICE, typeof(IMFSeekInfo).GUID, out o2);
            MFError.ThrowExceptionForHR(hr);

            IMFSeekInfo si = o2 as IMFSeekInfo;
        }
    }
}
