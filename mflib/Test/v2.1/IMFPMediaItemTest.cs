using System;
using System.Runtime.InteropServices;
using System.Text;

using MediaFoundation;
using MediaFoundation.MFPlayer;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv21
{
    class IMFPMediaItemTest
    {
        private IMFPMediaPlayer mediaPlayer;
        private IMFPMediaItem mediaItemWildLife;
        private IMFPMediaItem mediaItemHorse;

        string filenameWildLife = @"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv";
        private IntPtr userDataPtr1;
        private int userData1 = unchecked((int)0xdeadbeef);

        string filenameHorse = "http://www.w3schools.com/html/horse.mp3";
        private IntPtr userDataPtr2;
        private int userData2 = unchecked((int)0xbeefdead);
        private object byteStream;


        public void DoTests()
        {
            Initialize();
            TestCharacteristics();
            TestDuration();
            TestCreationObjects();
            TestStreams();
            TestMetaData();
            TestStartStopPosition();
            TestSetStreamSink();
        }

        private void Initialize()
        {
            // Set some "user data"
            userDataPtr1 = Marshal.AllocCoTaskMem(sizeof(int));
            Marshal.WriteInt32(userDataPtr1, userData1);

            userDataPtr2 = Marshal.AllocCoTaskMem(sizeof(int));
            Marshal.WriteInt32(userDataPtr1, userData2);

            // Create a IMFPMediaPlayer instance
            int hr = MFExtern.MFPCreateMediaPlayer(null, false, MFP_CREATION_OPTIONS.None, null, IntPtr.Zero, out mediaPlayer);
            MFError.ThrowExceptionForHR(hr);

            // Create a IMFPMediaItem from an URL (read local file).
            hr = mediaPlayer.CreateMediaItemFromURL(filenameWildLife, true, userDataPtr1, out mediaItemWildLife);
            MFError.ThrowExceptionForHR(hr);

            // Create a IMFPMediaItem from an object (A Byte Stream, here).
            IMFSourceResolver sourceResolver;
            MFObjectType objectType;

            // Get a network byte stream using the source resolver
            hr = MFExtern.MFCreateSourceResolver(out sourceResolver);
            MFError.ThrowExceptionForHR(hr);

            hr = sourceResolver.CreateObjectFromURL(filenameHorse, MFResolution.ByteStream, null, out objectType, out byteStream);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.CreateMediaItemFromObject(byteStream, true, userDataPtr1, out mediaItemHorse);
            MFError.ThrowExceptionForHR(hr);
        }

        private void TestCharacteristics()
        {
            MFP_MEDIAITEM_CHARACTERISTICS expectedValue = MFP_MEDIAITEM_CHARACTERISTICS.CanSeek | MFP_MEDIAITEM_CHARACTERISTICS.CanPause;

            MFP_MEDIAITEM_CHARACTERISTICS returnedValue;
            int hr = mediaItemWildLife.GetCharacteristics(out returnedValue);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(returnedValue == expectedValue);
        }

        private void TestDuration()
        {
            // Value read in the properties of the file in the explorer
            TimeSpan expectedValueLow = new TimeSpan(0, 0, 30);
            TimeSpan expectedValueHigh = new TimeSpan(0, 0, 31);

            PropVariant returnedVariant = new PropVariant();

            int hr = mediaItemWildLife.GetDuration(CLSID.MFP_POSITIONTYPE_100NS, returnedVariant);
            MFError.ThrowExceptionForHR(hr);

            TimeSpan returnedValue = TimeSpan.FromTicks((long)returnedVariant.GetULong());
            returnedVariant.Clear();

            Debug.Assert(returnedValue >= expectedValueLow && returnedValue < expectedValueHigh);
        }

        private void TestCreationObjects()
        {
            IMFPMediaPlayer returnedMFP;
            string returnedUrl;
            object returnedObject;
            const int MF_E_NOTFOUND = unchecked((int)0xc00d36d5);
            IntPtr returnedUserData;

            // Test GetMediaPlayer
            int hr = mediaItemWildLife.GetMediaPlayer(out returnedMFP);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(returnedMFP == mediaPlayer);

            // Test GetURL
            hr = mediaItemHorse.GetURL(out returnedUrl);
            Debug.Assert(hr == MF_E_NOTFOUND); // MediaItem created by object don't have URL

            hr = mediaItemWildLife.GetURL(out returnedUrl);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(string.Equals(returnedUrl, filenameWildLife));

            // Test GetObject
            hr = mediaItemWildLife.GetObject(out returnedObject);
            Debug.Assert(hr == MF_E_NOTFOUND); // MediaItem created by URL don't have Object

            hr = mediaItemHorse.GetObject(out returnedObject);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(returnedObject == byteStream);

            hr = mediaItemWildLife.GetUserData(out returnedUserData);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(returnedUserData == userDataPtr1);

            // Write another user data ptr
            hr = mediaItemWildLife.SetUserData(userDataPtr2);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaItemWildLife.GetUserData(out returnedUserData);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(returnedUserData == userDataPtr2);
        }

        private void TestStreams()
        {
            int streamCount;
            bool hasAudio, hasVideo, isSelected, isProtected;
            
            int hr = mediaItemWildLife.GetNumberOfStreams(out streamCount);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(streamCount == 2); //Wildlife.wmv have 2 streams

            hr = mediaItemWildLife.HasAudio(out hasAudio, out isSelected);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(hasAudio && isSelected);

            hr = mediaItemHorse.HasVideo(out hasVideo, out isSelected);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(!hasVideo && !isSelected); // MP3 don't have video

            hr = mediaItemHorse.IsProtected(out isProtected);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(isProtected == false); // No protection on MP3

            hr = mediaItemHorse.SetStreamSelection(0, false);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaItemHorse.GetStreamSelection(0, out isSelected);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(isSelected == false);

            hr = mediaItemHorse.SetStreamSelection(0, true);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaItemHorse.GetStreamSelection(0, out isSelected);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(isSelected == true);
        }

        private void TestMetaData()
        {
            IPropertyStore store;

            int hr = mediaItemWildLife.GetMetadata(out store);
            MFError.ThrowExceptionForHR(hr);

            // Test the returned store... (not needed)
            PropertyKey PKEY_Video_FrameWidth = new PropertyKey(new Guid("64440491-4C8B-11D1-8B70-080036B11A03"), 3);
            PropVariant value = new PropVariant();

            hr = store.GetValue(PKEY_Video_FrameWidth, value);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert((int)value.GetUInt() == 1280);

            // Test Presentation Attributes
            value.Clear();
            hr = mediaItemWildLife.GetPresentationAttribute(MFAttributesClsid.MF_PD_TOTAL_FILE_SIZE, value);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert((long)value.GetULong() == 26246026); // Size in bytes of wildlife.wmv

            // Test Stream Attributes
            value.Clear();
            hr = mediaItemWildLife.GetStreamAttribute(0, MFAttributesClsid.MF_MT_MAJOR_TYPE, value);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(value.GetGuid() == MFMediaType.Audio); // Stream 0 : Audio

            value.Clear();
            hr = mediaItemWildLife.GetStreamAttribute(1, MFAttributesClsid.MF_MT_MAJOR_TYPE, value);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(value.GetGuid() == MFMediaType.Video); // Stream 1 : Video
        }

        private void TestStartStopPosition()
        {
            long tickStop = TimeSpan.FromSeconds(10.0).Ticks;
            PropVariant stopPos = new PropVariant(tickStop);

            // Make sur that method accept null values
            int hr = mediaItemWildLife.SetStartStopPosition(null, null, CLSID.MFP_POSITIONTYPE_100NS, stopPos);
            MFError.ThrowExceptionForHR(hr);

            MFGuid returnedStopPosType = new MFGuid(Guid.NewGuid()); //Everything but Guid.Empty == MFP_POSITIONTYPE_100NS
            PropVariant returnedStopPos = new PropVariant();

            // Make sur that method accept null values too
            hr = mediaItemWildLife.GetStartStopPosition(null, null, returnedStopPosType, returnedStopPos);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(returnedStopPosType.ToGuid() == CLSID.MFP_POSITIONTYPE_100NS);
            Debug.Assert(returnedStopPos.GetLong() == tickStop);
        }

        private void TestSetStreamSink()
        {
            IMFMediaSink SARMediaSink;

            int hr = MFExtern.MFCreateAudioRenderer(null, out SARMediaSink);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaItemWildLife.SetStreamSink(0, SARMediaSink); // Link the audio stream with a new SAR instance...
            Debug.Assert(hr == 0);

            hr = mediaItemWildLife.SetStreamSink(0, null); // revert to default conf...
            Debug.Assert(hr == 0);
        }
    }
}
