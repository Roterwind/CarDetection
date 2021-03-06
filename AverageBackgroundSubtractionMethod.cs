﻿using System;
using OpenCvSharp;
using OpenCvSharp.Blob;

namespace CarDetection
{
	class AverageBackgroundSubtractionMethod
	{
		public const String FILE_NAME = "merged_day_small.avi";
		public const String BACKDROUND_IMAGE_FILE_NAME = "background.bmp";
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

			//int counter = 0;

			IplImage backgroundImage = IplImage.FromFile(BACKDROUND_IMAGE_FILE_NAME);
			IplImage grayBackgroundImage = Cv.CreateImage(backgroundImage.Size, backgroundImage.Depth, 1);
			Cv.CvtColor(backgroundImage, grayBackgroundImage, ColorConversion.RgbToGray);

			Console.WriteLine("NChannels = " + backgroundImage.NChannels);
			Console.ReadKey();
			//IplImage grayBackgroundImage = Cv.CreateImage(backgroundImage.Size, backgroundImage.Depth, 1);
			//Cv.CvtColor(backgroundImage, grayBackgroundImage, ColorConversion.RgbToGray););

			while(true)
			{
				var currentOgirinalFrame = videoCaprure.QueryFrame();
				if (currentOgirinalFrame == null)
					return;

				//counter++;
				//if (counter % 3 != 0)
				//    continue;

				IplImage grayOriginalFrame = Cv.CreateImage(currentOgirinalFrame.Size, currentOgirinalFrame.Depth, 1);
				Cv.CvtColor(currentOgirinalFrame, grayOriginalFrame, ColorConversion.RgbToGray);
				IplImage differenceBetweenFrames = Cv.CreateImage(grayOriginalFrame.Size, grayOriginalFrame.Depth, 1);

				Cv.AbsDiff(grayOriginalFrame, grayBackgroundImage, differenceBetweenFrames);
				//Cv.Smooth(differenceBetweenFrames, differenceBetweenFrames, SmoothType.Blur);
				//IplImage graydifferenceBetweenFrames = Cv.CreateImage(differenceBetweenFrames.Size, differenceBetweenFrames.Depth, 1);
				//Cv.CvtColor(differenceBetweenFrames, graydifferenceBetweenFrames, ColorConversion.RgbToGray);
				//Cv.ShowImage("differenceBetweenFrames", differenceBetweenFrames);


				Cv.Threshold(differenceBetweenFrames, differenceBetweenFrames, 50, 255, ThresholdType.Binary);

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
				//Cv.ShowImage("backgroundImage", backgroundImage);
				Cv.WaitKey(delay);

				//currentOgirinalFrame.Copy(differenceBetweenFrames);
			}
		}
	}
}
