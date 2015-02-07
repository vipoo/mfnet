using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv21
{
    class IMFByteStreamCacheControlTest
    {
        public void DoTests()
        {
            IMFSourceResolver sourceResolver;
            MFObjectType objectType;

            string filename = "http://www.w3schools.com/html/horse.mp3";

            // Get a network byte stream using the source resolver
            int hr = MFExtern.MFCreateSourceResolver(out sourceResolver);
            MFError.ThrowExceptionForHR(hr);

            object createdObject;
            hr = sourceResolver.CreateObjectFromURL(filename, MFResolution.ByteStream, null, out objectType, out createdObject);
            MFError.ThrowExceptionForHR(hr);

            IMFByteStream byteStream = createdObject as IMFByteStream;
            Debug.Assert(byteStream != null);

            // An HTTP byte stream should implement IMFByteStreamCacheControl
            IMFByteStreamCacheControl byteStreamCacheControl = byteStream as IMFByteStreamCacheControl;
            Debug.Assert(byteStreamCacheControl != null);

            // Just test that the interface definition is correct...
            hr = byteStreamCacheControl.StopBackgroundTransfer();
            Debug.Assert(hr == 0);
        }
    }
}
