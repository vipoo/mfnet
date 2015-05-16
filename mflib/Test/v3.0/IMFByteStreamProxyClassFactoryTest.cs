using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFByteStreamProxyClassFactoryTest
    {
        public void DoTests()
        {
            int hr;
            IMFByteStreamProxyClassFactory bspcf = new MFByteStreamProxyClassFactory() as IMFByteStreamProxyClassFactory;
            Debug.Assert(bspcf != null);

            object o;
            IMFByteStream bs;
            hr = MFExtern.MFCreateFile(MFFileAccessMode.Read, MFFileOpenMode.FailIfNotExist, MFFileFlags.None, Program.File1, out bs);
            MFError.ThrowExceptionForHR(hr);

            hr = bspcf.CreateByteStreamProxy(bs, null, typeof(IMFByteStream).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            Program.IsA(o, typeof(IMFByteStream).GUID);
        }
    }
}
