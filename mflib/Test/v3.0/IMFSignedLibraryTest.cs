using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFSignedLibraryTest
    {
        public void DoTests()
        {
#if false
            string [] sn = System.IO.Directory.GetFiles(@"c:\windows\system32", "*.dll");
            IMFSignedLibrary sl;
            int hr;

            Debug.WriteLine(sn.Length);

            foreach (string s in sn)
            {
                hr = MFExtern.MFLoadSignedLibrary(s, out sl);

                if (hr != MFError.MF_E_SIGNATURE_VERIFICATION_FAILED)
                    Debug.WriteLine(s);
            }
#endif
        }
    }
}
