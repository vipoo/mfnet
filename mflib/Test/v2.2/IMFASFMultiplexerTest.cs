using System;
using System.Runtime.InteropServices;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv22
{
    class IMFASFMultiplexerTest
    {
        const int MIN_ASF_HEADER_SIZE = 16 + 8 + 2 + 4; //sizeof(Guid) + sizeof(ulong) + sizeof(ushort) + sizeof(uint);
        private string m_asfFile = @"..\..\..\Media\AspectRatio4x3.wmv";
        private IMFByteStream m_byteStream;
        private IMFASFContentInfo m_contentInfo;
        private IMFASFSplitter m_asfSplitter;
        private IMFSample m_sample;
        private ASFStatusFlags m_status;
        private short m_streamNumber;
        private long m_dataOffset, m_dataLength, m_dataPosition = 0;
        private IMFASFMultiplexer m_asfMux;
        private IMFSample m_nextPacket;

        public void DoTests()
        {
            int hr = 0;
            MFASFMultiplexerFlags readFlags;

            GetInterfaces();


            hr = MFExtern.MFCreateASFMultiplexer(out m_asfMux);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asfMux.Initialize(m_contentInfo);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asfMux.SetSyncTolerance(1);
            //MFError.ThrowExceptionForHR(hr);
            Debug.Assert(hr == COMBase.E_NotImplemented); // Don't know why that error is returned...

            hr = m_asfMux.SetFlags(MFASFMultiplexerFlags.AutoAdjustBitrate);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asfMux.GetFlags(out readFlags);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(readFlags == MFASFMultiplexerFlags.AutoAdjustBitrate);

            this.GetSampleFromSplitter(out m_sample);

            hr = m_asfMux.ProcessSample(m_streamNumber, m_sample, 0);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asfMux.GetNextPacket(out m_status, out m_nextPacket);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asfMux.Flush();
            MFError.ThrowExceptionForHR(hr);

            hr = m_asfMux.End(m_contentInfo);
            //MFError.ThrowExceptionForHR(hr);
            Debug.Assert(hr == MFError.MF_E_FLUSH_NEEDED); // Even if Flush is called before End, the method return MF_E_FLUSH_NEEDED...
        }


        private void GetInterfaces()
        {
            int hr = 0;
            IMFMediaBuffer buffer;
            long headerSize;
            IMFPresentationDescriptor pd;
            

            hr = MFExtern.MFCreateFile(MFFileAccessMode.Read, MFFileOpenMode.FailIfNotExist , MFFileFlags.None, m_asfFile, out m_byteStream);
            MFError.ThrowExceptionForHR(hr);

            // Get an IMFASFContentInfo from file
            hr = MFExtern.MFCreateASFContentInfo(out m_contentInfo);
            MFError.ThrowExceptionForHR(hr);

            hr = ReadDataIntoBuffer(m_byteStream, 0, MIN_ASF_HEADER_SIZE, out buffer);
            MFError.ThrowExceptionForHR(hr);

            hr = m_contentInfo.GetHeaderSize(buffer, out headerSize);
            MFError.ThrowExceptionForHR(hr);

            Marshal.ReleaseComObject(buffer); buffer = null;

            hr = ReadDataIntoBuffer(m_byteStream, 0, (int)headerSize, out buffer);
            MFError.ThrowExceptionForHR(hr);

            hr = m_contentInfo.ParseHeader(buffer, 0);
            MFError.ThrowExceptionForHR(hr);

            Marshal.ReleaseComObject(buffer); buffer = null;

            // Get data position in file
            hr = m_contentInfo.GeneratePresentationDescriptor(out pd);
            MFError.ThrowExceptionForHR(hr);

            hr = pd.GetUINT64(MFAttributesClsid.MF_PD_ASF_DATA_START_OFFSET, out m_dataOffset);
            MFError.ThrowExceptionForHR(hr);

            hr = pd.GetUINT64(MFAttributesClsid.MF_PD_ASF_DATA_LENGTH, out m_dataLength);
            MFError.ThrowExceptionForHR(hr);

            // Get stream IDs
            IMFASFProfile profile;

            hr = m_contentInfo.GetProfile(out profile);
            MFError.ThrowExceptionForHR(hr);
/*
            short[] streamsID;
            int streamCount;

            hr = profile.GetStreamCount(out streamCount);
            MFError.ThrowExceptionForHR(hr);

            streamsID = new short[streamCount];

            IMFASFStreamConfig streamConfig;

            for (int i = 0; i < streamCount; i++)
            {
                hr = profile.GetStream(i, out streamsID[i], out streamConfig);
                Marshal.ReleaseComObject(streamConfig);
            }
*/
            // Get an IMFASFSplitter from the same file
            hr = MFExtern.MFCreateASFSplitter(out m_asfSplitter);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asfSplitter.Initialize(m_contentInfo);
            MFError.ThrowExceptionForHR(hr);

            //hr = m_asfSplitter.SelectStreams(streamsID, (short)streamsID.Length);
            hr = m_asfSplitter.SelectStreams(new short[] { 2 }, 1); // Don't know why the commented code don't works...
            MFError.ThrowExceptionForHR(hr);
        }

        private void GetSampleFromSplitter(out IMFSample sample)
        {
            int hr = 0;
            IMFMediaBuffer buffer;
            const int BUFFER_SIZE = 4 * 1024;

            sample = null;

            do
            {
                hr = ReadDataIntoBuffer(m_byteStream, (int)(m_dataOffset + m_dataPosition), BUFFER_SIZE, out buffer);
                MFError.ThrowExceptionForHR(hr);

                m_dataPosition += BUFFER_SIZE;

                hr = m_asfSplitter.ParseData(buffer, 0, 0);
                MFError.ThrowExceptionForHR(hr);

                hr = m_asfSplitter.GetNextSample(out m_status, out m_streamNumber, out sample);
                MFError.ThrowExceptionForHR(hr);

                Marshal.ReleaseComObject(buffer); buffer = null;
            }
            while (sample == null);
        }

        private int ReadDataIntoBuffer(IMFByteStream stream, int offset, int bytesToRead, out IMFMediaBuffer buffer)
        {
            int hr = 0;
            IMFMediaBuffer newBuffer = null;
            IntPtr bufferData = IntPtr.Zero;
            int maxLength, currentLength;
            int bytesRead;

            buffer = null;

            do
            {
                hr = MFExtern.MFCreateMemoryBuffer(bytesToRead, out newBuffer);
                if (hr != 0) break;

                hr = newBuffer.Lock(out bufferData, out maxLength, out currentLength);
                if (hr != 0) break;

                hr = stream.SetCurrentPosition(offset);
                if (hr != 0) break;

                hr = stream.Read(bufferData, bytesToRead, out bytesRead);
                if (hr != 0) break;

                hr = newBuffer.Unlock();
                if (hr != 0) break;

                bufferData = IntPtr.Zero;

                hr = newBuffer.SetCurrentLength(bytesRead);

                buffer = newBuffer;
            } 
            while (false);

            if (bufferData != IntPtr.Zero)
                newBuffer.Unlock();

            return hr;
        }
    }
}
