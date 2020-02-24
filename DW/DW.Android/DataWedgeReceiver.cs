using System;
using Android.Content;
using Android.OS;
 

namespace DW.Droid
{
    [BroadcastReceiver]
    public class DataWedgeReceiver : BroadcastReceiver
    {
        // This intent string contains the source of the data as a string
        private const string SourceTag = "com.motorolasolutions.emdk.datawedge.source";

        // This intent string contains the barcode symbology as a string
        private static readonly string LabelTypeTag = "com.motorolasolutions.emdk.datawedge.label_type";

        // This intent string contains the captured data as a string
        // (in the case of MSR this data string contains a concatenation of the track data)
        private static readonly string DataStringTag = "com.motorolasolutions.emdk.datawedge.data_string";

        // Intent Action for our operation
        public static string IntentAction = "barcodescanner.RECVR";
        public static string IntentCategory = "android.intent.category.DEFAULT";

        // TO RECIEVE NOTIFICATIONS -new 2/18
        public const string NOTIFICATION_ACTION = "com.symbol.datawedge.api.NOTIFICATION_ACTION";
        public const string NOTIFICATION_TYPE_SCANNER_STATUS = "SCANNER_STATUS";
        public const string NOTIFICATION_TYPE_PROFILE_SWITCH = "PROFILE_SWITCH";
        public const string NOTIFICATION_TYPE_CONFIGURATION_UPDATE = "CONFIGURATION_UPDATE";

        public event EventHandler<string> ScanDataReceived;

        public override void OnReceive(Context context, Intent i)
        {
             

            // check the intent action is for us
            if (i.Action.Equals(IntentAction))
            {
                // define a string that will hold our output
                var Out = "";
                // get the source of the data
                var source = i.GetStringExtra(SourceTag) ?? "scanner";
                // save it to use later
                // get the data from the intent
                var data = i.GetStringExtra(DataStringTag);
                // check if the data has come from the barcode scanner
                if (source.Equals("scanner"))
                {
                    // check if there is anything in the data
                    if (!string.IsNullOrEmpty(data))
                    {
                        // we have some data, so let's get it's symbology
                        var sLabelType = i.GetStringExtra(LabelTypeTag);
                        // check if the string is empty
                        sLabelType = !string.IsNullOrEmpty(sLabelType) ? sLabelType.Substring(11) : "Unknown";

                        // let's construct the beginning of our output string
                        Out = data;
                    }
                }

                ScanDataReceived?.Invoke(this, Out);
            }

            //NEW Added this code 2/18:
            if (i.Action.Equals(NOTIFICATION_ACTION))
            {

                if (i.HasExtra("com.symbol.datawedge.api.NOTIFICATION"))
                {
                    Bundle b = i.GetBundleExtra("com.symbol.datawedge.api.NOTIFICATION");
                    String NOTIFICATION_TYPE = b.GetString("NOTIFICATION_TYPE");
                    if (NOTIFICATION_TYPE != null)
                    {
                        switch (NOTIFICATION_TYPE)
                        {
                            case NOTIFICATION_TYPE_SCANNER_STATUS:
                                
                                UnRegisterBroadcastReciverforNotificationActions();
                                RegisterBroadcastReceiverforNotificationActions(); //new added 2/19
                                break;

                            case NOTIFICATION_TYPE_PROFILE_SWITCH:
                                
                                UnRegisterBroadcastReciverforNotificationActions();
                                RegisterBroadcastReceiverforNotificationActions(); //new added 2/19
                                break;

                            case NOTIFICATION_TYPE_CONFIGURATION_UPDATE:
                                break;
                        }
                    }
                }
            }
        }
        //New 2/19 added Register method 
        public void RegisterBroadcastReceiverforNotificationActions()
        {
            IntentFilter filter = new IntentFilter();
            filter.AddAction(NOTIFICATION_ACTION);
            Android.App.Application.Context.RegisterReceiver(this, filter);
        }

        //New 2/19 added Register method 
        public void UnRegisterBroadcastReciverforNotificationActions()
        {
            IntentFilter filter = new IntentFilter();
            Android.App.Application.Context.UnregisterReceiver(this);
        }
    }
}