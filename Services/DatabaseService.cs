using SQLite;
using WorkoutApp.Models;

namespace WorkoutApp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;
        private readonly string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout_db.db3");
        }

        private async Task Init()
        {
            if (_database != null) return;

            _database = new SQLiteAsyncConnection(_dbPath);

            await _database.CreateTableAsync<Workout>();
            await _database.CreateTableAsync<Exercise>();
            await _database.CreateTableAsync<WorkoutSet>();

            if (await _database.Table<Exercise>().CountAsync() == 0)
            {
                await _database.InsertAsync(new Exercise { Name = "Wyciskanie leżąc", MuscleGroup = "Klatka piersiowa" });
                await _database.InsertAsync(new Exercise { Name = "Przysiad ze sztangą", MuscleGroup = "Nogi" });
                await _database.InsertAsync(new Exercise { Name = "Podciąganie na drążku", MuscleGroup = "Plecy" });
            }
        }

        public async Task<List<Workout>> GetWorkoutsAsync()
        {
            await Init();
            return await _database!.Table<Workout>().OrderByDescending(w => w.Date).ToListAsync();
        }

        public async Task<int> SaveWorkoutAsync(Workout workout)
        {
            await Init();
            if (workout.Id != 0) return await _database!.UpdateAsync(workout);
            else return await _database!.InsertAsync(workout);
        }

        public async Task<int> DeleteWorkoutAsync(Workout workout)
        {
            await Init();
            await _database!.ExecuteAsync("DELETE FROM WorkoutSet WHERE WorkoutId = ?", workout.Id);
            return await _database!.DeleteAsync(workout);
        }

        public async Task<List<Exercise>> GetExercisesAsync()
        {
            await Init();
            return await _database!.Table<Exercise>().ToListAsync();
        }

        public async Task<int> SaveExerciseAsync(Exercise exercise)
        {
            await Init();
            return await _database!.InsertAsync(exercise);
        }

        public async Task SaveSetAsync(WorkoutSet set)
        {
            await Init();
            await _database!.InsertAsync(set);
        }

        public async Task<List<WorkoutSet>> GetSetsForWorkoutAsync(int workoutId)
        {
            await Init();
            return await _database!.Table<WorkoutSet>().Where(s => s.WorkoutId == workoutId).ToListAsync();
        }

        public async Task<int> DeleteSetAsync(WorkoutSet set)
        {
            await Init();
            return await _database!.DeleteAsync(set);
        }
    }
}