using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFVideoSampleAllocatorExTest
    {
        public void DoTests()
        {
            int hr;

            object o;
            hr = MFExtern.MFCreateVideoSampleAllocatorEx(typeof(IMFVideoSampleAllocatorEx).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            IMFVideoSampleAllocatorEx vsa = o as IMFVideoSampleAllocatorEx;

            IMFMediaType imfMediaType;
            hr = MFExtern.MFCreateMediaType(out imfMediaType);
            MFError.ThrowExceptionForHR(hr);

            hr = imfMediaType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            MFError.ThrowExceptionForHR(hr);

            hr = imfMediaType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.NV12);
            MFError.ThrowExceptionForHR(hr);

            hr = imfMediaType.SetUINT64(MFAttributesClsid.MF_MT_FRAME_SIZE, MFExtern.Pack2UINT32AsUINT64(800, 600));
            MFError.ThrowExceptionForHR(hr);

            IMFAttributes ia;

            hr = MFExtern.MFCreateAttributes(out ia, 5);
            MFError.ThrowExceptionForHR(hr);

            hr = vsa.InitializeSampleAllocatorEx(1, 1, ia, imfMediaType);
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
