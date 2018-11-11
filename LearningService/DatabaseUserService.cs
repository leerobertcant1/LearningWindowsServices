using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Timers;


//This video: https://www.youtube.com/watch?v=1_mkwuA7KVI
namespace LearningService
{
    public partial class DatabaseUserService : ServiceBase
    {
        private Timer _timer;

        public DatabaseUserService()
        {
            InitializeComponent();
        }

        public void Debug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            File.Create($"{AppDomain.CurrentDomain.BaseDirectory} UserData.txt").Dispose();

            _timer = new Timer(10 * 60 * 100);  // 1 minute expressed as milliseconds
            _timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
            _timer.AutoReset = true;
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            GetDatabaseData();
        }

        private void GetDatabaseData()
        {
            string sqlQuery = "SELECT * FROM sys.users WHERE DateEntered >= NOW() - INTERVAL 1 MINUTE";
            MySqlConnection sqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlQuery, sqlConnection);

            sqlConnection.Open();

            WriteToFile(sqlCommand);

            sqlConnection.Close();           
        }

        private void WriteToFile(MySqlCommand sqlCommand)
        {
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                string firstName, lastName, dateEntered;

                firstName = reader["FirstName"].ToString();
                lastName = reader["LastName"].ToString();
                dateEntered = reader["DateEntered"].ToString();

                using (StreamWriter writer = File.AppendText($"{AppDomain.CurrentDomain.BaseDirectory} UserData.txt"))
                {
                    writer.WriteLine($"{firstName}, {lastName}, {dateEntered}");
                }
            }
        }
    }
}
