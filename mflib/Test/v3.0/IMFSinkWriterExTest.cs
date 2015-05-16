using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.ReadWrite;
using MediaFoundation.Transform;

namespace Testv30
{
    class IMFSinkWriterExTest
    {
        const uint VIDEO_WIDTH = 640;
        const uint VIDEO_HEIGHT = 480;
        const uint VIDEO_FPS = 25;
        const uint VIDEO_BIT_RATE = 800000;
        Guid VIDEO_ENCODING_FORMAT = MFMediaType.WMV3;
        Guid VIDEO_INPUT_FORMAT = MFMediaType.RGB32;
        const uint VIDEO_PELS = VIDEO_WIDTH * VIDEO_HEIGHT;
        const uint VIDEO_FRAME_COUNT = 10 * VIDEO_FPS;

        public void DoTests()
        {
            IMFSinkWriter pWrite;
            int iStream;

            InitializeSinkWriter(out pWrite, out iStream);

            IMFSinkWriterEx pWriteEx = pWrite as IMFSinkWriterEx;

            Guid g;
            IMFTransform t;
            int hr = pWriteEx.GetTransformForStream(iStream, 0, out g, out t);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(g == MFTransformCategory.MFT_CATEGORY_VIDEO_ENCODER);
            Program.IsA(t, typeof(IMFTransform).GUID);
        }
        private void InitializeSinkWriter(out IMFSinkWriter ppWriter, out int pStreamIndex)
        {
            ppWriter = null;
            pStreamIndex = -1;

            IMFMediaType pMediaTypeOut = null;
            IMFMediaType pMediaTypeIn = null;
            int hr;

            IMFAttributes ia2;
            hr = MFExtern.MFCreateAttributes(out ia2, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = ia2.SetUnknown(MFAttributesClsid.MF_SINK_WRITER_ASYNC_CALLBACK, this);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateSinkWriterFromURL("output.wmv", null, ia2, out ppWriter);
            MFError.ThrowExceptionForHR(hr);

            // Set the output media type.
            hr = MFExtern.MFCreateMediaType(out pMediaTypeOut);
            MFError.ThrowExceptionForHR(hr);

            hr = pMediaTypeOut.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            MFError.ThrowExceptionForHR(hr);

            hr = pMediaTypeOut.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, VIDEO_ENCODING_FORMAT);
            MFError.ThrowExceptionForHR(hr);

            hr = pMediaTypeOut.SetUINT32(MFAttributesClsid.MF_MT_AVG_BITRATE, (int)VIDEO_BIT_RATE);
            MFError.ThrowExceptionForHR(hr);

            hr = pMediaTypeOut.SetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, (int)MFVideoInterlaceMode.Progressive);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(pMediaTypeOut, MFAttributesClsid.MF_MT_FRAME_SIZE, (int)VIDEO_WIDTH, (int)VIDEO_HEIGHT);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(pMediaTypeOut, MFAttributesClsid.MF_MT_FRAME_RATE, (int)VIDEO_FPS, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(pMediaTypeOut, MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = ppWriter.AddStream(pMediaTypeOut, out pStreamIndex);
            MFError.ThrowExceptionForHR(hr);

            // Set the input media type.
            hr = MFExtern.MFCreateMediaType(out pMediaTypeIn);
            MFError.ThrowExceptionForHR(hr);

            hr = pMediaTypeIn.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            MFError.ThrowExceptionForHR(hr);

            hr = pMediaTypeIn.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, VIDEO_INPUT_FORMAT);
            MFError.ThrowExceptionForHR(hr);

            hr = pMediaTypeIn.SetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, (int)MFVideoInterlaceMode.Progressive);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(pMediaTypeIn, MFAttributesClsid.MF_MT_FRAME_SIZE, (int)VIDEO_WIDTH, (int)VIDEO_HEIGHT);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(pMediaTypeIn, MFAttributesClsid.MF_MT_FRAME_RATE, (int)VIDEO_FPS, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(pMediaTypeIn, MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
            MFError.ThrowExceptionForHR(hr);

            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = ppWriter.SetInputMediaType(pStreamIndex, pMediaTypeIn, ia);
            MFError.ThrowExceptionForHR(hr);

            // Tell the sink writer to start accepting data.
            hr = ppWriter.BeginWriting();
            MFError.ThrowExceptionForHR(hr);
        }

    }
}
