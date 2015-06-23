using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Testv30
{
    [UnmanagedName("CLSID_CColorConvertDMO"),
    ComImport,
    Guid("3e58004e-4ce5-4681-ba56-785a67f9f0dc")]
    public class geru
    {
    }

    class IPlayToSourceClassFactoryTest
    {
        public void DoTests()
        {
#if false
            //IPlayToSourceClassFactory x1 = new geru() as IPlayToSourceClassFactory;
            //IPlayToControl x2 = new geru() as IPlayToControl;

            IPlayToSourceClassFactory pts = new PlayToSourceClassFactory() as IPlayToSourceClassFactory;
            IPlayToControl pts2 = new PlayToSourceClassFactory() as IPlayToControl;

            Debug.Assert(pts != null);
            int hr;

            object o;
            IPlayToControl pc = null; // Where do we get one?
            hr = pts.CreateInstance(PLAYTO_SOURCE_CREATEFLAGS.Audio, pc, out o);
            MFError.ThrowExceptionForHR(hr);
#endif
        }
    }
}
