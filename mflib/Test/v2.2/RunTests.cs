using System;
using System.Runtime.InteropServices;

using MediaFoundation;
using MediaFoundation.Misc;

namespace Testv22
{
    static public class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            try
            {
                MFExtern.MFStartup(0x20070, MFStartup.Full);
                MFExtern.MFLockPlatform();

                //TestExterns t1 = new TestExterns();
                //t1.DoTests();

                //IMFASFMultiplexerTest t2 = new IMFASFMultiplexerTest();
                //t2.DoTests();

                //IMFASFStreamPrioritizationTest t3 = new IMFASFStreamPrioritizationTest();
                //t3.DoTests();

                //IMFAudioPolicyTest t4 = new IMFAudioPolicyTest();
                //t4.DoTests();

                //IMFAudioStreamVolumeTest t5 = new IMFAudioStreamVolumeTest();
                //t5.DoTests();

                //IMFByteStreamBufferingTest t6 = new IMFByteStreamBufferingTest();
                //t6.DoTests();

                //IMFInputTrustAuthorityTest t7 = new IMFInputTrustAuthorityTest();
                //t7.DoTests();

                //IMFLocalMFTRegistrationTest t8 = new IMFLocalMFTRegistrationTest();
                //t8.DoTests();

                //IMFMediaSinkPrerollTest t9 = new IMFMediaSinkPrerollTest();
                //t9.DoTests();

                //IMFNetCredentialManagerTest t10 = new IMFNetCredentialManagerTest();
                //t10.DoTests();

                //IMFNetProxyLocatorFactoryTest t11 = new IMFNetProxyLocatorFactoryTest();
                //t11.DoTests();

                //IMFNetSchemeHandlerConfigTest t12 = new IMFNetSchemeHandlerConfigTest();
                //t12.DoTests();

                //IMFObjectReferenceStreamTest t13 = new IMFObjectReferenceStreamTest();
                //t13.DoTests();

                //IMFOutputPolicyTest t14 = new IMFOutputPolicyTest();
                //t14.DoTests();

                //IMFOutputSchemaTest t15 = new IMFOutputSchemaTest();
                //t15.DoTests();

                //IMFOutputTrustAuthorityTest t16 = new IMFOutputTrustAuthorityTest();
                //t16.DoTests();

                //IMFPMPClientTest t17 = new IMFPMPClientTest();
                //t17.DoTests();

                //IMFPMPHostTest t18 = new IMFPMPHostTest();
                //t18.DoTests();

                //IMFQualityManagerTest t19 = new IMFQualityManagerTest();
                //t19.DoTests();

                //IMFRealTimeClientTest t20 = new IMFRealTimeClientTest();
                //t20.DoTests();

                //IMFRemoteProxyTest t21 = new IMFRemoteProxyTest();
                //t21.DoTests();

                //IMFSAMIStyleTest t22 = new IMFSAMIStyleTest();
                //t22.DoTests();

                //IMFSampleProtectionTest t23 = new IMFSampleProtectionTest();
                //t23.DoTests();

                //IMFSaveJobTest t24 = new IMFSaveJobTest();
                //t24.DoTests();

                //IMFSchemeHandlerTest t25 = new IMFSchemeHandlerTest();
                //t25.DoTests();

                //IMFSecureChannelTest t26 = new IMFSecureChannelTest();
                //t26.DoTests();

                //IMFTopoLoaderTest t27 = new IMFTopoLoaderTest();
                //t27.DoTests();

                //IMFTopologyNodeAttributeEditorTest t28 = new IMFTopologyNodeAttributeEditorTest();
                //t28.DoTests();

                //IMFTrustedInputTest t29 = new IMFTrustedInputTest();
                //t29.DoTests();

                //IMFTrustedOutputTest t30 = new IMFTrustedOutputTest();
                //t30.DoTests();

                //IMFWorkQueueServicesTest t31 = new IMFWorkQueueServicesTest();
                //t31.DoTests();
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
