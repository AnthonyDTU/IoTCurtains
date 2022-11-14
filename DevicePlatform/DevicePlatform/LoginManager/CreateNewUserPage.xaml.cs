using DevicePlatform.Data;
using System.Xml;
using System.Net;
using DevicePlatform.BackendControllers;

namespace DevicePlatform;

public partial class CreateNewUserPage : ContentPage
{
	public CreateNewUserPage()
	{
		InitializeComponent();
	}

	private async void CreateNewUser_Clicked(object sender, EventArgs e)
	{
		try
		{
			if (!passwordEntry.Text.Equals(confirmPasswordEntry.Text))
			{
				passwordEntry.Text = string.Empty;
				confirmPasswordEntry.Text = string.Empty;
				return;
			}

            HttpStatusCode status = await ActiveUser.apiController.CreateNewUser(usernameEntry.Text, passwordEntry.Text);
            if (status == HttpStatusCode.OK)
            {
				Navigation.PopAsync();
				Navigation.PopAsync();
            }
            else if (status == HttpStatusCode.Conflict)
            {
				usernameEntry.TextColor = Colors.Red;
				infoMessageLabel.Text = "Username not availiable";
            }
        }
		catch (Exception)
		{
			infoMessageLabel.Text = "Connection to server timed out";
			throw;
		}
	}
}