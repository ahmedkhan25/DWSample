using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DW
{
    public partial class MainPage : ContentPage
    {
        public string data { get; set; }
        public MainPage()
        {
            InitializeComponent();
    


        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<App, string>(this,
                "ScanBarcode",
                async (app, s) =>
                {
                    data = s;
                   await DisplayAlert("Got Barcode", "Barcode Data: " + data, "ok");
                });

             
        }


    }
}
