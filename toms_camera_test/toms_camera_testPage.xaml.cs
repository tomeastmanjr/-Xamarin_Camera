using System;
using Plugin.Media;
using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Media.Abstractions;

namespace toms_camera_test
{
    public partial class toms_camera_testPage : ContentPage
    {
        public toms_camera_testPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Takes a photo
        /// </summary>
        /// <param name="sender">Button on Xaml page</param>
        /// <param name="args">None being passed currently</param>
		public async void TakePhoto(object sender, EventArgs args)
		{
			// This is required. Initialize the plugin to gain access to API's
			await CrossMedia.Current.Initialize();

            // Check to see if permission has been granted by the user to use the camera and storage
			var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
			var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            // If the user hasn't granted permission to use the camera or storage, request again
			if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
			{
				var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
				cameraStatus = results[Permission.Camera];
				storageStatus = results[Permission.Storage];
			}
		    
            // Check to see if the camera is avialble and taking a photo is supported
		    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
		    {

				await DisplayAlert("No Camera", ":( No camera available.", "OK");
		        return;
		    }

            // If permission is granted for the camera AND the storage, take a photo.
            // Currently, save the file in a sample directory with the name test.jpg
			if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
			{
				var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
				{
					Directory = "Sample",
					Name = "test.jpg"
				});

				if (file == null)
					return;

                // Display an alert showing where the file was saved
				await DisplayAlert("File Location", file.Path, "OK");

                // Update the Source for the image tag on the Xaml page with where we just saved the file
				image.Source = ImageSource.FromStream(() =>
				{
					var stream = file.GetStream();
					file.Dispose();
					return stream;
				});

				//or:
				//image.Source = ImageSource.FromFile(file.Path);
				//image.Dispose();
			}
			else
			{
				await DisplayAlert("Permissions Denied", "Unable to take photos.", "OK");
				//On iOS you may want to send your user to the settings screen.
				//CrossPermissions.Current.OpenAppSettings();
			}
		}

        /// <summary>
        /// Pick a photo from your local storage
        /// </summary>
        /// <param name="sender">Button on the Xaml page</param>
        /// <param name="args">None currently being passed</param>
        public async void PickPhoto(object sender, EventArgs args)
        {
			// This is required. Initialize the plugin to gain access to API's
			await CrossMedia.Current.Initialize();

			// Check to see if permission has been granted by the user to use the storage
			var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

			// If the user hasn't granted permission to use the camera or storage, request again
			if (storageStatus != PermissionStatus.Granted)
			{
				var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
				storageStatus = results[Permission.Storage];
			}

			// If permission is granted for the storage, open the gallery to choose a photo.
			if (storageStatus == PermissionStatus.Granted)
			{
				var file = await CrossMedia.Current.PickPhotoAsync();

				if (file == null)
					return;

				// Display an alert showing where the file was saved
				await DisplayAlert("File Location", file.Path, "OK");

				// Update the Source for the image tag on the Xaml page with where we just saved the file
				image.Source = ImageSource.FromStream(() =>
				{
					var stream = file.GetStream();
					file.Dispose();
					return stream;
				});

				//or:
				//image.Source = ImageSource.FromFile(file.Path);
				//image.Dispose();
			}
			else
			{
				await DisplayAlert("Permissions Denied", "Unable to access photos.", "OK");
				//On iOS you may want to send your user to the settings screen.
				//CrossPermissions.Current.OpenAppSettings();
			}

        }
    }
}
