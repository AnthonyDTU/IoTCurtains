using Microsoft.Maui.Platform;
using SmartDevicePlatformPlugin;
using System.Diagnostics;
using System.Text.Json;

namespace SmartCurtainsPlatformPlugin;

public partial class SmartCurtainsContentPageUI : ContentPage
{
    public delegate void ConfigureDevice(DeviceData data);
    private ConfigureDevice configureDevice;

	private SignalRController signalRController;

	public delegate bool DeleteDeviceCallBack();
	private DeleteDeviceCallBack deleteDeviceCallBack;

    public SmartCurtainsContentPageUI(SignalRController signalRController, ConfigureDevice configureDevice, DeleteDeviceCallBack deleteDeviceCallBack)
	{
		InitializeComponent();	
		this.signalRController = signalRController;
		this.configureDevice = configureDevice;
		this.deleteDeviceCallBack = deleteDeviceCallBack;
	}

	/// <summary>
	/// Send Roll Up command to device
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void RollUpButton_Clicked(object sender, EventArgs e)
	{
		signalRController.SendCommandToDevice("RollUp");
	}

	/// <summary>
	/// Sends Roll Down command to device
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void RollDown_Clicked(object sender, EventArgs e)
	{
		signalRController.SendCommandToDevice("RollDown");
	}

	/// <summary>
	/// Configures the controls in the UI to display the device data
	/// </summary>
	/// <param name="deviceData"></param>
	public void ConfigureData(DeviceData deviceData)
	{
        List<string> downTime = new List<string>(deviceData.RollDownTime.Split(":"));
        TimeSpan downTimeSpan = new TimeSpan(Convert.ToInt16(downTime[0]), Convert.ToInt16(downTime[1]), 0);

        List<string> upTime = new List<string>(deviceData.RollUpTime.Split(":"));
        TimeSpan upTimeSpan = new TimeSpan(Convert.ToInt16(upTime[0]), Convert.ToInt16(upTime[1]), 0);

        Dispatcher.Dispatch(() =>
		{
            CurrentLevelLabel.Text = $"{deviceData.CurrentLevel}%";
            RollDownTimeEntry.Time = downTimeSpan;
            RollUpTimeEntry.Time = upTimeSpan;
            FollowSunriseCheckbox.IsChecked = deviceData.FollowSunrise;
            FollowSunsetCheckbox.IsChecked = deviceData.FollowSunset;
			StatusLabel.Text = "";
        });
	}

	/// <summary>
	/// Gets the device data based on the values in the UI controls
	/// </summary>
	/// <returns></returns>
	private DeviceData GetDeviceData()
	{
		return new DeviceData
		{
			RollDownTime = $"{RollDownTimeEntry.Time.Hours}:{RollDownTimeEntry.Time.Minutes}",
            RollUpTime = $"{RollUpTimeEntry.Time.Hours}:{RollUpTimeEntry.Time.Minutes}",
			FollowSunrise = FollowSunriseCheckbox.IsChecked,
			FollowSunset = FollowSunsetCheckbox.IsChecked,
		};
	}

	/// <summary>
	/// Handler for when a device sends an acknowledge
	/// </summary>
	public void DataAcknowledgedByDevice()
	{
        configureDevice.Invoke(GetDeviceData());

        Dispatcher.Dispatch(() =>
        {
            StatusLabel.Text = "Device Configured!";
        });
	}

	/// <summary>
	/// Handler for sending data to the device, when the user presses the button
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void SendDataToDeviceButton_Clicked(object sender, EventArgs e)
	{
		StatusLabel.Text = "";

        string jsonData = JsonSerializer.Serialize<DeviceData>(GetDeviceData());
        signalRController.TransmitDataToDevice(jsonData);
    }

	/// <summary>
	/// Handler for deleting the device, when the user presses the button
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void DeleteDeviceButton_Clicked(object sender, EventArgs e)
	{
		if (deleteDeviceCallBack.Invoke())
		{
			StatusLabel.Text = "Device Deleted";
		}		
	}
}