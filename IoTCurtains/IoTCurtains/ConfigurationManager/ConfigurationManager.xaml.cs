namespace IoTCurtains.ConfigurationManager;

using System.IO.Ports;
using System.Text.Json;
using System.Timers;

public partial class ConfigurationManager : ContentPage
{
	SerialPort serialPort;

	private string COMPort = "COM1";
    private int baudRate = 115200;
	private int dataBits = 8;

#pragma warning disable CA1416 // Validate platform compatibility
    private Parity parity = Parity.None;
    private StopBits stopBits = StopBits.One;
#pragma warning restore CA1416 // Validate platform compatibility

    private const string getConfigCommand = "getConfig";
	private const string configureDeviceCommand = "configureDevice";
	private const string resetDeviceCommand = "resetDevice";

	Timer recheckComportsTimer = new Timer(1000);
	public ConfigurationManager()
	{
		InitializeComponent();

		COMPortPicker.ItemsSource = SerialPort.GetPortNames();

		ParityPicker.ItemsSource = new List<string>()
		{
			"None",
			"Odd",
			"Even"
		};
		ParityPicker.SelectedIndex = 0;

		StopBitsPicker.ItemsSource = new List<string>()
		{
			"One",
			"Two"
		};
		StopBitsPicker.SelectedIndex = 0;

		recheckComportsTimer.Elapsed += RecheckComportsTimer_Elapsed;
		recheckComportsTimer.Enabled = true;
		recheckComportsTimer.Start();
	
	}

	private void RecheckComportsTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
        COMPortPicker.ItemsSource = SerialPort.GetPortNames();
    }

	private void OpenDeviceButton_Clicked(object sender, EventArgs e)
	{
		// Set parameters from UI:
		// ***

		try
		{
#pragma warning disable CA1416 // Validate platform compatibility

            serialPort = new SerialPort(COMPort,
                                        baudRate,
                                        parity,
                                        dataBits,
                                        stopBits);
			serialPort.Open();
			serialPort.WriteLine(getConfigCommand);

			string configurationJson = serialPort.ReadLine();

#pragma warning restore CA1416 // Validate platform compatibility

			DeviceConfiguration configuration = (DeviceConfiguration)JsonSerializer.Deserialize(configurationJson, typeof(DeviceConfiguration));

			// Populate controls with parsed configuration
        }
		catch (Exception)
		{

			throw;
		}



	}

	private void SendConfigurationToDeviceButton_Clicked(object sender, EventArgs e)
	{

	}
}