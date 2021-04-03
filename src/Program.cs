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
				
				for (int i = 0; i < DAYTIME; i++)
				{
					philosopher.PickLeft();
					philosopher.PickRight();
					philosopher.Eat();
					philosopher.Release();
					philosopher.Think();
				}

				Console.WriteLine($"{philosopher.PrintOffset}| Thread {philosopher.Id} Finished	|");
			}
		}


	}
}
