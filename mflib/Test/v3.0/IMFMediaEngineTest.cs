using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using MediaFoundation.EVR;
using System.Drawing;

namespace Testv30
{
    #region WIC stuff

    [UnmanagedName("CLSID_WICImagingFactory"),
    ComImport,
    Guid("cacaf262-9370-4615-a13b-9f5539da4c0a")]
    public class WICImagingFactory
    {
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("00000121-a8f2-4877-ba0a-fd2b6645fb94"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICBitmap
    {
        [PreserveSig]
        int GetSize( 
            out int puiWidth,
            out int puiHeight);

        [PreserveSig]
        int GetPixelFormat( 
            out Guid pPixelFormat);
        
        [PreserveSig]
        int GetResolution( 
            out double pDpiX,
            out double pDpiY);
        
        int CopyPalette( 
            [In, MarshalAs(UnmanagedType.IUnknown)] object pIPalette);
        
        int CopyPixels( 
            [In] MFRect prc,
            int cbStride,
            int cbBufferSize,
            IntPtr pbBuffer);
        
        int Lock( 
            MFRect prcLock,
            int flags,
            [MarshalAs(UnmanagedType.IUnknown)]
            out object ppILock);
        
        int SetPalette( 
            [In, MarshalAs(UnmanagedType.IUnknown)] object pIPalette);
        
        int SetResolution( 
            double dpiX,
            double dpiY);       
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("ec5ec8a9-c395-4314-9c77-54d7a935ff70"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWICImagingFactory
    {
        int CreateDecoderFromFilename( );
        
        int CreateDecoderFromStream( );
        
        int CreateDecoderFromFileHandle( );
        
        int CreateComponentInfo( );
        
        int CreateDecoder( );
        
        int CreateEncoder( );
        
        int CreatePalette( );
        
        int CreateFormatConverter( );
        
        int CreateBitmapScaler( );
        
        int CreateBitmapClipper( );
        
        int CreateBitmapFlipRotator( );
        
        int CreateStream( );
        
        int CreateColorContext( );
        
        int CreateColorTransformer( );
        
        [PreserveSig]
        int CreateBitmap( 
            int  uiWidth,
            int uiHeight,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pixelFormat,
            int option,
            out IWICBitmap ppIBitmap);
    }

    // IWICImagingFactory::CreateBitmap
    // https://msdn.microsoft.com/en-us/library/windows/desktop/ee690282%28v=vs.85%29.aspx

    #endregion

    class IMFMediaEngineTest : COMBase, IMFMediaEngineNotify
    {
        IMFMediaEngine m_me;
        int m_Messages = 0;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetConsoleTitle(StringBuilder sb, int capacity);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        public void DoTests()
        {
            GetInterface();

            TestGetSet();
            SetWithActive();
            TestA();

            SafeRelease(m_me);
            m_me = null;

            /////////

            GetInterface2();

            TestB();
        }

        private void TestB()
        {
            int hr;

            long pts;
            hr = m_me.OnVideoStreamTick(out pts); // ime
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(hr == S_False);

            hr = m_me.SetSource(Program.File1);
            MFError.ThrowExceptionForHR(hr);

            hr = m_me.Play();
            MFError.ThrowExceptionForHR(hr);

            do
            {
                hr = m_me.OnVideoStreamTick(out pts);
                MFError.ThrowExceptionForHR(hr);
            } while (hr != S_Ok);

            Debug.Assert(pts > 0);

            MFVideoNormalizedRect nr = new MFVideoNormalizedRect(); nr.bottom = 1; nr.right = 1;
            MFRect mr = new MFRect(); mr.right = 800; mr.bottom = 600;
            MFARGB rgb = new MFARGB();

            Guid pPixelFormat = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x0e);
            IWICImagingFactory wif = new WICImagingFactory() as IWICImagingFactory;

            IWICBitmap pp;
            hr = wif.CreateBitmap(800, 600, pPixelFormat, 2, out pp);
            MFError.ThrowExceptionForHR(hr);

            hr = m_me.TransferVideoFrame(pp, nr, mr, rgb); // ime
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(100);

            IntPtr p = Marshal.AllocCoTaskMem(800 * 600 * 4);
            hr = pp.CopyPixels(mr, 800 * 4, 800 * 600 * 4, p);
            MFError.ThrowExceptionForHR(hr);

            Bitmap b = new Bitmap(800, 600, 800 * 4, System.Drawing.Imaging.PixelFormat.Format32bppRgb, p);
            b.Save("a.bmp");

            hr = m_me.Shutdown();
            MFError.ThrowExceptionForHR(hr);
        }

        private void TestA()
        {
            int hr;

            // hr = m_me.SetSourceElements(); Tested in IMFMediaEngineSrcElementsTest // ime

            /////////
            double st = m_me.GetStartTime(); // ime
            Debug.Assert(st == 0);

            double dur = m_me.GetDuration(); // ime
            Debug.Assert(dur > 0);

            MF_MEDIA_ENGINE_CANPLAY cpt;
            hr = m_me.CanPlayType("audio/mpeg3", out cpt); // ime
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(cpt == MF_MEDIA_ENGINE_CANPLAY.Maybe);

            ///////

            int x, y;
            hr = m_me.GetVideoAspectRatio(out x, out y); // ime
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(x == 4 && y == 3);

            ///////
            bool p = m_me.IsPaused(); // ime

            Debug.Assert(p);

            hr = m_me.Play(); // ime

            while (m_me.IsPaused())
                ;

            hr = m_me.Pause(); // ime

            while (!m_me.IsPaused())
                ;

            ///////

            bool e = m_me.IsEnded(); // ime

            Debug.Assert(!e);

            hr = m_me.Play();

            hr = m_me.SetLoop(false);

            while (!m_me.IsEnded())
                ;

            //////////
            IMFMediaTimeRange pp;
            hr = m_me.GetPlayed(out pp); // ime
            MFError.ThrowExceptionForHR(hr);

            int iCount = pp.GetLength();

            double de;
            hr = pp.GetEnd(0, out de);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(de == dur);

            ///////////////

            hr = m_me.Shutdown(); // ime
            MFError.ThrowExceptionForHR(hr);
        }

        private void TestGetSet()
        {
            int hr;

            hr = m_me.SetErrorCode(MF_MEDIA_ENGINE_ERR.Aborted); // ime
            MFError.ThrowExceptionForHR(hr);

            IMFMediaError pe;
            hr = m_me.GetError(out pe);  // ime
            MFError.ThrowExceptionForHR(hr);

            MF_MEDIA_ENGINE_ERR ec = pe.GetErrorCode();

            Debug.Assert(ec == MF_MEDIA_ENGINE_ERR.Aborted);

            //////

            MF_MEDIA_ENGINE_READY mer = m_me.GetReadyState();  // ime
            Debug.Assert(mer == MF_MEDIA_ENGINE_READY.HaveNothing);

            MF_MEDIA_ENGINE_NETWORK gns = m_me.GetNetworkState(); // ime
            Debug.Assert(gns == MF_MEDIA_ENGINE_NETWORK.Empty); 

            ///////////

            hr = m_me.SetSource(Program.File1);  // ime
            MFError.ThrowExceptionForHR(hr);
            
            while (m_Messages < 8)
                ;

            string s;
            hr = m_me.GetCurrentSource(out s);  // ime
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(s == Program.File1);

            //////

            mer = m_me.GetReadyState();
            Debug.Assert(mer == MF_MEDIA_ENGINE_READY.HaveEnoughData);

            gns = m_me.GetNetworkState();
            Debug.Assert(gns == MF_MEDIA_ENGINE_NETWORK.Idle);

            ////////////

            hr = m_me.Load(); // ime
            MFError.ThrowExceptionForHR(hr);

            while (m_Messages < 18)
                ;

            ////////

            IMFMediaTimeRange pb;
            hr = m_me.GetBuffered(out pb); // ime
            MFError.ThrowExceptionForHR(hr);

            int iBlen = pb.GetLength();
            double bb; hr = pb.GetEnd(0, out bb);
            Debug.Assert(bb == m_me.GetDuration());

            ////////
            IMFMediaTimeRange tr;
            hr = m_me.GetSeekable(out tr);  // ime

            int iseek = tr.GetLength();
            Debug.Assert(iseek == 1);
            double dd; tr.GetEnd(0, out dd);
            Debug.Assert(dd == m_me.GetDuration());

            //////

            hr = m_me.SetPreload(MF_MEDIA_ENGINE_PRELOAD.Empty);  // ime
            MFError.ThrowExceptionForHR(hr);

            MF_MEDIA_ENGINE_PRELOAD pl = m_me.GetPreload();  // ime

            Debug.Assert(pl == MF_MEDIA_ENGINE_PRELOAD.Empty);

            //////

            hr = m_me.SetDefaultPlaybackRate(2.2);  // ime
            MFError.ThrowExceptionForHR(hr);

            double pr = m_me.GetDefaultPlaybackRate(); // ime

            Debug.Assert(pr == 2.2);

            //////

            hr = m_me.SetPlaybackRate(1.3); // ime
            MFError.ThrowExceptionForHR(hr);

            double gpr = m_me.GetPlaybackRate(); // ime

            Debug.Assert(gpr == 1.3);

            //////

            hr = m_me.SetAutoPlay(true); // ime
            MFError.ThrowExceptionForHR(hr);

            bool b = m_me.GetAutoPlay(); // ime

            Debug.Assert(b == true);

            //////

            hr = m_me.SetLoop(true); // ime
            MFError.ThrowExceptionForHR(hr);

            bool L = m_me.GetLoop(); // ime

            Debug.Assert(L == true);

            //////

            hr = m_me.SetMuted(true); // ime
            MFError.ThrowExceptionForHR(hr);

            bool m = m_me.GetMuted(); // ime

            Debug.Assert(m == true);
        }

        void SetWithActive()
        {
            int hr;

            int x, y;
            hr = m_me.GetNativeVideoSize(out x, out y); // ime
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(x == 800 && y == 600);

            bool vid = m_me.HasVideo(); // ime
            Debug.Assert(vid == true);

            bool aud = m_me.HasAudio(); // ime
            Debug.Assert(aud == true);

            hr = m_me.SetCurrentTime(2.3456); // ime
            MFError.ThrowExceptionForHR(hr);

            // This works if you are running full speed.  Not if you are single stepping.
            bool ss = m_me.IsSeeking(); // ime
            Debug.Assert(ss == true);

            while (m_me.IsSeeking())
                ;

            double d = m_me.GetCurrentTime(); // ime

            Debug.Assert(d == 2.3456);

            hr = m_me.SetVolume(0.33); // ime
            MFError.ThrowExceptionForHR(hr);

            double v = m_me.GetVolume(); // ime

            Debug.Assert(v == 0.33);
        }

        private void GetInterface()
        {
            IMFMediaEngineClassFactory mecf = new MFMediaEngineClassFactory() as IMFMediaEngineClassFactory;

            int hr;
            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 5);
            MFError.ThrowExceptionForHR(hr);

            hr = ia.SetUnknown(MFAttributesClsid.MF_MEDIA_ENGINE_CALLBACK, this);
            MFError.ThrowExceptionForHR(hr);

            IntPtr p;
            StringBuilder sb = new StringBuilder(500);

            GetConsoleTitle(sb, sb.Capacity);
            p = FindWindow(null, sb.ToString());

            hr = ia.SetUINT64(MFAttributesClsid.MF_MEDIA_ENGINE_PLAYBACK_HWND, p.ToInt64());
            MFError.ThrowExceptionForHR(hr);

            hr = mecf.CreateInstance(MF_MEDIA_ENGINE_CREATEFLAGS.None, ia, out m_me);
            MFError.ThrowExceptionForHR(hr);
        }
        private void GetInterface2()
        {
            IMFMediaEngineClassFactory mecf = new MFMediaEngineClassFactory() as IMFMediaEngineClassFactory;
            Debug.Assert(mecf != null);

            int hr;
            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 5);
            MFError.ThrowExceptionForHR(hr);

            hr = ia.SetUnknown(MFAttributesClsid.MF_MEDIA_ENGINE_CALLBACK, this);
            MFError.ThrowExceptionForHR(hr);

            hr = ia.SetUINT32(MFAttributesClsid.MF_MEDIA_ENGINE_VIDEO_OUTPUT_FORMAT, 21);
            MFError.ThrowExceptionForHR(hr);

            hr = mecf.CreateInstance(MF_MEDIA_ENGINE_CREATEFLAGS.None, ia, out m_me);
            MFError.ThrowExceptionForHR(hr);
        }

        public int EventNotify(MF_MEDIA_ENGINE_EVENT eventid, IntPtr param1, int param2)
        {
            m_Messages++;
            Debug.WriteLine(eventid);

            return S_Ok;
        }
    }
}
