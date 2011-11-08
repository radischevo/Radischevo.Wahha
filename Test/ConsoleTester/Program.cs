using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;
using DC = Radischevo.Wahha.Data.Configurations;
using Radischevo.Wahha.Data.Serialization;
using Radischevo.Wahha.Web.Routing;
using Radischevo.Wahha.Web.Mvc;
using Radischevo.Wahha.Web.Text;

using A = Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Data.Provider;
using System.Threading;
using Radischevo.Wahha.Data.Caching;

namespace ConsoleTester
{
	class Program
	{
		static Random _random = new Random();

		static void Main(string[] args)
		{
			Program p = new Program();
			//p.SgmlTest();
			//p.TransactionTest();
			var threads = p.TransactionTestThread();

			Console.ReadKey();
			threads.ForEach(thread => thread.Abort());
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
					outer.IsolationLevel = System.Data.IsolationLevel.ReadUncommitted;

					Guid currentKey = outer.Execute(selectOperation).Scalar<Guid>();
					Console.WriteLine("Thread {0}, Current => {1}", 
						Thread.CurrentThread.ManagedThreadId, currentKey);

					var key = Guid.NewGuid();
					var updateOperation = new TextModifyOperation("UPDATE [dbo].[Workle.Users] SET [Key]=@key WHERE [Id]=@id AND [Key]=@current",
						new {
							id = 1,
							key = key,
							current = currentKey
						}, "users");

					Console.WriteLine("Thread {0}, Key changed to {1}, {2} rows affected",
						Thread.CurrentThread.ManagedThreadId, key, outer.Execute(updateOperation));

					Thread.Sleep(4000);

					Console.WriteLine("Thread {0}, Updated => {1}\n", 
						Thread.CurrentThread.ManagedThreadId, 
						outer.Execute(selectOperation).Scalar<Guid>());

					outer.Commit();
				}
			}
		}

		private void TransactionTest2()
		{
			var selectOperation = new TextQueryOperation("SELECT [Key] FROM [dbo].[Workle.Users] WHERE [Id]=@id", new {
				id = 1
			}, "global", "users");
			while (true)
			{
				using (DbOperationScope outer = new DbOperationScope())
				{
					Console.WriteLine("Select => {0}", 
						outer.Execute(selectOperation).Scalar<Guid>());
				}
				Thread.Sleep(2400);
			}
		}

		public IEnumerable<Thread> TransactionTestThread()
		{
			DC.Configuration.Instance.Caching.ProviderType = typeof(InMemoryCacheProvider);

			Thread thread1 = new Thread(TransactionTest);
			Thread thread2 = new Thread(TransactionTest2);

			thread1.Start();
			thread2.Start();

			return new Thread[] { thread1, thread2 };
		}
	}
}
