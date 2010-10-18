using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;
using System.Threading;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;
using System.Diagnostics;

namespace ConsoleTester
{
	class Program
	{
		static Random _random = new Random();

		static void Main(string[] args)
		{
			Program p = new Program();
			//p.MultipleThreadTest();
			for (int i = 0; i < 10; ++i)
			{
				p.SingleThreadTest();
			}

			Console.ReadKey();
		}

		public void SingleThreadTest()
		{
			Stopwatch sw = new Stopwatch();
			IItemRepository items = ServiceLocator
				.Instance.GetService<IItemRepository>();

			Console.WriteLine("Single thread test");
			sw.Start();
			IEnumerable<Item> list = items.Select(13000, 20);			
			Output(list);
			sw.Stop();

			Console.WriteLine("{0}ms elapsed", sw.ElapsedMilliseconds);
		}

		public void MultipleThreadTest()
		{
			Console.WriteLine("Multiple thread test");
			for (int i = 0; i < 10; ++i)
			{
				Thread thread = new Thread(new ThreadStart(MultipleThreadTestTarget));
				thread.Name = String.Format("#{0}", i + 1);
				thread.Start();
				Thread.Sleep(10);
			}
		}

		public void MultipleThreadTestTarget()
		{
			IItemRepository items = Configuration.Instance
				.Container.Resolve<IItemRepository>();

			IEnumerable<Item> list = items.Select(_random.Next(10000, 51000), 20);
			Action<IEnumerable<Item>> action = new Action<IEnumerable<Item>>(Output);
			action.BeginInvoke(list.Take(10), null, null);
			action.BeginInvoke(list.Skip(10), null, null);

			MultipleThreadTestCallback();
		}

		public void Output(IEnumerable<Item> items)
		{
			//try
			//{
				foreach (Item item in items)
				{
					Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
						item.Id, item.Alias, item.Name, item.DateCreated.ToString("dd.MM.yyyy"),
						item.DateLastModified.ToString("dd.MM.yyyy"),
						item.Data.Value.Comments);
				}
			//}
			//catch
			//{
			//	Console.WriteLine("!!! Thread conflict occured");
			//}
		}

		public void MultipleThreadTestCallback()
		{
			Console.WriteLine("Execution of thread {0} complete.", Thread.CurrentThread.Name);
		}
	}
}
