# VidLingo: Cross-Platform Video Player with Subtitles, Word Translation, and Reminders

**VidLingo** is a cross-platform video player built for language learners and movie enthusiasts. Available on **Windows, macOS, and Linux**, **VidLingo** allows users to watch videos with subtitles, click on words for instant translation, and set reminders to review translations later. It's the perfect tool for turning your video-watching experience into a language learning opportunity.

## Features

- **Cross-Platform Support**: Runs on Windows, macOS, and Linux using .NET 6+ and Avalonia or MAUI.
- **Video Playback**: Play, pause, and stop videos with ease.
- **Subtitle Support**: Load subtitles in `.srt` or `.vtt` format and display them in sync with your video.
- **Word Translation**: Click on any word in the subtitles for an instant translation using an external API.
- **Translation Storage**: Save translated words for future reference using SQLite or file-based storage.
- **Reminders**: Set reminders to review tricky words and improve your vocabulary retention over time.

## Screenshots

![Video Player Interface](screenshots/player.png)
![Word Translation Popup](screenshots/translation.png)

## Getting Started

### Prerequisites

- .NET 6.0 or later
- Visual Studio 2022 (or any other IDE supporting .NET development)
- An API key for a translation service (e.g., Google Translate or DeepL)
- SQLite (or another preferred storage method)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/vidlingo.git
   cd vidlingo
