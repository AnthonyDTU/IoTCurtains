using SmartDevice;
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

	private async void CreateUser()
	{
		NewUser newUser = new NewUser()
		{
			UserName = "Anton Lage",
			Password = "test"
		};

		await backendAPI.PostAsJsonAsync<NewUser>("api/Users", newUser);

        //currentUser = await backendAPI.PostAsJsonAsync<NewUser>("api/Users", newUser).Result.Content.ReadFromJsonAsync<User>();
    }


	public MainPage()
	{
		InitializeComponent();
        InitBackendConnection();

		CreateUser();
		GetUserData();

        if (!LoadLocalData())
		{
			// Present Login Screen
		}














		deviceCollection = new DeviceCollection();








		if (deviceCollection.Devices.Count == 0)
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


	private async void GetUserData()
	{

		UserCredentials credentials = new UserCredentials("Anton Lage", "test");
		//currentUser = await backendAPI.GetFromJsonAsync<User>($"api/user/loginRequest?");

		currentUser = await backendAPI.GetFromJsonAsync<User>($"api/Users{JsonSerializer.Serialize(credentials)}");

    }








	private async void AddNewDeviceButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DeviceConfigurationManager.ConfigurationManager(deviceCollection, false));
    }
}

