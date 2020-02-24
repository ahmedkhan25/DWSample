using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using Android.Content;

namespace DW.Droid
{
    [Activity(Label = "DW", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        private const string ActionDataWedgeFrom62 = "com.symbol.datawedge.api.ACTION";
        private const string ExtraCreateProfile = "com.symbol.datawedge.api.CREATE_PROFILE";
        private const string ExtraSetConfig = "com.symbol.datawedge.api.SET_CONFIG";
        private const string ExtraProfileName = "DWSample";
        private DataWedgeReceiver _broadcastReceiver;
        private const string FirstExecution = "first_execution";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            _broadcastReceiver = new DataWedgeReceiver();

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var dwApp = new App();

            LoadApplication(new App());

            CreateProfile();

            _broadcastReceiver.ScanDataReceived += (s, scanData) =>
            {
                MessagingCenter.Send(dwApp, "ScanBarcode", scanData);
            };
        }

        



        protected override void OnResume()
        {
            base.OnResume();

            if (null != _broadcastReceiver)
            {
                IntentFilter filter = new IntentFilter(DataWedgeReceiver.IntentAction);
                filter.AddCategory(DataWedgeReceiver.IntentCategory);
                Android.App.Application.Context.RegisterReceiver(_broadcastReceiver, filter);
                _broadcastReceiver.RegisterBroadcastReceiverforNotificationActions();

            }
        }


        protected override void OnPause()
        {
            if (null != _broadcastReceiver)
            {

                _broadcastReceiver.UnRegisterBroadcastReciverforNotificationActions();
            }
            base.OnPause();
        }

        /// <summary>
        /// Creates DataWedge Profile used by Zebra device to trap incoming hardware actions such as barcode and ring scanner scan
        /// </summary>
        private void CreateProfile()
        {
            String profileName = ExtraProfileName;
            SendDataWedgeIntentWithExtra(ActionDataWedgeFrom62, ExtraCreateProfile, profileName);

            //  Now configure that created profile to apply to our application
            Bundle profileConfig = new Bundle();
            profileConfig.PutString("PROFILE_NAME", ExtraProfileName);
            profileConfig.PutString("PROFILE_ENABLED", "true"); //  Seems these are all strings
            profileConfig.PutString("CONFIG_MODE", "UPDATE");

            Bundle barcodeConfig = new Bundle();
            barcodeConfig.PutString("PLUGIN_NAME", "BARCODE");
            barcodeConfig.PutString("RESET_CONFIG", "true"); //  This is the default but never hurts to specify

            Bundle barcodeProps = new Bundle();
            barcodeConfig.PutBundle("PARAM_LIST", barcodeProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", barcodeConfig);

            Bundle appConfig = new Bundle();
            appConfig.PutString("PACKAGE_NAME", PackageName);      //  Associate the profile with this app
            appConfig.PutStringArray("ACTIVITY_LIST", new[] { "*" });
            profileConfig.PutParcelableArray("APP_LIST", new IParcelable[] { appConfig });
            SendDataWedgeIntentWithExtra(ActionDataWedgeFrom62, ExtraSetConfig, profileConfig);

            //  You can only configure one plugin at a time, we have done the barcode input, now do the intent output
            profileConfig.Remove("PLUGIN_CONFIG");
            Bundle intentConfig = new Bundle();
            intentConfig.PutString("PLUGIN_NAME", "INTENT");
            intentConfig.PutString("RESET_CONFIG", "true");

            Bundle intentProps = new Bundle();
            intentProps.PutString("intent_output_enabled", "true");
            intentProps.PutString("intent_action", DataWedgeReceiver.IntentAction);
            intentProps.PutString("intent_delivery", "2");
            intentConfig.PutBundle("PARAM_LIST", intentProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", intentConfig);

            SendDataWedgeIntentWithExtra(ActionDataWedgeFrom62, ExtraSetConfig, profileConfig);

            //NEW - 2/18: Register for notifications - PROFILE_SWITCH

            Bundle notificationRegisterBundle = new Bundle();
            notificationRegisterBundle.PutString("com.symbol.datawedge.api.APPLICATION_NAME", "com.sample.dw");
            notificationRegisterBundle.PutString("com.symbol.datawedge.api.NOTIFICATION_TYPE", "PROFILE_SWITCH");
            SendDataWedgeIntentWithExtra(ActionDataWedgeFrom62, "com.symbol.datawedge.api.REGISTER_FOR_NOTIFICATION", notificationRegisterBundle);

            //NEW - 2/18: UnRegister for notifications - PROFILE_SWITCH

            Bundle notificationUnRegisterBundle = new Bundle();
            notificationUnRegisterBundle.PutString("com.symbol.datawedge.api.APPLICATION_NAME", "com.sample.dw");
            notificationUnRegisterBundle.PutString("com.symbol.datawedge.api.NOTIFICATION_TYPE", "PROFILE_SWITCH");
            SendDataWedgeIntentWithExtra(ActionDataWedgeFrom62, "com.symbol.datawedge.api.UNREGISTER_FOR_NOTIFICATION", notificationUnRegisterBundle);

            //NEW - 2/18: Register for notifications - Scanner_Status

            Bundle ScannerStatusRegisterBundle = new Bundle();
            ScannerStatusRegisterBundle.PutString("com.symbol.datawedge.api.APPLICATION_NAME", "com.sample.dw");
            ScannerStatusRegisterBundle.PutString("com.symbol.datawedge.api.NOTIFICATION_TYPE", "SCANNER_STATUS");
            SendDataWedgeIntentWithExtra(ActionDataWedgeFrom62, "com.symbol.datawedge.api.REGISTER_FOR_NOTIFICATION", ScannerStatusRegisterBundle);


            //NEW - 2/18: UnRegister for notifications - Scanner_status

            Bundle ScannerStatusUnRegisterBundle = new Bundle();
            ScannerStatusUnRegisterBundle.PutString("com.symbol.datawedge.api.APPLICATION_NAME", "com.sample.dw");
            ScannerStatusUnRegisterBundle.PutString("com.symbol.datawedge.api.NOTIFICATION_TYPE", "SCANNER_STATUS");
            SendDataWedgeIntentWithExtra(ActionDataWedgeFrom62, "com.symbol.datawedge.api.UNREGISTER_FOR_NOTIFICATION", ScannerStatusUnRegisterBundle);

            _broadcastReceiver.RegisterBroadcastReceiverforNotificationActions();
        }


      

        private void SendDataWedgeIntentWithExtra(string action, string extraKey, Bundle extras)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extras);
            SendBroadcast(dwIntent);
        }

        private void SendDataWedgeIntentWithExtra(string action, string extraKey, string extraValue)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extraValue);
            SendBroadcast(dwIntent);
        }

    }
     
}