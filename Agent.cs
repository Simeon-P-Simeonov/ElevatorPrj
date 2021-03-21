using System;
using System.IO;
using System.Resources;
using System.Threading;

namespace ElevatorProject
{

    class Agent
    {
        public string name;
        public ConfidentialLevel type;
        private readonly Elevator _elevator;
        public readonly Floor[] _accessibleFloors;
        private int workDone = 0;
        private Floor current_floor;
        private static readonly Semaphore semaphore = new Semaphore(1, 1);
        private readonly ManualResetEvent atHome = new ManualResetEvent(false);
        private TextWriter writer;
        public bool AtHome { get { return atHome.WaitOne(0); } }

        public Agent(string Name, ConfidentialLevel Type, Elevator elevator, TextWriter writer)
        {
            name = Name;
            type = Type;
            _elevator = elevator;
            this.writer = writer;
            switch (Type)
            {
                case ConfidentialLevel.Confidential: _accessibleFloors = new Floor[] { Floor.G }; break;
                case ConfidentialLevel.Secret: _accessibleFloors = new Floor[] { Floor.G, Floor.S }; break;
                case ConfidentialLevel.Top_secret: _accessibleFloors = new Floor[] { Floor.G, Floor.S, Floor.T1, Floor.T2 }; break;
                default: throw new ArgumentException("Invalid Confidential Level");
            }
        }

        private void Leave()
        {
            writer.WriteLine(name + " is out of the Elevator!");
            //Thread.Sleep(1000);
            current_floor = _elevator.currentFloor;
            semaphore.Release();
        }
        private void insideElevatorRoutine()
        {
            var chosenFloor = ChooseRandomButton();
            writer.WriteLine($"{name} wants to go to floor {chosenFloor}.");
            _elevator.Move(chosenFloor);
            //Thread.Sleep(1000);
            _elevator.AgentInElevator(chosenFloor);
        }
        private void Enter()
        {
            writer.WriteLine(name + " is waiting for the elevator.");
            //Thread.Sleep(1000);
            semaphore.WaitOne();
            writer.WriteLine(name + $" called the elevator from {current_floor}.");
            //Thread.Sleep(1000);
            _elevator.Move(current_floor);
            _elevator.Enter(this);
            writer.WriteLine(name + " entered the elevator.");
            //Thread.Sleep(1000);
            do
            {
                insideElevatorRoutine();
            } while (!_elevator.canPass);
            //Thread.Sleep(1000);
        }

        private void DailyRoutine()
        {
            writer.WriteLine(name + " enters the building.");
            //Thread.Sleep(1000);
            while (workDone < 100)
            {
                Enter();
                Leave();
                writer.WriteLine(name + " is working!");
                //Thread.Sleep(5000);
                workDone += new Random().Next(5, 30);
                if (workDone > 100) workDone = 100;
                writer.Write($"{name} has done {workDone}% of their work.\n");
            }
            GoHomeRoutine();
        }
        private void GoHomeRoutine()
        {
            writer.WriteLine($"{name}'s working day is over!");
            //Thread.Sleep(1000);
            if (current_floor != Floor.G)
            {
                writer.WriteLine($"{name} is waiting for the elevator.");
                //Thread.Sleep(1000);
                semaphore.WaitOne();
                writer.WriteLine($"{name} called the elevator from {current_floor}.");
                //Thread.Sleep(1000);
                _elevator.Move(current_floor);
                _elevator.Enter(this);
                writer.WriteLine($"{name} entered the elevator.");
                //Thread.Sleep(1000);
                writer.WriteLine($"{name} wants to go to floor G.");
                _elevator.Move(Floor.G);
                //Thread.Sleep(1000);
                _elevator.AgentInElevator(Floor.G);
            }
            writer.WriteLine($"{name} is leaving the building!");
            atHome.Set();
        }
        public void ThreadWorker()
        {
            new Thread(DailyRoutine).Start();
        }

        internal Floor ChooseRandomButton()
        {
            return _elevator.GetAvailableButtons()[new Random().Next(0, 3)];
        }
    }
}
