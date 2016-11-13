using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace SLAM.Preview.CustomControls {

    using RapidCellularViewportDependencies;

    [TemplatePart(Name = RapidCellularViewport.CellSurfaceControl, Type = typeof(Image))]
    public sealed class RapidCellularViewport : Control {

        #region DependencyProperties

        public static readonly DependencyProperty CellularDataProperty;
        public static readonly DependencyProperty SpacingBetweenCellsProperty;

        public Color[,] CellularData {
            get { return (Color[,])GetValue(CellularDataProperty); }
            set { SetValue(CellularDataProperty, value); }
        }

        public int SpacingBetweenCells {
            get { return (int)GetValue(SpacingBetweenCellsProperty); }
            set { SetValue(SpacingBetweenCellsProperty, value); }
        }

        static RapidCellularViewport() {

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(RapidCellularViewport), new FrameworkPropertyMetadata(typeof(RapidCellularViewport)));

            var cellularDataPropertyMetadata = new FrameworkPropertyMetadata();
            cellularDataPropertyMetadata.DefaultValue = null;
            cellularDataPropertyMetadata.BindsTwoWayByDefault = true;
            cellularDataPropertyMetadata.AffectsRender = true;
            cellularDataPropertyMetadata.PropertyChangedCallback = OnCellularDataChangedCallback;

            var spacingBetweenCellsPropertyMetadata = new FrameworkPropertyMetadata();
            spacingBetweenCellsPropertyMetadata.DefaultValue = 0;
            spacingBetweenCellsPropertyMetadata.BindsTwoWayByDefault = true;
            spacingBetweenCellsPropertyMetadata.AffectsRender = true;

            CellularDataProperty =
                DependencyProperty.Register(
                    "CellularData", typeof(Color[,]), typeof(RapidCellularViewport), cellularDataPropertyMetadata);

            SpacingBetweenCellsProperty =
                DependencyProperty.Register(
                    "SpacingBetweenCells", typeof(int), typeof(RapidCellularViewport), spacingBetweenCellsPropertyMetadata);
        }

        private static void OnCellularDataChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e) {

            RapidCellularViewport cellViewport = (RapidCellularViewport)sender;
            cellViewport.CellularData = (Color[,])e.NewValue;
            cellViewport.oldCellularData = (Color[,])e.OldValue;
        }

        #endregion

        #region TemplateParts

        private const string CellSurfaceControl = "PART_CellSurfaceControl";
        private Image  cellSurfaceControl;

        #endregion

        private Color[,] oldCellularData;

        private Settings settings;
        private Renderer renderer;

        public override void OnApplyTemplate() {

            cellSurfaceControl = GetTemplateChild(CellSurfaceControl) as Image;
            CoerceParameters();

            settings = new Settings();
            renderer = new Renderer(settings);
        }

        private void CoerceParameters() {
            MinWidth = GetNonZeroGuaranteed(MinWidth);
            MinHeight = GetNonZeroGuaranteed(MinHeight);
        }

        protected override void OnRender(DrawingContext drawingContext) {

            if (CellularData == null) {
                return;
            }

            int surfaceWidth    = GetNonZeroGuaranteed(ActualWidth);
            int surfaceHeight   = GetNonZeroGuaranteed(ActualHeight);
            int cellsHorizontal = GetNonZeroGuaranteed(CellularData.GetLength(1));
            int cellsVertical   = GetNonZeroGuaranteed(CellularData.GetLength(0));

            renderer.Update(surfaceWidth, surfaceHeight, cellsHorizontal, cellsVertical, SpacingBetweenCells);
            cellSurfaceControl.Source = renderer.Render(oldCellularData, CellularData);
        }

        private int GetNonZeroGuaranteed(double value) {
            return value > 0 ? (int)value : 1;
        }
    }
}