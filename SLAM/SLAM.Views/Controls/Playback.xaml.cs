using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SLAM.Views.Controls {

    public partial class Playback : Border {

        static Playback() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Playback), new FrameworkPropertyMetadata(typeof(Playback)));

            var minimumPropertyMetadata = new FrameworkPropertyMetadata();
            var valuePropertyMetadata = new FrameworkPropertyMetadata();
            var maximumPropertyMetadata = new FrameworkPropertyMetadata();

            var playCommandPropertyMetadata = new FrameworkPropertyMetadata();
            var pauseCommandPropertyMetadata = new FrameworkPropertyMetadata();
            var stopCommandPropertyMetadata = new FrameworkPropertyMetadata();

            var previousFrameCommandPropertyMetadata = new FrameworkPropertyMetadata();
            var nextFrameCommandPropertyMetadata = new FrameworkPropertyMetadata();

            MinimumProperty =
                DependencyProperty.Register(
                    "Minimum", typeof(Color[,]), typeof(Playback), minimumPropertyMetadata);
            ValueProperty =
                DependencyProperty.Register(
                    "Value", typeof(Color[,]), typeof(Playback), valuePropertyMetadata);
            MaximumProperty =
                DependencyProperty.Register(
                    "Maximum", typeof(Color[,]), typeof(Playback), maximumPropertyMetadata);
            
        }

        public Playback() {
            InitializeComponent();
        }

        //private enum State { Played, Paused, Stopped }

        public static readonly DependencyProperty MinimumProperty;
        public static readonly DependencyProperty ValueProperty;
        public static readonly DependencyProperty MaximumProperty;

        public static readonly DependencyProperty PlayCommandProperty;
        public static readonly DependencyProperty PauseCommandProperty;
        public static readonly DependencyProperty StopCommandProperty;

        public static readonly DependencyProperty PreviousFrameCommandProperty;
        public static readonly DependencyProperty NextFrameCommandProperty;

        public int Minimum {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public int Value {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public int Maximum {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public ICommand PlayCommand {
            get { return (ICommand)GetValue(PlayCommandProperty); }
            set { SetValue(PlayCommandProperty, value); }
        }
        public ICommand PauseCommand {
            get { return (ICommand)GetValue(PauseCommandProperty); }
            set { SetValue(PauseCommandProperty, value); }
        }
        public ICommand StopCommand {
            get { return (ICommand)GetValue(StopCommandProperty); }
            set { SetValue(StopCommandProperty, value); }
        }

        public ICommand PreviousFrameCommand {
            get { return (ICommand)GetValue(PreviousFrameCommandProperty); }
            set { SetValue(PreviousFrameCommandProperty, value); }
        }
        public ICommand NextFrameCommand {
            get { return (ICommand)GetValue(NextFrameCommandProperty); }
            set { SetValue(NextFrameCommandProperty, value); }
        }

        
    }
}