using System;
using System.Configuration;
using System.Text.Encodings.Web;
using System.Xml.Serialization;
using TodoItemNamespace;
using TodoPriorityQueueNamespace;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;


namespace myTodosManager
{
    class Program
    {
        static string fileName = "todo.xml";
        static Dictionary<string, string> commandDict = new Dictionary<string, string>();
        static TodoPriorityQueue priorityQueue = new TodoPriorityQueue();
        static IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
        static IConfigurationRoot settings = builder.Build();

        public static void Main(string[] args)
        {
            InitCommandsDictionary();
            LoadList();
            

            while (true)
            {
                string command = GetUserInput();

                switch (command)
                {
                    case "quit":
                        Environment.Exit(1);
                        break;

                    case "add":
                        ReceiveNewTask();
                        SaveList();
                        WaitForKey();
                        Console.Clear();
                        break;

                    case "list":
                        priorityQueue.PrintValues();
                        WaitForKey();
                        Console.Clear();
                        break;

                    case "finish":
                        priorityQueue.PrintValues();
                        Finish();
                        WaitForKey();
                        SaveList();
                        Console.Clear();
                        break;

                    case "peek":
                        Console.WriteLine($"\nNEXT TASK:\n~~~~~~~\n{priorityQueue.Peek()}");
                        WaitForKey();
                        Console.Clear();
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid command. Try again.");
                        break;
                }
            }
        }

        private static string GetUserInput()
        {
            string option;
            bool isValidOption;

            do
            {
                Console.WriteLine("\nPROMPT: Enter a command:");
                foreach (KeyValuePair<string, string> pair in commandDict)
                {
                    Console.WriteLine("{0}. {1}", pair.Key, pair.Value);
                }

                option = Console.ReadLine();
                option = option!.TrimEnd().ToLower();
                
                if (commandDict.ContainsKey(option))
                {
                    option = commandDict[option];
                }

                switch (option)
                {
                    case "add":
                    case "peek":
                    case "list":
                    case "finish":
                    case "quit":
                        isValidOption = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        isValidOption = false;
                        break;
                }
            } while (!isValidOption);
            Console.Clear();
            return option;
        }

        private static void LoadList()
        {
            if (File.Exists(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TodoPriorityQueue));
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    priorityQueue = (TodoPriorityQueue)serializer.Deserialize(stream);
                }
            }
        }

        private static void SaveList()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TodoPriorityQueue));
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(stream, priorityQueue);
            }
        }

        
        private static void Finish()
        {
            bool success = false;
            while (!success)
            {
                Console.WriteLine("Which item to remove? (type: 0 to cancel)");
                
                int response = GetInt(0,priorityQueue.Count);
                if (response == 0)
                {
                    Console.WriteLine("Canceling action.");
                    success = true;
                }
                else if (0 < response && response <= priorityQueue.Count)
                {
                    // because we present values starting at 1 we subtract 1
                    int position = response - 1;
                    Console.WriteLine($"Removing {priorityQueue.Peek(position)}");
                    if (Confirm())
                    {
                        priorityQueue.RemovePosition(position);
                    } else
                    {
                        Console.WriteLine("Canceling action.");
                    }
                    success = true;
                }
                else
                {
                    Console.WriteLine($"Please give a number between 0 and {priorityQueue.Count} or type 'disregard' to go back to the prompt.");
                }
            }
            
        }

        private static void WaitForKey()
        {
            Console.WriteLine("\nPush enter to continue.");
            Console.ReadLine();
        }

        private static void ReceiveNewTask()
        {
            Console.WriteLine("Enter a task to add:");
            string task = HtmlEncoder.Default.Encode(Console.ReadLine());
            while (task == "" || task is null) {
                Console.WriteLine("Try Again");
                Console.WriteLine("Enter a task to add:");
                task = HtmlEncoder.Default.Encode(Console.ReadLine());
            }
            int urgency = GetScore("urgency");
            int importance = GetScore("importance");
            TodoItem item = new TodoItem(task, importance, urgency);
            priorityQueue.Enqueue(item);
        }

        private static void InitCommandsDictionary()
        {
            for (int i = 1; i <= 5; i++)
            {
                commandDict[$"{i}"] = settings[$"commandMapping:{i}"];
            }
        }

        private static int GetScore(string dim_name)
        {
            string content;
            Console.WriteLine($"What is the {dim_name}? (1-5)");

            for (int i = 1; i <= 5; i++)
            {
                content = settings[$"{dim_name}Options:{i}"];
                Console.WriteLine($"> {i}. {content}");
            }

            return GetInt(1, 5);
        }

        private static int GetInt(int min = 0, int max = 5)
        {
            string value;
            bool success = false;
            int number = 0;
            while (!success)
            {
                value = Console.ReadLine();
                if (int.TryParse(value, out number) &&
                    int.Parse(value) >= min &&
                    int.Parse(value) <= max)
                {
                    number = int.Parse(value);
                    success = true;
                }
                else
                {
                    Console.WriteLine($"Please enter a number between {min} and {max}");
                }
            }
            return number;
        }

        private static bool Confirm()
        {
            Console.WriteLine("Please type 'yes' to finish task.");
            string response = Console.ReadLine();
            if(response == "yes")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
