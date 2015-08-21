﻿/****************************************************************************
While the underlying library is covered by LGPL or BSD, this sample is released
as public domain.  It is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
 *
Written by:
Gerardo Hernandez
BrightApp.com

Modified by snarfle
*****************************************************************************/
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;

using MediaFoundation;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;

using SlimDX.Direct3D9;
using SlimDX;

namespace MFCaptureD3D
{
    class DrawDevice : COMBase
    {
        #region Definitions

        private const int NUM_BACK_BUFFERS = 2;

        /// <summary>
        /// A struct that describes a YUYV pixel
        /// </summary>
        private struct YUYV
        {
            public byte Y;
            public byte U;
            public byte Y2;
            public byte V;

            public YUYV(byte y, byte u, byte y2, byte v)
            {
                Y = y;
                U = u;
                Y2 = y2;
                V = v;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RGBQUAD
        {
            public byte B;
            public byte G;
            public byte R;
            public byte A;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RGB24
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
        }

        private struct VideoFormatGUID
        {
            public Guid SubType;
            public VideoConversion VideoConvertFunction;

            public VideoFormatGUID(Guid FormatGuid, VideoConversion cvt)
            {
                SubType = FormatGuid;
                VideoConvertFunction = cvt;
            }
        }

        // Function to convert the video to RGB32
        private delegate void VideoConversion(
            IntPtr pDest, 
            int lDestStride, 
            IntPtr pSrc, 
            int lSrcStride, 
            int dwWidthInPixels, 
            int dwHeightInPixels);

        #endregion

        #region Private Members

        private IntPtr m_hwnd;
        private Device m_pDevice;
        private SwapChain m_pSwapChain;

        private PresentParameters[] m_d3dpp;

        // Format information
        private Format m_format;
        private int m_width;
        private int m_height;
        private int m_lDefaultStride;
        private MFRatio m_PixelAR;
        private Rectangle m_rcDest;       // Destination rectangle

        private VideoFormatGUID[] VideoFormatDefs;

        private VideoConversion m_convertFn;

        #endregion

        //-------------------------------------------------------------------
        // Constructor
        //-------------------------------------------------------------------
        public DrawDevice()
        {
            m_hwnd = IntPtr.Zero;
            m_pDevice = null;
            m_pSwapChain = null;

            m_d3dpp = null;

            m_format = Format.X8R8G8B8;
            m_width = 0;
            m_height = 0;
            m_lDefaultStride = 0;
            m_PixelAR.Denominator = m_PixelAR.Numerator = 1;
            m_rcDest = Rectangle.Empty;

            VideoFormatDefs = new VideoFormatGUID[] {
                new VideoFormatGUID(MFMediaType.RGB32, TransformImage_RGB32),
                new VideoFormatGUID(MFMediaType.RGB24, TransformImage_RGB24),
                new VideoFormatGUID(MFMediaType.YUY2, TransformImage_YUY2),
                new VideoFormatGUID(MFMediaType.NV12, TransformImage_NV12) 
            };

            m_convertFn = null;
        }

        //-------------------------------------------------------------------
        // Destructor
        //-------------------------------------------------------------------
#if DEBUG
        ~DrawDevice()
        {
            Debug.Assert(m_pSwapChain == null || m_pDevice == null);
            DestroyDevice();
        }
#endif

        #region Externs

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #endregion

        #region Private Methods

        private HResult TestCooperativeLevel()
        {
            if (m_pDevice == null)
            {
                return HResult.E_FAIL;
            }

            HResult hr = HResult.S_OK;

            // Check the current status of D3D9 device.
            SlimDX.Result r = m_pDevice.TestCooperativeLevel();

            hr = (HResult)r.Code;

            return hr;
        }

        //-------------------------------------------------------------------
        // SetConversionFunction
        //
        // Set the conversion function for the specified video format.
        //-------------------------------------------------------------------

        private HResult SetConversionFunction(Guid subtype)
        {
            HResult hr = HResult.MF_E_INVALIDMEDIATYPE;
            m_convertFn = null;

            for (int i = 0; i < VideoFormatDefs.Length; i++)
            {
                if (VideoFormatDefs[i].SubType == subtype)
                {
                    m_convertFn = VideoFormatDefs[i].VideoConvertFunction;
                    hr = HResult.S_OK;
                    break;
                }
            }

            return hr;
        }

        //-------------------------------------------------------------------
        // CreateSwapChains
        //
        // Create Direct3D swap chains.
        //-------------------------------------------------------------------

        private HResult CreateSwapChains()
        {
            HResult hr = HResult.S_OK;

            PresentParameters pp = new PresentParameters();

            if (m_pSwapChain != null)
            {
                m_pSwapChain.Dispose();
                m_pSwapChain = null;
            }

            pp.EnableAutoDepthStencil = false;
            pp.BackBufferWidth = m_width;
            pp.BackBufferHeight = m_height;
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Flip;
            pp.DeviceWindowHandle = m_hwnd;
            pp.BackBufferFormat = Format.X8R8G8B8;
            pp.PresentFlags = PresentFlags.DeviceClip | PresentFlags.LockableBackBuffer;
            pp.PresentationInterval = PresentInterval.Immediate;
            pp.BackBufferCount = NUM_BACK_BUFFERS;

            m_pSwapChain = new SwapChain(m_pDevice, pp);

            return hr;
        }

        //-------------------------------------------------------------------
        //  UpdateDestinationRect
        //
        //  Update the destination rectangle for the current window size.
        //  The destination rectangle is letterboxed to preserve the
        //  aspect ratio of the video image.
        //-------------------------------------------------------------------

        private void UpdateDestinationRect()
        {
            Rectangle rcSrc = new Rectangle(0, 0, m_width, m_height);
            Rectangle rcClient = GetClientRect(m_hwnd);
            Rectangle rectanClient = new Rectangle(rcClient.Left, rcClient.Top, rcClient.Right - rcClient.Left, rcClient.Bottom - rcClient.Top);

            rcSrc = CorrectAspectRatio(rcSrc, m_PixelAR);

            m_rcDest = LetterBoxRect(rcSrc, rectanClient);
        }

        #endregion

        #region Public Methods

        //-------------------------------------------------------------------
        // CreateDevice
        //
        // Create the Direct3D device.
        //-------------------------------------------------------------------
        public HResult CreateDevice(IntPtr hwnd)
        {
            if (m_pDevice != null)
            {
                return HResult.S_OK;
            }

            PresentParameters[] pp = new PresentParameters[1];

            pp[0] = new PresentParameters();
            pp[0].BackBufferFormat = Format.X8R8G8B8;
            pp[0].SwapEffect = SwapEffect.Copy;
            pp[0].PresentationInterval = PresentInterval.Immediate;
            pp[0].Windowed = true;
            pp[0].DeviceWindowHandle = hwnd;
            pp[0].BackBufferHeight = 0;
            pp[0].BackBufferWidth = 0;
            pp[0].EnableAutoDepthStencil = false;

            using (Direct3D d = new Direct3D())
            {
                m_pDevice = new Device(d, 0, DeviceType.Hardware, hwnd,
                    CreateFlags.HardwareVertexProcessing | CreateFlags.FpuPreserve | CreateFlags.Multithreaded, pp);
            }

            m_hwnd = hwnd;
            m_d3dpp = pp;

            return HResult.S_OK;
        }

        //-------------------------------------------------------------------
        // ResetDevice
        //
        // Resets the Direct3D device.
        //-------------------------------------------------------------------
        public HResult ResetDevice()
        {
            HResult hr = HResult.S_OK;

            if (m_pDevice != null)
            {
                PresentParameters[] d3dpp = (PresentParameters[])m_d3dpp.Clone();

                try
                {
                    if (m_pSwapChain != null)
                    {
                        m_pSwapChain.Dispose();
                        m_pSwapChain = null;
                    }
                    d3dpp[0].BackBufferHeight = 0;
                    d3dpp[0].BackBufferWidth = 0;
                    Result r = m_pDevice.Reset(d3dpp);

                    if (r.IsFailure)
                    {
                        DestroyDevice();
                    }
                }
                catch
                {
                    DestroyDevice();
                }
            }

            if (m_pDevice == null)
            {
                hr = CreateDevice(m_hwnd);

                if (Failed(hr))
                {
                    return hr;
                }
            }

            if ((m_pSwapChain == null) && (m_format != Format.Unknown))
            {
                hr = CreateSwapChains();
                if (Failed(hr)) { return hr; }

                UpdateDestinationRect();
            }

            return hr;
        }

        //-------------------------------------------------------------------
        // DestroyDevice
        //
        // Release all Direct3D resources.
        //-------------------------------------------------------------------
        public void DestroyDevice()
        {
            if (m_pSwapChain != null)
            {
                m_pSwapChain.Dispose();
                m_pSwapChain = null;
            }
            if (m_pDevice != null)
            {
                m_pDevice.Dispose();
                m_pDevice = null;
            }
        }

        //-------------------------------------------------------------------
        // SetVideoType
        //
        // Set the video format.
        //-------------------------------------------------------------------
        public HResult SetVideoType(IMFMediaType pType)
        {
            HResult hr = HResult.S_OK;
            Guid subtype;
            MFRatio PAR = new MFRatio();

            // Find the video subtype.
            hr = pType.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out subtype);
            if (Failed(hr)) { goto done; }

            // Choose a conversion function.
            // (This also validates the format type.)

            hr = SetConversionFunction(subtype);
            if (Failed(hr)) { goto done; }

            //
            // Get some video attributes.
            //

            // Get the frame size.
            hr = MFExtern.MFGetAttributeSize(pType, MFAttributesClsid.MF_MT_FRAME_SIZE, out m_width, out m_height);
            if (Failed(hr)) { goto done; }

            // Get the image stride.
            hr = GetDefaultStride(pType, out m_lDefaultStride);
            if (Failed(hr)) { goto done; }

            // Get the pixel aspect ratio. Default: Assume square pixels (1:1)
            hr = MFExtern.MFGetAttributeRatio(pType, MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, out PAR.Numerator, out PAR.Denominator);

            if (Succeeded(hr))
            {
                m_PixelAR = PAR;
            }
            else
            {
                m_PixelAR.Numerator = m_PixelAR.Denominator = 1;
            }

            FourCC f = new FourCC(subtype);
            m_format = (Format)f.ToInt32();

            // Create Direct3D swap chains.

            hr = CreateSwapChains();
            if (Failed(hr)) { goto done; }

            // Update the destination rectangle for the correct
            // aspect ratio.

            UpdateDestinationRect();

        done:
            if (Failed(hr))
            {
                m_format = Format.Unknown;
                m_convertFn = null;
            }

            return hr;
        }

        //-------------------------------------------------------------------
        // DrawFrame
        //
        // Draw the video frame.
        //-------------------------------------------------------------------
        public HResult DrawFrame(IMFMediaBuffer pCaptureDeviceBuffer)
        {
            if (m_convertFn == null)
            {
                return HResult.MF_E_INVALIDREQUEST;
            }

            HResult hr = HResult.S_OK;
            IntPtr pbScanline0;
            int lStride = 0;
            Result res;

            Surface pSurf = null;
            Surface pBB = null;

            if (m_pDevice == null || m_pSwapChain == null)
            {
                return HResult.S_OK;
            }

            // Helper object to lock the video buffer.
            using (VideoBufferLock xbuffer = new VideoBufferLock(pCaptureDeviceBuffer))
            {
                hr = TestCooperativeLevel();
                if (Failed(hr)) { goto done; }

                // Lock the video buffer. This method returns a pointer to the first scan
                // line in the image, and the stride in bytes.

                hr = xbuffer.LockBuffer(m_lDefaultStride, m_height, out pbScanline0, out lStride);
                if (Failed(hr)) { goto done; }

                // Get the swap-chain surface.
                pSurf = m_pSwapChain.GetBackBuffer(0);

                // Lock the swap-chain surface and get Graphic stream object.
                DataRectangle dr = pSurf.LockRectangle(LockFlags.NoSystemLock);

                try
                {
                    using (dr.Data)
                    {
                        // Convert the frame. This also copies it to the Direct3D surface.
                        m_convertFn(dr.Data.DataPointer, dr.Pitch, pbScanline0, lStride, m_width, m_height);
                    }
                }
                finally
                {
                    res = pSurf.UnlockRectangle();
                    MFError.ThrowExceptionForHR(res.Code);
                }
            }

            // Color fill the back buffer.
            pBB = m_pDevice.GetBackBuffer(0, 0);

            m_pDevice.ColorFill(pBB, Color.FromArgb(0, 0, 0x80));

            // Blit the frame.
            Rectangle r = new Rectangle(0, 0, m_width, m_height);

            res = m_pDevice.StretchRectangle(pSurf, r, pBB, m_rcDest, TextureFilter.Linear);
            hr = (HResult)res.Code;

            if (res.IsSuccess)
            {
                // Present the frame.
                res = m_pDevice.Present();
                hr = (HResult)res.Code;
            }

        done:
            SafeRelease(pBB);
            SafeRelease(pSurf);

            return hr;
        }

        //-------------------------------------------------------------------
        //  IsFormatSupported
        //
        //  Query if a format is supported.
        //-------------------------------------------------------------------
        public bool IsFormatSupported(Guid subtype)
        {
            for (int i = 0; i < VideoFormatDefs.Length; i++)
            {
                if (subtype == VideoFormatDefs[i].SubType)
                {
                    return true;
                }
            }

            return false;
        }

        //-------------------------------------------------------------------
        // GetFormat
        //
        // Get a supported output format by index.
        //-------------------------------------------------------------------
        public HResult GetFormat(int index, out Guid pSubtype)
        {
            if (index < VideoFormatDefs.Length)
            {
                pSubtype = VideoFormatDefs[index].SubType;
                return HResult.S_OK;
            }

            pSubtype = Guid.Empty;
            return HResult.MF_E_NO_MORE_TYPES;
        }

        #endregion

        #region Static Methods

        //-------------------------------------------------------------------
        //
        // Conversion functions
        //
        //-------------------------------------------------------------------

        private static byte Clip(int clr)
        {
            return (byte)(clr < 0 ? 0 : (clr > 255 ? 255 : clr));
        }

        private static RGBQUAD ConvertYCrCbToRGB(byte y, byte cr, byte cb)
        {
            RGBQUAD rgbq = new RGBQUAD();

            int c = y - 16;
            int d = cb - 128;
            int e = cr - 128;

            rgbq.R = Clip((298 * c + 409 * e + 128) >> 8);
            rgbq.G = Clip((298 * c - 100 * d - 208 * e + 128) >> 8);
            rgbq.B = Clip((298 * c + 516 * d + 128) >> 8);

            return rgbq;
        }

        //-------------------------------------------------------------------
        // TransformImage_RGB24
        //
        // RGB-24 to RGB-32
        //-------------------------------------------------------------------
        unsafe private static void TransformImage_RGB24(IntPtr pDest, int lDestStride, IntPtr pSrc, int lSrcStride, int dwWidthInPixels, int dwHeightInPixels)
        {
            RGB24* source = (RGB24*)pSrc;
            RGBQUAD* dest = (RGBQUAD*)pDest;

            lSrcStride /= 3;
            lDestStride /= 4;

            for (int y = 0; y < dwHeightInPixels; y++)
            {
                for (int x = 0; x < dwWidthInPixels; x++)
                {
                    dest[x].R = source[x].rgbRed;
                    dest[x].G = source[x].rgbGreen;
                    dest[x].B = source[x].rgbBlue;
                    dest[x].A = 0;
                }

                source += lSrcStride;
                dest += lDestStride;
            }
        }

        //-------------------------------------------------------------------
        // TransformImage_RGB32
        //
        // RGB-32 to RGB-32
        //
        // Note: This function is needed to copy the image from system
        // memory to the Direct3D surface.
        //-------------------------------------------------------------------
        private static void TransformImage_RGB32(IntPtr pDest, int lDestStride, IntPtr pSrc, int lSrcStride, int dwWidthInPixels, int dwHeightInPixels)
        {
            MediaFoundation.MFExtern.MFCopyImage(pDest, lDestStride, pSrc, lSrcStride, dwWidthInPixels * 4, dwHeightInPixels);
        }

        //-------------------------------------------------------------------
        // TransformImage_YUY2
        //
        // YUY2 to RGB-32
        //-------------------------------------------------------------------
        unsafe private static void TransformImage_YUY2(
            IntPtr pDest,
            int lDestStride,
            IntPtr pSrc,
            int lSrcStride,
            int dwWidthInPixels,
            int dwHeightInPixels)
        {
            YUYV* pSrcPel = (YUYV*)pSrc;
            RGBQUAD* pDestPel = (RGBQUAD*)pDest;

            lSrcStride /= 4; // convert lSrcStride to YUYV
            lDestStride /= 4; // convert lDestStride to RGBQUAD

            for (int y = 0; y < dwHeightInPixels; y++)
            {
                for (int x = 0; x < dwWidthInPixels / 2; x++)
                {
                    pDestPel[x * 2] = ConvertYCrCbToRGB(pSrcPel[x].Y, pSrcPel[x].V, pSrcPel[x].U);
                    pDestPel[(x * 2) + 1] = ConvertYCrCbToRGB(pSrcPel[x].Y2, pSrcPel[x].V, pSrcPel[x].U);
                }

                pSrcPel += lSrcStride;
                pDestPel += lDestStride;
            }
        }

        //-------------------------------------------------------------------
        // TransformImage_NV12
        //
        // NV12 to RGB-32
        //-------------------------------------------------------------------
        unsafe private static void TransformImage_NV12(IntPtr pDest, int lDestStride, IntPtr pSrc, int lSrcStride, int dwWidthInPixels, int dwHeightInPixels)
        {
            Byte* lpBitsY = (byte*)pSrc;
            Byte* lpBitsCb = lpBitsY + (dwHeightInPixels * lSrcStride);
            Byte* lpBitsCr = lpBitsCb + 1;

            Byte* lpLineY1;
            Byte* lpLineY2;
            Byte* lpLineCr;
            Byte* lpLineCb;

            Byte* lpDibLine1 = (Byte*)pDest;
            for (UInt32 y = 0; y < dwHeightInPixels; y += 2)
            {
                lpLineY1 = lpBitsY;
                lpLineY2 = lpBitsY + lSrcStride;
                lpLineCr = lpBitsCr;
                lpLineCb = lpBitsCb;

                Byte* lpDibLine2 = lpDibLine1 + lDestStride;

                for (UInt32 x = 0; x < dwWidthInPixels; x += 2)
                {
                    byte y0 = lpLineY1[0];
                    byte y1 = lpLineY1[1];
                    byte y2 = lpLineY2[0];
                    byte y3 = lpLineY2[1];
                    byte cb = lpLineCb[0];
                    byte cr = lpLineCr[0];

                    RGBQUAD r = ConvertYCrCbToRGB(y0, cr, cb);
                    lpDibLine1[0] = r.B;
                    lpDibLine1[1] = r.G;
                    lpDibLine1[2] = r.R;
                    lpDibLine1[3] = 0; // Alpha

                    r = ConvertYCrCbToRGB(y1, cr, cb);
                    lpDibLine1[4] = r.B;
                    lpDibLine1[5] = r.G;
                    lpDibLine1[6] = r.R;
                    lpDibLine1[7] = 0; // Alpha

                    r = ConvertYCrCbToRGB(y2, cr, cb);
                    lpDibLine2[0] = r.B;
                    lpDibLine2[1] = r.G;
                    lpDibLine2[2] = r.R;
                    lpDibLine2[3] = 0; // Alpha

                    r = ConvertYCrCbToRGB(y3, cr, cb);
                    lpDibLine2[4] = r.B;
                    lpDibLine2[5] = r.G;
                    lpDibLine2[6] = r.R;
                    lpDibLine2[7] = 0; // Alpha

                    lpLineY1 += 2;
                    lpLineY2 += 2;
                    lpLineCr += 2;
                    lpLineCb += 2;

                    lpDibLine1 += 8;
                    lpDibLine2 += 8;
                }

                pDest += (2 * lDestStride);
                lpBitsY += (2 * lSrcStride);
                lpBitsCr += lSrcStride;
                lpBitsCb += lSrcStride;
            }
        }

        //-------------------------------------------------------------------
        // LetterBoxDstRect
        //
        // Takes a src rectangle and constructs the largest possible
        // destination rectangle within the specifed destination rectangle
        // such that the video maintains its current shape.
        //
        // This function assumes that pels are the same shape within both the
        // source and destination rectangles.
        //
        //-------------------------------------------------------------------
        private static Rectangle LetterBoxRect(Rectangle rcSrc, Rectangle rcDst)
        {
            int iDstLBWidth;
            int iDstLBHeight;

            if (MulDiv(rcSrc.Width, rcDst.Height, rcSrc.Height) <= rcDst.Width)
            {
                // Column letter boxing ("pillar box")

                iDstLBWidth = MulDiv(rcDst.Height, rcSrc.Width, rcSrc.Height);
                iDstLBHeight = rcDst.Height;
            }
            else
            {
                // Row letter boxing.

                iDstLBWidth = rcDst.Width;
                iDstLBHeight = MulDiv(rcDst.Width, rcSrc.Height, rcSrc.Width);
            }

            // Create a centered rectangle within the current destination rect

            int left = rcDst.Left + ((rcDst.Width - iDstLBWidth) / 2);
            int top = rcDst.Top + ((rcDst.Height - iDstLBHeight) / 2);

            Rectangle rc = new Rectangle(left, top, iDstLBWidth, iDstLBHeight);

            return rc;
        }

        //-----------------------------------------------------------------------------
        // CorrectAspectRatio
        //
        // Converts a rectangle from the source's pixel aspect ratio (PAR) to 1:1 PAR.
        // Returns the corrected rectangle.
        //
        // For example, a 720 x 486 rect with a PAR of 9:10, when converted to 1x1 PAR,
        // is stretched to 720 x 540.
        //-----------------------------------------------------------------------------
        private static Rectangle CorrectAspectRatio(Rectangle src, MFRatio srcPAR)
        {
            // Start with a rectangle the same size as src, but offset to the origin (0,0).
            Rectangle rc = new Rectangle(0, 0, src.Right - src.Left, src.Bottom - src.Top);
            int rcNewWidth = rc.Right;
            int rcNewHeight = rc.Bottom;

            if ((srcPAR.Numerator != 1) || (srcPAR.Denominator != 1))
            {
                // Correct for the source's PAR.

                if (srcPAR.Numerator > srcPAR.Denominator)
                {
                    // The source has "wide" pixels, so stretch the width.
                    rcNewWidth = MulDiv(rc.Right, srcPAR.Numerator, srcPAR.Denominator);
                }
                else if (srcPAR.Numerator < srcPAR.Denominator)
                {
                    // The source has "tall" pixels, so stretch the height.
                    rcNewHeight = MulDiv(rc.Bottom, srcPAR.Denominator, srcPAR.Numerator);
                }
                // else: PAR is 1:1, which is a no-op.
            }

            rc = new Rectangle(0, 0, rcNewWidth, rcNewHeight);
            return rc;
        }

        //-----------------------------------------------------------------------------
        // GetDefaultStride
        //
        // Gets the default stride for a video frame, assuming no extra padding bytes.
        //
        //-----------------------------------------------------------------------------
        private static HResult GetDefaultStride(IMFMediaType pType, out int plStride)
        {
            int lStride = 0;
            plStride = 0;

            // Try to get the default stride from the media type.
            HResult hr = pType.GetUINT32(MFAttributesClsid.MF_MT_DEFAULT_STRIDE, out lStride);

            if (Failed(hr))
            {
                // Attribute not set. Try to calculate the default stride.
                Guid subtype = Guid.Empty;

                int width = 0;
                int height = 0;

                // Get the subtype and the image size.
                hr = pType.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out subtype);
                if (Succeeded(hr))
                {
                    hr = MFExtern.MFGetAttributeSize(pType, MFAttributesClsid.MF_MT_FRAME_SIZE, out width, out height);
                }

                if (Succeeded(hr))
                {
                    FourCC f = new FourCC(subtype);

                    hr = MediaFoundation.MFExtern.MFGetStrideForBitmapInfoHeader(f.ToInt32(), width, out lStride);
                }

                // Set the attribute for later reference.
                if (Succeeded(hr))
                {
                    hr = pType.SetUINT32(MFAttributesClsid.MF_MT_DEFAULT_STRIDE, lStride);
                }
            }

            if (Succeeded(hr))
            {
                plStride = lStride;
            }

            return hr;
        }

        public static Rectangle GetClientRect(IntPtr hWnd)
        {
            Rectangle result = new Rectangle();
            GetClientRect(hWnd, out result);
            return result;
        }

        #endregion
    }
}
