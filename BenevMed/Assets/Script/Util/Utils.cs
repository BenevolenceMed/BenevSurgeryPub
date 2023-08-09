using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    static public double FrameToTime(long frame, int fps=30)
    {
        return ((double)frame) / ((double)fps);
    }

    static public long TimeToFrame(double time, int fps=30)
    {
        return (long)(time*((double)fps));
    }
}
