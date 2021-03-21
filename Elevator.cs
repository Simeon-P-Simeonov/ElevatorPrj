using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace ElevatorProject
{

    class Elevator
    {
        public List<Floor> floors = new List<Floor>() { Floor.G, Floor.S, Floor.T1, Floor.T2 };
        private Agent _agent = null;
        public Floor currentFloor;
        private TextWriter writer;
        public bool canPass = false;
        public Elevator(TextWriter writer)
        {
            this.writer = writer;
        }
        internal void Enter(Agent a)
        {
            _agent = a;
        }

        private void Leave()
        {
            _agent = null;
        }

        internal List<Floor> GetAvailableButtons()
        {
            var _ = new List<Floor>();
            foreach (Floor f in floors)
            {
                if (f == currentFloor) continue;
                else _.Add(f);
            }
            return _;
        }
        private bool canAgentPass()
        {
            return _agent._accessibleFloors.Contains(currentFloor);
        }
        internal void Move(Floor dest)
        {
            canPass = false;
            if (dest.Equals(currentFloor))
            {
                writer.WriteLine("Elevator is already here.");
                Thread.Sleep(1000);
                canPass = true;
            }
            else
            {
                if (currentFloor > dest)
                {
                    canPass = false;
                    writer.WriteLine("Elevator is going down.");
                    Thread.Sleep(1000);
                }
                else
                {
                    canPass = false;
                    writer.WriteLine("Elevator is going up.");
                    Thread.Sleep(1000);

                }
                for (int i = 0; i < Math.Abs(currentFloor - dest); i++)
                {
                    writer.WriteLine($"\t{i + 1} second(s) have passed!");
                    Thread.Sleep(1000);
                }
                currentFloor = dest;
                writer.WriteLine("Elevator is at " + dest);
                Thread.Sleep(1000);
                canPass = true;
            }

        }
        internal void AgentInElevator(Floor floor)
        {
            canPass = false;
            currentFloor = floor;
            if (canAgentPass())
            {
                currentFloor = floor;
                canPass = true;
                Leave();
            }
            else
            {
                currentFloor = floor;
                writer.WriteLine($"Agent of type {_agent.type} cannot enter floor {floor}");
                Thread.Sleep(1000);
            }
        }
    }
   
}
