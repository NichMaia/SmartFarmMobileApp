using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinScanditSDKSampleAndroid
{
    class Movimentacao
    {
        private int Id { get; set; }
        public DateTime Data { get; set; }
        public float Quantidade { get; set; }
        public int IDAnimal { get; set; }
    }
}