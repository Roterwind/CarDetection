using System;
using OpenCvSharp;

namespace CarDetection
{
	class Program
	{
		public const String FILE_NAME = "merged_night.avi";
		public const String MAIN_WINDOW_NAME = "main";

		static void Main(string[] args)
		{
			CvCapture videoCaprure = null;
			try
			{
				videoCaprure = CvCapture.FromFile(FILE_NAME);
			}
			catch (Exception e)
			{
				Console.WriteLine("Unable to open file {0}", FILE_NAME);
				Console.WriteLine(e.ToString());
				Console.ReadKey();
			}

			Cv.NamedWindow(MAIN_WINDOW_NAME, WindowMode.AutoSize);

			double rate = videoCaprure.GetCaptureProperty(CvConst.CV_CAP_PROP_FPS);
			int delay = (int) (1000/ rate);

			IplImage previousOriginalFrame = null;

			int counter = 0;

			while(true)
			{
				var currentOgirinalFrame = videoCaprure.QueryFrame();
				if (currentOgirinalFrame == null)
					return;

				if (previousOriginalFrame == null)
				{
					previousOriginalFrame = Cv.CreateImage(currentOgirinalFrame.Size, currentOgirinalFrame.Depth, currentOgirinalFrame.NChannels);
					currentOgirinalFrame.Copy(previousOriginalFrame);
					continue;
				}

				counter++;
				if (counter % 5 != 0)
					continue;

				//algo
				IplImage currentGrayImage = Cv.CreateImage(currentOgirinalFrame.Size, currentOgirinalFrame.Depth, 1);
				Cv.CvtColor(currentOgirinalFrame, currentGrayImage, ColorConversion.RgbToGray);
				IplImage previousGrayImage = Cv.CreateImage(previousOriginalFrame.Size, previousOriginalFrame.Depth, 1);
				Cv.CvtColor(previousOriginalFrame, previousGrayImage, ColorConversion.RgbToGray);

				IplImage differenceBetweenFrames = Cv.CreateImage(currentGrayImage.Size, currentGrayImage.Depth, 1);
				Cv.Sub(currentGrayImage, previousGrayImage, differenceBetweenFrames);
				Cv.Threshold(differenceBetweenFrames, differenceBetweenFrames, 10, 255, ThresholdType.Binary);
				Cv.Erode(differenceBetweenFrames, differenceBetweenFrames);

				Cv.ShowImage(MAIN_WINDOW_NAME, currentOgirinalFrame);
				Cv.ShowImage("Result", differenceBetweenFrames);
				Cv.WaitKey(delay * 4);

				currentOgirinalFrame.Copy(previousOriginalFrame);
			}
		}
	}
}
