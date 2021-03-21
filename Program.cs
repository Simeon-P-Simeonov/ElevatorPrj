using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ElevatorProject
{
    public enum ConfidentialLevel { Confidential, Secret, Top_secret };
    public enum Floor { G, S, T1, T2 };
    public static class Extensions
    {
        public static bool Contains(this Array array, Object obj)
        {
            foreach (var el in array)
            {
                if (el.Equals(obj)) return true;
            }
            return false;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            var projectName = Assembly.GetExecutingAssembly().GetName().Name;
            var outputPath = projectPath + "\\" + projectName + "\\Output.txt";
            if (!File.Exists(outputPath)) 
                throw new IOException("Output File Not Found!");
            Console.WriteLine(outputPath);
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                var synchronousWriter = TextWriter.Synchronized(writer);
                Elevator elevator = new Elevator(synchronousWriter);
                Stopwatch stopwatch = new Stopwatch();
                List<Agent> agents = new List<Agent>() {
                new Agent("Secret", ConfidentialLevel.Secret, elevator, synchronousWriter),
                new Agent("Confidential", ConfidentialLevel.Confidential, elevator, synchronousWriter),
                new Agent("Top-secret", ConfidentialLevel.Top_secret, elevator, synchronousWriter) };
                stopwatch.Start();
                agents.ForEach(x => x.ThreadWorker());
                while (agents.Any(x => !x.AtHome)) { }
                stopwatch.Stop();
                synchronousWriter.WriteLine($"\nEllapsed time: {stopwatch.Elapsed.Minutes}:{stopwatch.Elapsed.Seconds}");
            }
        }
    }
}
