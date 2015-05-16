using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IPlayToSourceClassFactoryTest
    {
        public void DoTests()
        {
            IPlayToSourceClassFactory pts = new PlayToSourceClassFactory() as IPlayToSourceClassFactory;
            Debug.Assert(pts != null);
            int hr;

            object o;
            IPlayToControl pc = null; // Where do we get one?
            hr = pts.CreateInstance(PLAYTO_SOURCE_CREATEFLAGS.Audio, pc, out o);
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
