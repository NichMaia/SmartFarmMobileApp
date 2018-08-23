using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Clipboard;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace XamarinScanditSDKSampleAndroid
{
    [Activity(Label = "XamarinScanditSDKSampleAndroid", MainLauncher = true)]
    public class MainActivity : Activity
    {
        TextView IDVaca;
        Button btnMySql;
        TextView txtprod;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            CrossClipboard.Current.SetText("");
            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.BtnScan);
            IDVaca = FindViewById<TextView>(Resource.Id.txtIDVaca);
            btnMySql = FindViewById<Button>(Resource.Id.btnSalvar);
            txtprod = FindViewById<TextView>(Resource.Id.txtProd);
            button.Click += delegate
            {
                // start the scanner
                CrossClipboard.Current.SetText("");
                StartActivity(typeof(ScanActivity));
                
            };
            btnMySql.Click += BtnMySql_Click;

        }
        protected override void OnResume()
        {
            base.OnResume();

            string Clipboard = CrossClipboard.Current.GetText();
            if(Clipboard != "")
            {
                IDVaca.Text = Clipboard;
                CrossClipboard.Current.SetText("");
            }

        }

        public async void BtnMySql_Click(object sender, System.EventArgs e)
        {
            try
            {
                ////conexao com MySQL
                //MySqlCon db = new MySqlCon();
                //Toast.MakeText(this, "Acesso ao MySQL feito com sucesso", ToastLength.Short).Show();

                ////carrega lista de strings com dados dos produtos
                //db.ProducaoDia(float.Parse(txtprod.Text), int.Parse(IDVaca.Text));

                ////exibe os dados no ListView
                ////   ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, Producao);
                //// listView.Adapter = adapter;

                Movimentacao Mov = new Movimentacao();
                Mov.IDAnimal = int.Parse(IDVaca.Text);
                Mov.Data = DateTime.Now;
                Mov.Quantidade = float.Parse(txtprod.Text);

                HttpClient client = new HttpClient();
                string url = "http://smartfarmapi.azurewebsites.net/api/ProducaoDia";
                var uri = new Uri(url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response;
                var json = JsonConvert.SerializeObject(Mov);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(uri, content);
                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
                    {
                '"'
                    });
                    Toast.MakeText(this, errorMessage1, ToastLength.Long).Show();
                }
                else
                {
                    var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
                    {
                '"'
                    });
                    Toast.MakeText(this, errorMessage1, ToastLength.Long).Show();
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Erro : " + ex.Message, ToastLength.Short).Show();
            }
        }
    }
}


