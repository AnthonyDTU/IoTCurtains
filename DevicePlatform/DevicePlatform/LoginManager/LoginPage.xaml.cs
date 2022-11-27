using DevicePlatform.BackendControllers;
using DevicePlatform.Data;
using DevicePlatform.Models;
using System.Net;

namespace DevicePlatform;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async void LoginButton_Clicked(object sender, EventArgs e)
	{
		try
		{
			HttpStatusCode status = await ActiveUserSingleton.Instance.apiController.Login(usernameEntry.Text, passwordEntry.Text);

			if (status == HttpStatusCode.OK)
			{
				Navigation.PopAsync();
			}
			else if (status == HttpStatusCode.NotFound)
			{
				infoMessageLabel.Text = "User not found";
			}
			else if (status == HttpStatusCode.Ambiguous)
			{
				infoMessageLabel.Text = "Exeption Thrown While Dealing With Login";
			}
			else
			{
				infoMessageLabel.Text = "An error has occured while fetching user";
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
	private async void GoToCreateNewUserButton_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new CreateNewUserPage());
	}
}