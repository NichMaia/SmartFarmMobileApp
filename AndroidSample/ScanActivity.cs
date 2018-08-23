using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Scandit;
using ScanditBarcodePicker.Android;
using ScanditBarcodePicker.Android.Recognition;
using Plugin.Clipboard;

namespace XamarinScanditSDKSampleAndroid
{
    [Activity (Label = "ScanActivity")]
    public class ScanActivity : Activity, IOnScanListener, IDialogInterfaceOnCancelListener
    {
        public static string appKey = "AZHbPTZjKiOIO6iEhhAqOm8kT1okINTy5m9epvNwrpOmN1e2W28RHudW5l1zTZZVKkSS76c3TQZ9TOCEB2T5NaFvVm5hdIYMDTBSDoV0UFKNQRf4aGxB+7dUEsWwdYoBOkTNu99mfvl4YIW+tEc9XiISNmqhYWR2amsiXe96vPcETOdfokshQa1k/zVueMyyRHODD7tStHiRYUBhXmVCVb1PoH5bUm83/2uYCaRx1meBeG1yym8+pBxejIavVoq+jy8R6OtbLpjASqz1L0h98B5XiXzVRbrezUGS0/FWFq9GUmwItVX4QphAxpQ+b74CoVMDtmF2IxFFXplWZkLrEzJc9OG9TmboGE4gcSh674t/Y18Wo1Dtt6h2mS6gYrutGW6zYbFRvel+SLPONWcRd01DSho5ZLA2nEM6Y3tECC4Rb1S2zXOnzv5SOjpqTff3ekr8qedGXCZGGY9mRz5CvH0IQWPZ+qa4tUFhRZAaQ6XygYTVSEMJ6f6KWeDHWsBJo6xA+3LK0g//YEOOwi6EvBmUlKCfVcQVFdPOhsUrzmTry6rtoTQlAFU+F6yD9sHtBN5gNLIYqBN5MNyYrvWMFK3vJTCiJZKBoT66+zCcaoS2KIYyA7+TwT2+5ILAisd7MrJ3b2/d2l7JqifKAQgG1Vz4noXKQAHfOk1ZOvINEbUfNlNP/PzgY3+064MLDxt3Ep+2ZVjNi4lCVFcilAd2Y2yFuO8DJsGtgryOTbSiS3I6du59Z2XO0H1ngkWcEwy7tY7Vh2lOdCeLNKD0HnFpmDcKR6AhBnWC644pP5tvefoqsuhsXY/9g1HfoGkSIrZXkYo81SqQFLa6OGtCZzks42ZOb6lNj29Z4QB0cMyBYJsjdeXGnPGyoqut4nKZ5n1Fl6SUH6Xpim0dExkI0w1w4E9LxtX20D1gKOTdVMo/8I8KAAmuDLZPJjFWE7f7Zuc2j5noDAANnEUZSxTI54XRhDbGXMHyW9GN6ed3O9U/U9DCic0x6xfsfIsyLIj1vUt66brhF3IPnj2wmfgBoipXZdiI0pWwCrq0vyrlRU7kuxSH0Kb3EskNNLEE7cF3NYyz5c+U5Qb6zx2SIs9HW5rNwXBwEH3Lp0/IepGff6eyqYYvEdSj0/z/Kmr1S2x8WrTjur4=";

        private const int CameraPermissionRequest = 0;

        private BarcodePicker barcodePicker;
        private bool deniedCameraAccess = false;
        private bool paused = true;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            // Set the app key before instantiating the picker.
            ScanditLicense.AppKey = appKey;

            InitializeAndStartBarcodeScanning();
        }

        protected override void OnPause()
        {
            base.OnPause();

            // Call GC.Collect() before stopping the scanner as the garbage collector for some reason does not
            // collect objects without references asap but waits for a long time until finally collecting them.
            GC.Collect();
            barcodePicker.StopScanning();
            paused = true;
        }

        void GrantCameraPermissionsThenStartScanning()
        {
            if (CheckSelfPermission(Manifest.Permission.Camera) != (int)Permission.Granted)
            {
                if (deniedCameraAccess == false)
                {
                    // It's pretty clear for why the camera is required. We don't need to give a
                    // detailed reason.
                    RequestPermissions(new String[] { Manifest.Permission.Camera }, CameraPermissionRequest);
                }

            }
            else
            {
                Console.WriteLine("starting scanning");
                // We already have the permission.
                barcodePicker.StartScanning();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == CameraPermissionRequest)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    deniedCameraAccess = false;
                    if (!paused)
                    {
                        barcodePicker.StartScanning();
                    }
                }
                else
                {
                    deniedCameraAccess = true;
                }
                return;
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();

            paused = false;
            // Handle permissions for Marshmallow and onwards.
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                GrantCameraPermissionsThenStartScanning();
            }
            else
            {
                // Once the activity is in the foreground again, restart scanning.
                barcodePicker.StartScanning();
            }
        }

        void InitializeAndStartBarcodeScanning()
        {
            // The scanning behavior of the barcode picker is configured through scan
            // settings. We start with empty scan settings and enable a very generous
            // set of symbologies. In your own apps, only enable the symbologies you
            // actually need.
            ScanSettings settings = ScanSettings.Create ();
            int[] symbologiesToEnable = new int[] {
                Barcode.SymbologyEan13,
                Barcode.SymbologyEan8,
                Barcode.SymbologyUpca,
                Barcode.SymbologyDataMatrix,
                Barcode.SymbologyQr,
                Barcode.SymbologyCode39,
                Barcode.SymbologyCode128,
                Barcode.SymbologyInterleaved2Of5,
                Barcode.SymbologyUpce
            };

            foreach (int symbology in symbologiesToEnable)
            {
                settings.SetSymbologyEnabled (symbology, true);
            }

            // Some 1d barcode symbologies allow you to encode variable-length data. By default, the
            // Scandit BarcodeScanner SDK only scans barcodes in a certain length range. If your
            // application requires scanning of one of these symbologies, and the length is falling
            // outside the default range, you may need to adjust the "active symbol counts" for this
            // symbology. This is shown in the following few lines of code.

            SymbologySettings symSettings = settings.GetSymbologySettings(Barcode.SymbologyCode128);
            short[] activeSymbolCounts = new short[] {
                7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
            };
            symSettings.SetActiveSymbolCounts(activeSymbolCounts);
            // For details on defaults and how to calculate the symbol counts for each symbology, take
            // a look at http://docs.scandit.com/stable/c_api/symbologies.html.

            barcodePicker = new BarcodePicker (this, settings);

            // Set listener for the scan event.
            barcodePicker.SetOnScanListener (this);

            // Show the scan user interface
            SetContentView (barcodePicker);
        }

        public void DidScan(IScanSession session)
        {
            if (session.NewlyRecognizedCodes.Count > 0) {
                Barcode code = session.NewlyRecognizedCodes [0];
                Console.WriteLine ("barcode scanned: {0}, '{1}'", code.SymbologyName, code.Data);

                // Call GC.Collect() before stopping the scanner as the garbage collector for some reason does not
                // collect objects without references asap but waits for a long time until finally collecting them.
                GC.Collect ();

                // Stop the scanner directly on the session.
                session.StopScanning ();

                // If you want to edit something in the view hierarchy make sure to run it on the UI thread.
                RunOnUiThread (() => {
                    AlertDialog alert = new AlertDialog.Builder (this)
                        .SetTitle (code.SymbologyName + " Barcode Detected")
                        .SetMessage (code.Data)
                        .SetPositiveButton("OK", delegate {
                            barcodePicker.StartScanning ();
                        })
                        .SetOnCancelListener(this)
                        .Create ();

                    alert.Show ();
                    CrossClipboard.Current.SetText(code.Data);
                });
            }
        }

        public void OnCancel(IDialogInterface dialog) {
            barcodePicker.StartScanning ();
        }

        public override void OnBackPressed ()
        {
            base.OnBackPressed ();
            Finish ();
        }
    }
}
