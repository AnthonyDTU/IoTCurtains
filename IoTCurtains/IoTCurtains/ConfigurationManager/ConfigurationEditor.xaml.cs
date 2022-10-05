using DeviceConfiguration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IoTCurtains.ConfigurationManager;

public partial class ConfigurationEditor : ContentView
{
	ConfigurationManager parent;

	public ConfigurationEditor()
	{
		InitializeComponent();
		parent = (ConfigurationManager)Parent;
	}

	/// <summary>
	/// Set the textfields equal to the values in the provided NodeConfiguration
	/// </summary>
	/// <param name="configuration"></param>
	public void SetConfiguration(NodeConfiguration configuration)
	{
		wifiSSIDEntry.Text = configuration.WiFiSSID;
		wifiPasswordEntry.Text = configuration.WiFiPassword;
		macAddressEntry.Text = configuration.MACAddress;
		deviceIDEntry.Text = configuration.DeviceID;
	}

	/// <summary>
	/// Collects all the values from the textfields into a NodeConfiguration object and serializes it to Json format.
	/// This is then sent to the parent Configuration Manager.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void configureDeviceButton_Clicked(object sender, EventArgs e)
	{
		NodeConfiguration configuration = new NodeConfiguration();
		configuration.WiFiSSID = wifiSSIDEntry.Text;
		configuration.WiFiPassword = wifiPasswordEntry.Text;
		configuration.MACAddress = macAddressEntry.Text;
		configuration.DeviceID = deviceIDEntry.Text;
		string jsonConfiguration = JsonSerializer.Serialize(configuration, typeof(NodeConfiguration));
		parent.SendConfigurationToDevice(jsonConfiguration);
	}

	/// <summary>
	/// Disables the editor controls
	/// </summary>
	public void DisableControls()
	{
        wifiSSIDEntry.IsEnabled = false;
		wifiPasswordEntry.IsEnabled = false;
		macAddressEntry.IsEnabled = false;
		deviceIDEntry.IsEnabled = false;
		configureDeviceButton.IsEnabled = false;
    }

    /// <summary>
    /// Enables the editor controls
    /// </summary>
    public void EnableControls()
	{
        wifiSSIDEntry.IsEnabled = true;
        wifiPasswordEntry.IsEnabled = true;
        macAddressEntry.IsEnabled = true;
        deviceIDEntry.IsEnabled = true;
        configureDeviceButton.IsEnabled = true;
    }
}