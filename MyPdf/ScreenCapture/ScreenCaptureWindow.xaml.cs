using System.Drawing; // For screen capturing (Bitmap, Graphics)
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreenCaptureLib
{
    public partial class ScreenCaptureWindow : Window
    {
        private System.Windows.Point StartPoint, LastPoint;
        private System.Windows.Shapes.Rectangle dashedRectangle;
        bool isClosed;

        public ScreenCaptureWindow()
        {
            InitializeComponent();
            Topmost = true;
            Initialize();
        }

        public ScreenCaptureWindow(bool isTopMost = true)
        {
            InitializeComponent();
            if (isTopMost) Topmost = true;
            Initialize();
        }

        void Initialize()
        {
            this.Loaded += (s, e) =>
            {
                Overlay.Width = this.ActualWidth;
                Overlay.Height = this.ActualHeight;
            };
            Cursor = Cursors.Cross;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) 
            {
                this.Close();
                e.Handled = true; 
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartPoint = Mouse.GetPosition(canvas);
            LastPoint = StartPoint;

            // Start tracking mouse move and mouse up events
            canvas.MouseMove += canvas_MouseMove;
            canvas.MouseUp += canvas_MouseUp;
            canvas.CaptureMouse();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (StartPoint == null) return;

            LastPoint = Mouse.GetPosition(canvas);

            // Calculate the selected area
            double x = Math.Min(LastPoint.X, StartPoint.X);
            double y = Math.Min(LastPoint.Y, StartPoint.Y);
            double width = Math.Abs(LastPoint.X - StartPoint.X);
            double height = Math.Abs(LastPoint.Y - StartPoint.Y);

            // Create a combined geometry to clip out the selected rectangle area from the overlay
            RectangleGeometry fullArea = new RectangleGeometry(new Rect(0, 0, canvas.ActualWidth, canvas.ActualHeight));
            RectangleGeometry selectionArea = new RectangleGeometry(new Rect(x, y, width, height));
            CombinedGeometry combinedGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, fullArea, selectionArea);

            // Apply the clipping to the overlay rectangle
            Overlay.Clip = combinedGeometry;

            // Create or update the dashed rectangle

            if (dashedRectangle == null)
            {
                dashedRectangle = new System.Windows.Shapes.Rectangle
                {
                    Stroke = System.Windows.Media.Brushes.Red, // Set the outline color
                    StrokeDashArray = new DoubleCollection { 4, 2 }, // Dash pattern
                    StrokeThickness = 2 // Set outline thickness
                };
                canvas.Children.Add(dashedRectangle); // Add it to the canvas
            }

            // Set the position and size of the dashed rectangle
            Canvas.SetLeft(dashedRectangle, x);
            Canvas.SetTop(dashedRectangle, y);
            dashedRectangle.Width = width;
            dashedRectangle.Height = height;
        }


        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            canvas.ReleaseMouseCapture();
            canvas.MouseMove -= canvas_MouseMove;
            canvas.MouseUp -= canvas_MouseUp;

            CaptureScreen();

            // Reset the overlay clip and remove selection
            Overlay.Clip = null;
        }

        bool isBusy;
        private void CaptureScreen()
        {
            if (isBusy) return;
            isBusy = true;

            this.Hide();
            if (this.Owner != null) { this.Owner.Activate(); }

            var screenStart = PointToScreen(StartPoint);
            var screenEnd = PointToScreen(LastPoint);

            int x = (int)Math.Min(screenStart.X, screenEnd.X);
            int y = (int)Math.Min(screenStart.Y, screenEnd.Y);
            int width = (int)Math.Abs(screenEnd.X - screenStart.X);
            int height = (int)Math.Abs(screenEnd.Y - screenStart.Y);

            if (width > 0 && height > 0)
            {
                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height));
                    }

                    MemoryStream memory = new MemoryStream();
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    PreviewWindow previewWindow = new PreviewWindow(bitmapImage, memory);
                    if (this.Owner != null) { previewWindow.Owner = this.Owner; }
                    previewWindow.Loaded += (s, e) => { this.Close(); };
                    previewWindow.Show();
                }
            }
        }
    }
}
