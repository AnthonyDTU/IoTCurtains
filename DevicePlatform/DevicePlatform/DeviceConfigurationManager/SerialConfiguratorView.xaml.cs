namespace DevicePlatform.DeviceConfigurationManager;

using System.IO.Ports;

#pragma warning disable CA1416 // Validate platform compatibility
public partial class SerialConfiguratorView : ContentView
{
    private string COMPort = "COM1";

    ConfigurationManager parent;

    public delegate void ConnectButtonPressed(string COMPort);
    public ConnectButtonPressed connectButtonPressed;

    public SerialConfiguratorView(ConnectButtonPressed callbackHandler)
    {
        connectButtonPressed = callbackHandler;
        InitializeComponent();
        COMPortPicker.ItemsSource = SerialPort.GetPortNames();
        COMPortPicker.SelectedIndex = 0;


        parent = (ConfigurationManager)Parent;

    }

    private void OpenDeviceButton_Clicked(object sender, EventArgs e)
    {
        if (connectButtonPressed != null)
            connectButtonPressed(COMPortPicker.SelectedItem as string);        
    }

    private void UpdatePorts_Clicked(object sender, EventArgs e)
    {
        string selectedPort = (string)COMPortPicker.SelectedItem;
        COMPortPicker.ItemsSource = SerialPort.GetPortNames();
        COMPortPicker.SelectedItem = selectedPort;
    }

    public string GetSelectedComport()
    {
        return COMPortPicker.SelectedItem as string;
    }
}
#pragma warning restore CA1416 // Validate platform compatibility