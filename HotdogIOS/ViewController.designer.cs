// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace HotdogIOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel hotdogLbl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView image { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ramenLbl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch ramenOrHotdog { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton returnToMenu { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SEEFOOD { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton selectCamRollButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton showDebugInfo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView spinnyboy { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel stat { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton takePhotoButton { get; set; }

        [Action ("ReturnToMenu_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ReturnToMenu_TouchUpInside (UIKit.UIButton sender);

        [Action ("ShowDebugInfo_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShowDebugInfo_TouchUpInside (UIKit.UIButton sender);

        [Action ("TakePhotoButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TakePhotoButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (hotdogLbl != null) {
                hotdogLbl.Dispose ();
                hotdogLbl = null;
            }

            if (image != null) {
                image.Dispose ();
                image = null;
            }

            if (ramenLbl != null) {
                ramenLbl.Dispose ();
                ramenLbl = null;
            }

            if (ramenOrHotdog != null) {
                ramenOrHotdog.Dispose ();
                ramenOrHotdog = null;
            }

            if (returnToMenu != null) {
                returnToMenu.Dispose ();
                returnToMenu = null;
            }

            if (SEEFOOD != null) {
                SEEFOOD.Dispose ();
                SEEFOOD = null;
            }

            if (selectCamRollButton != null) {
                selectCamRollButton.Dispose ();
                selectCamRollButton = null;
            }

            if (showDebugInfo != null) {
                showDebugInfo.Dispose ();
                showDebugInfo = null;
            }

            if (spinnyboy != null) {
                spinnyboy.Dispose ();
                spinnyboy = null;
            }

            if (stat != null) {
                stat.Dispose ();
                stat = null;
            }

            if (takePhotoButton != null) {
                takePhotoButton.Dispose ();
                takePhotoButton = null;
            }
        }
    }
}