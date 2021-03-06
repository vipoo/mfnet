﻿using System;
using System.Runtime.InteropServices;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv31
{
    static public class Program
    {
        public static readonly string File1 = @"C:\SourceForge\mflib\Test\Media\AspectRatio4x3.wmv";

        public static readonly string Timed1 = @"C:\SourceForge\mflib\Test\Media\timed.xml";
        public static readonly string Timed2 = @"C:\SourceForge\mflib\Test\Media\junk.xml";

        [MTAThread]
        static void Main(string[] args)
        {
            try
            {
                HResult hr;

                // Check for Windows 8
                Debug.Assert(Environment.OSVersion.Version.Major > 6 || (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 3));

                hr = MFExtern.MFStartup(0x20070, MFStartup.Full);
                MFError.ThrowExceptionForHR(hr);

                hr = MFExtern.MFLockPlatform();
                MFError.ThrowExceptionForHR(hr);

                //TestExterns t1 = new TestExterns();
                //t1.DoTests();

                //IMFBufferListNotifyTest t37 = new IMFBufferListNotifyTest();
                //t37.DoTests();

                //IMFCaptureEngineOnSampleCallback2Test t38 = new IMFCaptureEngineOnSampleCallback2Test();
                //t38.DoTests();

                //IMFCaptureSink2Test t39 = new IMFCaptureSink2Test();
                //t39.DoTests();

                //IMFCdmSuspendNotifyTest t40 = new IMFCdmSuspendNotifyTest();
                //t40.DoTests();

                //IMFDXGIDeviceManagerSourceTest t41 = new IMFDXGIDeviceManagerSourceTest();
                //t41.DoTests();

                //IMFMediaEngineClassFactory2Test t42 = new IMFMediaEngineClassFactory2Test();
                //t42.DoTests();

                //IMFMediaEngineClassFactoryExTest t43 = new IMFMediaEngineClassFactoryExTest();
                //t43.DoTests();

                //IMFMediaEngineEMETest t44 = new IMFMediaEngineEMETest();
                //t44.DoTests();

                //IMFMediaEngineNeedKeyNotifyTest t45 = new IMFMediaEngineNeedKeyNotifyTest();
                //t45.DoTests();

                //IMFMediaEngineOPMInfoTest t46 = new IMFMediaEngineOPMInfoTest();
                //t46.DoTests();

                //IMFMediaEngineSrcElementsExTest t47 = new IMFMediaEngineSrcElementsExTest();
                //t47.DoTests();

                //IMFMediaEngineSupportsSourceTransferTest t48 = new IMFMediaEngineSupportsSourceTransferTest();
                //t48.DoTests();

                //IMFMediaKeysTest t49 = new IMFMediaKeysTest();
                //t49.DoTests();

                //IMFMediaKeySessionTest t50 = new IMFMediaKeySessionTest();
                //t50.DoTests();

                //IMFMediaKeySessionNotifyTest t51 = new IMFMediaKeySessionNotifyTest();
                //t51.DoTests();

                //IMFMediaSourceExtensionTest t52 = new IMFMediaSourceExtensionTest();
                //t52.DoTests();

                //IMFMediaSourceExtensionNotifyTest t53 = new IMFMediaSourceExtensionNotifyTest();
                //t53.DoTests();

                //IMFMediaStreamSourceSampleRequestTest t54 = new IMFMediaStreamSourceSampleRequestTest();
                //t54.DoTests();

                //IMFSinkWriterEncoderConfigTest t55 = new IMFSinkWriterEncoderConfigTest();
                //t55.DoTests();

                //IMFSourceBufferTest t56 = new IMFSourceBufferTest();
                //t56.DoTests();

                //IMFSourceBufferListTest t57 = new IMFSourceBufferListTest();
                //t57.DoTests();

                //IMFSourceBufferNotifyTest t58 = new IMFSourceBufferNotifyTest();
                //t58.DoTests();

                //IPlayToControlWithCapabilitiesTest t59 = new IPlayToControlWithCapabilitiesTest();
                //t59.DoTests();

                //IMFSinkWriterCallback2Test t60 = new IMFSinkWriterCallback2Test();
                //t60.Dotests();

                //IMFSourceReaderCallback2Test t62 = new IMFSourceReaderCallback2Test();
                //t62.Dotests();

                //IMFTimedTextTest t63 = new IMFTimedTextTest();
                //t63.Dotests();

                //IMFTimedTextBinaryTest t64 = new IMFTimedTextBinaryTest();
                //t64.Dotests();

                //IMFTimedTextCueTest t65 = new IMFTimedTextCueTest();
                //t65.Dotests();

                //IMFTimedTextCueListTest t66 = new IMFTimedTextCueListTest();
                //t66.Dotests();

                //IMFTimedTextFormattedTextTest t67 = new IMFTimedTextFormattedTextTest();
                //t67.Dotests();

                //IMFTimedTextTrackTest t68 = new IMFTimedTextTrackTest();
                //t68.Dotests();

                //IMFTimedTextNotifyTest t69 = new IMFTimedTextNotifyTest();
                //t69.Dotests();

                //IMFTimedTextTrackListTest t70 = new IMFTimedTextTrackListTest();
                //t70.Dotests();

                //IMFVideoProcessorControl2Test t71 = new IMFVideoProcessorControl2Test();
                //t71.Dotests();

                //IMFVideoSampleAllocatorNotifyExTest t72 = new IMFVideoSampleAllocatorNotifyExTest();
                //t72.DoTests();
            }
            catch (Exception e)
            {
                int hr = Marshal.GetHRForException(e);
                string s = MFError.GetErrorText(hr);

                if (s == null)
                {
                    s = e.Message;
                }
                else
                {
                    s = string.Format("{0} ({1})", s, e.Message);
                }

                System.Windows.Forms.MessageBox.Show(string.Format("0x{0:x}: {1}", hr, s), "Exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally
            {
                MFExtern.MFUnlockPlatform();
                MFExtern.MFShutdown();
            }
        }

        static public void IsA(object a, Guid g)
        {
            IntPtr ppv;
            IntPtr p = Marshal.GetIUnknownForObject(a);
            int hr = Marshal.QueryInterface(p, ref g, out ppv);
            MFError.ThrowExceptionForHR(hr);
            Marshal.Release(p);
            Marshal.Release(p);
        }

    }
}
