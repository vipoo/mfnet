using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Testv21
{
    class IMFReadWriteClassFactoryTest
    {
        IMFReadWriteClassFactory classFactory;
        IMFAttributes sourceReaderAttributes;

        public void DoTests()
        {
            Init();
            CreateInstanceFromURLTest();
            CreateInstanceFromObjectTest();
        }

        private void Init()
        {
            int hr = 0;

            classFactory = new MFReadWriteClassFactory() as IMFReadWriteClassFactory;
            Debug.Assert(classFactory != null);

            hr = MFExtern.MFCreateAttributes(out sourceReaderAttributes, 1);
            Debug.Assert(hr == 0);

            hr = sourceReaderAttributes.SetUINT32(MFAttributesClsid.MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, -1);
            Debug.Assert(hr == 0);
        }

        private void CreateInstanceFromURLTest()
        {
            string filename = @"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv";
            object createdObject;

            int hr = classFactory.CreateInstanceFromURL(CLSID.CLSID_MFSourceReader, filename, sourceReaderAttributes, typeof(IMFSourceReader).GUID, out createdObject);
            Debug.Assert(hr == 0);

            IMFSourceReader sourceReader = createdObject as IMFSourceReader;
            Debug.Assert(sourceReader != null);

            Marshal.FinalReleaseComObject(sourceReader);
        }

        private void CreateInstanceFromObjectTest()
        {
            IMFByteStream byteStream;
            string filename = @"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv";
            object createdObject;

            int hr = MFExtern.MFCreateFile(MFFileAccessMode.Read, MFFileOpenMode.FailIfNotExist, MFFileFlags.None, filename, out byteStream);
            Debug.Assert(hr == 0);

            hr = classFactory.CreateInstanceFromObject(CLSID.CLSID_MFSourceReader, byteStream, sourceReaderAttributes, typeof(IMFSourceReader).GUID, out createdObject);
            Debug.Assert(hr == 0);

            IMFSourceReader sourceReader = createdObject as IMFSourceReader;
            Debug.Assert(sourceReader != null);

            Marshal.FinalReleaseComObject(sourceReader);
        }
    }
}
