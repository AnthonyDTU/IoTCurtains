using Microsoft.Maui.Controls;
using SmartDevicePlatformPlugin;
using System.Text.Json.Nodes;

namespace SmartCurtainsPlatformPlugin;

public partial class ConfigurationView : ContentView
{
	private DeviceDescriptor deviceDescriptor;

	public ConfigurationView(DeviceDescriptor deviceDescriptor)
	{
		InitializeComponent();
		this.deviceDescriptor = deviceDescriptor;
	}

    public void PopulateControls()
    {
        DeviceIDLabel.Text = deviceDescriptor.DeviceID.ToString();
        UserIDLabel.Text = deviceDescriptor.UserID.ToString();
		DeviceKeyLabel.Text = deviceDescriptor.DeviceKey.ToString();
        BackedURILabel.Text = deviceDescriptor.backendUri.ToString();
    }


    public void PopulateControls(NodeConfiguration nodeConfiguration)
	{
        DeviceIDLabel.Text = deviceDescriptor.DeviceID.ToString();
        UserIDLabel.Text = deviceDescriptor.UserID.ToString();
        DeviceKeyLabel.Text = deviceDescriptor.DeviceKey.ToString();
        BackedURILabel.Text = deviceDescriptor.backendUri.ToString();

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