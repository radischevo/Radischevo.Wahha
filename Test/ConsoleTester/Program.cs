using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;
using System.Threading;

using Radischevo.Wahha.Core;
using System.Diagnostics;
using System.IO;
using Radischevo.Wahha.Web.Routing;
using Radischevo.Wahha.Web.Mvc;
using System.Web;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Radischevo.Wahha.Web.Text;

using A = Radischevo.Wahha.Web.Abstractions;

namespace ConsoleTester
{
	public class Maza
	{
		public int Value
		{
			get
			{
				return 10;
			}
		}
	}

	class Program
	{
		static Random _random = new Random();

		static void Main(string[] args)
		{
			Program p = new Program();
			//p.MultipleThreadTest();
			//p.SingleThreadTest();
			//p.RouteTest();
			p.SgmlTest();
			//p.InheritanceTest();

			Console.ReadKey();
		}

		private void SerializeLink(Item item)
		{
			using (FileStream fs = new FileStream(Path.Combine(
				Environment.CurrentDirectory, "data-{0}.txt".Format(item.Id)), 
				FileMode.OpenOrCreate, FileAccess.Write))
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(fs, item);
			}
		}

		private Item DeserializeLink(long id)
		{
			using (FileStream fs = new FileStream(Path.Combine(
				Environment.CurrentDirectory, "data-{0}.txt".Format(id)),
				FileMode.Open, FileAccess.Read))
			{
				BinaryFormatter bf = new BinaryFormatter();
				return (Item)bf.Deserialize(fs);
			}
		}

		public void InheritanceTest()
		{
			IEnumerable<Type> controllerTypes = GetDerivedTypes(typeof(IController));
			foreach (Type type in controllerTypes)
				Console.WriteLine(type.FullName);
		}

		private IEnumerable<Type> GetDerivedTypes(Type type)
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type candidate in assembly.GetExportedTypes())
				{
					if (type.IsInterface && type.IsAssignableFrom(candidate) && 
						IsValidType(candidate))
						yield return candidate;

					if (candidate.IsSubclassOf(type) && IsValidType(candidate))
						yield return candidate;
				}
			}
		}

		private bool IsValidType(Type type)
		{
			return !(type.IsAbstract ||
				type.IsInterface || 
				type.IsNested ||
				type.IsGenericTypeDefinition);
		}

		public void RouteTest()
		{
			A.HttpContextBase httpContext = new A.HttpContextWrapper(new HttpContext(
				new HttpRequest("default.aspx", "http://sergey.starcafe.ru/blog/797.html", null),
				new HttpResponse(Console.Out)));
			RequestContext context = new RequestContext(httpContext, new RouteData());

			RouteTable.Routes.Add("blog-list", new Route("{user}.[host]/blog/{id}.html", new MvcRouteHandler()));
			RouteTable.Routes.Variables.Add("host", "starcafe.ru");

			var data = RouteTable.Routes.GetVirtualPath(context, "blog-list", new ValueDictionary(new {
				user = "sergey", id = 127
			}));

			var route = RouteTable.Routes.GetRouteData(httpContext);

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

					SerializeLink(item);

					Item otherItem = DeserializeLink(item.Id);
					Console.WriteLine("Deserialized => {0}", otherItem.Id);
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

		public void SgmlTest()
		{
			HtmlProcessor filter = SgmlSetup();
			SgmlTest1(filter, "jessica-alba", 182);
			SgmlTest1(filter, "bruce-willis", 183);
			
			Console.WriteLine("Complete");
		}

		public HtmlProcessor SgmlSetup()
		{
			HtmlProcessor html = new HtmlProcessor();

			html.Filter.Mode = HtmlFilteringMode.DenyByDefault;
			html.Filter.DefaultOptions = HtmlElementOptions.AllowContent | HtmlElementOptions.UseTypography;

			html.Filter.Treat(a => a.Attributes("xmlns", "ns").As(HtmlAttributeOptions.Denied))
				.RegularContent().Links().Images().Abstract().Youtube();

			html.Typographics.EncodeSpecialSymbols = false;
			html.Typographics.Replaces();

			return html;
		}

		public void SgmlTest1(HtmlProcessor filter, string blog, int topic)
		{
			ValueDictionary parameters = new ValueDictionary();

			parameters["link-domain"] = "starcafe.ru";
			parameters["link-redirect"] = "http://starcafe.ru/redirect";
			parameters["url"] = String.Format("http://starcafe.ru/blog/{0}/{1}.html", blog, topic);

			using (StreamReader sr = new StreamReader(
				Path.Combine(Environment.CurrentDirectory, "sgml-test.htm"), Encoding.UTF8))
			{
				using (StreamWriter sw = new StreamWriter(Path.Combine(Environment.CurrentDirectory, 
					String.Format("html-test-{0}-{1}.htm", blog, topic)), false, Encoding.UTF8))
				{
					sw.Write(filter.Execute(sr, parameters));
				}
			}
		}
	}
}
