using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using VidLingo.Classes;

namespace VidLingo
{
    public partial class MainPage : ContentPage
    {
        private MediaElement mediaElement;
        private Label subtitleLabel;
        private Label secondarySubtitleLabel; // Additional label for secondary subtitles

        private string currentSubtitle = "";
        private string targetLanguage = "fa"; // Default to Spanish, can be changed
        private List<Subtitle> primarySubtitles = new List<Subtitle>(); // List for the primary subtitles
        private List<Subtitle> secondarySubtitles = new List<Subtitle>(); // List for the secondary subtitles


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

            var chooseVideoButton = new Button
            {
                Text = "Choose Video File",
                WidthRequest = 150, // Set the width to a smaller size
                HeightRequest = 40 // Set the height to a smaller size
            };
            chooseVideoButton.Clicked += OnChooseVideoFileClicked;

            var chooseSubtitleButton = new Button
            {
                Text = "Choose Subtitle File",
                WidthRequest = 150, // Set the width to a smaller size
                HeightRequest = 40 // Set the height to a smaller size
            };
            chooseSubtitleButton.Clicked += OnChooseSubtitleFileClicked;

            mediaElement = new MediaElement
            {
                ShouldAutoPlay = true,
                ShouldShowPlaybackControls = true,
                HorizontalOptions = LayoutOptions.FillAndExpand, // Make the width fill available space
                Aspect = Aspect.AspectFill // Maintain aspect ratio
            };

            subtitleLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Colors.Black
            };
            secondarySubtitleLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                TextColor = Colors.Gray // Different color for the secondary subtitle
            };
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnSubtitleTapped;
            subtitleLabel.GestureRecognizers.Add(tapGestureRecognizer);

            grid.Add(chooseVideoButton, 0, 0);
            grid.Add(chooseSubtitleButton, 1, 0);
            grid.Add(mediaElement, 0, 2);
            grid.Add(subtitleLabel, 0, 3); 
            grid.Add(secondarySubtitleLabel, 0, 4); // Place the secondary label below the main one

            Content = grid;

            // Subscribe to the PositionChanged event
            mediaElement.PositionChanged += OnPositionChanged;
        }


        private async void OnChooseVideoFileClicked(object sender, EventArgs e)
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
                    mediaElement.Play();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnChooseSubtitleFileClicked(object sender, EventArgs e)
        {
            await LoadSubtitlesAsync(primarySubtitles, "Pick the primary subtitle file (SRT)");
            await LoadSubtitlesAsync(secondarySubtitles, "Pick the secondary subtitle file (SRT)");

        }

        private async Task LoadSubtitlesAsync(List<Subtitle> subtitleList, string pickerTitle)
        {
            try
            {
                // Define custom file type for SRT files
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.text" } },
                        { DevicePlatform.Android, new[] { "text/plain" } },
                        { DevicePlatform.WinUI, new[] { ".srt" } },
                        { DevicePlatform.MacCatalyst, new[] { "public.text" } }
                    });

                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = customFileType,
                    PickerTitle = pickerTitle
                });

                if (result != null)
                {
                    string srtContent = await File.ReadAllTextAsync(result.FullPath);
                    subtitleList.Clear();
                    subtitleList.AddRange(ParseSrt(srtContent));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load subtitles: {ex.Message}", "OK");
            }
        }
        private void OnPositionChanged(object sender, EventArgs e)
        {
            var position = mediaElement.Position;

            var primarySub = FindSubtitleForPosition(primarySubtitles, position);
            var secondarySub = FindSubtitleForPosition(secondarySubtitles, position);

            if (primarySub != null && primarySub.Text != currentSubtitle)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnSubtitleChanged(subtitleLabel, primarySub.Text);
                });
            }

            if (secondarySub != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    secondarySubtitleLabel.Text = secondarySub.Text;
                });
            }
        }

        private Subtitle FindSubtitleForPosition(List<Subtitle> subtitleList, TimeSpan position)
        {
            foreach (var subtitle in subtitleList)
            {
                if (position >= subtitle.StartTime && position <= subtitle.EndTime)
                {
                    return subtitle;
                }
            }
            return null;
        }

        private List<Subtitle> ParseSrt(string srtContent)
        {
            var subtitleList = new List<Subtitle>();
            var lines = srtContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            Subtitle currentSubtitle = null;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (currentSubtitle != null)
                    {
                        subtitleList.Add(currentSubtitle);
                        currentSubtitle = null;
                    }
                }
                else if (currentSubtitle == null)
                {
                    currentSubtitle = new Subtitle();
                }
                else if (currentSubtitle.StartTime == TimeSpan.Zero)
                {
                    var times = line.Split(" --> ");
                    if (times.Length == 2)
                    {
                        currentSubtitle.StartTime = TimeSpan.ParseExact(times[0], "hh\\:mm\\:ss\\,fff", CultureInfo.InvariantCulture);
                        currentSubtitle.EndTime = TimeSpan.ParseExact(times[1], "hh\\:mm\\:ss\\,fff", CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    currentSubtitle.Text += line + " ";
                }
            }
            return subtitleList;
        }

        private void OnSubtitleChanged(Label label, string newSubtitle)
        {
            currentSubtitle = newSubtitle;

            var formattedString = new FormattedString();
            var words = newSubtitle.Split(' ');

            foreach (var word in words)
            {
                var span = new Span
                {
                    Text = word + " ",
                    TextColor = Colors.Black
                };

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => OnWordTapped(word);
                span.GestureRecognizers.Add(tapGestureRecognizer);

                formattedString.Spans.Add(span);
            }

            label.FormattedText = formattedString;
        }

        private async void OnWordTapped(string word)
        {
            // Handle the tapped word, e.g., show a translation dialog
            string translatedText = await TranslateText(word, targetLanguage);
            await DisplayAlert("Word Selected", $"You selected: {word}\nTranslation: {translatedText}", "OK");
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
            string translatedText = await Translator.TranslateTextAsync(text, "en", targetLanguage);

            // Note: Replace with your preferred translation API
            return $"Translated: {text} (to {translatedText})";
        }

        // Subtitle class to store subtitle details
        public class Subtitle
        {
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public string Text { get; set; } = string.Empty;
            public string Language { get; set; } = string.Empty; // Add language property
        }

    }
}
