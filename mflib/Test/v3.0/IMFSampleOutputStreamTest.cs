using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFSampleOutputStreamTest
    {
        public void DoTests()
        {
#if false
            int hr;

            IMFByteStream bs;
            hr = MFExtern.MFCreateFile(MFFileAccessMode.Write, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, "output.wmv", out bs);

            IMFSampleOutputStream sos = bs as IMFSampleOutputStream;
#endif
        }
    }
}
