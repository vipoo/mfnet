using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

using MediaFoundation;
using MediaFoundation.Misc;
using MediaFoundation.EVR;

namespace Testv10
{
    class IMFMediaEventTest
    {
        IMFMediaEvent m_me;

        public void DoTests()
        {
            GetInterface();

            TestGetType();
            TestGetExtendedType();
            TestGetStatus();
            TestGetValue();
        }

        void TestGetType()
        {
            MediaEventType m;

            m_me.GetType(out m);

            Debug.Assert(MediaEventType.MESourceStarted == m);
        }

        void TestGetExtendedType()
        {
            Guid g;
            m_me.GetExtendedType(out g);

            Debug.Assert(g != Guid.Empty);
        }

        void TestGetStatus()
        {
            int i;
            m_me.GetStatus(out i);

            Debug.Assert(i == 313);
        }

        void TestGetValue()
        {
            PropVariant p = new PropVariant("FDSA");
            m_me.GetValue(p);

            Debug.Assert(p.GetString() == "asdf");
        }

        private void GetInterface()
        {
            MFExtern.MFCreateMediaEvent(
                MediaEventType.MESourceStarted,
                Guid.NewGuid(),
                313,
                new PropVariant("asdf"),
                out m_me
                );
        }
    }
}
