using DevicePlatform.Data;
using DevicePlatform.Models;
using SmartDevicePlatformPlugin;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using DevicePlatform.BackendControllers;

namespace DevicePlatform;

public partial class MainPage : ContentPage
{
    string storedUser = null;
	string storedPassword = "test";
    

	public MainPage()
	{
		InitializeComponent();
        
        if (!LoadLocalData())
		{
			PresentLoginScreen().Wait();
		}
		else
		{
            ActiveUser.Instance.apiController.Login(storedUser, storedPassword).Wait();
		}		

        RenderUI();
	}

    /// <summary>
    /// Renders the UI
    /// </summary>
	private void RenderUI()
	{
        MainContentView.Children.Clear();
        
        if (ActiveUser.Instance.LoggedIn)
        {
            if (ActiveUser.Instance.DevicePlugins.Count != 0)
            {
                foreach (var plugin in ActiveUser.Instance.DevicePlugins.Plugins)
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
                addNewDeviceButton.Clicked += ConfigureNewDeviceButton_Clicked;
                MainContentView.Children.Add(addNewDeviceButton);
            }
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

    /// <summary>
    /// When a plugin button is clicked, the corresponding device UI should be presented on a new page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void PluginButton_Clicked(object sender, EventArgs e)
    {
        foreach (var plugin in ActiveUser.Instance.DevicePlugins.Plugins)
        {
            if (((Button)sender).Text == plugin.Value.DeviceDescriptor.DeviceName)
            {
                await Navigation.PushAsync(plugin.Value.GetPluginContentPageUI());
            }
        }
    }

    /// <summary>
    /// Not yet implemented
    /// </summary>
    /// <returns></returns>
    private bool LoadLocalData()
	{
		// Pretend to try and look for stored User
		// If none is found, present login/create user screen.

		return storedUser != null;
	}

    /// <summary>
    /// Navigates to the login screetn
    /// </summary>
    /// <returns></returns>
	private async Task<bool> PresentLoginScreen()
	{
		await Navigation.PushAsync(new LoginPage());
		return true;
	}

    /// <summary>
    /// Navigates to the configurator
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private async void ConfigureNewDeviceButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DeviceConfigurationManager.ConfigurationManager());
    }


    /// <summary>
    /// Navigates to the login screen when a button is clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void GoToLoginButtonPage_Clicked(object sender, EventArgs e)
    {
        await PresentLoginScreen();
    }


    /// <summary>
    /// Whenever the main page is appering
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        RenderUI();
    }
}

