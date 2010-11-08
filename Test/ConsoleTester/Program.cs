using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;
using System.Threading;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;
using System.Diagnostics;
using System.IO;
using Radischevo.Wahha.Web.Scripting.Templates;
using Radischevo.Wahha.Web.Routing;
using Radischevo.Wahha.Web.Mvc;
using System.Globalization;
using Radischevo.Wahha.Web;
using Radischevo.Wahha.Web.Abstractions;
using System.Web;

namespace ConsoleTester
{
	class Program
	{
		static Random _random = new Random();

		static void Main(string[] args)
		{
			Program p = new Program();
			//p.MultipleThreadTest();
			//p.SingleThreadTest();
			//p.RouteTest();

			Console.ReadKey();
		}

		public void RouteTest()
		{
			RequestContext context = new RequestContext(
				new HttpContextWrapper(new HttpContext(
					new HttpRequest("default.aspx", "http://sergey.starcafe.com/blog/797.html", null),
					new HttpResponse(Console.Out))), new RouteData()
				);

			RouteTable.Routes.Add("blog-item", new Route("{user}.[host].{tld}/blog/{id}.html", new MvcRouteHandler()));
			RouteTable.Routes.Variables.Add("host", "inthecity");

			var data = RouteTable.Routes.GetVirtualPath(context, "blog-item", new ValueDictionary(new {
				user = "sergey", id = 15, tld = "ru"
			}));

			Console.WriteLine(data.VirtualPath);

			RouteTable.Routes.Variables["host"] = "men.inthecity";

			data = RouteTable.Routes.GetVirtualPath(context, "blog-item", new ValueDictionary(new {
				user = "max", id = 797, tld = "ru"
			}));

			Console.WriteLine(data.VirtualPath);
			// http://ksu.mysite.ru/blog/14898.html
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
			try
			{
				foreach (Item item in items)
				{
					Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",
						item.Id, item.Alias, item.Name, item.DateCreated.ToString("dd.MM.yyyy"),
						item.DateLastModified.ToString("dd.MM.yyyy"), 
						item.Data.Value.Comments);

					//foreach (ItemData data in item.Values)
					//	Console.WriteLine("data => {0}", data.Comments);
				}
			}
			catch
			{
				Console.WriteLine("!!! Thread conflict occured");
			}
		}

		public void MultipleThreadTestCallback()
		{
			Console.WriteLine("Execution of thread {0} complete.", Thread.CurrentThread.Name);
		}
	}
}
