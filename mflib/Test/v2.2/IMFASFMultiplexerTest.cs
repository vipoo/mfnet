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
        private IMFASFMultiplexer m_asfMux;
        private long m_dataOffset, m_dataLength;

        public void DoTests()
        {
            int hr = 0;
            IMFMediaBuffer buffer;

            GetInterfaces();


            hr = MFExtern.MFCreateASFMultiplexer(out m_asfMux);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asfMux.Initialize(m_contentInfo);
            MFError.ThrowExceptionForHR(hr);



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
            short[] streamsID;
            int streamCount;

            hr = m_contentInfo.GetProfile(out profile);
            MFError.ThrowExceptionForHR(hr);

            hr = profile.GetStreamCount(out streamCount);
            MFError.ThrowExceptionForHR(hr);

            streamsID = new short[streamCount];

            IMFASFStreamConfig streamConfig;

            for (int i = 0; i < streamCount; i++)
            {
                hr = profile.GetStream(i, out streamsID[i], out streamConfig);
                Marshal.ReleaseComObject(streamConfig);
            }

            // Get an IMFASFSplitter from the same file
            hr = MFExtern.MFCreateASFSplitter(out m_asfSplitter);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asfSplitter.Initialize(m_contentInfo);
            MFError.ThrowExceptionForHR(hr);

            //hr = m_asfSplitter.SelectStreams(streamsID, (short)streamsID.Length);
            hr = m_asfSplitter.SelectStreams(new short[] { 2 }, 1);
            MFError.ThrowExceptionForHR(hr);

            // Get a sample from the splitter
            hr = ReadDataIntoBuffer(m_byteStream, (int)m_dataOffset, 4 * 1024, out buffer);
            MFError.ThrowExceptionForHR(hr);

            hr = m_asfSplitter.ParseData(buffer, 0, 0);
            MFError.ThrowExceptionForHR(hr);

            ASFStatusFlags status;
            short streamNumber;

            hr = m_asfSplitter.GetNextSample(out status, out streamNumber, out m_sample);
            MFError.ThrowExceptionForHR(hr);
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
