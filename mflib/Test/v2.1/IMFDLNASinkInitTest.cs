using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv21
{
    class IMFDLNASinkInitTest
    {
        public void DoTests()
        {
            string filename = System.IO.Path.GetTempFileName();
            IMFByteStream tempByteStream;

            int hr = MFExtern.MFCreateFile(MFFileAccessMode.ReadWrite, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, filename, out tempByteStream);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaSink dlnaMediaSink = (IMFMediaSink) new MPEG2DLNASink();
            
            IMFDLNASinkInit dlnaSinkInit = dlnaMediaSink as IMFDLNASinkInit;
            Debug.Assert(dlnaSinkInit != null);

            hr = dlnaSinkInit.Initialize(tempByteStream, true);
            Debug.Assert(hr == 0);

            // Calling twice return an error
            hr = dlnaSinkInit.Initialize(tempByteStream, true);
            Debug.Assert(hr == MFError.MF_E_ALREADY_INITIALIZED);

        }
    }
}
