using System;
using System.Collections.Generic;
using System.Threading;
using PhilosophersConsoleApp.Model;

namespace PhilosophersConsoleApp
{
	class Program
	{
		public static readonly int DAYTIME = 2;

		private static Mutex[] _mutex;

		private static void SetTable()
		{
			_mutex = new Mutex[5]
			{
				new Mutex(),
				new Mutex(),
				new Mutex(),
				new Mutex(),
				new Mutex(),
			};

			Console.WriteLine($" We have { Environment.ProcessorCount} Processor \n");
		}

		static void Main(string[] args)
		{
			// Chopsticks:  4 0 1 2 3 4
			// Philosophers: 0 1 2 3 4

			SetTable();
			DoDailyLife();
		}

		private static void DoDailyLife()
		{
			// Invite philosophers
			List<Model.Philosopher> customers = new List<Model.Philosopher>
			{
				new Philosopher("Aatrox"),
				new Philosopher("Aurelion Sol"),
				new Philosopher("Pantheon"),
				new Philosopher("Ornn"),
				new Philosopher("Volibear"),
			};
			List<AutoResetEvent> evnets = new List<AutoResetEvent>();

			// Invite them to sitting and having dinner
			foreach (var philosopher in customers)
			{
				// Distributing the cutlery
				philosopher.InitSemaphores
				(
					_mutex[philosopher.LeftMutexIndex],
					_mutex[philosopher.RightMutexIndex]
				);

				Thread dailyLife = new Thread(new ParameterizedThreadStart(DailyLife));
				dailyLife.Start(philosopher);
			}
		}

		private static void DailyLife(object item)
		{
			if (item is Model.Philosopher)
			{
				var philosopher = item as Model.Philosopher;
				try
				{
					for (int i = 0; i < DAYTIME; i++)
					{
						philosopher.PickLeftChopstick();
						philosopher.PickRightChopstick();
						philosopher.Eat();
						philosopher.Release();
						philosopher.Think();
					}
					philosopher.Finish();

				}
				catch (ThreadAbortException e)
				{
					Console.WriteLine("Thread - caught ThreadAbortException - resetting.");
					Console.WriteLine($"Exception message: {e.Message}");
					Thread.ResetAbort();
				}
			}
		}
	}
}
