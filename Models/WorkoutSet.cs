using SQLite;

namespace WorkoutApp.Models
{
    public class WorkoutSet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int WorkoutId { get; set; }

        [Indexed]
        public int ExerciseId { get; set; }

        public int Reps { get; set; }
        public double Weight { get; set; }
    }
}
