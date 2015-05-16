using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.ReadWrite;
using MediaFoundation.Transform;
using System.Runtime.InteropServices;

namespace Testv30
{
    class IMFSourceReaderExTest
    {
        IMFSourceReaderEx sre;
        public void DoTests()
        {
            Init();

            int hr;
            Guid g;

            //////////////////////
            IMFMediaType imfMediaType;

            int iStream = 1;

            hr = sre.GetNativeMediaType(iStream, 0, out imfMediaType);
            MFError.ThrowExceptionForHR(hr);

            imfMediaType.GetMajorType(out g);

            MF_SOURCE_READER_FLAG sf;
            hr = sre.SetNativeMediaType(iStream, imfMediaType, out sf);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(sf == MF_SOURCE_READER_FLAG.None);

            //////////////////////

            object o;

            // Need to find something that supports this media type.  framecounterasync
            // accepts anything.
            Type tc = Type.GetTypeFromCLSID(new Guid("2137B262-D5D7-4F81-AE90-D3A2ECC66E14")); // c# framecounterasync
            o = Activator.CreateInstance(tc);

            hr = sre.AddTransformForStream(iStream, o);
            MFError.ThrowExceptionForHR(hr);

            IMFTransform t;
            hr = sre.GetTransformForStream(iStream, 0, out g, out t);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(t == o);

            hr = sre.RemoveAllTransformsForStream(iStream);
            MFError.ThrowExceptionForHR(hr);

            hr = sre.GetTransformForStream(iStream, 0, out g, out t);
            Debug.Assert(hr == MFError.MF_E_INVALIDINDEX);
        }

        void Init()
        {
            int hr;

            IMFMediaSource ms;
            object pSource;
            IMFSourceResolver sr;
            MFObjectType pObjectType;

            hr = MFExtern.MFCreateSourceResolver(out sr);
            MFError.ThrowExceptionForHR(hr);

            hr = sr.CreateObjectFromURL(
                Program.File1,
                MFResolution.MediaSource,
                null,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            ms = pSource as IMFMediaSource;

            IMFSourceReader rdr;
            hr = MFExtern.MFCreateSourceReaderFromMediaSource(ms, null, out rdr);
            MFError.ThrowExceptionForHR(hr);

            sre = rdr as IMFSourceReaderEx;
        }
    }
}
