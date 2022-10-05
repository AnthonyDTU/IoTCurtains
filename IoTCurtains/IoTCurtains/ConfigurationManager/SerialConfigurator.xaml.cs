using DeviceConfiguration;
using System.IO.Ports;
using System.Text.Json;

namespace IoTCurtains.ConfigurationManager;

#pragma warning disable CA1416 // Validate platform compatibility
public partial class SerialConfigurator : ContentView
{
    SerialPort serialPort;

    private string COMPort = "COM1";
    private int baudRate = 115200;
    private int dataBits = 8;

    private Parity parity = Parity.None;
    private StopBits stopBits = StopBits.One;


    ConfigurationManager parent;

    public SerialConfigurator()
	{
		InitializeComponent();

        parent = (ConfigurationManager)Parent;

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
        try
        {
            SerialPort serialPort = new SerialPort(COMPort,
                                                   baudRate,
                                                   parity,
                                                   dataBits,
                                                   stopBits);
            serialPort.Open();
            parent.SetSerialPort(serialPort);            
        }
        catch (Exception)
        {

            throw;
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility