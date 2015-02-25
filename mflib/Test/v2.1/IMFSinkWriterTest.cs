/* https://msdn.microsoft.com/en-us/library/windows/desktop/ff819477%28v=vs.85%29.aspx */

using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.ReadWrite;
using System.Runtime.InteropServices;
using System.Threading;

namespace Testv21
{
    class IMFSinkWriterTest : COMBase, IMFSinkWriterCallback
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
        uint []videoFrameBuffer;
        ManualResetEvent m_Done = new ManualResetEvent(false);
        ManualResetEvent m_Done2 = new ManualResetEvent(false);

        private void Green()
        {
            // Set all pixels to green
            for (uint i = 0; i < VIDEO_PELS; ++i)
            {
                videoFrameBuffer[i] = 0x0000FF00;
            }
        }

        private static int MFSetAttribute2UINT32asUINT64(IMFAttributes pAttributes, Guid g, int nNumerator, int nDenominator)
        {
            int hr;
            long ul = nNumerator;

            ul <<= 32;
            ul |= (UInt32)nDenominator;

            hr = pAttributes.SetUINT64(g, ul);
            return hr;
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

            hr = MFSetAttribute2UINT32asUINT64(pMediaTypeOut, MFAttributesClsid.MF_MT_FRAME_SIZE, (int)VIDEO_WIDTH, (int)VIDEO_HEIGHT);
            MFError.ThrowExceptionForHR(hr);

            hr = MFSetAttribute2UINT32asUINT64(pMediaTypeOut, MFAttributesClsid.MF_MT_FRAME_RATE, (int)VIDEO_FPS, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = MFSetAttribute2UINT32asUINT64(pMediaTypeOut, MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
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

            hr = MFSetAttribute2UINT32asUINT64(pMediaTypeIn, MFAttributesClsid.MF_MT_FRAME_SIZE, (int)VIDEO_WIDTH, (int)VIDEO_HEIGHT);
            MFError.ThrowExceptionForHR(hr);

            hr = MFSetAttribute2UINT32asUINT64(pMediaTypeIn, MFAttributesClsid.MF_MT_FRAME_RATE, (int)VIDEO_FPS, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = MFSetAttribute2UINT32asUINT64(pMediaTypeIn, MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
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

        private void WriteFrame(
            IMFSinkWriter pWriter,
            int streamIndex,
            long rtStart,        // Time stamp.
            long rtDuration      // Frame duration.
            )
        {
            IMFSample pSample = null;
            IMFMediaBuffer pBuffer = null;

            const int cbWidth = (int)(4 * VIDEO_WIDTH);
            const int cbBuffer = (int)(cbWidth * VIDEO_HEIGHT);

            IntPtr pData = IntPtr.Zero;

            // Create a new memory buffer.
            int hr = MFExtern.MFCreateMemoryBuffer(cbBuffer, out pBuffer);
            MFError.ThrowExceptionForHR(hr);

            int iMaxLen, iCurLen;
            // Lock the buffer and copy the video frame to the buffer.
            hr = pBuffer.Lock(out pData, out iMaxLen, out iCurLen);
            MFError.ThrowExceptionForHR(hr);

            GCHandle h = GCHandle.Alloc(videoFrameBuffer, GCHandleType.Pinned);

            hr = MFExtern.MFCopyImage(
                pData,                      // Destination buffer.
                (int)cbWidth,                    // Destination stride.
                h.AddrOfPinnedObject(),    // First row in source image.
                cbWidth,                    // Source stride.
                cbWidth,                    // Image width in bytes.
                (int)VIDEO_HEIGHT                // Image height in pixels.
                );
            hr = pBuffer.Unlock();
            MFError.ThrowExceptionForHR(hr);

            // Set the data length of the buffer.
            hr = pBuffer.SetCurrentLength(cbBuffer);
            MFError.ThrowExceptionForHR(hr);

            // Create a media sample and add the buffer to the sample.
            hr = MFExtern.MFCreateSample(out pSample);
            MFError.ThrowExceptionForHR(hr);

            hr = pSample.AddBuffer(pBuffer);
            MFError.ThrowExceptionForHR(hr);

            // Set the time stamp and the duration.
            hr = pSample.SetSampleTime(rtStart);
            MFError.ThrowExceptionForHR(hr);

            hr = pSample.SetSampleDuration(rtDuration);
            MFError.ThrowExceptionForHR(hr);

            // Send the sample to the Sink Writer.
            hr = pWriter.WriteSample(streamIndex, pSample);
            MFError.ThrowExceptionForHR(hr);

            SafeRelease(pSample);
            SafeRelease(pBuffer);
        }

        public void DoTests()
        {
            videoFrameBuffer = new uint[VIDEO_PELS];

            int hr;
            IMFSinkWriter pSinkWriter;
            int StreamIndex;

            Green();

            InitializeSinkWriter(out pSinkWriter, out StreamIndex);

            // Send frames to the sink writer.
            long rtStart = 0;
            long rtDuration;

            hr = MFExtern.MFFrameRateToAverageTimePerFrame((int)VIDEO_FPS, 1, out rtDuration);
            MFError.ThrowExceptionForHR(hr);

            for (int i = 0; i < VIDEO_FRAME_COUNT; ++i)
            {
                if (i == 200)
                {
                    hr = pSinkWriter.SendStreamTick(StreamIndex, rtStart);
                    MFError.ThrowExceptionForHR(hr);
                    rtStart += rtDuration;
                }
                else
                {
                    WriteFrame(pSinkWriter, StreamIndex, rtStart, rtDuration);
                    rtStart += rtDuration;
                }
            }
            object o;
            Guid gunk = new Guid("00000000-0000-0000-C000-000000000046");
            hr = pSinkWriter.GetServiceForStream(StreamIndex, Guid.Empty, gunk, out o);
            Debug.Assert(o != null);

            // This can only be tested in IMFSinkWriteCallback
            hr = pSinkWriter.PlaceMarker(StreamIndex, new IntPtr(12));
            MFError.ThrowExceptionForHR(hr);

            m_Done2.WaitOne();

            hr = pSinkWriter.NotifyEndOfSegment(StreamIndex);
            MFError.ThrowExceptionForHR(hr);

            hr = pSinkWriter.Flush(StreamIndex);
            MFError.ThrowExceptionForHR(hr);

            hr = pSinkWriter.Finalize_();
            MFError.ThrowExceptionForHR(hr);

            m_Done.WaitOne();

            MF_SINK_WRITER_STATISTICS stat;
            stat.cb = Marshal.SizeOf(typeof(MF_SINK_WRITER_STATISTICS));

            hr = pSinkWriter.GetStatistics(StreamIndex, out stat);
            MFError.ThrowExceptionForHR(hr);

            // Account for SendStreamTick
            Debug.Assert(stat.qwNumSamplesEncoded == VIDEO_FRAME_COUNT - 1);
        }

        public int OnFinalize(int hrStatus)
        {
            m_Done.Set();
            return S_Ok;
        }

        public int OnMarker(int dwStreamIndex, IntPtr pvContext)
        {
            m_Done2.Set();
            Debug.Assert(pvContext.ToInt32() == 12);

            return S_Ok;
        }
    }
}
