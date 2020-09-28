using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace CAAPSHelper_00
{
    class EmguTempMatch : ITempMatcher
    {

        public Point ReturnMatch(
            Image<Gray, Byte> in_sourceImage, 
            Image<Gray, Byte> in_templateImage, 
            float in_threshold,
            bool showres = false
            )
        {
            //Possible template matching methods:
            //method=CV_TM_SQDIFF
            //method=CV_TM_SQDIFF_NORMED
            //method=CV_TM_CCORR
            //method=CV_TM_CCORR_NORMED
            //method=CV_TM_CCOEFF
            //method=CV_TM_CCOEFF_NORMED

            //Image<Gray, Byte> templateImage = new Image<Gray, Byte>(templateImageFile);
            //Image<Gray, Byte> sourceImage2 = new Image<Gray, Byte>(sourceImageFile);
            Image<Gray, float> my_imgMatch =
            in_sourceImage.MatchTemplate(in_templateImage, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCOEFF_NORMED);

            double[] min, max;
            Point[] pointMin, pointMax;
            my_imgMatch.MinMax(out min, out max, out pointMin, out pointMax);

            if (showres)
            {
                in_sourceImage.ROI = new Rectangle(pointMax[0].X, pointMax[0].Y, in_templateImage.Width, in_templateImage.Height);

                Image<Gray, Byte> imgotcha = in_sourceImage.Copy();

                String win1 = "ImReco"; //The name of the window
                CvInvoke.cvNamedWindow(win1); //Create the window using the specific name

                CvInvoke.cvShowImage(win1, imgotcha); //Show the image

                CvInvoke.cvWaitKey(0);  //Wait for the key pressing event

                CvInvoke.cvDestroyWindow(win1); //Destory the window
            }
            
            //Array structures will contain all matchings
            //Probably the best match is the first item [0]

            if (max[0] > in_threshold)
            {
                return pointMax[0];
            }
            else
            {
                return new Point(0, 0);
            }
        }
    }
}
