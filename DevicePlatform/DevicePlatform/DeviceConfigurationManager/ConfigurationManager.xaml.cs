namespace DevicePlatform.DeviceConfigurationManager;

using DevicePlatform.Data;
using SmartDevicePlatformPlugin;
using System.IO.Ports;
using System.Text.Json;

#pragma warning disable CA1416 // Validate platform compatibility
public partial class ConfigurationManager : ContentPage
{
    private int baudRate = 115200;
	private int dataBits = 8;
    private Parity parity = Parity.None;
    private StopBits stopBits = StopBits.One;

    private SerialPort serialPort;
	private string deviceModel;
	private DevicePluginCollection devices;
	private Guid deviceID;
	private bool newDevice;

	Uri deviceUri;

    IPlatformPlugin workingDevicePlugin;

	/// <summary>
	/// Creates a new device and starts the configuration process
	/// </summary>
	/// <param name="backendAPI"></param>
	/// <param name="devices"></param>
	public ConfigurationManager(Uri deviceUri, DevicePluginCollection devices)
	{
		InitializeComponent();

		this.deviceUri = deviceUri;
		this.devices = devices;
		newDevice = true;

		SerialConfiguratorView serialConfiguratorView = new SerialConfiguratorView(SetupSerialConnection);
		ConfigurationView.Children.Add(serialConfiguratorView);	
	}


	/// <summary>
	/// Starts the configuration process of an existing device
	/// </summary>
	/// <param name="backendAPI"></param>
	/// <param name="devices"></param>
	/// <param name="deviceID"></param>
    public ConfigurationManager(Uri deviceUri, DevicePluginCollection devices, Guid deviceID)
    {
        InitializeComponent();

        this.deviceUri = deviceUri;
        this.devices = devices;
        this.deviceID = deviceID;
		newDevice = false;

        SerialConfiguratorView serialConfiguratorView = new SerialConfiguratorView(SetupSerialConnection);
        ConfigurationView.Children.Add(serialConfiguratorView);
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="COMPort"></param>
    private void SetupSerialConnection(string COMPort)
	{
        try
        {
            serialPort = new SerialPort(COMPort,
                                        baudRate,
                                        parity,
                                        dataBits,
                                        stopBits);
            serialPort.Open();

			if (serialPort.IsOpen)
			{
				QueryDeviceModel();
			}
        }
        catch (Exception)
        {

            throw;
        }
    }

	/// <summary>
	/// 
	/// </summary>
	private void QueryDeviceModel()
	{
		serialPort.WriteLine("deviceModel?");
		while (serialPort.BytesToRead == 0) ; // TODO: Add timeout
        deviceModel = serialPort.ReadLine();

		SetupConfigurationView();
    }

	/// <summary>
	/// 
	/// </summary>
	private void SetupConfigurationView()
	{
		switch (deviceModel)
		{
			case "Smart Curtains":
				if (newDevice)
					workingDevicePlugin = new SmartCurtainsPlatformPlugin.SmartCurtainsPlatformPlugin(ActiveUser.User.UserID, deviceUri);					
				else
					workingDevicePlugin = devices.GetDevice(deviceID);				
				break;

			default:
				break;
		}

		Button sendConfigurationToDeviceButton = new Button()
		{
			Text = "Send Configuration To Device",
			WidthRequest = 400,

		};
		sendConfigurationToDeviceButton.Clicked += SendConfigurationToDeviceButton_Clicked;


        ConfigurationView.Children.Add(workingDevicePlugin.DeviceConfigurator.GetConfigurationView());
		ConfigurationView.Children.Add(sendConfigurationToDeviceButton);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <exception cref="NotImplementedException"></exception>
	private void SendConfigurationToDeviceButton_Clicked(object sender, EventArgs e)
	{
		ActiveUser.DevicesPlugins.AddNewDevicePlugin("", workingDevicePlugin);
		serialPort.WriteLine("config");
		while(serialPort.BytesToRead == 0) ;
		string response = serialPort.ReadLine();
		if (response == "readyForConfig")
		{
			string jsonData = JsonSerializer.Serialize(workingDevicePlugin.DeviceConfigurator.BuildNewConfiguration());
            serialPort.WriteLine(jsonData);
		}
	}

}
#pragma warning restore CA1416 // Validate platform compatibility