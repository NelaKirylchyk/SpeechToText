using System;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SpeechToTextApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly SpeechRecognizer _speechRecognizer;

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            _speechRecognizer = new SpeechRecognizer();
            _speechRecognizer.StateChanged += SpeechRecognizerOnStateChanged;
            _speechRecognizer.RecognitionQualityDegrading += SpeechRecognizerOnRecognitionQualityDegrading;

            // Create an instance of SpeechRecognizer.
            var topicConstraint = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.WebSearch, "webSearch");
            _speechRecognizer.Constraints.Add(topicConstraint);

            // Compile the dictation grammar by default.
            _speechRecognizer.CompileConstraintsAsync();
        }

        private async void SpeechRecognizerOnRecognitionQualityDegrading(SpeechRecognizer sender, SpeechRecognitionQualityDegradingEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(
                       () =>
                       {
                           Problem.Text = args.Problem.ToString();
                       }));
        }

        private async void SpeechRecognizerOnStateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(
                    () =>
                    {
                        Status.Text = args.State.ToString();
                    }));
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            // Start recognition.
            IAsyncOperation<SpeechRecognitionResult> speechRecognitionResult = _speechRecognizer.RecognizeAsync();
            //IAsyncOperation<SpeechRecognitionResult> speechRecognitionResult = _speechRecognizer.RecognizeWithUIAsync();
            speechRecognitionResult.Completed += Completed;
        }

        private async void Completed(IAsyncOperation<SpeechRecognitionResult> asyncInfo, AsyncStatus asyncStatus)
        {
            try
            {
                var results = asyncInfo.GetResults();

                if (results.Confidence != SpeechRecognitionConfidence.Rejected)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(
                        () =>
                        {
                            Result.Text = results.Text;
                        }));
                    //var messageDialog = new Windows.UI.Popups.MessageDialog(results.Text, "Text spoken");
                    //await messageDialog.ShowAsync();
                }
                else
                {
                    Result.Text = "Sorry, I did not get that.";
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
