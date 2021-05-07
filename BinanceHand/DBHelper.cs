using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceHand
{
    class DBHelper
    {
        private static DBHelper dbHelper;

        public static string path = @"C:\Users\tmdwn\source\repos\BinanceHand\";
        public static string incomeHistoryPath = @"Data Source=C:\Users\tmdwn\source\repos\BinanceHand\DB\Income_History.db";

        SQLiteConnection conn0;
        SQLiteConnection conn1;

        string date;

        Queue<Task> requestDBTaskQueue = new Queue<Task>();

        Thread taskWorker;
        public DBHelper()
        {
            try
            {
                date = DateTime.UtcNow.ToString("yyyy-MM-dd");

                conn0 = new SQLiteConnection("Data Source=" + path + @"DB\days\" + date + ".db");
                conn0.Open();
                conn1 = new SQLiteConnection(incomeHistoryPath);
                conn1.Open();

                taskWorker = new Thread(delegate ()
                {
                    while (true)
                    {
                        while (requestDBTaskQueue.Count > 0 && requestDBTaskQueue.Peek() != null)
                            requestDBTaskQueue.Dequeue().RunSynchronously();
                        Thread.Sleep(1000);
                    }
                });
                taskWorker.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        public void SaveData(string d, string tableName, string column1Name, string column1, string column2Name, string column2, string column3Name, string column3)
        {
            CheckDate(d);

            requestDBTaskQueue.Enqueue(new Task(() =>
            {
                var command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS '" + tableName + "' ('" + column1Name + "' TEXT, '" + column2Name + "' TEXT, '" + column3Name + "' TEXT)", conn0);
                command.ExecuteNonQuery();

                command = new SQLiteCommand(
                    "INSERT INTO '" + tableName + "' ('" + column1Name + "', '" + column2Name + "', '" + column3Name + "') values ('" + column1 + "','" + column2 + "','" + column3 + "')", conn0);
                command.ExecuteNonQuery();
            }));
        }

        public void SaveCSVData(string dir, string name, string data)
        {
            requestDBTaskQueue.Enqueue(new Task(() =>
            {
                System.IO.File.AppendAllText(path + dir + @"\" + name + ".csv", data + "\n");
            }));
        }

        private void CheckDate(string d)
        {
            if (d != date)
            {
                date = d;
                conn0 = new SQLiteConnection("Data Source=" + path + @"DB\" + date + ".db");
                conn0.Open();
            }
        }

        public static DBHelper GetInstance()
        {
            if (dbHelper == null)
                dbHelper = new DBHelper();
            return dbHelper;
        }
        public void Close()
        {
            if (taskWorker != null)
                taskWorker.Abort();
        }
    }
}
