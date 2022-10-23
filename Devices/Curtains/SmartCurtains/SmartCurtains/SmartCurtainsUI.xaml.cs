using SmartDevice;
using System;

namespace SmartCurtains;

public partial class SmartCurtainsUI : ContentView
{
	public SmartCurtainsUI(string deviceName)
	{
		InitializeComponent();
		DeviceNameLabel.Text = deviceName;
	}

	public void ConfigureUI(int currentLevel,
							TimeOnly rollUpTime,
							TimeOnly rollDownTime,
							bool followSunrise,
							bool followSunset)
	{
		CurrentLevelEntry.Text = $"{currentLevel}%";
		RollUpTimeEntry.Time = rollUpTime.ToTimeSpan();
		RollDownTimeEntry.Time = rollDownTime.ToTimeSpan();
		FollowSunriseCheckbox.IsChecked = followSunrise;
		FollowSunsetCheckbox.IsChecked = followSunset;

		// Bind buttonhandles to callback methods
	}
}