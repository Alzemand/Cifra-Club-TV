using AppStudio.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AppStudio.Views
{
    public sealed partial class MPBSambaBossaNovaDetail : Page
    {
        private DataTransferManager _dataTransferManager;

        private NavigationHelper _navigationHelper;

        private DisplayOrientations _currentOrientations;

        public MPBSambaBossaNovaDetail()
        {
            this.InitializeComponent();
            _navigationHelper = new NavigationHelper(this);

            ytViewer.NavigationHelper = _navigationHelper;

            MPBSambaBossaNovaModel = new MPBSambaBossaNovaViewModel();

            DataContext = this;

            ApplicationView.GetForCurrentView().
                SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
        }

        public MPBSambaBossaNovaViewModel MPBSambaBossaNovaModel { get; private set; }

        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _dataTransferManager = DataTransferManager.GetForCurrentView();
            _dataTransferManager.DataRequested += OnDataRequested;

            _navigationHelper.OnNavigatedTo(e);

            if (MPBSambaBossaNovaModel != null)
            {
                await MPBSambaBossaNovaModel.LoadItemsAsync();
                MPBSambaBossaNovaModel.SelectItem(e.Parameter);

                MPBSambaBossaNovaModel.ViewType = ViewTypes.Detail;
            }

            ytViewer.OnNavigatedTo();

            // Allow this page to rotate
            _currentOrientations = DisplayInformation.AutoRotationPreferences;
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait
                                                        | DisplayOrientations.Landscape
                                                        | DisplayOrientations.LandscapeFlipped
                                                        | DisplayOrientations.PortraitFlipped;


            DisplayInformation.GetForCurrentView().OrientationChanged += this.OnOrientationChanged;
            this.TransitionStoryboardState();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
            
            ytViewer.EmbedUrl = null;

            ytViewer.OnNavigatedFrom();

            // Restore previous rotation preferences
            DisplayInformation.AutoRotationPreferences = _currentOrientations;
            DisplayInformation.GetForCurrentView().OrientationChanged -= this.OnOrientationChanged;
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (MPBSambaBossaNovaModel != null)
            {
                MPBSambaBossaNovaModel.GetShareContent(args.Request);
            }
        }

        private void OnOrientationChanged(DisplayInformation sender, object args)
        {
            this.TransitionStoryboardState();
        }

        private async void TransitionStoryboardState()
        {
            StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            switch (DisplayInformation.GetForCurrentView().CurrentOrientation)
            {
                case DisplayOrientations.Landscape:
                case DisplayOrientations.LandscapeFlipped:
                    AppBar.Visibility = Visibility.Collapsed;
                    statusBar.HideAsync();
                    break;
                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped:
                default:
                    AppBar.Visibility = Visibility.Visible;
                    statusBar.ShowAsync();
                    break;
            }
        }
    }
}
