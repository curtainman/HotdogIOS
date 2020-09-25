using Acr.UserDialogs;
using CoreFoundation;
using CoreGraphics;
using CoreML;
using CoreVideo;
using Foundation;
using Plugin.Media;
using System;
using System.Drawing;
using System.IO;
using UIKit;
using Vision;

namespace HotdogIOS
{
    public partial class ViewController : UIViewController
    {

        public ViewController (IntPtr handle) : base (handle)
        {
        }

        private float confidence;

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


        private async void DoCamera()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable ||
                  !CrossMedia.Current.IsTakePhotoSupported)
            {
                UserDialogs.Instance.Alert("No camera found.", null, "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                
            });

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
            spinnyboy.StartAnimating();
            returnToMenu.Hidden = false;
            this.stat.Text = "Analyzing...";
            this.stat.TextColor = UIKit.UIColor.Black;
            showDebugInfo.Hidden = false;
            var data = NSData.FromArray(result);
            image.Image = UIImage.LoadFromData(data);

            //ML

            var assetPath = NSBundle.MainBundle.GetUrlForResource("model", "mlmodel");
            var assetActual = MLModel.CompileModel(assetPath, out NSError err);
            var model = MLModel.Create(assetActual, out NSError compErr);
            var vModel = VNCoreMLModel.FromMLModel(model, out NSError vnErr);

            var ClassificationRequest = new VNCoreMLRequest(vModel, (response, e) => { });
            var handler = new VNImageRequestHandler(this.ToCVPixelBuffer(this.image.Image), new VNImageOptions());
            DispatchQueue.DefaultGlobalQueue.DispatchAsync(() =>
            {
                handler.Perform(new VNRequest[] { ClassificationRequest }, out NSError errs);
            });
            var observations = ClassificationRequest.GetResults<VNClassificationObservation>();

            var best = observations[0]; // first/best classification result
            switch (best.Identifier)
            {
                case "hotdog":
                    this.stat.Text = "✅ Hotdog";
                    this.stat.TextColor = UIKit.UIColor.Green;
                    spinnyboy.Hidden = true;
                    spinnyboy.StopAnimating();
                    break;
                case "nothotdog":
                    this.stat.Text = "❌ Not Hotdog";
                    this.stat.TextColor = UIKit.UIColor.Red;
                    spinnyboy.Hidden = true;
                    spinnyboy.StopAnimating();
                    break;
            }

            this.confidence = best.Confidence;
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
            showDebugInfo.Hidden = true;

            this.stat.Text = "Analyzing...";
            this.stat.TextColor = UIKit.UIColor.Black;

        }

        partial void ShowDebugInfo_TouchUpInside(UIButton sender)
        {
            UserDialogs.Instance.Alert("Confidence: " + confidence, "Debug", "Continue");
        }
    }
}