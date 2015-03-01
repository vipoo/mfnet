using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using MediaFoundation;
using MediaFoundation.EVR;
using MediaFoundation.Misc;
using MediaFoundation.MFPlayer;
using MediaFoundation.Transform;

namespace Testv21
{
    [ComImport, Guid("798059f0-89ca-4160-b325-aeb48efe4f9a")]
    public class ColorControlTransformDSP { }

    class IMFPMediaPlayerTest : Form, IMFPMediaPlayerCallback
    {
        private IMFPMediaPlayer mediaPlayer;
        private IMFPMediaItem[] mediaItems = new IMFPMediaItem[2];
        private string[] filenames = new string[] { @"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv", "http://www.w3schools.com/html/horse.mp3" };

        private bool mediaItemIsSet = false;
        private bool rateIsSet = false;
        private bool positionIsSet = false;
        private bool playIsSet = false;
        private bool pauseIsSet = false;
        private bool frameStepIsSet = false;
        private bool stopIsSet = false;

        public void DoTests()
        {
            Initialize();

            TestEffectMethods();
            TestMediaItems();
            TestAudioMethods();
            TestVideoMethods();
            TestRateAndPositionMethods();
            TestPlaybackControls();
            TestShutdown();
        }

        private void Initialize()
        {
            int hr = 0;
            IntPtr hwnd;

            // Create a IMFPMediaPlayer instance
            hr = MFExtern.MFPCreateMediaPlayer(null, false, MFP_CREATION_OPTIONS.FreeThreadedCallback, this, this.Handle, out mediaPlayer);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.GetVideoWindow(out hwnd);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(hwnd == this.Handle);

            // Create a IMFPMediaItem from an URL (read local file).
            hr = mediaPlayer.CreateMediaItemFromURL(filenames[0], true, IntPtr.Zero, out mediaItems[0]);
            MFError.ThrowExceptionForHR(hr);

            IMFSourceResolver sourceResolver;
            MFObjectType objectType;
            object byteStream;

            // Get a network byte stream using the source resolver
            hr = MFExtern.MFCreateSourceResolver(out sourceResolver);
            MFError.ThrowExceptionForHR(hr);

            hr = sourceResolver.CreateObjectFromURL(filenames[1], MFResolution.ByteStream, null, out objectType, out byteStream);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.CreateMediaItemFromObject(byteStream, true, IntPtr.Zero, out mediaItems[1]);
            MFError.ThrowExceptionForHR(hr);

        }

        private void TestEffectMethods()
        {
            int hr = 0;

            IMFTransform effect = (IMFTransform) new ColorControlTransformDSP();

            hr = mediaPlayer.InsertEffect(effect, true);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.RemoveEffect(effect);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.RemoveAllEffects();
            MFError.ThrowExceptionForHR(hr);
        }

        private void TestMediaItems()
        {
            int hr = 0;
            IMFPMediaItem currentItem;

            hr = mediaPlayer.ClearMediaItem();
            MFError.ThrowExceptionForHR(hr);

            // Add first item
            mediaItemIsSet = false;
            hr = mediaPlayer.SetMediaItem(mediaItems[0]);
            MFError.ThrowExceptionForHR(hr);

            while(mediaItemIsSet == false)
                System.Threading.Thread.Sleep(500);

            hr = mediaPlayer.GetMediaItem(out currentItem);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(currentItem == mediaItems[0]); // GetMediaItem should return mediaItems[0]
/*
            // Add second item
            mediaItemIsSet = false;
            hr = mediaPlayer.SetMediaItem(mediaItems[1]);
            MFError.ThrowExceptionForHR(hr);

            while (mediaItemIsSet == false)
                System.Threading.Thread.Sleep(500);
*/
        }

        private void TestAudioMethods()
        {
            int hr = 0;
            bool muted = true;
            float volume = 0.0f;
            float balance = 0.0f;

            hr = mediaPlayer.SetMute(false);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.GetMute(out muted);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(muted == false);

            hr = mediaPlayer.SetVolume(0.5f);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.GetVolume(out volume);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(volume == 0.5f);

            hr = mediaPlayer.SetVolume(0.5f);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.GetVolume(out volume);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(volume == 0.5f);

            hr = mediaPlayer.SetBalance(-0.5f);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.GetBalance(out balance);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(balance == -0.5f);
        }

        private void TestVideoMethods()
        {
            int hr = 0;

            MFVideoAspectRatioMode arMode;
            MFSize videoSize = new MFSize();
            MFSize arSize = new MFSize();
            MFSize minSize = new MFSize();
            MFSize maxSize = new MFSize();
            Color readColor;
            MFVideoNormalizedRect srcRect = new MFVideoNormalizedRect(0.25f, 0.25f, 0.75f, 0.75f);
            MFVideoNormalizedRect readRect = new MFVideoNormalizedRect();


            hr = mediaPlayer.SetAspectRatioMode(MFVideoAspectRatioMode.PreservePixel);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.GetAspectRatioMode(out arMode);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(arMode == MFVideoAspectRatioMode.PreservePixel);

            hr = mediaPlayer.GetNativeVideoSize(videoSize, arSize);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(videoSize.Width == 1280 && videoSize.Height == 720); // Values for Wildlife.wmv
            Debug.Assert(arSize.Width == 1280 && arSize.Height == 720); // Values for Wildlife.wmv


            hr = mediaPlayer.GetIdealVideoSize(minSize, maxSize);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(minSize.Width == 1 && minSize.Height == 1);
            Debug.Assert(maxSize.Width == 1920 && maxSize.Height == 1080); // Sould be the screen resolution...

            hr = mediaPlayer.SetBorderColor(Color.LimeGreen); // ;)
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.GetBorderColor(out readColor);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(readColor == Color.LimeGreen);

            hr = mediaPlayer.SetVideoSourceRect(srcRect);
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.GetVideoSourceRect(readRect);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(readRect.Equals(srcRect));

            hr = mediaPlayer.UpdateVideo();
            MFError.ThrowExceptionForHR(hr);
        }

        private void TestRateAndPositionMethods()
        {
            int hr = 0;
            float slowest, fastest;
            float currentRate;

            hr = mediaPlayer.GetSupportedRates(true, out slowest, out fastest);
            MFError.ThrowExceptionForHR(hr);

            rateIsSet = false;
            hr = mediaPlayer.SetRate(2.0f); // assuming 2.0 is between slowest and fastest
            MFError.ThrowExceptionForHR(hr);

            while (rateIsSet == false)
                System.Threading.Thread.Sleep(500);

            hr = mediaPlayer.GetRate(out currentRate);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(currentRate == 2.0f);

            PropVariant duration = new PropVariant();
            hr = mediaPlayer.GetDuration(CLSID.MFP_POSITIONTYPE_100NS, duration);
            MFError.ThrowExceptionForHR(hr);

            PropVariant newPosition = new PropVariant((long)(duration.GetULong() / 2));
            hr = mediaPlayer.SetPosition(CLSID.MFP_POSITIONTYPE_100NS, newPosition);
            MFError.ThrowExceptionForHR(hr);

            while (positionIsSet == false)
                System.Threading.Thread.Sleep(500);

            PropVariant readPostion = new PropVariant();
            hr = mediaPlayer.GetPosition(CLSID.MFP_POSITIONTYPE_100NS, readPostion);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(newPosition == readPostion);

        }

        private void TestPlaybackControls()
        {
            int hr = 0;
            MFP_MEDIAPLAYER_STATE state;

            // Play
            playIsSet = false;
            hr = mediaPlayer.Play();
            MFError.ThrowExceptionForHR(hr);

            while (playIsSet == false)
                System.Threading.Thread.Sleep(500);

            hr = mediaPlayer.GetState(out state);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(state == MFP_MEDIAPLAYER_STATE.Playing);

            // Pause
            pauseIsSet = false;
            hr = mediaPlayer.Pause();
            MFError.ThrowExceptionForHR(hr);

            while (pauseIsSet == false)
                System.Threading.Thread.Sleep(500);

            hr = mediaPlayer.GetState(out state);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(state == MFP_MEDIAPLAYER_STATE.Paused);

            // FrameStep
            frameStepIsSet = false;
            hr = mediaPlayer.FrameStep();
            MFError.ThrowExceptionForHR(hr);

            while (frameStepIsSet == false)
                System.Threading.Thread.Sleep(500);

            hr = mediaPlayer.GetState(out state);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(state == MFP_MEDIAPLAYER_STATE.Paused);

            // Stop
            stopIsSet = false;
            hr = mediaPlayer.Stop();
            MFError.ThrowExceptionForHR(hr);

            while (stopIsSet == false)
                System.Threading.Thread.Sleep(500);

            hr = mediaPlayer.GetState(out state);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(state == MFP_MEDIAPLAYER_STATE.Stopped);


        }

        private void TestShutdown()
        {
            int hr = mediaPlayer.Shutdown();
            MFError.ThrowExceptionForHR(hr);

            hr = mediaPlayer.Play();
            Debug.Assert(hr == MFError.MF_E_SHUTDOWN); // After a shutdown, avery method calls return MF_E_SHUTDOWN
        }

        #region IMFPMediaPlayerCallback Members

        public int OnMediaPlayerEvent(MFP_EVENT_HEADER pEventHeader)
        {
            switch (pEventHeader.eEventType)
            {
                case MFP_EVENT_TYPE.MediaItemSet :
                {
                    mediaItemIsSet = true;
                    break;
                }
                case MFP_EVENT_TYPE.RateSet:
                {
                    rateIsSet = true;
                    break;
                }
                case MFP_EVENT_TYPE.PositionSet:
                {
                    positionIsSet = true;
                    break;
                }
                case MFP_EVENT_TYPE.Play:
                {
                    playIsSet = true;
                    break;
                }
                case MFP_EVENT_TYPE.Pause:
                {
                    pauseIsSet = true;
                    break;
                }
                case MFP_EVENT_TYPE.FrameStep:
                {
                    frameStepIsSet = true;
                    break;
                }
                case MFP_EVENT_TYPE.Stop:
                {
                    stopIsSet = true;
                    break;
                }



                default:
                {
                    Debug.WriteLine(pEventHeader.eEventType);
                    break;
                }
            }

            return 0;
        }

        #endregion
    }
}
