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
			};

            configureDeviceBtn.Clicked += ConfigureBtn_Clicked;	
			stackLayout.Children.Add(configureDeviceBtn);
		}
    }

	private void ConfigureBtn_Clicked(object sender, EventArgs e)
	{
		var secondWindow = new Window
		{
			Page = new ConfigurationManager.ConfigurationManager()
        };

        Application.Current.OpenWindow(secondWindow);
    }

	private void OnCounterClicked(object sender, EventArgs e)
	{
		
	}
}

