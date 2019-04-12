using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using LERF_Library;
using System.Diagnostics;

namespace GUI_Demo.ViewModels
{
    public class GuiDemoVM : BaseVM
    {
        #region Overlay Randomizer Variables
        private int _minimumOverlayCount = 4;
        public int MinimumOverlayCount
        {
            get => _minimumOverlayCount;
            set
            {
                _minimumOverlayCount = value;
                OnPropertyChanged("MinimumOverlayCount");
            }
        }

        private int _maximumOverlayCount = 12;
        public int MaximumOverlayCount
        {
            get => _maximumOverlayCount;
            set
            {
                _maximumOverlayCount = value;
                OnPropertyChanged("MaximumOverlayCount");
            }
        }

        private int _minimumOverlayWidth = 5;
        public int MinimumOverlayWidth
        {
            get => _minimumOverlayWidth;
            set
            {
                _minimumOverlayWidth = value;
                OnPropertyChanged("MinimumOverlayWidth");
            }
        }

        private int _maximumOverlayWidth = 100;
        public int MaximumOverlayWidth
        {
            get => _maximumOverlayWidth;
            set
            {
                _maximumOverlayWidth = value;
                OnPropertyChanged("MaximumOverlayWidth");
            }
        }

        private int _minimumOverlayHeight = 5;
        public int MinimumOverlayHeight
        {
            get => _minimumOverlayHeight;
            set
            {
                _minimumOverlayHeight = value;
                OnPropertyChanged("MinimumOverlayHeight");
            }
        }

        private int _maximumOverlayHeight = 100;
        public int MaximumOverlayHeight
        {
            get => _maximumOverlayHeight;
            set
            {
                _maximumOverlayHeight = value;
                OnPropertyChanged("MaximumOverlayHeight");
            }
        }
        #endregion Overlay Randomizer Variables

        #region Display Data
        public ObservableCollection<BaseRectangleVM> Rectangles { get; } = new ObservableCollection<BaseRectangleVM>();
        private TimeSpan _findTime;
        public TimeSpan FindTime {
            get => _findTime;
            private set
            {
                _findTime = value;
                OnPropertyChanged("FindTime");
            }
        }
        #endregion Display Data

        #region Local State
        private bool Working = false;
        #endregion Local State

        #region GenerateSampleCommand
        protected bool CanGenerateSample(object param)
        {
            return !Working;
        }
        protected async Task ExecuteGenerateSample(object param)
        {
            Working = true;
            await DoSampleRun(param);
            Working = false;
        }
        private ICommand _generateSample;
        public ICommand GenerateSample
        {
            get
            {
                if (_generateSample == null)
                {
                    _generateSample = new RelayCommandAsync(
                        param => ExecuteGenerateSample(param),
                        param => CanGenerateSample(param)
                        );
                }
                return _generateSample;
            }
        }

        protected async Task DoSampleRun(object param)
        {
            Rectangles.Clear();
            var rand = new Random();

            int width;
            int height;
            // Set canvas width and height from param if possible,
            // else just assume it.
            if (param is ItemsControl ic)
            {
                // Get canvas dimensions
                width = (int)ic.ActualWidth;
                height = (int)ic.ActualHeight;
            }
            else
            {
                width = 800;
                height = 600;
            }

            // Create 'Main' rectangle
            int mainX = rand.Next(0, 51);
            int mainY = rand.Next(0, 51);
            int mainWidth = rand.Next(100, width - mainX + 1);
            int mainHeight = rand.Next(100, height - mainY + 1);

            var main = new MainRectangleVM(new Rectangle(mainX, mainY, mainWidth, mainHeight));

            // Create 'Overlay' rectangles
            var count = rand.Next(MinimumOverlayCount, MaximumOverlayCount + 1);
            var overlays = new List<OverlayRectangleVM>();
            for (int i = 0; i < count; i++)
            {
                int overlayWidth = rand.Next(MinimumOverlayWidth, MaximumOverlayWidth + 1);
                int overlayHeight = rand.Next(MinimumOverlayHeight, MaximumOverlayHeight + 1);
                int overlayX = rand.Next(0, width - overlayWidth);
                int overlayY = rand.Next(0, height - overlayHeight);
                var overlay = new OverlayRectangleVM(new Rectangle(overlayX, overlayY, overlayWidth, overlayHeight));
                overlays.Add(overlay);
            }

            // Get the "Result" rectangle
            var sw = new Stopwatch();
            sw.Start();
            var (results, breakdowns) = await Task.Run(() => LargestEmptyRectangle.Find(main.Rectangle, overlays.Select(o => o.Rectangle)));
            sw.Stop();
            FindTime = sw.Elapsed;

            // Add the rectangles to the VM collection
            Rectangles.Add(main);
            foreach (var overlay in overlays)
            {
                Rectangles.Add(overlay);
            }
            foreach (var result in results)
            {
                var rectangle = new ResultRectangleVM(result);
                Rectangles.Add(rectangle);
            }
            foreach (var breakdown in breakdowns)
            {
                var rectangle = new ExtentRectangleVM(breakdown);
                Rectangles.Add(rectangle);
            }
        }
        #endregion GenerateSampleCommand
    }
}
