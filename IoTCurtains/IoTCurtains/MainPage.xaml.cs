namespace IoTCurtains;


using System.IO.Ports;

public partial class MainPage : ContentPage
{
	int count = 0;


	public MainPage()
	{
		InitializeComponent();

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

