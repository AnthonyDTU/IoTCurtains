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
	private string deviceID;
	private bool newDevice;

	IDevice workingDevice;


	public ConfigurationManager(DeviceCollection devices, bool newDevice, string deviceID = null)
	{
		InitializeComponent();

		this.devices = devices;
		this.newDevice = newDevice;
		this.deviceID = deviceID;

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
		while (serialPort.BytesToRead == 0) ;
        deviceModel = serialPort.ReadLine();

		SetupConfigurationView();
    }

	private void SetupConfigurationView()
	{
		switch (deviceModel)
		{
			case "Smart Curtains":
				if (newDevice)
					workingDevice = new SmartCurtains.Device();						
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