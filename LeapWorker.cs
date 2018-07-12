using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Leap;
namespace AffdexMe
{
    class LeapWorker
    {
        TextBox LogOut;
        string[] Emotions = { "Tremor", "Fright", "Neutral" };
        Classifier classifier = new Classifier();
        public LeapWorker(ref TextBox text) { LogOut = text; }
        public void OnServiceConnect(object sender, ConnectionEventArgs args)
        {
            //  Console.WriteLine("Service Connected");
        }

        public void OnConnect(object sender, DeviceEventArgs args)
        {
            LogOut.AppendText(Environment.NewLine + "Controller has been connected.." + Environment.NewLine);
        }

        public void OnFrame(object sender, FrameEventArgs args)
        {
            // Console.WriteLine("Frame Available.");

            Leap.Frame frame = args.frame;

            // Console.WriteLine(
            //     "Frame id: {0}, timestamp: {1}, hands: {2}",
            //    frame.Id, frame.Timestamp, frame.Hands.Count
            // );
            //LogOut.AppendText("hCount-"+frame.Hands.Count);

            foreach (Hand hand in frame.Hands)
            {
                //    Console.WriteLine("  Hand id: {0}, palm position: {1}, fingers: {2}",
                //    hand.Id, hand.PalmPosition, hand.Fingers.Count);
                // Get the hand's normal vector and direction
                Leap.Vector normal = hand.PalmNormal;

                Leap.Vector direction = hand.Direction;
                // Console.WriteLine(Fright(hand.PalmVelocity, 500));
                // LogOut.AppendText(" hVel-"+hand.PalmVelocity+" hDir-"+hand.PalmPosition);
                LogOut.AppendText(hand.PalmNormal.+Environment.NewLine);

                classifier.GetHandMetric(Tremor(hand.PalmVelocity, hand, 500));
                classifier.eClassificate();


                // Console.WriteLine(
                //  "  Hand pitch: {0} degrees, roll: {1} degrees, yaw: {2} degrees",
                //   direction.Pitch * 180.0f / (float)Math.PI,
                //  normal.Roll * 180.0f / (float)Math.PI,
                //   direction.Yaw * 180.0f / (float)Math.PI
                // );
                //LogOut.AppendText(hand.Fingers.Count + " " +direction.Pitch*180.0f / (float)Math.PI+
                //    " "+ normal.Roll * 180.0f / (float)Math.PI+" "+ direction.Yaw * 180.0f / (float)Math.PI);
            }
            //  LogOut.AppendText(Environment.NewLine);
            LogOut.Focus();
            LogOut.CaretIndex = LogOut.Text.Length;
            LogOut.ScrollToEnd();
        }

        public string Fright(Leap.Vector xyz, int Velocity) // Hand movements on xyz directions
        {
            // Console.WriteLine("X velocity"+xyz[0]+" Y velocity"+xyz[1]+"  Z velocity"+xyz[2]);
            if (xyz[0] > Velocity || xyz[1] > Velocity || xyz[2] >= Velocity) return Emotions[4];
            else return Emotions[6];
        }

        public string Tremor(Leap.Vector v, Hand hand, int Velocity) // recognize tremor
        {
            //Console.WriteLine("x "+v[0]+" y "+v[1]+" z "+v[2] + " Angle " +hand.Rotation);
            // Console.WriteLine(xyz.Magnitude);
            /*
               Console.WriteLine(
                "  Hand pitch: {0} degrees, roll: {1} degrees, yaw: {2} degrees, x {3} y {4} z{5}",
                 xyz.Pitch * 180.0f / (float)Math.PI,
                xyz.Roll * 180.0f / (float)Math.PI,
                 xyz.Yaw * 180.0f / (float)Math.PI,
                 v.x,v.y,v.z
               );
               */
            if (Math.Abs(v[0]) >= 10 && Math.Abs(v[1]) <= 20)
            {

                return Emotions[0];
            }
            else if (Math.Abs(v[0]) > Velocity || Math.Abs(v[1]) > Velocity || Math.Abs(v[2]) >= Velocity) return Emotions[1];
            else if (v[0] >= -2 || v[0] <= 2 && v.y >= -5 || v.y <= 5)
            {

                return Emotions[2];
            }
            else return Emotions[2];

        }
    }
}
