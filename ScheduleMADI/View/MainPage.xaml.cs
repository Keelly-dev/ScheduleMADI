using System.ComponentModel;

namespace ScheduleMADI;

/// <summary>
/// ПУБЛИКОВАТЬ НУЖНО ВСЁ ИЗ "ОПУБЛИКОВАТЬ", НЕ ИЗ КОНСОЛИ!!!!!! КОНСОЛЬ КРИВО ПОДПИСЫВАЕТ!!!
/// </summary>
public partial class MainPage : ContentPage
{
    private readonly MainPageVM mainPageVM = new();
    private bool carouselLoaded;
    private bool restoringPosition;

    public MainPage()
    {
        InitializeComponent();
        mainPageVM.PropertyChanged += MainPageVM_PropertyChanged;
        BindingContext = mainPageVM;
    }

    private async void Mainpage_Appearing(object sender, EventArgs e)
    {
        if (!Preferences.Default.ContainsKey("swipe_instr"))
        {
            await DisplayAlertAsync("Подсказка",
                "Перелистывайте расписание свайпом влево или вправо. " +
                "Конкретную дату можно выбрать в календаре.", "Ок");
            Preferences.Default.Set("swipe_instr", 1);
        }
    }

    private void Carousel_Loaded(object sender, EventArgs e)
    {
        carouselLoaded = true;
        RestoreCarouselPosition();
    }

    private void Carousel_Unloaded(object sender, EventArgs e) => carouselLoaded = false;

    private void Carousel_PositionChanged(object sender, PositionChangedEventArgs e)
    {
        if (carouselLoaded && !restoringPosition && mainPageVM.Datepicker_is_enabled)
            mainPageVM.Carousel.Position = e.CurrentPosition;
    }

    private void MainPageVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainPageVM.Datepicker_is_enabled) && mainPageVM.Datepicker_is_enabled)
            RestoreCarouselPosition();
    }

    private void RestoreCarouselPosition()
    {
        // Дождаться обновления видимости и не принять начальную позицию 0 за свайп.
        restoringPosition = true;
        Dispatcher.Dispatch(() =>
        {
            if (carouselLoaded && mainPageVM.Datepicker_is_enabled)
                ScheduleCarousel.Position = mainPageVM.Carousel.Position;
            restoringPosition = false;
        });
    }
}
