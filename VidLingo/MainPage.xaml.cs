using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.MediaElement; // Add this for MediaElement support


namespace VidLingo
{
    public partial class MainPage : ContentPage
    {
        private MediaElement mediaElement;
        private Label subtitleLabel;
        private string currentSubtitle = "";
        private string targetLanguage = "es"; // Default to Spanish, can be changed

        public MainPage()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(50) },
                    new RowDefinition { Height = new GridLength(3, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                }
            };

            var chooseFileButton = new Button
            {
                Text = "Choose Video File"
            };
            chooseFileButton.Clicked += OnChooseFileClicked;

            mediaElement = new MediaElement
            {
                ShouldAutoPlay = true,
                ShouldShowPlaybackControls = true
            };

            subtitleLabel = new Label
            {
                Text = "Subtitles will appear here",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnSubtitleTapped;
            subtitleLabel.GestureRecognizers.Add(tapGestureRecognizer);

            grid.Add(chooseFileButton, 0, 0);
            grid.Add(mediaElement, 0, 1);
            grid.Add(subtitleLabel, 0, 2);

            Content = grid;

            mediaElement.PositionChanged += OnPositionChanged;
        }

        private async void OnChooseFileClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Videos,
                    PickerTitle = "Pick a video file"
                });

                if (result != null)
                {
                    mediaElement.Source = MediaSource.FromFile(result.FullPath);
                    await mediaElement.Play();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private void OnPositionChanged(object sender, MediaPositionChangedEventArgs e)
        {
            // This is where you would typically update subtitles based on video time
            // For demonstration, we'll just update every 5 seconds
            if ((int)e.Position.TotalSeconds % 5 == 0)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnSubtitleChanged(this, $"Sample subtitle at {(int)e.Position.TotalSeconds} seconds");
                });
            }
        }

        private void OnSubtitleChanged(object sender, string newSubtitle)
        {
            currentSubtitle = newSubtitle;
            subtitleLabel.Text = currentSubtitle;
        }

        private async void OnSubtitleTapped(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentSubtitle))
                return;

            string translatedText = await TranslateText(currentSubtitle, targetLanguage);
            await DisplayAlert("Translation", translatedText, "OK");
        }

        private async Task<string> TranslateText(string text, string targetLanguage)
        {
            // Note: Replace with your preferred translation API
            // This is a placeholder and won't actually translate anything
            await Task.Delay(1000); // Simulate network delay
            return $"Translated: {text} (to {targetLanguage})";
        }
    }

}
