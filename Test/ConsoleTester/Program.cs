using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Transactions;

using Radischevo.Wahha.Data.Mapping;

namespace ConsoleTester
{
	class Program
	{
		//static Random _random = new Random();

		static void Main(string[] args)
		{
			Program p = new Program();
			//p.SgmlTest();
			//p.TransactionTest();
			//var threads = p.TransactionTestThread();
			
			p.MappingTest();
			//p.XmlTest();
			//p.MaterializerTest2();

			Console.ReadKey();
			//threads.ForEach(thread => thread.Abort());
		}
		
		public void XmlTest()
		{
			/*string xml = @"<model>
				<id>10</id>
				<title>Sample</title>
				<files>
					<file title=""sample.png"" url=""/storage/17120ac_12167612"" />
					<file title=""sample-1.png"" url=""/storage/41661fc_17272662"" />
				</files>
				<beverages>
					<vodka>
						<volume>0.500</volume>
						<degrees>40.0</degrees>
					</vodka>
					<beer>
						<volume>0.500</volume>
						<degrees>5.2</degrees>
					</beer>
				</beverages>
			</model>";*/
		}
		
		public void MappingTest()
		{
			var provider = new AttributedMetaMappingFactory();
			var map1 = provider.CreateMapping(typeof(Topic));
			var map2 = provider.CreateMapping(typeof(Comment));
			
			var topic = new Topic();
			topic.Title = "Yourbunnywrote";
			
			var field = map1.Members["title"];
			Console.WriteLine(field.Accessor.GetValue(topic));
			field.Accessor.SetValue(topic, "My New Title");
			
			Console.WriteLine(map1.Table);
			Console.WriteLine(map2.Table);
			
			var serializer = map1.Serializer;
			
			var item = (Topic)serializer.Deserialize(new ValueDictionary(new {
				id = 10050,
				dateCreated = 129708701869781110,
				title = "Why Linq-to-SQL is so fucking slow?",
				content = "Just because"
			}).ToDbValueSet());
			
			using (Stream stream = new FileStream("/home/sergey/Projects/output.txt", FileMode.Create, FileAccess.Write))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, item);
			}
			Console.WriteLine(item.Id);
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
					outer.IsolationLevel = System.Data.IsolationLevel.RepeatableRead;

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

		public void MaterializerTest2()
		{
			var users = SelectUsers(30);
			Console.WriteLine("Displaying {0} of {1} users.", users.Count(), users.Total());
			Console.WriteLine();
			foreach (var user in users)
			{
				Console.WriteLine("{0} => {1}, {2} {3}, {4} ({5})",
					user.Id, user.Email, user.Profile.FirstName, user.Profile.LastName,
					user.Speciality.Evaluate(a => a.Id), user.Speciality.Evaluate(a => a.Name));
			}
		}

		private IEnumerable<User> SelectUsers(int count)
		{
			var operation = new SelectUsersOperation(count);
			using (var scope = new DbOperationScope())
			{
				return scope.Execute(operation);
			}
		}
	}
}
