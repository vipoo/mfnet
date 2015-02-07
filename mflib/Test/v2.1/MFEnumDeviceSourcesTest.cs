using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv21
{
    class MFEnumDeviceSourcesTest
    {
        private IMFAttributes enumAttributes;

        // This test assume that the test computer have at least one audio capture device...
        public void DoTests()
        {
            int hr = 0;

            hr = MFExtern.MFCreateAttributes(out enumAttributes, 1);
            MFError.ThrowExceptionForHR(hr);

            // Enumerate Audio devices
            hr = enumAttributes.SetGUID(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE, CLSID.MF_DEVSOURCE_ATTRIBUTE_SOURCE_TYPE_AUDCAP_GUID);
            MFError.ThrowExceptionForHR(hr);

            IMFActivate[] audioDevices;
            int audioDevicesCount;

            hr = MFExtern.MFEnumDeviceSources(enumAttributes, out audioDevices, out audioDevicesCount);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(audioDevicesCount != 0);
            Debug.Assert(audioDevices.Length == audioDevicesCount);

            // Just to test that returned array contain audio capture devices...
            for (int i = 0; i < audioDevicesCount; i++)
            {
                int deviceNameLength;

                hr = audioDevices[i].GetStringLength(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME, out deviceNameLength);

                StringBuilder sb = new StringBuilder(deviceNameLength);

                hr = audioDevices[i].GetString(MFAttributesClsid.MF_DEVSOURCE_ATTRIBUTE_FRIENDLY_NAME, sb, sb.Capacity * sizeof(char), out deviceNameLength);
                MFError.ThrowExceptionForHR(hr);

                Debug.WriteLine("Device[{0}]: {1}", i, sb.ToString());
            }
        }
    }
}
