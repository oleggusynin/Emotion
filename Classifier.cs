using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AffdexMe
{
    class Classifier
    {

        static string Facemetric;
        static string HandMetric;
        StreamWorker stream = new StreamWorker();
        static TextBox Log = new TextBox();
        public Classifier(ref TextBox text) { Log = text; }
        public Classifier() { }
        public void GetFaceMetric(string metric)
        {
            Facemetric = metric;
            //stream.write(Facemetric+Environment.NewLine);
        }
        public void GetHandMetric(string metric)
        {
            HandMetric = metric;
            //stream.write("Hand "+HandMetric+Environment.NewLine);
        }

        public void eClassificate()
        {

            if (Facemetric == "Joy" && HandMetric == "Neutral")
            {
                // stream.write("Result emotion: Joy; Hand metric: "+HandMetric+"; Face metric: "+Facemetric); 
                Log.AppendText("Emotion: Joy; Leap: " + HandMetric + "; Wcam: " + Facemetric + Environment.NewLine);
                return;
            }
            if (Facemetric == "Joy" && HandMetric == "Fright")
            {
                //stream.write("Result emotion: Joy; Hand metric: " + HandMetric + "; Face metric: " + Facemetric);
                Log.AppendText("Emotion: Joy; Leap: " + HandMetric + "; Wcam: " + Facemetric + Environment.NewLine);
                return;
            }
            if (Facemetric == "Fear" && HandMetric == "Fright")
            {
                //  stream.write("Result emotion: Fear; Hand metric: " + HandMetric + "; Face metric: " + Facemetric);
                Log.AppendText("Emotion: Fear; Leap: " + HandMetric + "; Wcam: " + Facemetric + Environment.NewLine);
                return;
            }
            if (Facemetric == "Fear" && HandMetric == "Tremor")
            {
                //stream.write("Result emotion: Fear; Hand metric: " + HandMetric + "; Face metric: " + Facemetric);
                Log.AppendText("Emotion: Fear; Leap: " + HandMetric + "; Wcam: " + Facemetric + Environment.NewLine);
                return;
            }
            if (Facemetric == "Fear" && HandMetric == "Neutral")
            {
                //stream.write("Result emotion: Fear; Hand metric: " + HandMetric + "; Face metric: " + Facemetric);
                Log.AppendText("Emotion: Fear; Leap: " + HandMetric + "; Wcam: " + Facemetric + Environment.NewLine);
                return;
            }
            if (Facemetric == "Disgust" && HandMetric == "Neutral")
            {
                // stream.write("Result emotion: Disgust; Hand metric: " + HandMetric + "; Face metric: " + Facemetric);
                Log.AppendText("Emotion: Disgust; Leap: " + HandMetric + "; Wcam: " + Facemetric + Environment.NewLine);
                return;
            }
            if (Facemetric == "Surprise" && HandMetric == "Tremor")
            {
                // stream.write("Result emotion: Fear; Hand metric: " + HandMetric + "; Face metric: " + Facemetric);
                Log.AppendText("Emotion: Fear; Leap: " + HandMetric + "; Wcam: " + Facemetric + Environment.NewLine);
                return;
            }
            if (Facemetric == "Surprise" && HandMetric == "Neutral")
            {
                // stream.write("Result emotion: Fear; Hand metric: " + HandMetric + "; Face metric: " + Facemetric);
                Log.AppendText("Emotion: Surprise; Leap: " + HandMetric + "; Wcam: " + Facemetric + Environment.NewLine);
                return;
            }
        }

    }
}
