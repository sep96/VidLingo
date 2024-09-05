# VidLingo: Video Player with Subtitles, Word Translation, and Reminders

**VidLingo** is an innovative video player designed for language learners and movie enthusiasts. With **VidLingo**, you can watch videos with subtitles, click on any word for instant translation, save translations to build your vocabulary, and set reminders to review words later. It's the perfect tool for turning your video-watching experience into a language learning opportunity.

## Features

- **Video Playback**: Play, pause, and stop videos using the WPF `MediaElement`.
- **Subtitle Support**: Load and display subtitles in sync with videos (.srt, .vtt supported).
- **Word Translation**: Click any word in the subtitles to get its translation using an external API (e.g., Google Translate).
- **Translation Storage**: Save translated words for future review using SQLite or file-based storage.
- **Reminders**: Set reminders to revisit saved words and boost your language learning over time.
  
## Screenshots

![Video Player Interface](screenshots/player.png)
![Word Translation Popup](screenshots/translation.png)

## Getting Started

### Prerequisites

- .NET 6.0 or later
- Visual Studio 2022 (or any other IDE supporting .NET development)
- An API key for a translation service (e.g., Google Translate or DeepL)
- SQLite (or any other preferred storage method)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/yourusername/vidlingo.git
   cd vidlingo
