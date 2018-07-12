using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Shapes;
using System.IO;
using Affdex;
using Leap;
namespace AffdexMe
{

    public partial class MainWindow : Window, ImageListener, ProcessStatusListener
    {

        Controller controller;
        LeapWorker listener;
        Classifier cl;
        public MainWindow()
        {
            InitializeComponent();
            CenterWindowOnScreen();
            Log.AppendText(" Welcome to EmotionCam!");
            Log.Visibility = Visibility.Hidden;
            cl = new Classifier(ref Log);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Detector = null;



            EnabledClassifiers = Settings.Default.Classifiers;
            canvas.MetricNames = EnabledClassifiers;


            btnStopCamera.IsEnabled =
            btnExit.IsEnabled = true;

            btnStartCamera.IsEnabled =
            btnResetCamera.IsEnabled =

            Metrics.IsEnabled =

            btnResetCamera.IsEnabled =
            btnAppShot.IsEnabled =
            btnStopCamera.IsEnabled = false;
            
            btnStartCamera.Click += btnStartCamera_Click;
            btnStopCamera.Click += btnStopCamera_Click;

            Metrics.Click += Metrics_Click;
            btnResetCamera.Click += btnResetCamera_Click;
            btnExit.Click += btnExit_Click;
            btnAppShot.Click += btnAppShot_Click;

            ShowEmojis = canvas.DrawEmojis = AffdexMe.Settings.Default.ShowEmojis;
            ShowAppearance = canvas.DrawAppearance = AffdexMe.Settings.Default.ShowAppearance;

            ShowMetrics = canvas.DrawMetrics = AffdexMe.Settings.Default.ShowMetrics;

            changeButtonStyle(Metrics, AffdexMe.Settings.Default.ShowMetrics);

            this.ContentRendered += MainWindow_ContentRendered;
        }


        void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            StartCameraProcessing();
        }


        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }



        public void onImageResults(Dictionary<int, Affdex.Face> faces, Affdex.Frame image)
        {
            DrawData(image, faces);
        }

        public void onImageCapture(Affdex.Frame image)
        {
            DrawCapturedImage(image);

        }

        public void onProcessingException(Affdex.AffdexException ex)
        {
            String message = String.IsNullOrEmpty(ex.Message) ? "error encountered." : ex.Message;
            ShowExceptionAndShutDown(message);
        }

        public void onProcessingFinished() { }


        private void ShowExceptionAndShutDown(String exceptionMessage)
        {
            MessageBoxResult result = MessageBox.Show(exceptionMessage,
                                                        "Error",
                                                        MessageBoxButton.OK,
                                                        MessageBoxImage.Error);
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                StopCameraProcessing();
                ResetDisplayArea();
            }));
        }

        private BitmapSource ConstructImage(byte[] imageData, int width, int height)
        {
            try
            {
                if (imageData != null && imageData.Length > 0)
                {
                    var stride = (width * PixelFormats.Bgr24.BitsPerPixel + 7) / 8;
                    var imageSrc = BitmapSource.Create(width, height, 96d, 96d, PixelFormats.Bgr24, null, imageData, stride);
                    return imageSrc;
                }
            }
            catch (Exception ex)
            {
                String message = String.IsNullOrEmpty(ex.Message) ? "AffdexMe error encountered." : ex.Message;
                ShowExceptionAndShutDown(message);
            }

            return null;
        }

        private void DrawData(Affdex.Frame image, Dictionary<int, Affdex.Face> faces)
        {
            try
            {

                if (faces != null)
                {
                    var result = this.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if ((Detector != null) && (Detector.isRunning()))
                        {
                            canvas.Faces = faces;
                            canvas.Width = cameraDisplay.ActualWidth;
                            canvas.Height = cameraDisplay.ActualHeight;
                            canvas.XScale = canvas.Width / image.getWidth();
                            canvas.YScale = canvas.Height / image.getHeight();
                            canvas.InvalidateVisual();
                            DrawSkipCount = 0;

                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                String message = String.IsNullOrEmpty(ex.Message) ? "AffdexMe error encountered." : ex.Message;
                ShowExceptionAndShutDown(message);
            }
        }


        private void DrawCapturedImage(Affdex.Frame image)
        {

            var result = this.Dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {

                    cameraDisplay.Source = ConstructImage(image.getBGRByteArray(), image.getWidth(), image.getHeight());

                    if (++DrawSkipCount > 4)
                    {
                        canvas.Faces = new Dictionary<int, Affdex.Face>();
                        canvas.InvalidateVisual();
                        DrawSkipCount = 0;
                    }

                    if (image != null)
                    {
                        image.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    String message = String.IsNullOrEmpty(ex.Message) ? "AffdexMe error encountered." : ex.Message;
                    ShowExceptionAndShutDown(message);
                }
            }));
        }


        void SaveSettings()
        {
      
          //  Settings.Default.ShowAppearance = ShowAppearance;
           // Settings.Default.ShowEmojis = ShowEmojis;
            Settings.Default.ShowMetrics = ShowMetrics;
            Settings.Default.Classifiers = EnabledClassifiers;
            Settings.Default.Save();
        }

        private void ResetDisplayArea()
        {
            try
            {


                canvas.Faces = new Dictionary<int, Face>();
                canvas.InvalidateVisual();




                Metrics.IsEnabled = true;

            }
            catch (Exception ex)
            {
                String message = String.IsNullOrEmpty(ex.Message) ? "error encountered." : ex.Message;
                ShowExceptionAndShutDown(message);
            }
        }


        private void TurnOnClassifiers()
        {
            Detector.setDetectAllEmotions(false);
            Detector.setDetectAllExpressions(false);
            Detector.setDetectAllEmojis(true);
            Detector.setDetectGender(true);
            Detector.setDetectGlasses(true);
            foreach (String metric in EnabledClassifiers)
            {
                MethodInfo setMethodInfo = Detector.GetType().GetMethod(String.Format("setDetect{0}", canvas.NameMappings(metric)));
                setMethodInfo.Invoke(Detector, new object[] { true });
            }
        }


        private void StartCameraProcessing()
        {
            try
            {
                btnStartCamera.IsEnabled = false;
                btnResetCamera.IsEnabled =
                Metrics.IsEnabled =
                btnStopCamera.IsEnabled =
                btnAppShot.IsEnabled =
                btnExit.IsEnabled = true;


                const int cameraId = 0;
                const int numberOfFaces = 1;
                const int cameraFPS = 15;
                const int processFPS = 15;
                Detector = new CameraDetector(cameraId, cameraFPS, processFPS, numberOfFaces, Affdex.FaceDetectorMode.LARGE_FACES);
                
                Detector.setClassifierPath(FilePath.GetClassifierDataFolder());

 
                TurnOnClassifiers();

                Detector.setImageListener(this);
                Detector.setProcessStatusListener(this);

                Detector.start();

                canvas.Visibility = Visibility.Visible;
                cameraDisplay.Visibility = Visibility.Visible;
            }
            catch (AffdexException ex)
            {
                if (!String.IsNullOrEmpty(ex.Message))
                {

                    if (ex.Message.Equals("Unable to open webcam."))
                    {
                        MessageBoxResult result = MessageBox.Show(ex.Message,
                                                                "Error",
                                                                MessageBoxButton.OK,
                                                                MessageBoxImage.Error);
                        StopCameraProcessing();
                        return;
                    }
                }

                String message = String.IsNullOrEmpty(ex.Message) ? "AffdexMe error encountered." : ex.Message;
                ShowExceptionAndShutDown(message);
            }
            catch (Exception ex)
            {
                String message = String.IsNullOrEmpty(ex.Message) ? "error encountered." : ex.Message;
                ShowExceptionAndShutDown(message);
            }
        }

        private void ResetCameraProcessing()
        {
            try
            {
                Detector.reset();
            }
            catch (Exception ex)
            {
                String message = String.IsNullOrEmpty(ex.Message) ? "error encountered." : ex.Message;
                ShowExceptionAndShutDown(message);
            }
        }


        private void StopCameraProcessing()
        {
            try
            {
                if ((Detector != null) && (Detector.isRunning()))
                {
                    Detector.stop();
                    Detector.Dispose();
                    Detector = null;
                }


                btnStartCamera.IsEnabled = true;
                btnResetCamera.IsEnabled =
                btnAppShot.IsEnabled =
                btnStopCamera.IsEnabled = false;

            }
            catch (Exception ex)
            {
                String message = String.IsNullOrEmpty(ex.Message) ? "error encountered." : ex.Message;
                ShowExceptionAndShutDown(message);
            }
        }


        private void changeButtonStyle(Button button, bool isOn)
        {
            Style style;
            String buttonText = String.Empty;

            if (isOn)
            {
                style = this.FindResource("PointsOnButtonStyle") as Style;
                buttonText = "Hide " + button.Name;
            }
            else
            {
                style = this.FindResource("CustomButtonStyle") as Style;
                buttonText = "Show " + button.Name;
            }
            button.Style = style;
            button.Content = buttonText;
        }


        private void TakeScreenShot(String fileName)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(this);
            double dpi = 96d;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height,
                                                                       dpi, dpi, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(this);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            renderBitmap.Render(dv);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(fileName))
            {
                encoder.Save(file);
            }

            appShotLocLabel.Content = String.Format("Screenshot saved to: {0}", fileName);
            ((System.Windows.Media.Animation.Storyboard)FindResource("autoFade")).Begin(appShotLocLabel);
        }


        private void btnAppShot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                String fileName = String.Format("ScreenShot {0:MMMM dd yyyy h mm ss}.png", DateTime.Now);
                fileName = System.IO.Path.Combine(picturesFolder, fileName);
                this.TakeScreenShot(fileName);
            }
            catch (Exception ex)
            {
                String message = String.Format("Error encountered while trying to take a screenshot, details={0}", ex.Message);
                ShowExceptionAndShutDown(message);
            }
        }


        private void Metrics_Click(object sender, RoutedEventArgs e)
        {
            ShowMetrics = !ShowMetrics;
            canvas.DrawMetrics = ShowMetrics;
            changeButtonStyle((Button)sender, ShowMetrics);
        }




        private void btnResetCamera_Click(object sender, RoutedEventArgs e)
        {
            ResetCameraProcessing();
        }


        private void btnStartCamera_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                StartCameraProcessing();
            }
            catch (Exception ex)
            {
                String message = String.IsNullOrEmpty(ex.Message) ? " error encountered." : ex.Message;
                ShowExceptionAndShutDown(message);
            }
        }

        private void btnStopCamera_Click(object sender, RoutedEventArgs e)
        {
            StopCameraProcessing();
            ResetDisplayArea();
        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopCameraProcessing();
            SaveSettings();
            Application.Current.Shutdown();
        }

        private void btnChooseWin_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                controller = new Controller();

                listener = new LeapWorker(ref Log);
                Log.Visibility = Visibility.Visible;


                controller.Connect += listener.OnServiceConnect;
                controller.Device += listener.OnConnect;
                controller.FrameReady += listener.OnFrame;

            }
            catch (Exception ex) { Log.AppendText(ex.Message + Environment.NewLine); }
        }

        private int DrawSkipCount { get; set; }

        private Detector Detector { get; set; }


        private StringCollection EnabledClassifiers { get; set; }

        private bool ShowFacePoints { get; set; }
        private bool ShowAppearance { get; set; }
        private bool ShowEmojis { get; set; }
        private bool ShowMetrics { get; set; }
    }

    ///
    ///
    ///




}
