using Emgu.CV;
using System;
using System.Collections.Generic;

namespace CAAPSHelper_00
{
    public interface IProcSearcher
    {
        List<string> getWTitlesByProcName(string in_procname);
        List<string> getAllProcessNames();
    }

    public interface ITempMatcher
    {
        System.Drawing.Point ReturnMatch(
            Image<Emgu.CV.Structure.Gray, Byte> in_sourceImage, 
            Image<Emgu.CV.Structure.Gray, Byte> in_templateImage, 
            float in_threshold,
            bool showres = false
            );
    }
}
