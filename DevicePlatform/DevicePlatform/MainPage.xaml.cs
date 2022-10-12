using SmartDevice;

namespace DevicePlatform;

public partial class MainPage : ContentPage
{
	DeviceCollection deviceCollection;


	public MainPage()
	{
		InitializeComponent();

		deviceCollection = new DeviceCollection();

		if (deviceCollection.Devices.Count == 0)
		{
			MainContentView.Children.Add(new Label() { Text = "No Devices Configured...", HorizontalOptions = LayoutOptions.Center } );
		}

		if (DeviceInfo.Current.Platform == Microsoft.Maui.Devices.DevicePlatform.WinUI)
		{
			Button addNewDeviceButton = new Button()
			{
				WidthRequest = 250,
				HeightRequest = 100,
				CornerRadius = 10,
				BackgroundColor = Colors.Cyan,
				Text = "Configure New Device",
				TextColor = Colors.Black,
				FontSize = 15,
				

			};
			addNewDeviceButton.Clicked += AddNewDeviceButton_Clicked;
			MainContentView.Children.Add(addNewDeviceButton);
		}
	}

	private async void AddNewDeviceButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DeviceConfigurationManager.ConfigurationManager(deviceCollection, false));
    }
}

