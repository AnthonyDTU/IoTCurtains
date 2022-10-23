using DevicePlatform.Models;
using SmartDevice;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevicePlatform;

public partial class MainPage : ContentPage
{
	DeviceCollection deviceCollection;


    Uri UriBase = new Uri("https://localhost:7173/");
    int timeoutMs = 5000;

    HttpClient backendAPI;
	User currentUser;


	private void InitBackendConnection()
	{
		backendAPI = new HttpClient();
		backendAPI.BaseAddress = UriBase;
	}

	

	public MainPage()
	{
		InitializeComponent();
        InitBackendConnection();


        if (!LoadLocalData())
		{
			// Present Login Screen
		}
		else
		{
			// Get devices for user, stored in backend
		}

		deviceCollection = new DeviceCollection();



		if (currentUser.Devices != null &&
			currentUser.Devices.Count != 0)
		{
			foreach (var device in currentUser.Devices)
			{
				switch (device.DeviceType)
				{
					case "Smart Curtains":
						deviceCollection.Devices.Add(device.DeviceId.ToString(), new SmartCurtains.SmartCurtains(backendAPI, device.DeviceId, device.DeviceName, device.DeviceKey));
						break;

					default:
						break;
				}
			}
		}



		if (deviceCollection.Devices.Count != 0)
		{
			foreach (var device in deviceCollection.Devices)
			{

                MainContentView.Children.Add(device.Value.GetDeviceUI(""));                
			}
		}
		else
		{
			MainContentView.Children.Add(new Label() { Text = "No Devices Configured...", HorizontalOptions = LayoutOptions.Center } );
		}

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
	}


	private bool LoadLocalData()
	{
		// Pretend to try and look for stored User
		// If none is found, present login/create user screen.

		return true;
	}


	/// <summary>
	/// Tries to create a new user in backend
	/// </summary>
	/// <param name="username"></param>
	/// <param name="password"></param>
    private async void CreateUser(string username, string password)
    {
        NewUser newUser = new NewUser()
        {
            UserName = username,
            Password = password
        };

        try
        {
            var result = await backendAPI.PostAsJsonAsync<NewUser>("api/Users", newUser);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                UsernameInput.TextColor = Colors.White;
				currentUser = await result.Content.ReadFromJsonAsync<User>();

                UsernameOutput.Text = currentUser.UserName;
                PasswordOutput.Text = currentUser.Password;
                GUIDOutput.Text = currentUser.UserID.ToString();

            }
            else if (result.StatusCode == HttpStatusCode.Conflict)
            {
                UsernameInput.TextColor = Colors.Red;
            }
			else
			{
				// Other error
			}
        }
        catch (Exception)
        {

            throw;
        }
    }


    private async Task<User> GetUserData(string username, string password)
	{
		try
		{
            var result = await backendAPI.GetAsync(($"api/Users/loginAttempt?username={username}&pass={password}").Replace(" ", "%20"));
            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                // Non existing user
            }
            else if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                // Wrong password
            }
			else
			{
                User user = await result.Content.ReadFromJsonAsync<User>();
                return user;
            }

			return new User();

		}
        catch (Exception ex)
		{
			return new User();
		}



    }








	private async void AddNewDeviceButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DeviceConfigurationManager.ConfigurationManager(backendAPI, deviceCollection));
		ReRenderView();
    }


	private void ReRenderView()
	{

	}


	private async void GetUserButton_Clicked(object sender, EventArgs e)
	{
		User user = await GetUserData(UsernameInput.Text, PasswordInput.Text);
		UsernameOutput.Text = user.UserName;
		PasswordOutput.Text = user.Password;
		GUIDOutput.Text = user.UserID.ToString();
	}

	private void NewUserButton_Clicked(object sender, EventArgs e)
	{
		CreateUser(UsernameInput.Text, PasswordInput.Text);
	}
}

