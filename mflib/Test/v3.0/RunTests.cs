using System;
using System.Runtime.InteropServices;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    static public class Program
    {
        public static string File1 = @"c:\SourceForge\mflib\Test\Media\AspectRatio4x3.wmv";
        public static string File2 = @"http://www.LimeGreenSocks.com/AspectRatio4x3.wmv";

        [MTAThread]
        static void Main(string[] args)
        {
            try
            {
                int hr;

                // Check for Windows 8
                Debug.Assert(Environment.OSVersion.Version.Major > 6 || (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 2));

                hr = MFExtern.MFStartup(0x20070, MFStartup.Full);
                MFError.ThrowExceptionForHR(hr);

                hr = MFExtern.MFLockPlatform();
                MFError.ThrowExceptionForHR(hr);

                //TestExterns t1 = new TestExterns();
                //t1.DoTests();

                //IAdvancedMediaCaptureTest t2 = new IAdvancedMediaCaptureTest();
                //t2.DoTests();

                //IAdvancedMediaCaptureInitializationSettingsTest t3 = new IAdvancedMediaCaptureInitializationSettingsTest();
                //t3.DoTests();

                //IAdvancedMediaCaptureSettingsTest t4 = new IAdvancedMediaCaptureSettingsTest();
                //t4.DoTests();

                //IMF2DBuffer2Test t5 = new IMF2DBuffer2Test();
                //t5.DoTests();

                //IMFAsyncCallbackLoggingTest t6 = new IMFAsyncCallbackLoggingTest();
                //t6.DoTests();

                //IMFByteStreamCacheControl2Test t7 = new IMFByteStreamCacheControl2Test();
                //t7.DoTests();

                //IMFByteStreamProxyClassFactoryTest t8 = new IMFByteStreamProxyClassFactoryTest();
                //t8.DoTests();

                //IMFByteStreamTimeSeekTest t9 = new IMFByteStreamTimeSeekTest();
                //t9.DoTests();

                //IMFMediaEngineTest t10 = new IMFMediaEngineTest();
                //t10.DoTests();

                //IMFMediaEngineClassFactoryTest t11 = new IMFMediaEngineClassFactoryTest();
                //t11.DoTests();

                //IMFMediaEngineExTest t12 = new IMFMediaEngineExTest();
                //t12.DoTests();

                //IMFMediaEngineExtensionTest t13 = new IMFMediaEngineExtensionTest();
                //t13.DoTests();

                //IMFMediaEngineNotifyTest t14 = new IMFMediaEngineNotifyTest();
                //t14.DoTests();

                //IMFMediaEngineProtectedContentTest t15 = new IMFMediaEngineProtectedContentTest();
                //t15.DoTests();

                //IMFMediaEngineSrcElementsTest t16 = new IMFMediaEngineSrcElementsTest();
                //t16.DoTests();

                //IMFMediaErrorTest t17 = new IMFMediaErrorTest();
                //t17.DoTests();

                //IMFMediaSourceExTest t18 = new IMFMediaSourceExTest();
                //t18.DoTests();

                //IMFMediaTimeRangeTest t19 = new IMFMediaTimeRangeTest();
                //t19.DoTests();

                //IMFNetResourceFilterTest t20 = new IMFNetResourceFilterTest();
                //t20.DoTests();

                //IMFPluginControl2Test t21 = new IMFPluginControl2Test();
                //t21.DoTests();

                //IMFPMPClientAppTest t22 = new IMFPMPClientAppTest();
                //t22.DoTests();

                //IMFPMPHostAppTest t23 = new IMFPMPHostAppTest();
                //t23.DoTests();

                //IMFProtectedEnvironmentAccessTest t24 = new IMFProtectedEnvironmentAccessTest();
                //t24.DoTests();

                //IMFRealTimeClientExTest t25 = new IMFRealTimeClientExTest();
                //t25.DoTests();

                //IMFSampleOutputStreamTest t26 = new IMFSampleOutputStreamTest();
                //t26.DoTests();

                //IMFSeekInfoTest t27 = new IMFSeekInfoTest();
                //t27.DoTests();

                //IMFSignedLibraryTest t28 = new IMFSignedLibraryTest();
                //t28.DoTests();

                //IMFSimpleAudioVolumeTest t29 = new IMFSimpleAudioVolumeTest();
                //t29.DoTests();

                //IMFSinkWriterExTest t30 = new IMFSinkWriterExTest();
                //t30.DoTests();

                //IMFSourceReaderExTest t31 = new IMFSourceReaderExTest();
                //t31.DoTests();

                //IMFSystemIdTest t32 = new IMFSystemIdTest();
                //t32.DoTests();

                //IMFVideoProcessorControlTest t33 = new IMFVideoProcessorControlTest();
                //t33.DoTests();

                //IMFVideoSampleAllocatorExTest t34 = new IMFVideoSampleAllocatorExTest();
                //t34.DoTests();

                //IMFWorkQueueServicesExTest t35 = new IMFWorkQueueServicesExTest();
                //t35.DoTests();

                //IMFCaptureEngineTest t61 = new IMFCaptureEngineTest();
                //t61.DoTests();

                //IMFCaptureEngineClassFactoryTest t62 = new IMFCaptureEngineClassFactoryTest();
                //t62.DoTests();

                //IMFCaptureEngineOnEventCallbackTest t63 = new IMFCaptureEngineOnEventCallbackTest();
                //t63.DoTests();

                //IMFCaptureEngineOnSampleCallbackTest t64 = new IMFCaptureEngineOnSampleCallbackTest();
                //t64.DoTests();

                //IMFCapturePreviewSinkTest t65 = new IMFCapturePreviewSinkTest();
                //t65.DoTests();

                //IMFCaptureRecordSinkTest t66 = new IMFCaptureRecordSinkTest();
                //t66.DoTests();

                //IMFCaptureSinkTest t67 = new IMFCaptureSinkTest();
                //t67.DoTests();

                //IMFCaptureSourceTest t68 = new IMFCaptureSourceTest();
                //t68.DoTests();

                //IMFImageSharingEngineTest t69 = new IMFImageSharingEngineTest();
                //t69.DoTests();

                //IMFImageSharingEngineClassFactoryTest t70 = new IMFImageSharingEngineClassFactoryTest();
                //t70.DoTests();

                //IMFMediaSharingEngineTest t71 = new IMFMediaSharingEngineTest();
                //t71.DoTests();

                //IMFMediaSharingEngineClassFactoryTest t72 = new IMFMediaSharingEngineClassFactoryTest();
                //t72.DoTests();

                //IMFSharingEngineClassFactoryTest t73 = new IMFSharingEngineClassFactoryTest();
                //t73.DoTests();

                //IPlayToControlTest t74 = new IPlayToControlTest();
                //t74.DoTests();

                //IPlayToSourceClassFactoryTest t75 = new IPlayToSourceClassFactoryTest();
                //t75.DoTests();

                //IMFCapturePhotoSinkTest t76 = new IMFCapturePhotoSinkTest();
                //t76.DoTests();
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
