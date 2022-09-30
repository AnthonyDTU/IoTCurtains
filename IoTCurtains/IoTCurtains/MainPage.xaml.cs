namespace IoTCurtains;


using System.IO.Ports;

public partial class MainPage : ContentPage
{
	int count = 0;


	SerialPort serialPort;



	public MainPage()
	{
		InitializeComponent();

        serialPort = new SerialPort("COM9", 115200, Parity.None, 8, StopBits.One);
		serialPort.Open();
    }

	private void OnCounterClicked(object sender, EventArgs e)
	{
		serialPort.WriteLine("Test");
		
	}
}

