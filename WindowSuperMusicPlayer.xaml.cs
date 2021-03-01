using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TagLib;
using TagLib.Image;
using TagLib.Jpeg;
using TagLib.Audible;
using Microsoft.WindowsAPICodePack.Shell;

namespace odtwarzacz_video {
	public partial class WindowSuperVideoPlayer : Window {
		String[] files, paths;
		//Bitmap Bitmap = new Bitmap();
		bool loadedPlaylist = false;
		int trackIndex, videoDuration;
		TagLib.File fileToTagging;
		Boolean isPaused = false, isLooping = false;

		public WindowSuperVideoPlayer() {
			InitializeComponent();
			sztuczna.Visibility = Visibility.Hidden;
			videoPlayer.Opacity = 0.5;
			buttonPlay.IsEnabled = false;
			buttonPlay.Opacity = 0.5;
			buttonPrevious.IsEnabled = false;
			buttonPrevious.Opacity = 0.5;
			buttonBackwards.IsEnabled = false;
			buttonBackwards.Opacity = 0.5;
			buttonForwards.IsEnabled = false;
			buttonForwards.Opacity = 0.5;
			buttonNext.IsEnabled = false;
			buttonNext.Opacity = 0.5;
			textBlockAlbumName.IsEnabled = false;
			textBlockAlbumName.Opacity = 0.5;
			textBlockArtistName.IsEnabled = false;
			textBlockArtistName.Opacity = 0.5;
			textBlockSongName.Opacity = 0.5;

			try {
				videoPlayer.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\movie.png");
				videoPlayer.ScrubbingEnabled = true;    //shows video preview
				videoPlayer.Play();
				videoPlayer.Stop();

				buttonPlay.Visibility = Visibility.Visible;
				buttonPause.Visibility = Visibility.Hidden;
			}
			catch (FileNotFoundException e) {
				MessageBox.Show($"The music visualiser file was not found: '{e}'");
			}
		}

		private void ButtonSelectVideos_Click(object sender, RoutedEventArgs e) {
			SelectVideos();
			//videoPlayer.Source = new Uri(@"C:\Users\admin\Videos\Saszan - Ostatni raz (Lyric Video) - YouTube.mp4");
			videoPlayer.ScrubbingEnabled = true;    //shows video preview
			videoPlayer.Play();
			videoPlayer.Stop();
		}

		private void PlayNextTrack() {
			if (trackIndex < listBoxPlaylist.Items.Count - 1) {
				trackIndex += 1;
				isPaused = false;
				PlayVideo();

				textBlockSongName.Text = listBoxPlaylist.Items[trackIndex].ToString();
				videoPlayer.Position = TimeSpan.FromSeconds(1);
			}
		}

		private void CreateVideoThumbnail() {
			MemoryStream ms = new MemoryStream();
			ShellFile shellFile = ShellFile.FromFilePath(paths[trackIndex]);
			Bitmap bm = shellFile.Thumbnail.Bitmap;
			bm.Save(ms, ImageFormat.Png);
			BitmapImage bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = ms;
			bi.EndInit();
			imageAlbumCoverInBackground.Source = bi;
		}

		private void PlayPreviousTrack() {
			if (trackIndex > 0) {
				trackIndex -= 1;
				isPaused = false;
				PlayVideo();
				textBlockSongName.Text = listBoxPlaylist.Items[trackIndex].ToString();
				videoPlayer.Position = TimeSpan.FromSeconds(1);
				buttonNext.IsEnabled = true;
				buttonNext.Opacity = 1;
			}
		}
		private void PlayVideo() {
			sztuczna.Visibility = Visibility.Visible;
			if (loadedPlaylist == true) {
				fileToTagging = TagLib.File.Create(paths[trackIndex]);
				if (!isPaused) {                //do these if video is being played
					videoPlayer.Source = new Uri(paths[trackIndex]);
					textBlockAlbumName.Text = fileToTagging.Tag.Album;
					textBlockArtistName.Text = fileToTagging.Tag.FirstPerformer;
					videoPlayer.ScrubbingEnabled = true;
					textBlockSongName.Text = listBoxPlaylist.Items[trackIndex].ToString();
					try {
						CreateVideoThumbnail();

					}
					catch (Exception ex) {
						MessageBox.Show("Error during looking for an artwork");
					}

				}
				buttonPlay.Visibility = Visibility.Hidden;
				buttonPause.Visibility = Visibility.Visible;
				videoPlayer.Play();
				isPaused = false;

				if (trackIndex == listBoxPlaylist.Items.Count - 1) {
					buttonNext.Opacity = 0.5;
					buttonNext.IsEnabled = false;
				} else if (trackIndex == 0) {
					buttonPrevious.Opacity = 0.5;
					buttonPrevious.IsEnabled = false;
				} else {
					buttonNext.Opacity = 1;
					buttonNext.IsEnabled = true;
					buttonPrevious.Opacity = 1;
					buttonPrevious.IsEnabled = true;
				}
			} else MessageBox.Show("You can add some songs obviously :)");
		}

		private void PauseVideo() {
			if (loadedPlaylist == true) {
				videoPlayer.Pause();
				isPaused = true;
				buttonPlay.Visibility = Visibility.Visible;
				buttonPause.Visibility = Visibility.Hidden;
			}
		}
		private void SelectVideos() {             //puts the chosen songs to the playlistListBox
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos); // "C:\\Users\\admin\\Videos";
			openFileDialog.Filter = "video files (*.mp4)|*.mp4|" +
									"Flash Video (*.flv)|*.flv|" +
									"Windows Media Video files (*.wmv)|*.wmv|" +
									"Ogg Video (*.ogv)|*.ogv|" +
									"WebM (*.webm)|*.webm|" +
									"All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;

			openFileDialog.Multiselect = true;
			if (openFileDialog.ShowDialog() == true) {
				try {
					files = openFileDialog.SafeFileNames;    //saves the track names in the tracks array
					paths = openFileDialog.FileNames;        //saves the path tracks in the tracks array

					for (int i = 0; i < files.Length; i++) {
						listBoxPlaylist.Items.Add(files[i]);
						//listBoxPlaylist.ItemTemplate.Template.
						//listBoxPlaylist.ItemTemplate.Resources.Source = new Uri(@"C:\Users\admin\Desktop\inżynieria oprogramowania\ekran startowy2.png"); 
						//TagLib.IPicture artwork = fileToTagging.Tag.Pictures[0];
						//BitmapFrame bitmapFrame = BitmapFrame.Create(new MemoryStream(artwork.Data.Data));
						//imageAlbumCover.Source = bitmapFrame;
					}

					loadedPlaylist = true;
				}
				catch (Exception ex) {
					MessageBox.Show("Error: Could not read the file from disk. Original error: " + ex.Message);
				}
			}
			videoPlayer.Opacity = 1;
			buttonPlay.IsEnabled = true;
			buttonPlay.Opacity = 1;
			buttonPrevious.IsEnabled = true;
			buttonPrevious.Opacity = 1;
			buttonBackwards.IsEnabled = true;
			buttonBackwards.Opacity = 1;
			buttonForwards.IsEnabled = true;
			buttonForwards.Opacity = 1;
			buttonNext.IsEnabled = true;
			buttonNext.Opacity = 1;
			textBlockAlbumName.IsEnabled = true;
			textBlockAlbumName.Opacity = 1;
			textBlockArtistName.IsEnabled = true;
			textBlockArtistName.Opacity = 1;
			textBlockSongName.Opacity = 1;
		}


		#region media controlling
		private void ButtonPrevious_MouseDown(object sender, MouseButtonEventArgs e) {
			PlayPreviousTrack();
		}

		private void ButtonBackwards_MouseDown(object sender, MouseButtonEventArgs e) { //sets 10 seconds rewinds in the media and video players
																						//TimeSpan ts = new TimeSpan(0, 0, 0, (int)(mediaPlayer.Position.Seconds - 10));
			TimeSpan ts = new TimeSpan(0, 0, 0, (int)(videoPlayer.Position.Seconds - 10));
			videoPlayer.Position = ts;
		}

		private void ButtonPause_MouseDown(object sender, MouseButtonEventArgs e) {
			PauseVideo();
		}

		private void ButtonPlay_MouseDown(object sender, MouseButtonEventArgs e) {
			PlayVideo();
		}

		private void ButtonForwards_MouseDown(object sender, MouseButtonEventArgs e) {
			TimeSpan ts = new TimeSpan(0, 0, 0, (int)(videoPlayer.Position.Seconds + 10));
			videoPlayer.Position = ts;
		}

		private void ButtonNext_MouseDown(object sender, MouseButtonEventArgs e) {
			try {
				PlayNextTrack();
			}
			catch (Exception ex) {
				MessageBox.Show(ex.ToString());
			}
		}

		//forwards or backwards the media and video players according to position of the slider
		private void MediaPlayerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			videoPlayer.Position = TimeSpan.FromSeconds(videoPlayerSlider.Value);
			TextBlockCurrentTimeSign.Text = TimeSpan.FromSeconds(videoPlayerSlider.Value).ToString(@"m\:ss");
		}
		#endregion

		private void TimerForLoop_Tick(object sender, EventArgs e) {
			labelIsLoop.Visibility = Visibility.Hidden;

		}

		private void Timer_Tick(object sender, EventArgs e) {       //
			try {
				if ((videoPlayer.Source != null) && (videoPlayer.NaturalDuration.HasTimeSpan)) {
					videoPlayerSlider.Minimum = 0;
					videoPlayerSlider.Maximum = videoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
					videoPlayerSlider.Value = videoPlayer.Position.TotalSeconds;
					videoDuration = (int)(videoPlayer.NaturalDuration.TimeSpan.TotalSeconds);
					textBlockVideoDuration.Text = TimeSpan.FromSeconds(videoDuration).ToString(@"m\:ss");

					if (videoPlayerSlider.Maximum == videoPlayerSlider.Value) //if end of the track the media player plays the next track
						PlayNextTrack();
				}
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
		#region other components

		private void PlaylistListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {  //plays the selected track
			try {
				DispatcherTimer timer = new DispatcherTimer();
				timer.Interval = TimeSpan.FromSeconds(1);
				timer.Tick += Timer_Tick;
				timer.Start();

				trackIndex = listBoxPlaylist.SelectedIndex;
				PlayVideo();
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void PlaylistListBox_Drop(object sender, DragEventArgs e) {
			string filename = (string)((DataObject)e.Data).GetFileDropList()[0];
			listBoxPlaylist.SelectionMode = SelectionMode.Extended;
			SelectVideos();
		}

		private void listBoxPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e) {


		}

		private void ButtonLoop_MouseDown(object sender, MouseButtonEventArgs e) {
			labelIsLoop.Visibility = Visibility.Visible;
			if (isLooping) {
				buttonLoop.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\noLoop.png"));
				isLooping = false;
				labelIsLoop.Content = " Looping is off";
			} else if (!isLooping) {
				buttonLoop.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\loop.png"));
				isLooping = true;
				labelIsLoop.Content = " Looping is on";
			}

			DispatcherTimer timeoutForLoop = new DispatcherTimer();
			timeoutForLoop.Interval = TimeSpan.FromSeconds(5);
			timeoutForLoop.Start();
			timeoutForLoop.Tick += TimerForLoop_Tick; //when time elepased

		}
		#endregion

		#region menu
		private void MenuItemNewPlaylist_Click(object sender, RoutedEventArgs e) {
			listBoxPlaylist.Items.Clear();
			SelectVideos();
		}
		#endregion

		private void Window_Closed(object sender, EventArgs e) {
			Environment.Exit(0);
		}
	}
}
