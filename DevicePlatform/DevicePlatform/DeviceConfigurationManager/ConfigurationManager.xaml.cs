namespace DevicePlatform.DeviceConfigurationManager;

using SmartDevice;
using System.IO.Ports;

#pragma warning disable CA1416 // Validate platform compatibility
public partial class ConfigurationManager : ContentPage
{
    private int baudRate = 115200;
	private int dataBits = 8;
    private Parity parity = Parity.None;
    private StopBits stopBits = StopBits.One;

    private SerialPort serialPort;
	private string deviceModel;
	private DeviceCollection devices;
	private Guid deviceID;
	private bool newDevice;

	HttpClient backendAPI;

	IDevice workingDevice;

	/// <summary>
	/// Creates a new device and starts the configuration process
	/// </summary>
	/// <param name="backendAPI"></param>
	/// <param name="devices"></param>
	public ConfigurationManager(HttpClient backendAPI, DeviceCollection devices)
	{
		InitializeComponent();

		this.backendAPI = backendAPI;
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
    public ConfigurationManager(HttpClient backendAPI, DeviceCollection devices, Guid deviceID)
    {
        InitializeComponent();

        this.backendAPI = backendAPI;
        this.devices = devices;
        this.deviceID = deviceID;
		newDevice = false;

        SerialConfiguratorView serialConfiguratorView = new SerialConfiguratorView(SetupSerialConnection);
        ConfigurationView.Children.Add(serialConfiguratorView);
    }

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

	private void QueryDeviceModel()
	{
		serialPort.WriteLine("DeviceModel?");
		while (serialPort.BytesToRead == 0) ; // TODO: Add timeout
        deviceModel = serialPort.ReadLine();

		SetupConfigurationView();
    }

	private void SetupConfigurationView()
	{
		switch (deviceModel)
		{
			case "Smart Curtains":
				if (newDevice)
					workingDevice = new SmartCurtains.SmartCurtains(backendAPI);					
				else
					workingDevice = devices.GetDevice(deviceID);				
				break;

			default:
				break;
		}
		ConfigurationView.Children.Add(workingDevice.DeviceConfigurator.GetConfigurationView());
	}
}
#pragma warning restore CA1416 // Validate platform compatibility