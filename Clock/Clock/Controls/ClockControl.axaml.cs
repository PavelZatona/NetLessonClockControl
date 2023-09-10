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
        #region Константы

        /// <summary>
        /// Множитель для радиуса часов. Радиус часов равен этому множителю, умноженному на меньшую из сторон окна, делённую на 2
        /// </summary>
        private const double ClockRadiusMultiplier = 0.9;

        #region Циферблат

        /// <summary>
        /// Кисть для рисования фона циферблата
        /// </summary>
        private readonly IBrush ClockFaceBackgroundBrush = new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// Перо для рисования циферблата
        /// </summary>
        private readonly IPen ClockFacePen = new Pen(new SolidColorBrush(Colors.Black), 5);

        #endregion

        /// <summary>
        /// Кисть для рисования всех стрелок
        /// </summary>
        private readonly IBrush HandsBrush = new SolidColorBrush(Colors.Black);

        #region Секундная стрелка

        /// <summary>
        /// Радиус, с которого начинается секундная стрелка
        /// </summary>
        private const double SecondsHandBeginRadius = 0;

        /// <summary>
        /// Секундная стрелка заканчивается в 0.9 радиуса часов
        /// </summary>
        private const double SecondsHandEndRadius = 0.9;

        /// <summary>
        /// Толщина секундной стрелки
        /// </summary>
        private const double SecondsHandThickness = 1;

        #endregion

        #region Минутная стрелка

        /// <summary>
        /// Радиус, с которого начинается минутная стрелка
        /// </summary>
        private const double MinutesHandBeginRadius = 0;

        /// <summary>
        /// Минутная стрелка заканчивается в 0.7 радиуса часов
        /// </summary>
        private const double MinutesHandEndRadius = 0.7;

        /// <summary>
        /// Толщина минутной стрелки
        /// </summary>
        private const double MinutesHandThickness = 3;

        #endregion

        #region Деления на циферблате

        /// <summary>
        /// На этом радиусе расположены подписи на циферблате
        /// </summary>
        private const double HoursRangeTextRadius = 0.85;

        /// <summary>
        /// Радиус, с которого начинается отрисовка деления часа
        /// </summary>
        private const double HoursRangeBeginRadius = 0.9;

        /// <summary>
        /// Деление часа заканчивается в 0.98 радиуса часов
        /// </summary>
        private const double HoursRangeEndRadius = 0.98;

        /// <summary>
        /// Толщина минутной деления часа
        /// </summary>
        private const double HoursRangeThickness = 4;

        /// <summary>
        /// Размер шрифта для отрисовки цифр на циферблате
        /// </summary>
        private const double HoursRangeFontSize = 18;

        /// <summary>
        /// Кисть для цифр на циферблате
        /// </summary>
        private readonly IBrush HoursRangeTextBrush = new SolidColorBrush(Colors.Red);

        #endregion

        #region Минутная стрелка

        /// <summary>
        /// Радиус, с которого начинается Часовая стрелка
        /// </summary>
        private const double HoursHandBeginRadius = 0;

        /// <summary>
        /// Часовая стрелка заканчивается в 0.5 радиуса часов
        /// </summary>
        private const double HoursHandEndRadius = 0.5;

        /// <summary>
        /// Толщина Часовой стрелки
        /// </summary>
        private const double HoursHandThickness = 5;

        #endregion

        #endregion

        #region Время для отображения

        /// <summary>
        /// Время, отображаемое на часах, хранится здесь
        /// </summary>
        public static readonly AttachedProperty<DateTime> TimeProperty = AvaloniaProperty.RegisterAttached<ClockControl, Interactive, DateTime>(nameof(Time));

        /// <summary>
        /// Прокси для времени, отображаемого на часах
        /// </summary>
        public DateTime Time
        {
            get { return GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        #endregion

        /// <summary>
        /// Ширина контрола в пикселях
        /// </summary>
        private int _width;

        /// <summary>
        /// Высота контрола в пикселях
        /// </summary>
        private int _height;

        /// <summary>
        /// Меньшая из _width, _height
        /// </summary>
        private int _minSide;

        /// <summary>
        /// Центральная точка
        /// </summary>
        private Point _centerPoint;

        /// <summary>
        /// Радиус часов
        /// </summary>
        private double _clockRadius;

        public ClockControl()
        {
            InitializeComponent();

            // Подписываемся на изменение времени, отображаемого на часах
            TimeProperty.Changed.Subscribe(x => HandleTimeChanged(x.Sender, x.NewValue.GetValueOrDefault<DateTime>()));

            // Подписываемся на изменение свойств контрола
            PropertyChanged += OnPropertyChangedListener;
        }

        /// <summary>
        /// Этот метод вызывается когда свойство Time меняется извне контрола
        /// </summary>
        private void HandleTimeChanged(AvaloniaObject sender, DateTime dateTime)
        {
            InvalidateVisual(); // Встроенный в авалонию метод "перерисовать содержимое контрола"
        }

        /// <summary>
        /// Переопределяем метод рисования, пришедший нам от предка
        /// </summary>
        /// <param name="context">Холст, на котором мы будем рисовать</param>
        public override void Render(DrawingContext context)
        {
            base.Render(context); // Вызов метода отрисовки предка (рисует фон, границы и т.п.)

            // Рисуем кольцо циферблата
            context.DrawEllipse
            (
                ClockFaceBackgroundBrush,
                ClockFacePen,
                _centerPoint,
                _clockRadius,
                _clockRadius
            );

            // Берём текущее время (без даты) и выделяем количество секунд с начала дня
            var secondsSinceDayBegin = Time.TimeOfDay.TotalSeconds;

            // Рисуем на циферблате деления для каждого часа
            DrawHoursRange(context);

            // Рисуем секундную стрелку
            DrawSecondsHand(context, (int)(secondsSinceDayBegin % 60));

            // Рисуем минутную стрелку
            DrawMinutesHand(context, (secondsSinceDayBegin % 3600) / 60.0);

            // Рисуем часовую стрелку
            DrawHoursHand(context, secondsSinceDayBegin / 3600.0);
        }

        /// <summary>
        /// Этот метод вызывается когда меняется какое-либо ваще свойство контрола
        /// </summary>
        private void OnPropertyChangedListener(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name.Equals("Bounds")) // Если меняется свойство Bounds (границы контрола)
            {
                // То вызвать OnResize() с новыми границами
                OnResize((Rect)e.NewValue);
            }
        }

        /// <summary>
        /// Этот метод вызывается при изменении размеров контрола
        /// </summary>
        /// <param name="bounds">Прямоугольник, соответствующий контролу. 0, 0 - верхний левый угол контрола</param>
        private void OnResize(Rect bounds)
        {
            _width = (int)bounds.Width;
            _height = (int)bounds.Height;

            _minSide = Math.Min(_width, _height);
            _clockRadius = ClockRadiusMultiplier * _minSide / 2.0; // Делим на 2.0 чтобы было точное деление. Если поделить на 2 то будет округление к целому

            _centerPoint = new Point(_width / 2.0, _height / 2.0);
        }

        /// <summary>
        /// Получить координату точки на циферблате (радиусом r от центра циферблата, углом от 0 до 2 * Pi, 0 - вертикально вверх)
        /// </summary>
        private Point GetClockFacePoint(double r, double angle)
        {
            if (r < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(r), "Радиус не может быть отрицательным!");
            }

            if (angle < 0 || angle > 2 * Math.PI)
            {
                throw new ArgumentOutOfRangeException(nameof(angle), "Некорректный угол!");
            }

            var x = r * Math.Sin(angle) + _centerPoint.X;
            var y = -1 * r * Math.Cos(angle) + _centerPoint.Y;

            return new Point(x, y);
        }

        /// <summary>
        /// Рисует чёрточку - от радиуса r1 до радиуса r2 под углом angle толщиной thickness
        /// </summary>
        private void DrawLine(DrawingContext context, double r1, double r2, double angle, double thickness)
        {
            if (r2 < r1)
            {
                throw new ArgumentOutOfRangeException(nameof(r2), "R2 должен быть больше или равен R1");
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
        /// Метод рисует секундную стрелку
        /// </summary>
        private void DrawSecondsHand(DrawingContext context, int seconds)
        {
            if (seconds < 0 || seconds > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds), "Некорректное количество секунд!");
            }

            var angle = (seconds / 60.0) * 2 * Math.PI;

            DrawLine(context, _clockRadius * SecondsHandBeginRadius, _clockRadius * SecondsHandEndRadius, angle, SecondsHandThickness);
        }

        /// <summary>
        /// Метод рисует минутную стрелку
        /// </summary>
        /// <param name="context"></param>
        /// <param name="minutes"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void DrawMinutesHand(DrawingContext context, double minutes)
        {
            if (minutes < 0 || minutes >= 60)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes), "Некорректное количество минут!");
            }

            var angle = (minutes / 60.0) * 2 * Math.PI;

            DrawLine(context, _clockRadius * MinutesHandBeginRadius, _clockRadius * MinutesHandEndRadius, angle, MinutesHandThickness);
        }

        /// <summary>
        /// Метод рисует часовую стрелку
        /// </summary>
        /// <param name="context"></param>
        /// <param name="minutes"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void DrawHoursHand(DrawingContext context, double hours)
        {
            if (hours < 0 || hours >= 24)
            {
                throw new ArgumentOutOfRangeException(nameof(hours), "Некорректное количество часов!");
            }

            var angle = ((hours % 12) / 12.0) * 2 * Math.PI;

            DrawLine(context, _clockRadius * HoursHandBeginRadius, _clockRadius * HoursHandEndRadius, angle, HoursHandThickness);
        }
        /// <summary>
        /// Метод рисует деления на циферблате для каждого часа
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