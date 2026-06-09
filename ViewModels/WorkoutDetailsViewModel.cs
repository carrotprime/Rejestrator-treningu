using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkoutApp.Models;
using WorkoutApp.Services;

namespace WorkoutApp.ViewModels
{
    public class DisplaySetItem
    {
        public WorkoutSet OriginalSet { get; set; } = new();
        public string DisplayText { get; set; } = string.Empty;
    }

    [QueryProperty(nameof(SelectedWorkout), "SelectedWorkout")]
    public class WorkoutDetailsViewModel : BindableObject
    {
        private readonly DatabaseService _databaseService;

        private Workout _selectedWorkout = new();
        public Workout SelectedWorkout
        {
            get => _selectedWorkout;
            set { _selectedWorkout = value; OnPropertyChanged(); LoadDataAsync(); }
        }

        public ObservableCollection<Exercise> Exercises { get; set; } = new();

        public ObservableCollection<DisplaySetItem> DisplaySets { get; set; } = new();

        private Exercise? _selectedExercise;
        public Exercise? SelectedExercise
        {
            get => _selectedExercise;
            set { _selectedExercise = value; OnPropertyChanged(); }
        }

        private string _reps = string.Empty;
        public string Reps { get => _reps; set { _reps = value; OnPropertyChanged(); } }

        private string _weight = string.Empty;
        public string Weight { get => _weight; set { _weight = value; OnPropertyChanged(); } }

        private string _newExerciseName = string.Empty;
        public string NewExerciseName { get => _newExerciseName; set { _newExerciseName = value; OnPropertyChanged(); } }

        public ICommand AddSetCommand { get; }
        public ICommand AddExerciseCommand { get; }
        public ICommand DeleteSetCommand { get; } 

        public WorkoutDetailsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            AddSetCommand = new Command(async () => await AddSetAsync());
            AddExerciseCommand = new Command(async () => await AddCustomExerciseAsync());

            DeleteSetCommand = new Command<DisplaySetItem>(async (item) => await DeleteSetAsync(item));
        }

        private async void LoadDataAsync()
        {
            if (SelectedWorkout == null || SelectedWorkout.Id == 0) return;
            await RefreshExercisesListAsync();
            await LoadSetsAsync();
        }

        private async Task RefreshExercisesListAsync()
        {
            var exercises = await _databaseService.GetExercisesAsync();
            Exercises.Clear();
            foreach (var ex in exercises) Exercises.Add(ex);
        }

        private async Task LoadSetsAsync()
        {
            var sets = await _databaseService.GetSetsForWorkoutAsync(SelectedWorkout.Id);
            var exercises = await _databaseService.GetExercisesAsync();

            DisplaySets.Clear();
            foreach (var set in sets)
            {
                var ex = exercises.FirstOrDefault(e => e.Id == set.ExerciseId);
                string exName = ex != null ? ex.Name : "Ćwiczenie";

                DisplaySets.Add(new DisplaySetItem
                {
                    OriginalSet = set,
                    DisplayText = $"{exName}: {set.Reps} powt. x {set.Weight} kg"
                });
            }
        }

        private async Task AddSetAsync()
        {
            if (SelectedExercise == null || SelectedWorkout == null) return;
            if (!int.TryParse(Reps, out int repsCount) || !double.TryParse(Weight, out double weightCount)) return;

            var newSet = new WorkoutSet { WorkoutId = SelectedWorkout.Id, ExerciseId = SelectedExercise.Id, Reps = repsCount, Weight = weightCount };
            await _databaseService.SaveSetAsync(newSet);
            await LoadSetsAsync();

            Reps = string.Empty; Weight = string.Empty;
        }

        private async Task AddCustomExerciseAsync()
        {
            if (string.IsNullOrWhiteSpace(NewExerciseName)) return;

            var newExercise = new Exercise { Name = NewExerciseName, MuscleGroup = "Ogólne" };
            await _databaseService.SaveExerciseAsync(newExercise);
            NewExerciseName = string.Empty;
            await RefreshExercisesListAsync();
        }

        private async Task DeleteSetAsync(DisplaySetItem item)
        {
            if (item == null || item.OriginalSet == null) return;
            await _databaseService.DeleteSetAsync(item.OriginalSet);
            await LoadSetsAsync();
        }
    }
}
