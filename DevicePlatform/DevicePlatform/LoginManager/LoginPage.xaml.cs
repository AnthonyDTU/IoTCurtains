using DevicePlatform.Data;
using DevicePlatform.Models;
using System.Net;

namespace DevicePlatform;

public partial class LoginPage : ContentPage
{
	APIHandler apiHandler;

	public LoginPage(APIHandler apiHandler)
	{
		InitializeComponent();
		this.apiHandler = apiHandler;
	}

	private async void LoginButton_Clicked(object sender, EventArgs e)
	{
		try
		{
			HttpStatusCode status = await apiHandler.Login(usernameEntry.Text, passwordEntry.Text);

			if (status == HttpStatusCode.OK)
			{
				Navigation.PopAsync();
			}
			else if (status == HttpStatusCode.NotFound)
			{
				infoMessageLabel.Text = "User not found";
			}
        }
		catch (Exception)
		{

			throw;
		}
	}

	private async void GoToCreateNewUserButton_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new CreateNewUserPage(apiHandler));
	}
}