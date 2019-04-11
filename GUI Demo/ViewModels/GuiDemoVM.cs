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

namespace GUI_Demo.ViewModels
{
    public class GuiDemoVM : BaseVM
    {
        public ObservableCollection<BaseRectangleVM> Rectangles { get; } = new ObservableCollection<BaseRectangleVM>();


        #region GenerateSampleCommand
        protected bool CanGenerateSample(object param)
        {
            return true;
        }
        protected void ExecuteGenerateSample(object param)
        {
            DoSampleRun(param);
        }
        private ICommand _generateSample;
        public ICommand GenerateSample
        {
            get
            {
                if (_generateSample == null)
                {
                    _generateSample = new RelayCommand(
                        param => ExecuteGenerateSample(param),
                        param => CanGenerateSample(param)
                        );
                }
                return _generateSample;
            }
        }

        protected void DoSampleRun(object param)
        {
            if (param is ItemsControl ic)
            {
                Rectangles.Clear();
                var rand = new Random();
                // Get canvas dimensions
                int width = (int)ic.ActualWidth;
                int height = (int)ic.ActualHeight;

                // Create 'Main' rectangle
                int mainX = rand.Next(0, 51);
                int mainY = rand.Next(0, 51);
                int mainWidth = rand.Next(100, width-mainX+1);
                int mainHeight = rand.Next(100, height - mainY+1);

                var main = new MainRectangleVM(new Rectangle(mainX, mainY, mainWidth, mainHeight));

                // Create 4 - 12 "Overlay" rectangles
                var count = rand.Next(4, 13);
                var overlays = new List<OverlayRectangleVM>();
                for (int i = 0; i < count; i++)
                {
                    int overlayX = rand.Next(0, width - 5);
                    int overlayY = rand.Next(0, height - 5);
                    int overlayWidth = rand.Next(5, width - overlayX + 1);
                    int overlayHeight = rand.Next(5, height - overlayY + 1);
                    var overlay = new OverlayRectangleVM(new Rectangle(overlayX, overlayY, overlayWidth, overlayHeight));
                    overlays.Add(overlay);
                }

                // Get the "Result" rectangle
                var (results, breakdowns) = LargestEmptyRectangle.Find(main.Rectangle, overlays.Select(o => o.Rectangle));

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
        }
        #endregion GenerateSampleCommand
    }
}
