using MediaFoundation;
using MediaFoundation.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testv31
{
    class TestExterns
    {
        public void DoTests()
        {
            TestWave();
            TestAVI();
        }

        private void TestAVI()
        {
            int hr;

            IMFMediaType pType = null;
            IMFByteStream imfByteStreamWritable = null;

            hr = MFExtern.MFCreateMediaType(out pType);
            MFError.ThrowExceptionForHR(hr);
            hr = pType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            MFError.ThrowExceptionForHR(hr);
            hr = pType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.YUY2);
            MFError.ThrowExceptionForHR(hr);

            MFExtern.MFSetAttributeRatio(pType, MFAttributesClsid.MF_MT_FRAME_SIZE, 640, 480);
            pType.SetUINT32(MFAttributesClsid.MF_MT_AVG_BITRATE, 147456000);
            pType.SetUINT32(MFAttributesClsid.MF_MT_YUV_MATRIX, 2);
            pType.SetUINT32(MFAttributesClsid.MF_MT_VIDEO_LIGHTING, 3);
            pType.SetUINT32(MFAttributesClsid.MF_MT_DEFAULT_STRIDE, 1280);
            pType.SetUINT32(MFAttributesClsid.MF_MT_VIDEO_CHROMA_SITING, 6);
            pType.SetGUID(MFAttributesClsid.MF_MT_AM_FORMAT_TYPE, new Guid("f72a76a0-eb0a-11d0-ace4-0000c0cc16ba"));
            pType.SetUINT32(MFAttributesClsid.MF_MT_FIXED_SIZE_SAMPLES, 1);
            pType.SetUINT32(MFAttributesClsid.MF_MT_VIDEO_NOMINAL_RANGE, 2);
            MFExtern.MFSetAttributeRatio(pType, MFAttributesClsid.MF_MT_FRAME_RATE, 30, 1);
            MFExtern.MFSetAttributeRatio(pType, MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
            pType.SetUINT32(MFAttributesClsid.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
            pType.SetUINT64(MFAttributesClsid.MF_MT_FRAME_RATE_RANGE_MIN, 128849018881);
            pType.SetUINT32(MFAttributesClsid.MF_MT_SAMPLE_SIZE, 614400);
            pType.SetUINT32(MFAttributesClsid.MF_MT_VIDEO_PRIMARIES, 2);
            pType.SetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, (int)MFVideoInterlaceMode.Progressive);
            pType.SetUINT64(MFAttributesClsid.MF_MT_FRAME_RATE_RANGE_MAX, 128849018881);

            hr = MFExtern.MFCreateFile(MFFileAccessMode.ReadWrite, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, "output2.wmv", out imfByteStreamWritable);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaSink imfMediaSinkOut;
            hr = MFExtern.MFCreateAVIMediaSink(
                imfByteStreamWritable,
                pType,
                null,
                out imfMediaSinkOut
            );
            MFError.ThrowExceptionForHR(hr);
        }

        private void TestWave()
        {
            int hr;

            IMFMediaType pType = null;
            IMFByteStream imfByteStreamWritable = null;

            hr = MFExtern.MFCreateMediaType(out pType);
            MFError.ThrowExceptionForHR(hr);
            hr = pType.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Audio);
            MFError.ThrowExceptionForHR(hr);
            hr = pType.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.PCM);
            MFError.ThrowExceptionForHR(hr);

            pType.SetUINT32(MFAttributesClsid.MF_MT_AUDIO_AVG_BYTES_PER_SECOND, 352800);
            pType.SetUINT32(MFAttributesClsid.MF_MT_AUDIO_BLOCK_ALIGNMENT, 8);
            pType.SetUINT32(MFAttributesClsid.MF_MT_AUDIO_NUM_CHANNELS, 2);
            pType.SetUINT32(MFAttributesClsid.MF_MT_AUDIO_CHANNEL_MASK, 3);
            pType.SetUINT32(MFAttributesClsid.MF_MT_AUDIO_SAMPLES_PER_SECOND, 44100);
            pType.SetUINT32(MFAttributesClsid.MF_MT_ALL_SAMPLES_INDEPENDENT, 1);
            pType.SetUINT32(MFAttributesClsid.MF_MT_AUDIO_BITS_PER_SAMPLE, 32);
            pType.SetUINT32(MFAttributesClsid.MF_MT_AUDIO_AVG_BYTES_PER_SECOND, 352800);

            hr = MFExtern.MFCreateFile(MFFileAccessMode.ReadWrite, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, "output.wmv", out imfByteStreamWritable);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaSink imfMediaSinkOut;
            hr = MFExtern.MFCreateWAVEMediaSink(
                imfByteStreamWritable,
                pType,
                out imfMediaSinkOut);
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
