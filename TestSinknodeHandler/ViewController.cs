using System;
using System.Runtime.InteropServices;
using Foundation;
using AudioToolbox;
using AVFoundation;
using UIKit;

namespace TestSinknodeHandler
{
    public partial class ViewController : UIViewController
    {
        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            NSError error;

            var session = AVAudioSession.SharedInstance();
            session.SetCategory(AVAudioSessionCategory.Record);
            session.SetPreferredSampleRate(48000, out error);
            session.SetPreferredInputNumberOfChannels(1, out error);
            session.SetActive(true);

            var engine = new AVAudioEngine();

            var inputNode = engine.InputNode;
            var inputFormat = inputNode.GetBusOutputFormat(0);
            var outputFormat = inputFormat;
            var handler = new AVAudioSinkNodeReceiverHandler(SinkHandler);
            var sinkNode = new AVAudioSinkNode(handler);
            engine.AttachNode(sinkNode);
            engine.Connect(inputNode, sinkNode, inputFormat);
            engine.StartAndReturnError(out error);

        }

        int SinkHandler(AudioTimeStamp ts, uint n, ref AudioBuffers buffers)
        {
            float[] data = new float[n];

            int nCh = buffers.Count;  // Crash, buffers is not a valid reference to AudioBuffers
            for (int i = 0; i < nCh; i++)
            {
                var ptr = buffers[i].Data;
                Marshal.Copy(ptr, data, 0, (int)n);
            }

            return 0;
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}