using DevicePlatform.Data;
using DevicePlatform.Models;
using SmartDevicePlatformPlugin;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace DevicePlatform;

public partial class MainPage : ContentPage
{
    //Uri uriBase = new Uri("https://smartplatformbackendapi.azurewebsites.net");
    Uri uriBase = new Uri("https://localhost:7173/");

    int timeoutMs = 5000;


    APIHandler apiHandler;

    HubConnection hubConnection = null;


    string storedUser = null;
	string storedPassword = "test";

	public MainPage()
	{
		InitializeComponent();
        InitBackendConnection();

        if (!LoadLocalData())
		{
			PresentLoginScreen().Wait();
		}
		else
		{
            apiHandler.Login(storedUser, storedPassword).Wait();
		}		

        RenderUI();

	}


    private void InitBackendConnection()
    {
        apiHandler = new APIHandler();
    }


	private void RenderUI()
	{
        MainContentView.Children.Clear();

        if (ActiveUser.LoggedIn)
        {
            if (ActiveUser.DevicesPlugins.Count != 0)
            {
                foreach (var plugin in ActiveUser.DevicesPlugins.Plugins)
                {
                    Button pluginButton = new Button()
                    {
                        WidthRequest = 250,
                        HeightRequest = 100,
                        CornerRadius = 10,
                        BorderWidth = 3,
                        BorderColor = Colors.MediumPurple,
                        Text = plugin.Value.DeviceDescriptor.DeviceName,
                        FontSize = 18,
                    };

                    pluginButton.Clicked += PluginButton_Clicked;
                    MainContentView.Children.Add(pluginButton);
                }
            }
            else
            {
                MainContentView.Children.Add(new Label() 
                { 
                    Text = "No Devices Configured...", 
                    HorizontalOptions = LayoutOptions.Center 
                });
            }

            // On Windows:
            // Create a button for configuring a new device
            if (DeviceInfo.Current.Platform == Microsoft.Maui.Devices.DevicePlatform.WinUI)
            {
                Button addNewDeviceButton = new Button()
                {
                    WidthRequest = 250,
                    HeightRequest = 100,
                    CornerRadius = 10,
                    BackgroundColor = Colors.Cyan,
                    Text = "Configure New Device",
                    TextColor = Colors.Black,
                    FontSize = 15,


                };
                addNewDeviceButton.Clicked += AddNewDeviceButton_Clicked;
                MainContentView.Children.Add(addNewDeviceButton);
            }

            Button testButton = new Button()
            {
                WidthRequest = 250,
                HeightRequest = 100,
                CornerRadius = 10,
                BackgroundColor = Colors.Cyan,
                Text = "Test button",
                TextColor = Colors.Black,
                FontSize = 15,
            };

            testButton.Clicked += TestButton_Clicked;
            MainContentView.Children.Add(testButton);

        }
        else
        {
            Button goToLoginPageButton = new Button()
            {
                WidthRequest = 250,
                Text = "Login or Create User",
                FontSize = 15,
                HorizontalOptions = LayoutOptions.Center,
            };
            goToLoginPageButton.Clicked += GoToLoginButtonPage_Clicked;

            MainContentView.Children.Add(goToLoginPageButton);
            MainContentView.Children.Add(new Label() { Text = "To see and create devices", HorizontalOptions = LayoutOptions.Center });
        }      
    }

    private async void PluginButton_Clicked(object sender, EventArgs e)
    {
        foreach (var plugin in ActiveUser.DevicesPlugins.Plugins)
        {
            if (((Button)sender).Text == plugin.Value.DeviceDescriptor.DeviceName)
            {
                await Navigation.PushAsync(plugin.Value.GetPluginContentPageUI());
            }
        }
    }

    private async void TestButton_Clicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new DesignTestPage());




    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool LoadLocalData()
	{
		// Pretend to try and look for stored User
		// If none is found, present login/create user screen.

		return storedUser != null;
	}

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
	private async Task<bool> PresentLoginScreen()
	{
		await Navigation.PushAsync(new LoginPage(apiHandler));
		return true;
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private async void AddNewDeviceButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DeviceConfigurationManager.ConfigurationManager(ActiveUser.DevicesPlugins));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void GoToLoginButtonPage_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(apiHandler));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        RenderUI();

    }
}

