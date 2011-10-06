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
using Radischevo.Wahha.Data.Serialization;

namespace ConsoleTester
{
	class SerializedItem
	{
		public SerializedItem()
		{
			Currencies = new HashSet<string>();
		}

		public ICollection<string> Currencies
		{
			get;
			set;
		}

		public bool Required
		{
			get;
			set;
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
			//p.SgmlTest();
			//p.InheritanceTest();
			//p.JsonTest();
			//p.ValueSetTest();
			p.ReaderTest();
			p.DeserializeTest();

			Console.ReadKey();
		}

		public void ReaderTest()
		{
			var selectCommand = new ExecuteQueryCommand(0, 100);
			DbQueryResult results;
			using (var scope = new DbOperationScope())
			{
				results = scope.Execute(selectCommand);
			}

			using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "cities.dat"), FileMode.Create, FileAccess.Write))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(fs, results);
			}
			Console.WriteLine("Serialization done");
		}

		public void DeserializeTest()
		{
			using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "cities.dat"), FileMode.OpenOrCreate, FileAccess.Read))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				DbQueryResult results = (DbQueryResult)formatter.Deserialize(fs);

				using (var reader = new DbQueryResultReader(results))
				{
					while (reader.Read())
					{
						Console.WriteLine("[ id: {0}, title: {1}, region: {2} ]",
							reader.GetValue<long>("id"), reader.GetValue<string>("title"),
							reader.GetValue<long>("region.id"));
					}
					if (reader.NextResult() && reader.Read())
						Console.WriteLine("Total cities: {0}", reader.GetInt32(0));
				}
				Console.WriteLine("Task completed");
			}
		}

		public void ValueSetTest()
		{
			ValueDictionary values = new ValueDictionary();
			values["id"] = 10;
			values["name"] = "Name";
			values["region.id"] = 5;
			values["region.country.id"] = 1;

			IDbValueSet dbs = values.ToDbValueSet();
			Console.Write("id = {0}, ", dbs["id"]);
			Console.Write("name = {0}, ", dbs["name"]);

			IDbValueSet rs = dbs.Subset(k => k.StartsWith("region.")).Transform(k => k.Substring("region.".Length));
			Console.Write("region = {0}, ", rs["id"]);

			IDbValueSet cs = rs.Subset(k => k.StartsWith("country.")).Transform(k => k.Substring("country.".Length));
			Console.WriteLine("country = {0}", cs["id"]);

			Console.WriteLine("Accessed keys total: {0}", String.Join(", ", dbs.AccessedKeys));
			Console.WriteLine("Accessed keys in region: {0}", String.Join(", ", rs.AccessedKeys));
			Console.WriteLine("Accessed keys in country: {0}", String.Join(", ", cs.AccessedKeys));
		}

		public void JsonTest()
		{
			string data = @"{""Currencies"":null,""Required"":false}";
			JavaScriptSerializer service = new JavaScriptSerializer();
			var result = service.Deserialize(typeof(SerializedItem), data);
			Console.Write(result);
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
				new HttpRequest("default.aspx", "https://sergey.starcafe.ru/blog/797.html", null),
				new HttpResponse(Console.Out)));
			
			RequestContext context = new RequestContext(httpContext, new RouteData());

			Route blogRoute = new Route("{user}.[host]/blog/{id}.html", new MvcRouteHandler());
			blogRoute.SecureConnection = SecureConnectionOption.Preferred;

			RouteTable.Routes.Add("blog-list", blogRoute);
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
			var insertCommand = new InsertCityCommand(new City() {
				Title = "Мухосранск", RegionId = 3140
			});
			var insertCommand2 = new InsertCityCommand(new City() {
				Title = "Жепьебрильск", RegionId = 3140
			});
			var insertCommand3 = new InsertCityCommand(new City() {
				Title = "Собянинск",
				RegionId = 3140
			});

			using (var scope = new DbOperationScope())
			{
				PrintCities(scope.Execute(selectCommand));

				City added = scope.Execute(insertCommand);
				PrintCity(added);

				scope.Commit();

				City added2 = scope.Execute(insertCommand2);
				PrintCity(added2);

				scope.Commit();

				City added3 = scope.Execute(insertCommand3);
				PrintCity(added3);
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
