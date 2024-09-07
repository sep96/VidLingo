using Microsoft.Maui.Controls;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Newtonsoft.Json.Linq;

namespace VidLingo
{
    public partial class MainPage : ContentPage
    {
        private WebView videoPlayer;
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

            videoPlayer = new WebView
            {
                Source = new HtmlWebViewSource
                {
                    Html = "<html><body><p>Please choose a video file.</p></body></html>"
                }
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
            grid.Add(videoPlayer, 0, 1);
            grid.Add(subtitleLabel, 0, 2);

            Content = grid;
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
                    var videoPath = result.FullPath;
                    UpdateVideoSource(videoPath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private void UpdateVideoSource(string videoPath)
        {
            var html = $@"
            <html>
                <body style='margin:0;padding:0;'>
                    <video width='100%' height='100%' controls autoplay>
                        <source src='{videoPath}' type='video/mp4'>
                        Your browser does not support the video tag.
                    </video>
                </body>
            </html>";

            videoPlayer.Source = new HtmlWebViewSource { Html = html };

            // Simulating subtitle changes
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                OnSubtitleChanged(this, "This is a sample subtitle");
                return true;
            });
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
