using System;
using OpenCvSharp;
using OpenCvSharp.Blob;

namespace CarDetection
{
	class SubtractionMethod
	{
		public const String FILE_NAME = "better_video.mp4";
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
				if (counter % 3 != 0)
					continue;

				//algo
				IplImage currentGrayImage = Cv.CreateImage(currentOgirinalFrame.Size, currentOgirinalFrame.Depth, 1);
				Cv.CvtColor(currentOgirinalFrame, currentGrayImage, ColorConversion.RgbToGray);
				IplImage previousGrayImage = Cv.CreateImage(previousOriginalFrame.Size, previousOriginalFrame.Depth, 1);
				Cv.CvtColor(previousOriginalFrame, previousGrayImage, ColorConversion.RgbToGray);

				IplImage differenceBetweenFrames = Cv.CreateImage(currentGrayImage.Size, currentGrayImage.Depth, 1);
				Cv.AbsDiff(previousGrayImage, currentGrayImage, differenceBetweenFrames);
				Cv.Threshold(differenceBetweenFrames, differenceBetweenFrames, 10, 255, ThresholdType.Binary);
				Cv.Erode(differenceBetweenFrames, differenceBetweenFrames);

				//finding blobs
				CvBlobs blobs = new CvBlobs(differenceBetweenFrames);
				blobs.FilterByArea(300, 10000);
				//blobs.Label(differenceBetweenFrames);

				var currentFrameWithRedRects = Cv.CreateImage(currentOgirinalFrame.Size, currentOgirinalFrame.Depth, currentOgirinalFrame.NChannels);
				currentOgirinalFrame.Copy(currentFrameWithRedRects);
				foreach (var cvBlob in blobs)
				{
					Cv.Rectangle(currentFrameWithRedRects, cvBlob.Value.Rect, CvColor.Red, 4);
				}

				Console.WriteLine(blobs.Count);

				Cv.ShowImage(MAIN_WINDOW_NAME, currentFrameWithRedRects);
				Cv.ShowImage("Result", differenceBetweenFrames);
				Cv.WaitKey(delay * 4);

				currentOgirinalFrame.Copy(previousOriginalFrame);
			}
		}
	}
}
