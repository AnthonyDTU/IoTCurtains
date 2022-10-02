namespace IoTCurtains.ConfigurationManager;

using System.IO.Ports;
using System.Text.Json;
using System.Timers;

using DeviceConfiguration;

#pragma warning disable CA1416 // Validate platform compatibility
public partial class ConfigurationManager : ContentPage
{
	SerialPort serialPort;

	private string COMPort = "COM1";
    private int baudRate = 115200;
	private int dataBits = 8;

    private Parity parity = Parity.None;
    private StopBits stopBits = StopBits.One;

    private const string getConfigCommand = "getConfig";
	private const string configureDeviceCommand = "configureDevice";
	private const string resetDeviceCommand = "resetDevice";

	public ConfigurationManager()
	{
		InitializeComponent();

		COMPortPicker.ItemsSource = SerialPort.GetPortNames();
		if (COMPortPicker.Items.Count != 0)
		{
			COMPortPicker.SelectedIndex = 0;
		}

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
	}


	private void OpenDeviceButton_Clicked(object sender, EventArgs e)
	{
		// Set parameters from UI:
		// ***

		try
		{

            serialPort = new SerialPort(COMPort,
                                        baudRate,
                                        parity,
                                        dataBits,
                                        stopBits);
			serialPort.Open();
			serialPort.WriteLine(getConfigCommand);

			string configurationJson = serialPort.ReadLine();


			NodeConfiguration configuration = (NodeConfiguration)JsonSerializer.Deserialize(configurationJson, typeof(NodeConfiguration));

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
#pragma warning restore CA1416 // Validate platform compatibility