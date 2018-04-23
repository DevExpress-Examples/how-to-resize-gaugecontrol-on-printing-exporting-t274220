Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports DevExpress.Xpf.Printing

Namespace WpfApplication72
    Partial Public Class MainWindow
        Inherits Window

        Private gaugeWidth? As Double
        Private gaugeHeight? As Double

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub Button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim sl As New SimpleLink()
            sl.DetailCount = 1
            sl.DetailTemplate = DirectCast(Resources("Data"), DataTemplate)
            AddHandler sl.CreateDetail, AddressOf sl_CreateDetail
            sl.CreateDocument(True)
            sl.ShowPrintPreviewDialog(Me)
        End Sub


        Private Function IsPropertyValueSet(ByVal source As DependencyObject, ByVal [property] As DependencyProperty) As Boolean
            Return Not Object.ReferenceEquals(source.ReadLocalValue([property]), DependencyProperty.UnsetValue)
        End Function
        Private Sub SaveControlSize(ByVal control As Control)
            gaugeWidth = DirectCast(GetPropertyValue(control, System.Windows.Controls.Control.WidthProperty), Double?)
            gaugeHeight =DirectCast(GetPropertyValue(control, System.Windows.Controls.Control.HeightProperty), Double?)
        End Sub
        Private Sub RestoreControlSize(ByVal control As Control)
            SetPropertyValue(control, System.Windows.Controls.Control.WidthProperty, gaugeWidth)
            SetPropertyValue(control, System.Windows.Controls.Control.HeightProperty, gaugeHeight)
        End Sub
        Private Sub SetPropertyValue(ByVal target As DependencyObject, ByVal [property] As DependencyProperty, ByVal value As Object)
            If value Is Nothing Then
                target.ClearValue([property])
            Else
                target.SetValue([property], value)
            End If
        End Sub
        Private Function GetPropertyValue(ByVal source As DependencyObject, ByVal [property] As DependencyProperty) As Object
            Return If(IsPropertyValueSet(source, [property]), source.GetValue([property]), Nothing)
        End Function
        Private Sub ResizeControl(ByVal control As Control, ByVal newSize As Size)
            control.Width = newSize.Width
            control.Height = newSize.Height
        End Sub
        Private Sub MeasureAndArrangeControl(ByVal control As Control, ByVal newSize As Size)
            control.Measure(newSize)
            control.Arrange(New Rect(New Point(0, 0), newSize))
        End Sub
        Private Sub PrepareControlLayout(ByVal control As Control, ByVal newSize As Size)
            ResizeControl(control, newSize)
            control.UpdateLayout()
            MeasureAndArrangeControl(control, newSize)
        End Sub
        Private Sub RestoreControlLayout(ByVal control As Control, ByVal newSize As Size)
            ResizeControl(control, newSize)
            MeasureAndArrangeControl(control, newSize)
            control.UpdateLayout()
        End Sub
        Private Function CalculateActualControlSize(ByVal control As Control) As Size

            Dim width_Renamed As Double = If(Double.IsNaN(control.Width), control.ActualWidth, control.Width)

            Dim height_Renamed As Double = If(Double.IsNaN(control.Height), control.ActualHeight, control.Height)
            Return New Size(width_Renamed, height_Renamed)
        End Function

        Private Sub sl_CreateDetail(ByVal sender As Object, ByVal e As CreateAreaEventArgs)
            Dim printPageSize As New Size(500, 500) ' It is necessary to specify required image size here
            Dim initialGaugeSize As Size = CalculateActualControlSize(gauge)
            SaveControlSize(gauge)
            PrepareControlLayout(gauge, printPageSize)

            Dim brush As New VisualBrush(gauge)
            Dim visual As New DrawingVisual()
            Dim context As DrawingContext = visual.RenderOpen()

            context.DrawRectangle(brush, Nothing, New Rect(0, 0, printPageSize.Width, printPageSize.Height))
            context.Close()

            Dim bmp As New RenderTargetBitmap(CInt(printPageSize.Width), CInt(printPageSize.Height), 96, 96, PixelFormats.Pbgra32)
            bmp.Render(visual)
            e.Data = bmp

            RestoreControlLayout(gauge, initialGaugeSize)
            RestoreControlSize(gauge)
        End Sub
    End Class
End Namespace
