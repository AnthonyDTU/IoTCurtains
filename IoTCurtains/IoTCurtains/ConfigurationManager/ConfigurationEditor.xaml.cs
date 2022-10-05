using DeviceConfiguration;

namespace IoTCurtains.ConfigurationManager;

public partial class ConfigurationEditor : ContentView
{
	ConfigurationManager parent;

	public ConfigurationEditor()
	{
		InitializeComponent();

		parent = (ConfigurationManager)Parent;
	}

	public void SetConfiguration(NodeConfiguration configuration)
	{
		wifiSSIDEntry.Text = configuration.WiFiSSID;
		wifiPasswordEntry.Text = configuration.WiFiPassword;
		macAddressEntry.Text = configuration.MACAddress;
		deviceIDEntry.Text = configuration.DeviceID;
	}


	private void configureDeviceButton_Clicked(object sender, EventArgs e)
	{
		string jsonConfiguration = "";
		parent.SendConfigurationToDevice(jsonConfiguration);
	}

	public void DisableControls()
	{
        wifiSSIDEntry.IsEnabled = false;
		wifiPasswordEntry.IsEnabled = false;
		macAddressEntry.IsEnabled = false;
		deviceIDEntry.IsEnabled = false;
		configureDeviceButton.IsEnabled = false;
    }

	public void EnableControls()
	{
        wifiSSIDEntry.IsEnabled = true;
        wifiPasswordEntry.IsEnabled = true;
        macAddressEntry.IsEnabled = true;
        deviceIDEntry.IsEnabled = true;
        configureDeviceButton.IsEnabled = true;
    }
}