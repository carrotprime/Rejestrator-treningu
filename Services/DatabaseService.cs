using Microsoft.EntityFrameworkCore;
using WorkoutApp.Models;

namespace WorkoutApp.Services
{

    public class WorkoutDbContext : DbContext
    {
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutSet> WorkoutSets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=WorkoutAppDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    public class DatabaseService
    {
        private readonly WorkoutDbContext _context;

        public DatabaseService()
        {
            _context = new WorkoutDbContext();
        }

        private async Task Init()
        {
            bool created = await _context.Database.EnsureCreatedAsync();

            if (created || !await _context.Exercises.AnyAsync())
            {
                _context.Exercises.AddRange(
                    new Exercise { Name = "Wyciskanie leżąc", MuscleGroup = "Klatka piersiowa" },
                    new Exercise { Name = "Przysiad ze sztangą", MuscleGroup = "Nogi" },
                    new Exercise { Name = "Podciąganie na drążku", MuscleGroup = "Plecy" }
                );
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Workout>> GetWorkoutsAsync()
        {
            await Init();
            return await _context.Workouts.OrderByDescending(w => w.Date).ToListAsync();
        }

        public async Task<int> SaveWorkoutAsync(Workout workout)
        {
            await Init();
            if (workout.Id != 0)
            {
                _context.Workouts.Update(workout);
            }
            else
            {
                await _context.Workouts.AddAsync(workout);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteWorkoutAsync(Workout workout)
        {
            await Init();
            var relatedSets = _context.WorkoutSets.Where(s => s.WorkoutId == workout.Id);
            _context.WorkoutSets.RemoveRange(relatedSets);

            _context.Workouts.Remove(workout);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Exercise>> GetExercisesAsync()
        {
            await Init();
            return await _context.Exercises.ToListAsync();
        }

        public async Task<int> SaveExerciseAsync(Exercise exercise)
        {
            await Init();
            await _context.Exercises.AddAsync(exercise);
            return await _context.SaveChangesAsync();
        }

        public async Task SaveSetAsync(WorkoutSet set)
        {
            await Init();
            await _context.WorkoutSets.AddAsync(set);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WorkoutSet>> GetSetsForWorkoutAsync(int workoutId)
        {
            await Init();
            return await _context.WorkoutSets.Where(s => s.WorkoutId == workoutId).ToListAsync();
        }

        public async Task<int> DeleteSetAsync(WorkoutSet set)
        {
            await Init();
            _context.WorkoutSets.Remove(set);
            return await _context.SaveChangesAsync();
        }
    }
}
