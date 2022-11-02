using Microsoft.Maui.Controls;
using SmartDevicePlatformPlugin;
using System.Text.Json.Nodes;

namespace SmartCurtainsPlatformPlugin;

public partial class ConfigurationView : ContentView
{
	private DeviceParameters deviceParameters;

	public ConfigurationView(DeviceParameters deviceParameters)
	{
		InitializeComponent();
		this.deviceParameters = deviceParameters;
	}

    public void PopulateControls()
    {
        DeviceIDLabel.Text = deviceParameters.DeviceID.ToString();
        UserIDLabel.Text = deviceParameters.UserID.ToString();
		DeviceKeyLabel.Text = deviceParameters.DeviceKey.ToString();
        BackedURILabel.Text = deviceParameters.backendUri.ToString();
    }


    public void PopulateControls(NodeConfiguration nodeConfiguration)
	{
        DeviceIDLabel.Text = deviceParameters.DeviceID.ToString();
        UserIDLabel.Text = deviceParameters.UserID.ToString();
        DeviceKeyLabel.Text = deviceParameters.DeviceKey.ToString();
        BackedURILabel.Text = deviceParameters.backendUri.ToString();

        DeviceNameEntry.Text = nodeConfiguration.DeviceName;
		WiFiSSIDEntry.Text = nodeConfiguration.WiFiSSID;
		WiFiPasswordEntry.Text = nodeConfiguration.WiFiPassword;
	}

	public NodeConfiguration GetNodeConfigurationFromControls()
	{
        return new NodeConfiguration()
		{
			DeviceID = Guid.Parse(DeviceIDLabel.Text),
			UserID = Guid.Parse(UserIDLabel.Text),
			DeviceModel = modelLabel.Text,
			DeviceName = DeviceNameEntry.Text,
			WiFiSSID = WiFiSSIDEntry.Text,
			WiFiPassword = WiFiPasswordEntry.Text,
			backendConnectionUri = new Uri(BackedURILabel.Text),
		};
	}
}