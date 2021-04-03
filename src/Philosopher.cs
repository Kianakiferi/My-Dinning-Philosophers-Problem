using System;
using System.Threading;

namespace PhilosophersConsoleApp.Model
{
	class Philosopher
	{
		private const int MAX_THINKING_TIME = 2000;
		private const int MAX_EATING_TIME = 500;

		private const int MAX_WATING_TIME = 4000;

		private const int MAX_CHOPSTICKS = 5;

		private Random Random = new();
		private static int GenericId = 0;

		public int Id;
		public string Name;
		public string PrintOffset;

		public int LeftMutexIndex;
		public Mutex LeftMutex;

		public int RightMutexIndex;
		public Mutex RightMutex;
		
		private bool IsBothHandsWithaChopstick;
		
		#region CTOR
		public Philosopher(string name)
		{
			Id = GenericId;
			GenericId++;

			Name = name;

			if (Id is > 0 and < MAX_CHOPSTICKS)
			{
				LeftMutexIndex = Id - 1;
			}
			else
			{
				LeftMutexIndex = Math.Abs(Id - (MAX_CHOPSTICKS - 1));
			}

			RightMutexIndex = Id;

			for (int i = 0; i < Id; i++)
			{
				PrintOffset += "			";
			}

			Console.WriteLine($"Thread {Id} Now Online");
		}

		public void InitSemaphores(Mutex leftMutex, Mutex rightMutex)
		{
			this.LeftMutex = leftMutex;
			this.RightMutex = rightMutex;
		}
		#endregion

		#region Daily Life
		public void Think()
		{
			Console.WriteLine($"{PrintOffset}| {this.Id}: Thinking		|");
			Thread.Sleep(Random.Next(MAX_THINKING_TIME));
		}

		public void PickLeftChopstick()
		{
			Console.WriteLine($"{PrintOffset}| {this.Id}: Waiting		|");

			if (LeftMutex.WaitOne(MAX_WATING_TIME))
			{
				Console.WriteLine($"{PrintOffset}| {this.Id}: Got left {LeftMutexIndex}		|");
			}
			else
			{
				Console.WriteLine($"{PrintOffset}| {this.Id}: Starved to death	|");
			}
		}

		public void PickRightChopstick()
		{
			if (RightMutex.WaitOne(MAX_WATING_TIME))
			{
				Console.WriteLine($"{PrintOffset}| {this.Id}: Got right {RightMutexIndex}	|");
				IsBothHandsWithaChopstick = true;
			}
			else
			{
				Console.WriteLine($"{PrintOffset}| {this.Id}: Starved to death	|");
			}
		}

		public void Eat()
		{
			if (IsBothHandsWithaChopstick)
			{
				Console.WriteLine($"{PrintOffset}| {this.Id}: Eating		|");
				Thread.Sleep(Random.Next(MAX_EATING_TIME));
			}
		}

		public void Release()
		{
			try
			{
				Console.WriteLine($"{PrintOffset}| {Id}: Released left {LeftMutexIndex}	|");
				LeftMutex.ReleaseMutex();
			}
			catch (Exception)
			{

			}

			try
			{
				Console.WriteLine($"{PrintOffset}| {Id}: Released right {RightMutexIndex}	|");
				RightMutex.ReleaseMutex();
			}
			catch (Exception)
			{

			}
			
			IsBothHandsWithaChopstick = false;
		}
		#endregion
	}
}
