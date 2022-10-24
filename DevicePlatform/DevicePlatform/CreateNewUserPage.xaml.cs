using DevicePlatform.Data;
using System.Xml;
using System.Net;

namespace DevicePlatform;

public partial class CreateNewUserPage : ContentPage
{
	APIHandler apiHandler;

	public CreateNewUserPage(APIHandler apiHandler)
	{
		InitializeComponent();
		this.apiHandler = apiHandler;
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

            HttpStatusCode status = await apiHandler.CreateNewUser(usernameEntry.Text, passwordEntry.Text);
            if (status == HttpStatusCode.OK)
            {
				Navigation.PopAsync();
				Navigation.PopAsync();
            }
            else if (status == HttpStatusCode.Conflict)
            {
				usernameEntry.TextColor = Colors.Red;
            }
        }
		catch (Exception)
		{

			throw;
		}
	}
}