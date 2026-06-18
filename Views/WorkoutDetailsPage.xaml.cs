using WorkoutApp.ViewModels;

namespace WorkoutApp
{
    public partial class WorkoutDetailsPage : ContentPage
    {
        public WorkoutDetailsPage(WorkoutDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}