using System;
using System.Runtime.InteropServices;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;

namespace Testv10
{
    class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            try
            {
                int hr;
                hr = MFExtern.MFStartup(0x20070, MFStartup.Full);
                MFError.ThrowExceptionForHR(hr);
                hr = MFExtern.MFLockPlatform();
                MFError.ThrowExceptionForHR(hr);

                //TestPropVariant tpv = new TestPropVariant();
                //tpv.DoTests();

                //TestBMI t0a = new TestBMI();
                //t0a.DoTests();

                //TestWave t00 = new TestWave();
                //t00.DoTests();

                //IMFSourceResolverTest t01 = new IMFSourceResolverTest();
                //t01.DoTests();

                //IMFTopologyTest t02 = new IMFTopologyTest();
                //t02.DoTests();

                //IMFAttributesTest t03 = new IMFAttributesTest();
                //t03.DoTests();

                //IMFStreamDescriptorTest t04 = new IMFStreamDescriptorTest();
                //t04.DoTests();

                //IMFTopologyNodeTest t05 = new IMFTopologyNodeTest();
                //t05.DoTests();

                //IMFSampleTest t06 = new IMFSampleTest();
                //t06.DoTests();

                //IPropertyStoreTest t07 = new IPropertyStoreTest();
                //t07.DoTests();

                //IMFByteStreamTest t08 = new IMFByteStreamTest();
                //t08.DoTests();

                //IMFVideoDisplayControlTest t09 = new IMFVideoDisplayControlTest();
                //t09.DoTests();

                //IMFMediaEventQueueTest t10 = new IMFMediaEventQueueTest();
                //t10.DoTests();

                //IMFActivateTest t11 = new IMFActivateTest();
                //t11.DoTests();

                //IMFMediaBufferTest t12 = new IMFMediaBufferTest();
                //t12.DoTests();

                //IMFMediaEventTest t13 = new IMFMediaEventTest();
                //t13.DoTests();

                //IMFMediaTypeTest t14 = new IMFMediaTypeTest();
                //t14.DoTests();

                //IMFMediaEventGeneratorTest t15 = new IMFMediaEventGeneratorTest();
                //t15.DoTests();

                //IMFMediaStreamTest t16 = new IMFMediaStreamTest();
                //t16.DoTests();

                //IMFMediaTypeHandlerTest t17 = new IMFMediaTypeHandlerTest();
                //t17.DoTests();

                //IMFPresentationDescriptorTest t18 = new IMFPresentationDescriptorTest();
                //t18.DoTests();

                //IMFMediaSessionTest t19 = new IMFMediaSessionTest();
                //t19.DoTests();

                //IMFClockTest t20 = new IMFClockTest();
                //t20.DoTests();

                //IMFCollectionTest t21 = new IMFCollectionTest();
                //t21.DoTests();

                //IMFPresentationTimeSourceTest t22 = new IMFPresentationTimeSourceTest();
                //t22.DoTests();

                //MFTransformTest t23 = new MFTransformTest();
                //t23.DoTests();

                //INamedPropertyStoreTest t24 = new INamedPropertyStoreTest();
                //t24.DoTests();

                //TestExtern t25 = new TestExtern();
                //t25.DoTests();

                //IMFASFStreamSelectorTest t27 = new IMFASFStreamSelectorTest();
                //t27.DoTests();

                //IMFASFProfileTest t28 = new IMFASFProfileTest();
                //t28.DoTests();

                //IMFMediaSourceTopologyProviderTest t29 = new IMFMediaSourceTopologyProviderTest();
                //t29.DoTests();

                //IMFContentEnablerTest t30 = new IMFContentEnablerTest();
                //t30.DoTests();

                //IMFASFStreamConfigTest t31 = new IMFASFStreamConfigTest();
                //t31.DoTests();

                //IEVRFilterConfigTest t32 = new IEVRFilterConfigTest();
                //t32.DoTests();

                //IMFASFMutualExclusionTest t33 = new IMFASFMutualExclusionTest();
                //t33.DoTests();

                //IMFAsfIndexerTest t34 = new IMFAsfIndexerTest();
                //t34.DoTests();
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
    }
}
