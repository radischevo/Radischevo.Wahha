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
using Radischevo.Wahha.Data;
using System.Linq.Expressions;

namespace ConsoleTester
{
	class Program
	{
		static Random _random = new Random();

		static void Main(string[] args)
		{
			Program p = new Program();
			//p.MultipleThreadTest();
			p.SingleThreadTest();
			//p.RouteTest();
			//p.SgmlTest();
			//p.InheritanceTest();

			Console.ReadKey();
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
			var selectCommand = new SelectCityCommand();
			var subsetCommand = new SelectCitySubsetCommand(0, 10);
			var singleCommand = new SingleCityCommand(40289);
			var insertCommand = new InsertCityCommand(new City() {
				Title = "Мухосранск", RegionId = 3140
			});

			using (var scope = new DbOperationScope())
			{
				PrintCities(scope.Execute(subsetCommand));

				var oldCity = scope.Execute(singleCommand);
				PrintCity(scope.Execute(singleCommand));

				City added = scope.Execute(insertCommand);
				PrintCity(added);

				var delete = new DeleteCityCommand(oldCity);
				scope.Execute(delete);
			}
		}

		private void PrintCities(IEnumerable<City> cities)
		{
			Console.WriteLine("Printing cities... {0}", cities.Total());
			Console.WriteLine("=============================================");
			Console.WriteLine();

			foreach (City city in cities)
				PrintCity(city);

			Console.WriteLine();
		}

		private void PrintCity(City city)
		{
			if (city == null)
				Console.WriteLine("(NULL)");
			else
				Console.WriteLine("{0} (id: {1}, region: {2})", city.Title, city.Id, city.RegionId);
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
