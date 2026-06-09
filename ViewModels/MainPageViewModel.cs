using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkoutApp.Models;
using WorkoutApp.Services;

namespace WorkoutApp.ViewModels
{
    public class MainPageViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Workout> Workouts { get; set; } = new();

        private string _newWorkoutName = string.Empty;
        public string NewWorkoutName
        {
            get => _newWorkoutName;
            set { _newWorkoutName = value; OnPropertyChanged(); }
        }

        private string _newWorkoutDuration = string.Empty;
        public string NewWorkoutDuration
        {
            get => _newWorkoutDuration;
            set { _newWorkoutDuration = value; OnPropertyChanged(); }
        }

        public ICommand AddWorkoutCommand { get; }
        public ICommand DeleteWorkoutCommand { get; }
        public ICommand GoToDetailsCommand { get; }

        public MainPageViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            AddWorkoutCommand = new Command(async () => await SaveCustomWorkoutAsync());
            DeleteWorkoutCommand = new Command<Workout>(async (workout) => await DeleteWorkoutAsync(workout));
            GoToDetailsCommand = new Command<Workout>(async (workout) => await GoToDetailsAsync(workout));
        }

        public async Task LoadWorkoutsAsync()
        {
            var workouts = await _databaseService.GetWorkoutsAsync();
            Workouts.Clear();
            foreach (var w in workouts)
            {
                Workouts.Add(w);
            }
        }

        private async Task SaveCustomWorkoutAsync()
        {
            if (string.IsNullOrWhiteSpace(NewWorkoutName))
                return;

            if (!int.TryParse(NewWorkoutDuration, out int minutes))
                minutes = 60;

            var newWorkout = new Workout
            {
                Name = NewWorkoutName,
                Date = DateTime.Now,
                Duration = TimeSpan.FromMinutes(minutes)
            };

            await _databaseService.SaveWorkoutAsync(newWorkout);
            await LoadWorkoutsAsync();

            NewWorkoutName = string.Empty;
            NewWorkoutDuration = string.Empty;
        }

        private async Task DeleteWorkoutAsync(Workout workout)
        {
            if (workout == null) return;
            await _databaseService.DeleteWorkoutAsync(workout);
            await LoadWorkoutsAsync(); 
        }

        private async Task GoToDetailsAsync(Workout workout)
        {
            if (workout == null) return;

            await Shell.Current.GoToAsync("WorkoutDetailsPage", new Dictionary<string, object>
            {
                { "SelectedWorkout", workout }
            });
        }
    }
}