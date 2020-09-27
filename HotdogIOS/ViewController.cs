using Acr.UserDialogs;
using CoreFoundation;
using CoreGraphics;
using CoreImage;
using CoreML;
using CoreVideo;
using Foundation;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using UIKit;
using Vision;
using Xamarin.Essentials;

namespace HotdogIOS
{
    public partial class ViewController : UIViewController
    {

        public ViewController (IntPtr handle) : base (handle)
        {
        }

        private float confidence = 0.0f;
        public static string Type = "Hotdog";

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.
            spinnyboy.StopAnimating();
            spinnyboy.Hidden = true;
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void TakePhotoButton_TouchUpInside(UIButton sender)
        {
            DoCamera();
        }

        private async void PickCamera()
        {
            if(CrossMedia.Current == null)
            {
                await CrossMedia.Current.Initialize();
            }
            

            if (!CrossMedia.Current.IsCameraAvailable ||
                 !CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsPickPhotoSupported)
            {
                UserDialogs.Instance.Alert("Device options not supported.", null, "OK");
                return;
            }

            Console.WriteLine("Pcking Photo..");

            var file = await CrossMedia.Current.PickPhotoAsync();
            {
            };

            if (file == null)
            {
                UserDialogs.Instance.Alert("You didn't pick a photo.", null, "OK");
                return;
            }

            Stream s = file.GetStream();
            byte[] result = null;
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = s.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                result = ms.ToArray();
            }

            //show and tell shit, then Run ML
            SEEFOOD.Hidden = true;
            stat.Hidden = false;
            selectCamRollButton.Hidden = true;
            takePhotoButton.Hidden = true;
            image.Hidden = false;
            spinnyboy.Hidden = false;
            this.hotdogLbl.Hidden = true;
            this.ramenLbl.Hidden = true;
            this.ramenOrHotdog.Hidden = true;
            spinnyboy.StartAnimating();
            returnToMenu.Hidden = false;
            this.stat.Text = "Analyzing...";
            this.stat.TextColor = UIKit.UIColor.Black;
            showDebugInfo.Hidden = false;
            var data = NSData.FromArray(result);
            image.Image = UIImage.LoadFromData(data);
            await Task.Delay(1000);
            //ML

            Console.WriteLine("Selected: " + ViewController.Type.ToString());

            //First we check what type of thing we have here
            if (ViewController.Type.Equals("Hotdog"))
            {
                var assetPath = NSBundle.MainBundle.GetUrlForResource("model", "mlmodel");
                var transform = MLModel.CompileModel(assetPath, out NSError compErr);
                MLModel model = MLModel.Create(transform, out NSError fucl);
                var vnModel = VNCoreMLModel.FromMLModel(model, out NSError rror);
                var ciImage = new CIImage(image.Image);
                var classificationRequest = new VNCoreMLRequest(vnModel);

                //just do it
                var handler = new VNImageRequestHandler(ciImage, ImageIO.CGImagePropertyOrientation.Up, new VNImageOptions());
                handler.Perform(new[] { classificationRequest }, out NSError perfError);
                var results = classificationRequest.GetResults<VNClassificationObservation>();
                var thing = results[0];
                Console.WriteLine("Hotdog OUT " + thing.Identifier);
                switch (thing.Identifier)
                {
                    case "hotdog":
                        if (thing.Confidence > 0.85f)
                        {
                            this.stat.Text = "✅ Hotdog";
                            this.stat.TextColor = UIKit.UIColor.Green;
                            this.stat.TextAlignment = UITextAlignment.Center;
                            spinnyboy.Hidden = true;
                            spinnyboy.StopAnimating();
                        }
                        else
                        {
                            this.stat.Text = "❌ Not Hotdog";
                            this.stat.TextColor = UIKit.UIColor.Red;
                            this.stat.TextAlignment = UITextAlignment.Center;
                            spinnyboy.Hidden = true;
                            spinnyboy.StopAnimating();
                        }

                        break;

                    case "nothotdog":
                        this.stat.Text = "❌ Not Hotdog";
                        this.stat.TextColor = UIKit.UIColor.Red;
                        this.stat.TextAlignment = UITextAlignment.Center;
                        spinnyboy.Hidden = true;
                        spinnyboy.StopAnimating();
                        break;
                }
                this.confidence = thing.Confidence;
                Vibration.Vibrate(500);
            }
            else
            {
                NSUrl modelPath = NSBundle.MainBundle.GetUrlForResource("Ramen", "mlmodel");
                if (modelPath == null)
                {
                    Console.WriteLine("peeepee");
                }
                var transform = MLModel.CompileModel(modelPath, out NSError compErr);
                MLModel model = MLModel.Create(transform, out NSError fucl);
                var vnModel = VNCoreMLModel.FromMLModel(model, out NSError rror);
                var ciImage = new CIImage(image.Image);
                var classificationRequest = new VNCoreMLRequest(vnModel);

                //just do it
                var handler = new VNImageRequestHandler(ciImage, ImageIO.CGImagePropertyOrientation.Up, new VNImageOptions());
                handler.Perform(new[] { classificationRequest }, out NSError perfError);
                var results = classificationRequest.GetResults<VNClassificationObservation>();
                var thing = results[0];
                Console.WriteLine("Ramen OUT " + thing.Identifier);
                switch (thing.Identifier)
                {
                    case "ramen":
                        if (thing.Confidence > 0.85f)
                        {
                            this.stat.Text = "✅ Ramen";
                            this.stat.TextColor = UIKit.UIColor.Green;
                            this.stat.TextAlignment = UITextAlignment.Center;
                            spinnyboy.Hidden = true;
                            spinnyboy.StopAnimating();
                        }
                        else
                        {
                            this.stat.Text = "❌ Not Ramen";
                            this.stat.TextColor = UIKit.UIColor.Red;
                            this.stat.TextAlignment = UITextAlignment.Center;
                            spinnyboy.Hidden = true;
                            spinnyboy.StopAnimating();
                        }

                        break;

                    case "notramen":
                        this.stat.Text = "❌ Not Ramen";
                        this.stat.TextColor = UIKit.UIColor.Red;
                        this.stat.TextAlignment = UITextAlignment.Center;
                        spinnyboy.Hidden = true;
                        spinnyboy.StopAnimating();
                        break;
                }
                this.confidence = thing.Confidence;
                Vibration.Vibrate(500);
            }

        }


        private async void DoCamera()
        {
            if (CrossMedia.Current == null)
            {
                await CrossMedia.Current.Initialize();
            }

            if (!CrossMedia.Current.IsCameraAvailable ||
                  !CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsPickPhotoSupported)
            {
                UserDialogs.Instance.Alert("No camera found.", null, "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                
            });

            if(file == null)
            {
                UserDialogs.Instance.Alert("You didn't take a photo.", null, "OK");
                return;
            }

            Stream s = file.GetStream();
            byte[] result = null;
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = s.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                result = ms.ToArray();
            }

            //show and tell shit, then Run ML
            SEEFOOD.Hidden = true;
            stat.Hidden = false;
            selectCamRollButton.Hidden = true;
            takePhotoButton.Hidden = true;
            image.Hidden = false;
            spinnyboy.Hidden = false;
            this.hotdogLbl.Hidden = true;
            this.ramenLbl.Hidden = true;
            this.ramenOrHotdog.Hidden = true;
            spinnyboy.StartAnimating();
            returnToMenu.Hidden = false;
            this.stat.Text = "Analyzing...";
            this.stat.TextColor = UIKit.UIColor.Black;
            showDebugInfo.Hidden = false;
            var data = NSData.FromArray(result);
            image.Image = UIImage.LoadFromData(data);
            await Task.Delay(1000);
            //ML

            Console.WriteLine("Selected: " + ViewController.Type.ToString());

            //First we check what type of thing we have here
            if(ViewController.Type.Equals("Hotdog"))
            {
                var assetPath = NSBundle.MainBundle.GetUrlForResource("model", "mlmodel");
                var transform = MLModel.CompileModel(assetPath, out NSError compErr);
                MLModel model = MLModel.Create(transform, out NSError fucl);
                var vnModel = VNCoreMLModel.FromMLModel(model, out NSError rror);
                var ciImage = new CIImage(image.Image);
                var classificationRequest = new VNCoreMLRequest(vnModel);

                //just do it
                var handler = new VNImageRequestHandler(ciImage, ImageIO.CGImagePropertyOrientation.Up, new VNImageOptions());
                handler.Perform(new[] { classificationRequest }, out NSError perfError);
                var results = classificationRequest.GetResults<VNClassificationObservation>();
                var thing = results[0];
                Console.WriteLine("Hotdog OUT " + thing.Identifier);
                switch (thing.Identifier)
                {
                    case "hotdog":
                        if(thing.Confidence > 0.85f)
                        {
                            this.stat.Text = "✅ Hotdog";
                            this.stat.TextColor = UIKit.UIColor.Green;
                            this.stat.TextAlignment = UITextAlignment.Center;
                            spinnyboy.Hidden = true;
                            spinnyboy.StopAnimating();
                        }
                        else
                        {
                            this.stat.Text = "❌ Not Hotdog";
                            this.stat.TextColor = UIKit.UIColor.Red;
                            this.stat.TextAlignment = UITextAlignment.Center;
                            spinnyboy.Hidden = true;
                            spinnyboy.StopAnimating();
                        }

                        break;
                        
                    case "nothotdog":
                        this.stat.Text = "❌ Not Hotdog";
                        this.stat.TextColor = UIKit.UIColor.Red;
                        this.stat.TextAlignment = UITextAlignment.Center;
                        spinnyboy.Hidden = true;
                        spinnyboy.StopAnimating();
                        break;
                }
                this.confidence = thing.Confidence;
                // cache the instance
                var haptic = new UINotificationFeedbackGenerator();

                // Do this in advance so it is ready to be called on-demand without delay...
                haptic.Prepare();

                // produce the feedback as many times as needed
                haptic.NotificationOccurred(UINotificationFeedbackType.Success);

                // when done all done, clean up
                haptic.Dispose();
            }
            else
            {
                NSUrl modelPath = NSBundle.MainBundle.GetUrlForResource("Ramen", "mlmodel");
                if(modelPath == null)
                {
                    Console.WriteLine("peeepee");
                }
                var transform = MLModel.CompileModel(modelPath, out NSError compErr);
                MLModel model = MLModel.Create(transform, out NSError fucl);
                var vnModel = VNCoreMLModel.FromMLModel(model, out NSError rror);
                var ciImage = new CIImage(image.Image);
                var classificationRequest = new VNCoreMLRequest(vnModel);

                //just do it
                var handler = new VNImageRequestHandler(ciImage, ImageIO.CGImagePropertyOrientation.Up, new VNImageOptions());
                handler.Perform(new[] { classificationRequest }, out NSError perfError);
                var results = classificationRequest.GetResults<VNClassificationObservation>();
                var thing = results[0];
                Console.WriteLine("Ramen OUT " + thing.Identifier);
                switch (thing.Identifier)
                {
                    case "ramen":
                        if (thing.Confidence > 0.85f)
                        {
                            this.stat.Text = "✅ Ramen";
                            this.stat.TextColor = UIKit.UIColor.Green;
                            this.stat.TextAlignment = UITextAlignment.Center;
                            spinnyboy.Hidden = true;
                            spinnyboy.StopAnimating();
                        }
                        else
                        {
                            this.stat.Text = "❌ Not Ramen";
                            this.stat.TextColor = UIKit.UIColor.Red;
                            this.stat.TextAlignment = UITextAlignment.Center;
                            spinnyboy.Hidden = true;
                            spinnyboy.StopAnimating();
                        }

                        break;

                    case "notramen":
                        this.stat.Text = "❌ Not Ramen";
                        this.stat.TextColor = UIKit.UIColor.Red;
                        this.stat.TextAlignment = UITextAlignment.Center;
                        spinnyboy.Hidden = true;
                        spinnyboy.StopAnimating();
                        break;
                }
                this.confidence = thing.Confidence;
                var haptic = new UINotificationFeedbackGenerator();

                // Do this in advance so it is ready to be called on-demand without delay...
                haptic.Prepare();

                // produce the feedback as many times as needed
                haptic.NotificationOccurred(UINotificationFeedbackType.Success);

                // when done all done, clean up
                haptic.Dispose();
            }

        }


        public CVPixelBuffer ToCVPixelBuffer(UIImage self)
        {
            var attrs = new CVPixelBufferAttributes();
            attrs.CGImageCompatibility = true;
            attrs.CGBitmapContextCompatibility = true;

            var cgImg = self.CGImage;

            var pb = new CVPixelBuffer(cgImg.Width, cgImg.Height, CVPixelFormatType.CV32ARGB, attrs);
            pb.Lock(CVPixelBufferLock.None);
            var pData = pb.BaseAddress;
            var colorSpace = CGColorSpace.CreateDeviceRGB();
            var ctxt = new CGBitmapContext(pData, cgImg.Width, cgImg.Height, 8, pb.BytesPerRow, colorSpace, CGImageAlphaInfo.NoneSkipFirst);
            ctxt.TranslateCTM(0, cgImg.Height);
            ctxt.ScaleCTM(1.0f, -1.0f);
            UIGraphics.PushContext(ctxt);
            self.Draw(new CGRect(0, 0, cgImg.Width, cgImg.Height));
            UIGraphics.PopContext();
            pb.Unlock(CVPixelBufferLock.None);

            return pb;

        }

        partial void ReturnToMenu_TouchUpInside(UIButton sender)
        {
            SEEFOOD.Hidden = false;
            stat.Hidden = true;
            image.Hidden = true;
            selectCamRollButton.Hidden = false;
            takePhotoButton.Hidden = false;
            spinnyboy.Hidden = true;
            spinnyboy.StopAnimating();
            returnToMenu.Hidden = true;
            this.hotdogLbl.Hidden = false;
            this.ramenLbl.Hidden = false;
            this.ramenOrHotdog.Hidden = false;
            showDebugInfo.Hidden = true;

            this.stat.Text = "Analyzing...";
            this.stat.TextColor = UIKit.UIColor.Black;

        }

        partial void ShowDebugInfo_TouchUpInside(UIButton sender)
        {
            UserDialogs.Instance.Alert("Confidence: " + confidence, "Debug", "Continue");
        }

        partial void ImageTypeSwitched(UISwitch sender)
        {
            Console.WriteLine("Checker type switched.");
            if(this.ramenOrHotdog.On)
            {
                ViewController.Type = "Hotdog";
                Console.WriteLine("Switched to hotdog");
            }
            else
            {
                ViewController.Type = "Ramen";
                Console.WriteLine("Switched to ramen");
            }


        }

        partial void SelCamRollEvent(UIButton sender)
        {
            PickCamera();
        }

    }

}