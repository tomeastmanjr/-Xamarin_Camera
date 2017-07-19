using System;
using Plugin.Media;
using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.Media.Abstractions;
using System.Diagnostics;

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
		    
            // Check to see if the camera is avialble and taking a photo is supported
		    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
		    {

				await DisplayAlert("No Camera", ":( No camera available.", "OK");
		        return;
		    }

            // Take a photo.
            // Currently, save the file in a sample directory with the name test.jpg
			var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				
                // RESET PHOTO SIZE
				// By default the photo that is taken/picked is the maxiumum size and quality available. 
                // For most applications this is not needed and can be Resized. 
                // This can be accomplished by adjusting the PhotoSize property on the options. 
                // The easiest is to adjust it to Small, Medium, or Large, which is 25%, 50%, or 75% or the original.

				// Photo Size options examples

				PhotoSize = PhotoSize.Medium,

				//PhotoSize = PhotoSize.Custom,
                //CustomPhotoSize = 90, // Resize to 90% of original

                // PHOTO QUALITY
				// Set the CompressionQuality, which is a value from 0 the most compressed all the way to 100, which is no compression.
                // A good setting from testing is around 92
                CompressionQuality = 92,

                // SAVING PHOTO TO CAMERA ROLL
				// You can now save a photo or video to the camera roll/gallery.
				// When creating the StoreCameraMediaOptions or StoreVideoMediaOptions simply set SaveToAlbum to true.
				// When your user takes a photo it will still store temporary data, but also if needed make a copy to the public gallery (based on platform).
				// In the MediaFile you will now see a AlbumPath that you can query as well.
				// This will restult in 2 photos being saved for the photo.
                // One in your private folder and one in a public directory that is shown. The value will be returned at AlbumPath.
                // Android: When you set SaveToAlbum this will make it so your photos are public in the Pictures/YourDirectory or Movies/YourDirectory.
                // This is the only way Android can detect the photos.
                SaveToAlbum = true,

				// ALLOW CROPPING
                // iOS has crop controls built into the the camera control when taking a photo.
                // On iOS the default is false.
                // You can adjust the AllowCropping property when taking a photo to allow your user to crop.
                //AllowCropping = true,

                // DEFAULT CAMERA
                // By default when you take a photo or video the default system camera will be selected.
                // Simply set the DefaultCamera on StoreCameraMediaOptions.
                // This option does not guarantee that the actual camera will be selected because each platform is different.
                // It seems to work extremely well on iOS,
                // but not so much on Android.Your mileage may vary.
                //DefaultCamera = CameraDevice.Front,

                // Where should we save our private copy of the photo
				Directory = "toms_camera_test_photos",

                // What should we name the photo
                // Might be a good idea to add a time stamp on here
				Name = "testPhoto" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg",

			});

			if (file == null)
				return;

			//Get the public album path
			var aPpath = file.AlbumPath;
			Debug.WriteLine("This is the file.AlbumPath " + aPpath);

			//Get private path
			var path = file.Path;
            Debug.WriteLine("This is the file.Path " + path);

            // Display an alert showing where the file was saved
			await DisplayAlert("File Location private path", path, "OK");

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

        /// <summary>
        /// Pick a photo from your local storage
        /// </summary>
        /// <param name="sender">Button on the Xaml page</param>
        /// <param name="args">None currently being passed</param>
        public async void PickPhoto(object sender, EventArgs args)
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
    }
}
