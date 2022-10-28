using Microsoft.Maui.Controls;
using SmartDevicePlatformPlugin;
using System.Text.Json.Nodes;

namespace SmartCurtainsPlatformPlugin;

public partial class ConfigurationView : ContentView
{
	private NodeConfiguration nodeConfiguration;

	public ConfigurationView(NodeConfiguration nodeConfiguration)
	{
		InitializeComponent();
		this.nodeConfiguration = nodeConfiguration;
	}

	public void PopulateControls()
	{
		DeviceIDLabel.Text = nodeConfiguration.DeviceID.ToString();
		UserIDLabel.Text = nodeConfiguration.UserID.ToString();
		BackedURILabel.Text = nodeConfiguration.backendConnectionUri.ToString();
		DeviceNameEntry.Text = nodeConfiguration.DeviceName;
		WiFiSSIDEntry.Text = nodeConfiguration.WiFiSSID;
		WiFiPasswordEntry.Text = nodeConfiguration.WiFiPassword;
	}

	public NodeConfiguration GetNodeConfiguration()
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