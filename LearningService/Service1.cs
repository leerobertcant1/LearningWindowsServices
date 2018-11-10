using Microsoft.Exchange.WebServices.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Net;
using System.ServiceProcess;

//This video: https://www.youtube.com/watch?v=1_mkwuA7KVI
namespace LearningService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        public void Debug()
        {
            OnStart(null);
        }

        //Needed for start and stopping the service
        //Runs in Windows.System32
        protected override void OnStart(string[] args)
        {
            File.Create($"{AppDomain.CurrentDomain.BaseDirectory} StartService.txt");

            GetDatabaseData();
            //ReadEmails();
        }

        protected override void OnStop()
        {
            File.Create($"{AppDomain.CurrentDomain.BaseDirectory} StopService.txt");
            GetDatabaseData();
        }

        private void GetDatabaseData()
        {
            string sql = " SELECT * FROM sys.users  ";
            MySqlConnection con = new MySqlConnection("host=localhost;user=root;password=P3tur4b0;database=sys;");
            MySqlCommand cmd = new MySqlCommand(sql, con);

            con.Open();

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
            }
        }

        private void ReadEmails()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            service.TraceEnabled = true;
            service.Credentials = new NetworkCredential("lee.cant@ampetronic.co", "P3tur4b0");
            service.AutodiscoverUrl("lee.cant@ampetronic.co", RedirectionUrlValidationCallback);

            Folder inbox = Folder.Bind(service, WellKnownFolderName.Inbox);

            FindItemsResults <Item> findResults = service.FindItems(
               WellKnownFolderName.Inbox,
               new ItemView(10));

            foreach (Item item in findResults.Items)
            {
                Console.WriteLine(item.Subject);
            }
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }

            return result;
        }
    }
}
