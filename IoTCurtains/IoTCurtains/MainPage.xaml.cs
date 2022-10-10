namespace IoTCurtains;

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.IO.Ports;

public partial class MainPage : ContentPage
{
	int count = 0;


	public MainPage()
	{
		InitializeComponent();

		string hostname = "IoTCurtains.azure-devices.net";

        string connectionString = "HostName=IoTCurtains.azure-devices.net;DeviceId=Controller;SharedAccessKey=+AJ9aB8DUVoWK35MwSrDKoJB9TxBgazOofgRQ7raeGE=";




		//DeviceClient deviceClient = DeviceClient.Create(hostname, authenticationMethod);
		//DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
		//deviceClient.OpenAsync().Wait();
		//Task<Twin> getTwin = deviceClient.GetTwinAsync();
		//Twin twin = getTwin.Result;

		if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
		{
			Button configureDeviceBtn = new Button()
			{
				Text = "Open Device Configuration Manager",
				WidthRequest = 300,
				HorizontalOptions = LayoutOptions.Center,

			};

            configureDeviceBtn.Clicked += ConfigureBtn_Clicked;	
			stackLayout.Children.Add(configureDeviceBtn);
		}
    }

	private async void ConfigureBtn_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ConfigurationManager.ConfigurationManager());
    }

	private void OnCounterClicked(object sender, EventArgs e)
	{



		
	}
}

