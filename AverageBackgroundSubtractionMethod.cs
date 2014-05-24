using System;
using OpenCvSharp;
using OpenCvSharp.Blob;

namespace CarDetection
{
	class AverageBackgroundSubtractionMethod
	{
		public const String FILE_NAME = "better_video.mp4";
		public const String BACKDROUND_IMAGE_FILE_NAME = "background_image.jpg";
		public const String MAIN_WINDOW_NAME = "main";

		static void Main2(string[] args)
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
			//IplImage grayBackgroundImage = Cv.CreateImage(backgroundImage.Size, backgroundImage.Depth, 1);
			//Cv.CvtColor(backgroundImage, grayBackgroundImage, ColorConversion.RgbToGray);

			while(true)
			{
				var currentOgirinalFrame = videoCaprure.QueryFrame();
				if (currentOgirinalFrame == null)
					return;

				//counter++;
				//if (counter % 3 != 0)
				//    continue;

				//algo
				//IplImage currentGrayImage = Cv.CreateImage(currentOgirinalFrame.Size, currentOgirinalFrame.Depth, 1);
				//Cv.CvtColor(currentOgirinalFrame, currentGrayImage, ColorConversion.RgbToGray);





				IplImage differenceBetweenFrames = Cv.CreateImage(currentOgirinalFrame.Size, currentOgirinalFrame.Depth, currentOgirinalFrame.NChannels);

				Cv.AbsDiff(currentOgirinalFrame, backgroundImage, differenceBetweenFrames);
				Cv.Smooth(differenceBetweenFrames, differenceBetweenFrames, SmoothType.Blur);
				IplImage graydifferenceBetweenFrames = Cv.CreateImage(differenceBetweenFrames.Size, differenceBetweenFrames.Depth, 1);
				Cv.CvtColor(differenceBetweenFrames, graydifferenceBetweenFrames, ColorConversion.RgbToGray);
				Cv.ShowImage("differenceBetweenFrames", differenceBetweenFrames);

					
				Cv.Threshold(graydifferenceBetweenFrames, graydifferenceBetweenFrames, 50, 255, ThresholdType.Binary);
				
				Cv.Erode(graydifferenceBetweenFrames, graydifferenceBetweenFrames);

				//finding blobs
				CvBlobs blobs = new CvBlobs(graydifferenceBetweenFrames);
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
				Cv.ShowImage("Result", graydifferenceBetweenFrames);
				//Cv.ShowImage("backgroundImage", backgroundImage);
				Cv.WaitKey(delay * 2);

				//currentOgirinalFrame.Copy(differenceBetweenFrames);
			}
		}
	}
}
