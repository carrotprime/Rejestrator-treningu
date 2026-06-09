namespace WorkoutApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("WorkoutDetailsPage", typeof(WorkoutDetailsPage));
        }
    }
}