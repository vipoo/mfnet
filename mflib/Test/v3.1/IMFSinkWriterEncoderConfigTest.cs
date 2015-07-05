using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.ReadWrite;
using MediaFoundation.Transform;

namespace Testv31
{
    class IMFSinkWriterEncoderConfigTest
    {
        // Format constants
        const uint VIDEO_WIDTH = 640;
        const uint VIDEO_HEIGHT = 480;
        const uint VIDEO_FPS = 25;
        const uint VIDEO_BIT_RATE = 800000;
        Guid VIDEO_ENCODING_FORMAT = MFMediaType.WMV3;
        Guid VIDEO_INPUT_FORMAT = MFMediaType.RGB32;
        const uint VIDEO_PELS = VIDEO_WIDTH * VIDEO_HEIGHT;
        const uint VIDEO_FRAME_COUNT = 10 * VIDEO_FPS;

        private IMFSinkWriterEncoderConfig m_ppWriterEC;
        private IMFMediaType m_pMediaTypeOut;

        public void DoTests()
        {
            int hr;

            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 5);

            InitializeWriter();

            hr = m_ppWriterEC.SetTargetMediaType(0, m_pMediaTypeOut, ia);
            MFError.ThrowExceptionForHR(hr);

            hr = m_ppWriterEC.PlaceEncodingParameters(0, ia);
            MFError.ThrowExceptionForHR(hr);
        }

        void InitializeWriter()
        {
            int hr;
            IMFSinkWriter ppWriter;
            IMFAttributes ia = null;

            hr = MFExtern.MFCreateSinkWriterFromURL("output.wmv", null, ia, out ppWriter);
            MFError.ThrowExceptionForHR(hr);

            m_ppWriterEC = ppWriter as IMFSinkWriterEncoderConfig;

            // Set the output media type.
            hr = MFExtern.MFCreateMediaType(out m_pMediaTypeOut);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pMediaTypeOut.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pMediaTypeOut.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, VIDEO_ENCODING_FORMAT);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pMediaTypeOut.SetUINT32(MFAttributesClsid.MF_MT_AVG_BITRATE, (int)VIDEO_BIT_RATE);
            MFError.ThrowExceptionForHR(hr);

            hr = m_pMediaTypeOut.SetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, (int)MFVideoInterlaceMode.Progressive);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(m_pMediaTypeOut, MFAttributesClsid.MF_MT_FRAME_SIZE, (int)VIDEO_WIDTH, (int)VIDEO_HEIGHT);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(m_pMediaTypeOut, MFAttributesClsid.MF_MT_FRAME_RATE, (int)VIDEO_FPS, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFSetAttribute2UINT32asUINT64(m_pMediaTypeOut, MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
            MFError.ThrowExceptionForHR(hr);

            int pStreamIndex;
            hr = ppWriter.AddStream(m_pMediaTypeOut, out pStreamIndex);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaType pMediaTypeIn;

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
