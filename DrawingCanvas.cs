using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Windows.Controls;


namespace AffdexMe
{
    public class Loging
    {

        static TextBox LogOut;
        public Loging(ref TextBox text) { LogOut = text; }
        public void Print(string text)
        {
            try
            {
                LogOut.AppendText(text + Environment.NewLine);

            }
            catch (Exception ex)
            {
                LogOut.AppendText(ex.Message + Environment.NewLine);

            }
        }




    }

    public class DrawingCanvas : Canvas
    {
        // StreamWorker Stream = new StreamWorker();
        Classifier classifier = new Classifier();

        public DrawingCanvas()
        {


            DrawMetrics = true;

            DrawAppearance = true;
            DrawEmojis = true;
            boundingBrush = new SolidColorBrush(Colors.LightGray);
            emojiBrush = new SolidColorBrush(Colors.Black);
            pozMetricBrush = new SolidColorBrush(Colors.LimeGreen);
            negMetricBrush = new SolidColorBrush(Colors.Red);
            boundingPen = new Pen(boundingBrush, 1);

            NameToResourceConverter conv = new NameToResourceConverter();
            metricTypeFace = Fonts.GetTypefaces((Uri)conv.Convert("Square", null, "ttf", null)).First();

            Faces = new Dictionary<int, Affdex.Face>();
            emojiImages = new Dictionary<Affdex.Emoji, BitmapImage>();
            appImgs = new Dictionary<string, BitmapImage>();
            MetricNames = new StringCollection();
            upperConverter = new UpperCaseConverter();
            maxTxtWidth = 0;
            maxTxtHeight = 0;

            var emojis = Enum.GetValues(typeof(Affdex.Emoji));
            foreach (int emojiVal in emojis)
            {
                BitmapImage img = loadImage(emojiVal.ToString());
                emojiImages.Add((Affdex.Emoji)emojiVal, img);
            }

            var gender = Enum.GetValues(typeof(Affdex.Gender));
            foreach (int genderVal in gender)
            {
                for (int g = 0; g <= 1; g++)
                {
                    string name = ConcatInt(genderVal, g);
                    BitmapImage img = loadImage(name);
                    appImgs.Add(name, img);
                }

            }


        }


        protected override void OnRender(DrawingContext dc)
        {

            foreach (KeyValuePair<int, Affdex.Face> pair in Faces)
            {

                Affdex.Face face = pair.Value;

                var featurePoints = face.FeaturePoints;


                System.Windows.Point tl = new System.Windows.Point(featurePoints.Min(r => r.X) * XScale,
                                                   featurePoints.Min(r => r.Y) * YScale);
                System.Windows.Point br = new System.Windows.Point(featurePoints.Max(r => r.X) * XScale,
                                                                   featurePoints.Max(r => r.Y) * YScale);

                System.Windows.Point bl = new System.Windows.Point(tl.X, br.Y);

                if (DrawMetrics)
                {
                    double padding = (bl.Y - tl.Y) / MetricNames.Count;
                    double startY = tl.Y - padding;
                    foreach (string metric in MetricNames)
                    {
                        double width = maxTxtWidth;
                        double height = maxTxtHeight;
                        float value = -1;
                        PropertyInfo info;
                        if ((info = face.Expressions.GetType().GetProperty(NameMappings(metric))) != null)
                        {
                            value = (float)info.GetValue(face.Expressions, null);

                            //Stream.write("1- "+value.ToString());



                        }
                        else if ((info = face.Emotions.GetType().GetProperty(NameMappings(metric))) != null)
                        {
                            value = (float)info.GetValue(face.Emotions, null);
                            //if(face.Emotions.Joy >50)
                            // Stream.writeTrunc(face.Emotions.Joy+" "+value.ToString());

                        }



                        SolidColorBrush metricBrush = value > 0 ? pozMetricBrush : negMetricBrush;
                        value = Math.Abs(value);
                        SolidColorBrush txtBrush = value > 1 ? emojiBrush : boundingBrush;

                        double x = tl.X - width - margin;
                        double y = startY += padding;
                        double valBarWidth = width * (value / 100);
                        if (value >= 50) { //classifier.GetFaceMetric(metric);// classifier.eClassificate();
                        }
                        if (value > 1) { dc.DrawRectangle(null, boundingPen, new System.Windows.Rect(x, y, width, height)); }
                        dc.DrawRectangle(metricBrush, null, new System.Windows.Rect(x, y, valBarWidth, height));

                        FormattedText metricFTScaled = new FormattedText((String)upperConverter.Convert(metric, null, null, null),
                                                                System.Globalization.CultureInfo.CurrentCulture,
                                                                System.Windows.FlowDirection.LeftToRight,
                                                                metricTypeFace, metricFontSize * width / maxTxtWidth, txtBrush);

                        dc.DrawText(metricFTScaled, new System.Windows.Point(x, y));
                    }
                }

            }




            //base.OnRender(dc);
        }

        public String NameMappings(String classifierName)
        {
            if (classifierName == "Frown")
            {
                return "LipCornerDepressor";
            }
            return classifierName;
        }

        public Dictionary<int, Affdex.Face> Faces { get; set; }
        public StringCollection MetricNames
        {
            get
            {
                return metricNames;
            }
            set
            {
                metricNames = value;
                Dictionary<string, FormattedText> txtArray = new Dictionary<string, FormattedText>();

                foreach (string metric in metricNames)
                {
                    FormattedText metricFT = new FormattedText((String)upperConverter.Convert(metric, null, null, null),
                                                            System.Globalization.CultureInfo.CurrentCulture,
                                                            System.Windows.FlowDirection.RightToLeft,
                                                            metricTypeFace, metricFontSize, emojiBrush);
                    txtArray.Add(metric, metricFT);

                }

                if (txtArray.Count > 0)
                {
                    maxTxtWidth = txtArray.Max(r => r.Value.Width);
                    maxTxtHeight = txtArray.Max(r => r.Value.Height);
                }
            }
        }
        public double XScale { get; set; }
        public double YScale { get; set; }

        public bool DrawMetrics { get; set; }
        public bool DrawPoints { get; set; }
        public bool DrawEmojis { get; set; }
        public bool DrawAppearance { get; set; }

        private BitmapImage loadImage(string name, string extension = "png")
        {
            NameToResourceConverter conv = new NameToResourceConverter();
            var pngURI = conv.Convert(name, null, extension, null);
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = (Uri)pngURI;
            img.EndInit();
            return img;
        }

        private string ConcatInt(int x, int y)
        {
            return String.Format("{0}{1}", x, y);
        }

        private const int metricFontSize = 14;
        private const int fpRadius = 2;
        private const int margin = 5;
        private double maxTxtWidth;
        private double maxTxtHeight;
        private Typeface metricTypeFace;
        private SolidColorBrush boundingBrush;
        private SolidColorBrush pozMetricBrush;
        private SolidColorBrush negMetricBrush;
        private SolidColorBrush pointBrush;
        private SolidColorBrush emojiBrush;
        private Pen boundingPen;
        private UpperCaseConverter upperConverter;
        private StringCollection metricNames;
        private Dictionary<Affdex.Emoji, BitmapImage> emojiImages;
        private Dictionary<string, BitmapImage> appImgs;
    }
}
