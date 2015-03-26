using System;
using System.Collections.Generic;
using System.Text;

namespace Testv10
{
    class IMFPresentationClockTest
    {
        // Tested by adding this code to BasicPlayer's pause

#if false
            IMFPresentationClock m_pc;
            long lTime;
            IMFPresentationTimeSource pts;
            IMFClockStateSink pSink = new Test();
            IMFClock m_c;
            hr = m_pSession.GetClock(out m_c);
            MFError.ThrowExceptionForHR(hr);

            m_pc = m_c as IMFPresentationClock;
            hr = m_pc.AddClockStateSink(pSink);
            MFError.ThrowExceptionForHR(hr);
            hr = m_pc.GetTime(out lTime);
            MFError.ThrowExceptionForHR(hr);
            hr = m_pc.GetTimeSource(out pts);
            MFError.ThrowExceptionForHR(hr);
            hr = m_pc.SetTimeSource(pts);
            MFError.ThrowExceptionForHR(hr);
            hr = m_pc.Start(0x7fffffffffffffff);
            MFError.ThrowExceptionForHR(hr);
            hr = m_pc.Pause();
            MFError.ThrowExceptionForHR(hr);
            hr = m_pc.Start(lTime);
            MFError.ThrowExceptionForHR(hr);
            hr = m_pc.Stop();
            MFError.ThrowExceptionForHR(hr);
            hr = m_pc.RemoveClockStateSink(pSink);
            MFError.ThrowExceptionForHR(hr);

            m_state = PlayerState.PausePending;
            NotifyState();

class Test : COMBase, IMFClockStateSink
{
    public int iVal = 0;

        #region IMFClockStateSink Members

    public void OnClockStart(long hnsSystemTime, long llClockStartOffset)
    {
        iVal |= 1;
    }

    public void OnClockStop(long hnsSystemTime)
    {
        iVal |= 2;
    }

    public void OnClockPause(long hnsSystemTime)
    {
        iVal |= 4;
    }

    public void OnClockRestart(long hnsSystemTime)
    {
        iVal |= 8;
    }

    public void OnClockSetRate(long hnsSystemTime, float flRate)
    {
        iVal |= 16;
    }

        #endregion
}

#endif

    }
}
