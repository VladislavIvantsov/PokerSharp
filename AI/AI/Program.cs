using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AI
{
    class Program
    {
        static BotManager Manager = new BotManager();

        static void RunApp()
        {
            while (true)
            {
                string Input = string.Empty;
                string Command = string.Empty;
                Input = Console.ReadLine();
                Input += " ";
                Command = Parse.GetCommandConsole(ref Input);
                if (Command == "createroom")
                {
                    int Count = Convert.ToInt32(Parse.GetCommandConsole(ref Input));
                    string Name = Parse.GetCommandConsole(ref Input);
                    Manager.CreateBotRoom(Count, Name);
                }
                else if (Command == "deleteroom")
                {
                    string Name = Parse.GetCommandConsole(ref Input);
                    Manager.DeleteBotRoom(Name);
                }
                else if (Command == "readygame")
                {
                    string Name = Parse.GetCommandConsole(ref Input);
                    Manager.ReadyGame(Name);
                }
                else if (Command == "notreadygame")
                {
                    string Name = Parse.GetCommandConsole(ref Input);
                    Manager.NotReadyGame(Name);
                }
                else if (Command == "refreshbot")
                {
                }
                else if (Command == "addbot")
                {
                    string RoomName = Parse.GetCommandConsole(ref Input);
                    Manager.AddBot(RoomName);
                }
                else if (Command == "addannbot")
                {
                    string RoomName = Parse.GetCommandConsole(ref Input);
                    Manager.AddANNBot(RoomName);
                }
                else if (Command == "deletebot")
                {
                    string BotIndex = Parse.GetCommandConsole(ref Input);
                    Manager.DeleteBot(BotIndex);
                }
                else if (Command == "runga")
                {
                    GeneticAlgorithm GA = new GeneticAlgorithm();
                    GA.LoadPopulation();
                    while (GA.CountOfCycleEvolution != GA.CyclesOfEvolution)
                    {
                        Individ result = GA.Start();
                        Console.WriteLine("Cycle " + GA.CountOfCycleEvolution + ": " + result.Fitness);
                    }
                }
                else if (Command == "runnewga")
                {
                    GeneticAlgorithm GA = new GeneticAlgorithm();
                    GA.GenerateNewPopulation();
                    while (GA.CountOfCycleEvolution != GA.CyclesOfEvolution)
                    {
                        Individ result = GA.Start();
                        Console.WriteLine("Cycle " + GA.CountOfCycleEvolution + ": " + result.Fitness);
                    }
                }
                else if (Command == "exit")
                {
                    Manager.Exit();
                    break;
                }
            }
        }

        static void Main(string[] args)
        {
            Configuration.GAConfig = new GAConfiguration();
            Configuration.AnnConfig = new ANNConfiguration();
            RunApp();
        }        
    }
}
