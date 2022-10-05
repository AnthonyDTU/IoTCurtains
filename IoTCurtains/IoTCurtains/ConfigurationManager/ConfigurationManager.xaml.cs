namespace IoTCurtains.ConfigurationManager;

using System.IO.Ports;
using System.Text.Json;
using System.Timers;

using DeviceConfiguration;

#pragma warning disable CA1416 // Validate platform compatibility
public partial class ConfigurationManager : ContentPage
{
	SerialPort serialPort;

    private const string getConfigCommand = "getConfig";
    private const string configureDeviceCommand = "configureDevice";
    private const string resetDeviceCommand = "resetDevice";

    public ConfigurationManager()
	{
		InitializeComponent();
        configurationEditor.DisableControls();
        ResetDeviceButton.IsEnabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serialPort"></param>
	public void SetSerialPort(SerialPort serialPort)
	{
        this.serialPort = serialPort;
        GetDeviceConfig();	
    }

    /// <summary>
    /// 
    /// </summary>
    private void GetDeviceConfig()
    {
		try
		{
            serialPort.WriteLine(getConfigCommand);
            string configurationJson = serialPort.ReadLine();
            NodeConfiguration configuration = (NodeConfiguration)JsonSerializer.Deserialize(configurationJson, typeof(NodeConfiguration));
            configurationEditor.SetConfiguration(configuration);
            configurationEditor.EnableControls();
            ResetDeviceButton.IsEnabled = true;
        }
        catch (Exception)
		{
			throw;
		}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonConfiguration"></param>
	public void SendConfigurationToDevice(string jsonConfiguration)
	{
		try
		{
            serialPort.WriteLine(configureDeviceCommand);
            string response = serialPort.ReadLine();
            if (response == "")
            {
                serialPort.WriteLine(jsonConfiguration);
            }
            else
            {
                // Error
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
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ResetDeviceButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Message box verifying
            serialPort.WriteLine(resetDeviceCommand);
            string response = serialPort.ReadLine();
            // Print response
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    ~ConfigurationManager()
    {
        serialPort.Close();
    }

}
#pragma warning restore CA1416 // Validate platform compatibility