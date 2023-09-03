using ReactiveUI;
using System;
using System.Timers;

namespace Clock.ViewModels;

public class MainViewModel : ViewModelBase
{
    #region Текущее время

    /// <summary>
    /// Текущее время реально хранится здесь
    /// </summary>
    private DateTime _currentTime;

    /// <summary>
    /// Обёртка для доступа к текущему времени. Она нужна чтобы была возможность сбиндить время с пользовательским интерфейсом
    /// </summary>
    public DateTime CurrentTime
    {
        get => _currentTime; // Геттер. Он вызывается при чтении CurrentTime и как прокси отдаёт значение из _currentTime
        set => this.RaiseAndSetIfChanged(ref _currentTime, value); // Сеттер. Он вызывается при записи в CurrentTime и вызывает метод RaiseAndSetIfChanged (системный)
    }

    #endregion

    /// <summary>
    /// Это таймер, который заставит часы тикать
    /// </summary>
    private Timer _timer;

    public MainViewModel()
    {
        // Загружаем начальное время
        CurrentTime = DateTime.Now;

        #region Настройка таймера

        _timer = new Timer(1000); // Конструируем таймер, отсчитывающий от 1000 миллисекунд до 0
        _timer.Elapsed += OnTimerElapsed; // Добавляем в список методов, которые вызываются, когда таймер дотикал, наш OnTimerElapsed()
        _timer.AutoReset = true; // Перезапускаем таймер, после того, как он дотикал до 0
        _timer.Enabled = true; // Включаем таймер
        
        #endregion
    }

    /// <summary>
    /// Этот метод вызывается, когда таймер дотикивает до 0
    /// </summary>
    /// <param name="sender">Отправитель события (в нашем случае - таймер). Знак вопроса означает, что вместо отправителя может быть null</param>
    /// <param name="e">Аргументы события</param>
    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        // Обновляем текущее время
        CurrentTime = DateTime.Now;
    }
}
