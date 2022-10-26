using SmartDevicePlatformPlugin;
using System;

namespace SmartCurtainsPlatformPlugin;

public partial class SmartCurtainsUI : ContentView
{
	public SmartCurtainsUI(string deviceName)
	{
		InitializeComponent();
		DeviceNameLabel.Text = deviceName;
	}

	public void ConfigureUI(DeviceData currentDeviceState)
	{
		CurrentLevelEntry.Text = $"{currentDeviceState.CurrentLevel}%";
		RollUpTimeEntry.Time = currentDeviceState.RollUpTime.ToTimeSpan();
		RollDownTimeEntry.Time = currentDeviceState.RollDownTime.ToTimeSpan();
		FollowSunriseCheckbox.IsChecked = currentDeviceState.FollowSunrise;
		FollowSunsetCheckbox.IsChecked = currentDeviceState.FollowSunset;

		// Bind buttonhandles to callback methods
	}
}