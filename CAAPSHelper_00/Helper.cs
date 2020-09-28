using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput.Native;

namespace CAAPSHelper_00
{
    class Helper
    {

        protected Thread cl_helperthread = null;

        EmguTempMatch cl_etm = new EmguTempMatch();

        Image<Gray, Byte> cl_templImg_aam16 = new Image<Gray, Byte>(0, 0);
        Image<Gray, Byte> cl_templImg_stopmacro = new Image<Gray, Byte>(0, 0);

        Image<Gray, Byte> cl_sourceImage = new Image<Gray, Byte>(0, 0);

        WindowsInput.InputSimulator sim = new WindowsInput.InputSimulator();

        //PCSWS target Screen Rect: x,y,w,h
        int[] myWindow_Rect_Selected = { 20, 30, 800, 500 };

        //Get Screen res
        Rectangle resolution = Screen.PrimaryScreen.Bounds;

        int cl_resx;
        int cl_resy;

        ProcWindowHandler cl_handler;

        public Helper(ProcWindowHandler in_pwh)
        {
            this.cl_handler = in_pwh;
            cl_resx = Convert.ToInt16(65535 / resolution.Width);
            cl_resy = Convert.ToInt16(65535 / resolution.Height);

            try
            {
                cl_templImg_aam16 = new Image<Gray, Byte>(new Bitmap(@"tlimgs\am16.jpg"));
                cl_templImg_stopmacro = new Image<Gray, Byte>(new Bitmap(@"tlimgs\stopmacro.jpg"));
            }
            catch
            {
                MessageBox.Show("Unable to load tempImgs");
            }
        }

        public void StartHelperThread()
        {
            if (cl_helperthread != null)
            {
                if (cl_helperthread.ThreadState.ToString() != "Running")
                {
                    cl_helperthread = new Thread(HelperMethod);
                    cl_helperthread.Start();
                }
            }
            else
            {
                cl_helperthread = new Thread(HelperMethod);
                cl_helperthread.Start();
            }
        }

        private void HelperMethod()
        {

            bool my_gotMacroStopBtn = false;
            Point my_resultPoint_stopmacro = new Point(0, 0);
            Point my_resultPoint_am16 = new Point(0, 0);

            if (cl_handler.getHandleByPID() == IntPtr.Zero)
            {
                cl_helperthread.Abort();
                MessageBox.Show("Handler lost", "Helper thread aborted! ", MessageBoxButtons.OK);
            }

            while (true)
            {
                Thread.Sleep(2000);

                //Screenshot -> load into cl_sourceImage
                PrtScr();

                //Where is the macro stop btn?
                if (!my_gotMacroStopBtn)
                {
                    my_resultPoint_stopmacro = cl_etm.ReturnMatch(cl_sourceImage, cl_templImg_stopmacro, 0.6f);
                    if (my_resultPoint_stopmacro.X != 0 && my_resultPoint_stopmacro.Y != 0)
                    {
                        my_gotMacroStopBtn = true;
                        MessageBox.Show("sTOPMACRO BTN Match found @ X=" + my_resultPoint_stopmacro.X.ToString() + " Y=" + my_resultPoint_stopmacro.Y.ToString());
                    }
                }

                //Find annoying mann
                PrtScr();
                my_resultPoint_am16 = cl_etm.ReturnMatch(cl_sourceImage, cl_templImg_aam16, 0.6f);

                if (my_resultPoint_am16.X != 0 && my_resultPoint_am16.Y != 0)
                {
                    MessageBox.Show("A.M. Match found @ X=" + my_resultPoint_am16.X.ToString() + " Y=" + my_resultPoint_am16.Y.ToString());
                    if (my_gotMacroStopBtn)
                    {
                        cl_handler.bringToFront();

                        sim.Mouse
                            .Sleep(1000)
                            .MoveMouseTo(my_resultPoint_stopmacro.X * cl_resx, my_resultPoint_stopmacro.Y * cl_resy)
                            .LeftButtonClick();

                        MessageBox.Show("Clicked on macrostop");
                        my_resultPoint_am16 = new Point(0, 0);
                    }
                }

            }
        }


        public void PrtScr()
        {
            //Try to make a screenshot
            try
            {
                cl_handler.bringToFront();
                cl_handler.SetWRect(myWindow_Rect_Selected[0], myWindow_Rect_Selected[1], myWindow_Rect_Selected[2], myWindow_Rect_Selected[3]);
                cl_handler.bringToFront();
                CAAPSHelper_00.ProcWindowHandler.Rect ProcWindow = cl_handler.GetWRect();

                int x = Convert.ToInt32(ProcWindow.Left);
                int y = Convert.ToInt32(ProcWindow.Right);
                int w = Convert.ToInt32(ProcWindow.Bottom - ProcWindow.Left);
                int h = Convert.ToInt32(ProcWindow.Top - ProcWindow.Right);

                System.Threading.Thread.Sleep(100);
                cl_sourceImage = new Image<Gray, Byte>(w, h);
                cl_sourceImage = Screener.PrtScr_EmguImg(x, y, w, h);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot make screenshot!    " + ex);
            }
        }

    }

    static class Screener
    {

        private static Bitmap bmpScreenshot;
        private static Graphics gfxScreenshot;

        public static void PrtScr_File(int x, int y, int w, int h)
        {
            // Set the bitmap object to the size of the screen
            bmpScreenshot = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            // Create a graphics object from the bitmap
            gfxScreenshot = Graphics.FromImage(bmpScreenshot);
            // Take the screenshot from the upper left corner to the right bottom corner
            gfxScreenshot.CopyFromScreen(x, y, 0, 0, bmpScreenshot.Size);
            // Save the screenshot to the specified path that the user has chosen
            bmpScreenshot.Save(@"C:\Temp\printscreen.png", ImageFormat.Png);

        }

        public static Image<Gray, Byte> PrtScr_EmguImg(int x, int y, int w, int h)
        {
            // Set the bitmap object to the size of the screen
            bmpScreenshot = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            // Create a graphics object from the bitmap
            gfxScreenshot = Graphics.FromImage(bmpScreenshot);
            // Take the screenshot from the upper left corner to the right bottom corner
            gfxScreenshot.CopyFromScreen(x, y, 0, 0, bmpScreenshot.Size);

            return new Image<Gray, Byte>(bmpScreenshot);
        }



    }

}
