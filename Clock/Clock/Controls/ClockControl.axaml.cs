using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Clock.Controls
{
    public partial class ClockControl : UserControl
    {
        #region ���������

        /// <summary>
        /// ��������� ��� ������� �����. ������ ����� ����� ����� ���������, ����������� �� ������� �� ������ ����, ������� �� 2
        /// </summary>
        private const double ClockRadiusMultiplier = 0.9;

        #region ���������

        /// <summary>
        /// ����� ��� ��������� ���� ����������
        /// </summary>
        private readonly IBrush ClockFaceBackgroundBrush = new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// ���� ��� ��������� ����������
        /// </summary>
        private readonly IPen ClockFacePen = new Pen(new SolidColorBrush(Colors.Black), 5);

        #endregion

        /// <summary>
        /// ����� ��� ��������� ���� �������
        /// </summary>
        private readonly IBrush HandsBrush = new SolidColorBrush(Colors.Black);

        #region ��������� �������

        /// <summary>
        /// ������, � �������� ���������� ��������� �������
        /// </summary>
        private const double SecondsHandBeginRadius = 0;

        /// <summary>
        /// ��������� ������� ������������� � 0.9 ������� �����
        /// </summary>
        private const double SecondsHandEndRadius = 0.9;

        /// <summary>
        /// ������� ��������� �������
        /// </summary>
        private const double SecondsHandThickness = 1;

        #endregion

        #region �������� �������

        /// <summary>
        /// ������, � �������� ���������� �������� �������
        /// </summary>
        private const double MinutesHandBeginRadius = 0;

        /// <summary>
        /// �������� ������� ������������� � 0.7 ������� �����
        /// </summary>
        private const double MinutesHandEndRadius = 0.7;

        /// <summary>
        /// ������� �������� �������
        /// </summary>
        private const double MinutesHandThickness = 3;

        #endregion

        #region ������� �� ����������

        /// <summary>
        /// �� ���� ������� ����������� ������� �� ����������
        /// </summary>
        private const double HoursRangeTextRadius = 0.85;

        /// <summary>
        /// ������, � �������� ���������� ��������� ������� ����
        /// </summary>
        private const double HoursRangeBeginRadius = 0.9;

        /// <summary>
        /// ������� ���� ������������� � 0.98 ������� �����
        /// </summary>
        private const double HoursRangeEndRadius = 0.98;

        /// <summary>
        /// ������� �������� ������� ����
        /// </summary>
        private const double HoursRangeThickness = 4;

        /// <summary>
        /// ������ ������ ��� ��������� ���� �� ����������
        /// </summary>
        private const double HoursRangeFontSize = 18;

        /// <summary>
        /// ����� ��� ���� �� ����������
        /// </summary>
        private readonly IBrush HoursRangeTextBrush = new SolidColorBrush(Colors.Red);

        #endregion

        #region �������� �������

        /// <summary>
        /// ������, � �������� ���������� ������� �������
        /// </summary>
        private const double HoursHandBeginRadius = 0;

        /// <summary>
        /// ������� ������� ������������� � 0.5 ������� �����
        /// </summary>
        private const double HoursHandEndRadius = 0.5;

        /// <summary>
        /// ������� ������� �������
        /// </summary>
        private const double HoursHandThickness = 5;

        #endregion

        #endregion

        #region ����� ��� �����������

        /// <summary>
        /// �����, ������������ �� �����, �������� �����
        /// </summary>
        public static readonly AttachedProperty<DateTime> TimeProperty = AvaloniaProperty.RegisterAttached<ClockControl, Interactive, DateTime>(nameof(Time));

        /// <summary>
        /// ������ ��� �������, ������������� �� �����
        /// </summary>
        public DateTime Time
        {
            get { return GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        #endregion

        /// <summary>
        /// ������ �������� � ��������
        /// </summary>
        private int _width;

        /// <summary>
        /// ������ �������� � ��������
        /// </summary>
        private int _height;

        /// <summary>
        /// ������� �� _width, _height
        /// </summary>
        private int _minSide;

        /// <summary>
        /// ����������� �����
        /// </summary>
        private Point _centerPoint;

        /// <summary>
        /// ������ �����
        /// </summary>
        private double _clockRadius;

        public ClockControl()
        {
            InitializeComponent();

            // ������������� �� ��������� �������, ������������� �� �����
            TimeProperty.Changed.Subscribe(x => HandleTimeChanged(x.Sender, x.NewValue.GetValueOrDefault<DateTime>()));

            // ������������� �� ��������� ������� ��������
            PropertyChanged += OnPropertyChangedListener;
        }

        /// <summary>
        /// ���� ����� ���������� ����� �������� Time �������� ����� ��������
        /// </summary>
        private void HandleTimeChanged(AvaloniaObject sender, DateTime dateTime)
        {
            InvalidateVisual(); // ���������� � �������� ����� "������������ ���������� ��������"
        }

        /// <summary>
        /// �������������� ����� ���������, ��������� ��� �� ������
        /// </summary>
        /// <param name="context">�����, �� ������� �� ����� ��������</param>
        public override void Render(DrawingContext context)
        {
            base.Render(context); // ����� ������ ��������� ������ (������ ���, ������� � �.�.)

            // ������ ������ ����������
            context.DrawEllipse
            (
                ClockFaceBackgroundBrush,
                ClockFacePen,
                _centerPoint,
                _clockRadius,
                _clockRadius
            );

            // ���� ������� ����� (��� ����) � �������� ���������� ������ � ������ ���
            var secondsSinceDayBegin = Time.TimeOfDay.TotalSeconds;

            // ������ �� ���������� ������� ��� ������� ����
            DrawHoursRange(context);

            // ������ ��������� �������
            DrawSecondsHand(context, (int)(secondsSinceDayBegin % 60));

            // ������ �������� �������
            DrawMinutesHand(context, (secondsSinceDayBegin % 3600) / 60.0);

            // ������ ������� �������
            DrawHoursHand(context, secondsSinceDayBegin / 3600.0);
        }

        /// <summary>
        /// ���� ����� ���������� ����� �������� �����-���� ���� �������� ��������
        /// </summary>
        private void OnPropertyChangedListener(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name.Equals("Bounds")) // ���� �������� �������� Bounds (������� ��������)
            {
                // �� ������� OnResize() � ������ ���������
                OnResize((Rect)e.NewValue);
            }
        }

        /// <summary>
        /// ���� ����� ���������� ��� ��������� �������� ��������
        /// </summary>
        /// <param name="bounds">�������������, ��������������� ��������. 0, 0 - ������� ����� ���� ��������</param>
        private void OnResize(Rect bounds)
        {
            _width = (int)bounds.Width;
            _height = (int)bounds.Height;

            _minSide = Math.Min(_width, _height);
            _clockRadius = ClockRadiusMultiplier * _minSide / 2.0; // ����� �� 2.0 ����� ���� ������ �������. ���� �������� �� 2 �� ����� ���������� � ������

            _centerPoint = new Point(_width / 2.0, _height / 2.0);
        }

        /// <summary>
        /// �������� ���������� ����� �� ���������� (�������� r �� ������ ����������, ����� �� 0 �� 2 * Pi, 0 - ����������� �����)
        /// </summary>
        private Point GetClockFacePoint(double r, double angle)
        {
            if (r < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(r), "������ �� ����� ���� �������������!");
            }

            if (angle < 0 || angle > 2 * Math.PI)
            {
                throw new ArgumentOutOfRangeException(nameof(angle), "������������ ����!");
            }

            var x = r * Math.Sin(angle) + _centerPoint.X;
            var y = -1 * r * Math.Cos(angle) + _centerPoint.Y;

            return new Point(x, y);
        }

        /// <summary>
        /// ������ �������� - �� ������� r1 �� ������� r2 ��� ����� angle �������� thickness
        /// </summary>
        private void DrawLine(DrawingContext context, double r1, double r2, double angle, double thickness)
        {
            if (r2 < r1)
            {
                throw new ArgumentOutOfRangeException(nameof(r2), "R2 ������ ���� ������ ��� ����� R1");
            }

            var begin = GetClockFacePoint(r1, angle);
            var end = GetClockFacePoint(r2, angle);

            context.DrawLine
            (
                new Pen(HandsBrush, thickness),
                begin,
                end
            );
        }

        /// <summary>
        /// ����� ������ ��������� �������
        /// </summary>
        private void DrawSecondsHand(DrawingContext context, int seconds)
        {
            if (seconds < 0 || seconds > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds), "������������ ���������� ������!");
            }

            var angle = (seconds / 60.0) * 2 * Math.PI;

            DrawLine(context, _clockRadius * SecondsHandBeginRadius, _clockRadius * SecondsHandEndRadius, angle, SecondsHandThickness);
        }

        /// <summary>
        /// ����� ������ �������� �������
        /// </summary>
        /// <param name="context"></param>
        /// <param name="minutes"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void DrawMinutesHand(DrawingContext context, double minutes)
        {
            if (minutes < 0 || minutes >= 60)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes), "������������ ���������� �����!");
            }

            var angle = (minutes / 60.0) * 2 * Math.PI;

            DrawLine(context, _clockRadius * MinutesHandBeginRadius, _clockRadius * MinutesHandEndRadius, angle, MinutesHandThickness);
        }

        /// <summary>
        /// ����� ������ ������� �������
        /// </summary>
        /// <param name="context"></param>
        /// <param name="minutes"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void DrawHoursHand(DrawingContext context, double hours)
        {
            if (hours < 0 || hours >= 24)
            {
                throw new ArgumentOutOfRangeException(nameof(hours), "������������ ���������� �����!");
            }

            var angle = ((hours % 12) / 12.0) * 2 * Math.PI;

            DrawLine(context, _clockRadius * HoursHandBeginRadius, _clockRadius * HoursHandEndRadius, angle, HoursHandThickness);
        }
        /// <summary>
        /// ����� ������ ������� �� ���������� ��� ������� ����
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void DrawHoursRange(DrawingContext context)
        {
            for (int hoursAmount = 1; hoursAmount <= 12; hoursAmount++)
            {
                var angle = ((hoursAmount % 12) / 12.0) * 2 * Math.PI;

                DrawLine(context, _clockRadius * HoursRangeBeginRadius, _clockRadius * HoursRangeEndRadius, angle, HoursRangeThickness);

                var textCoordinates = GetClockFacePoint(_clockRadius * HoursRangeTextRadius, angle);

                var formattedText = new FormattedText
                    (
                        $"{hoursAmount}",
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        Typeface.Default,
                        HoursRangeFontSize,
                        HoursRangeTextBrush
                    );

                var correctedTextCoordinates = new Point
                    (
                        textCoordinates.X - formattedText.Width / 2.0,
                        textCoordinates.Y - formattedText.Height / 2.0
                    );

                context.DrawText
                (
                    formattedText,
                    correctedTextCoordinates
                );
            }

        }
    }
}