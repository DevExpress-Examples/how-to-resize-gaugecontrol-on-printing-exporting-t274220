using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Printing;

namespace WpfApplication72 {
    public partial class MainWindow : Window {
        double? gaugeWidth;
        double? gaugeHeight;

        public MainWindow() {
            InitializeComponent();
        }

        void Button_Click(object sender, RoutedEventArgs e) {
            SimpleLink sl = new SimpleLink();
            sl.DetailCount = 1;
            sl.DetailTemplate = (DataTemplate)Resources["Data"];
            sl.CreateDetail += new EventHandler<CreateAreaEventArgs>(sl_CreateDetail);
            sl.CreateDocument(true);
            sl.ShowPrintPreviewDialog(this);
        }

        
        bool IsPropertyValueSet(DependencyObject source, DependencyProperty property) {
            return !Object.ReferenceEquals(source.ReadLocalValue(property), DependencyProperty.UnsetValue);
        }
        void SaveControlSize(Control control) {
            gaugeWidth = (double?)GetPropertyValue(control, Control.WidthProperty);
            gaugeHeight =(double?)GetPropertyValue(control, Control.HeightProperty);
        }
        void RestoreControlSize(Control control) {
            SetPropertyValue(control, Control.WidthProperty, gaugeWidth);
            SetPropertyValue(control, Control.HeightProperty, gaugeHeight);
        }
        void SetPropertyValue(DependencyObject target, DependencyProperty property, object value) {
            if (value == null)
                target.ClearValue(property);
            else
                target.SetValue(property, value);
        }
        object GetPropertyValue(DependencyObject source, DependencyProperty property) {
            return IsPropertyValueSet(source, property) ? source.GetValue(property) : null;
        }
        void ResizeControl(Control control, Size newSize) {
            control.Width = newSize.Width;
            control.Height = newSize.Height;
        }
        void MeasureAndArrangeControl(Control control, Size newSize) {
            control.Measure(newSize);
            control.Arrange(new Rect(new Point(0, 0), newSize));
        }
        void PrepareControlLayout(Control control, Size newSize) {
            ResizeControl(control, newSize);
            control.UpdateLayout();
            MeasureAndArrangeControl(control, newSize);
        }
        void RestoreControlLayout(Control control, Size newSize) {
            ResizeControl(control, newSize);
            MeasureAndArrangeControl(control, newSize);
            control.UpdateLayout();
        }
        Size CalculateActualControlSize(Control control) {
            double width = double.IsNaN(control.Width) ? control.ActualWidth : control.Width;
            double height = double.IsNaN(control.Height) ? control.ActualHeight : control.Height;
            return new Size(width, height);
        }

        void sl_CreateDetail(object sender, CreateAreaEventArgs e) {
            Size printPageSize = new Size(500, 500); // It is necessary to specify required image size here
            Size initialGaugeSize = CalculateActualControlSize(gauge);
            SaveControlSize(gauge);
            PrepareControlLayout(gauge, printPageSize);

            VisualBrush brush = new VisualBrush(gauge);
            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();

            context.DrawRectangle(brush, null, new Rect(0, 0, printPageSize.Width, printPageSize.Height));
            context.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)printPageSize.Width, (int)printPageSize.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(visual);
            e.Data = bmp;

            RestoreControlLayout(gauge, initialGaugeSize);
            RestoreControlSize(gauge);
        }
    }
}
