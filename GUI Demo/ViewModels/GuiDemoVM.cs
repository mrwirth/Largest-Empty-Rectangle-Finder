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
                var main = new MainRectangleVM(new Rectangle(0, 0, (int)ic.ActualWidth, (int)ic.ActualHeight));
                var overlay = new OverlayRectangleVM(new Rectangle(1, 5, (int)(ic.ActualWidth/3) , (int)(ic.ActualHeight / 2)));
                //var main = new MainRectangleVM(new Rectangle(0, 0, 800, 600));
                //var overlay = new OverlayRectangleVM(new Rectangle(10, 50, 266, 300));
                var results = LargestEmptyRectangle.Find(main.Rectangle, new List<Rectangle>() { overlay.Rectangle });
                Rectangles.Add(main);
                Rectangles.Add(overlay);
                foreach (var result in results)
                {
                    var rectangle = new ResultRectangleVM(result);
                    Rectangles.Add(rectangle);
                }
            }
        }
        #endregion GenerateSampleCommand
    }
}
