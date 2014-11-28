// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bing.Speech;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SpeechToTextApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private SpeechRecognizer SR;
        private const string LanguageTag = "en-US";
        private readonly List<string> speechCache = new List<string>(8);
        private SpeechRecognizerAudioCaptureState lastState;

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Inititalize SpeechRecognizer with ADM credentials.
            SR = new SpeechRecognizer(LanguageTag, new SpeechAuthorizationParameters()
            {
                ClientId = "speechToTextApp",
                ClientSecret = "Z6VMUYLWZcUklsEE9PhOpYA2viJJdxikCotQk2YUSaM="
            });
            SR.AudioCaptureStateChanged += SR_AudioCaptureStateChanged;
            SR.RecognizerResultReceived += SR_RecognizerResultReceived;

        }

        /// <summary>
        /// Starts a speech recognition session through the custom UI.
        /// </summary>
        private async void SpeakButton_Click(object sender, RoutedEventArgs e)
        {
            // Always call RecognizeSpeechToTextAsync from inside
            // a try block because it calls a web service.
            try
            {
                while (true)
                {
                    SpeechRecognitionResult result = await SR.RecognizeSpeechToTextAsync();

                    // Write the result to the TextBlock.
                    if (result.TextConfidence != SpeechRecognitionConfidence.Rejected)
                    {
                        speechCache.Add(result.Text);
                        ResultText.Text = string.Join("\n", speechCache);
                    }
                    else
                    {
                        ResultText.Text = "Sorry, I did not get that.";
                    }
                }
            }
            catch (Exception ex)
            {
                // If there's an exception, show the Type and Message.
                ResultText.Text = string.Format("{0}: {1}", ex.GetType(), ex.Message);
            }
        }


        /// <summary>
        /// Update the speech recognition audio capture state.
        /// </summary>
        private async void SR_AudioCaptureStateChanged(SpeechRecognizer sender, SpeechRecognitionAudioCaptureStateChangedEventArgs args)
        {
            Debug.WriteLine(args.State);
            lastState = args.State;
            switch (args.State)
            {
                case SpeechRecognizerAudioCaptureState.Canceled:
                    Status.Text = "Operation cancelled.";
                    break;
                case SpeechRecognizerAudioCaptureState.Cancelling:
                    Status.Text = "Cancelling capture operation...";
                    break;
                case SpeechRecognizerAudioCaptureState.Complete:
                    Status.Text = "Audio capture complete.";
                    break;
                case SpeechRecognizerAudioCaptureState.Initializing:
                    Status.Text = "Initializing audio capture...";
                    break;
                case SpeechRecognizerAudioCaptureState.Listening:
                    Status.Text = "Listening...";
                    break;
                case SpeechRecognizerAudioCaptureState.Thinking:
                    Status.Text = "Interpreting audio input...";
                    break;
                default:
                    Status.Text = "Unknown capture state.";
                    break;
            }
        }

        /// <summary>
        /// A result was received. Whether or not it is intermediary depends on the capture state.
        /// </summary>
        private void SR_RecognizerResultReceived(SpeechRecognizer sender, SpeechRecognitionResultReceivedEventArgs args)
        {
            Debug.WriteLine(args.Text);
            if (args.Text != null)
            {
                if (lastState != SpeechRecognizerAudioCaptureState.Thinking)
                    ResultText.Text = String.Join("\n", speechCache.Concat(Enumerable.Repeat(args.Text, 1)));
            }
        }

        private void StopListeningButton_OnClick(object sender, RoutedEventArgs e)
        {
            //SR.StopListeningAndProcessAudio();
            SR.RequestCancelOperation();
        }
    }
}
