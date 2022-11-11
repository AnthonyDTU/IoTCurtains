namespace SmartCurtainsPlatformPlugin;

public partial class SmartCurtainsContentPageUI : ContentPage
{
	public SmartCurtainsContentPageUI()
	{
		InitializeComponent();
	}

	private void RollUpButton_Clicked(object sender, EventArgs e)
	{

	}

	private void RollDown_Clicked(object sender, EventArgs e)
	{

	}

	public void ConfigureData(DeviceData deviceData)
	{
		CurrentLevelLabel.Text = $"{deviceData.CurrentLevel}%";
		RollDownTimeEntry.Time = deviceData.RollDownTime.TimeOfDay;
		RollUpTimeEntry.Time = deviceData.RollUpTime.TimeOfDay;
		FollowSunriseCheckbox.IsChecked = deviceData.FollowSunrise;
		FollowSunsetCheckbox.IsChecked = deviceData.FollowSunset;
	}

	public DeviceData GetDeviceData()
	{
		return new DeviceData
		{
			RollDownTime = Convert.ToDateTime(RollDownTimeEntry.Time),
			RollUpTime = Convert.ToDateTime(RollUpTimeEntry.Time),
			FollowSunrise = FollowSunriseCheckbox.IsChecked,
			FollowSunset = FollowSunsetCheckbox.IsChecked,
		};
	}
}