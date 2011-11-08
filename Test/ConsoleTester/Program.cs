using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;
using Radischevo.Wahha.Data.Serialization;
using Radischevo.Wahha.Web.Routing;
using Radischevo.Wahha.Web.Mvc;
using Radischevo.Wahha.Web.Text;

using A = Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Data.Provider;
using System.Threading;

namespace ConsoleTester
{
	class Program
	{
		static Random _random = new Random();

		static void Main(string[] args)
		{
			Program p = new Program();
			//p.SgmlTest();
			p.TransactionTest();
			Console.ReadKey();
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

		public void TransactionTest()
		{
			var selectOperation = new TextQueryOperation("SELECT [Key] FROM [dbo].[Workle.Users] WHERE [Id]=@id", new {
				id = 1
			}, "global", "users");
			while (true)
			{
				using (DbOperationScope outer = new DbOperationScope())
				{
					Console.WriteLine("Current => {0}", outer.Execute(selectOperation).Scalar<Guid>());
					var key = Guid.NewGuid();
					var updateOperation = new TextModifyOperation("UPDATE [dbo].[Workle.Users] SET [Key]=@key WHERE [Id]=@id",
						new {
							id = 1,
							key = key
						}, "users");
					outer.Execute(updateOperation);
					Console.WriteLine("Key changed to {0}", key);
					Console.WriteLine("Updated => {0}", outer.Execute(selectOperation).Scalar<Guid>());
				}
				Thread.Sleep(4000);
			}
		}
	}
}
