using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;
using System.Data;
using Radischevo.Wahha.Data.Provider;

public class Blog
{
	public class Materializer : IDbMaterializer<Blog>
	{
		#region IDbMaterializer<Blog> Members
		public Blog Materialize(IValueSet source)
		{
			return Materialize(new Blog(), source);
		}

		public Blog Materialize(Blog entity, IValueSet source)
		{
			entity.Id = source.GetValue<int>("Id");
			entity.Title = source.GetValue<string>("Title");
			entity.Description = source.GetValue<string>("Description");

			return entity;
		}
		#endregion
	}

	public int Id
	{
		get;
		set;
	}

	public string Title
	{
		get;
		set;
	}

	public string Description
	{
		get;
		set;
	}
}

public class BlogPost
{
	public class Materializer : IDbMaterializer<BlogPost>
	{
		#region IDbMaterializer<BlogPost> Members
		public BlogPost Materialize(IValueSet source)
		{
			return Materialize(new BlogPost(), source);
		}

		public BlogPost Materialize(BlogPost entity, IValueSet source)
		{
			IRepository<Blog, int> blogs = new BlogRepository();
			IDbMaterializer<Blog> bm = new Blog.Materializer();

			entity.Id = source.GetValue<int>("Id");
			entity.Title = source.GetValue<string>("Title");
			entity.Text = source.GetValue<string>("Text");
			entity.DateCreated = source.GetValue<DateTime>("CreationDate");

			int blogId = source.GetValue<int>("BlogId");
			entity._blog.Source = () => blogs.Single(blogId);
			entity._blog.Tag = blogId;

			if (source.Keys.Contains("BlogTitle"))
				entity._blog.Value = bm.Materialize(source
					.Subset(key => key.StartsWith("Blog"))
					.Transform(key => key.Substring(4)));

			return entity;
		}
		#endregion
	}

	public BlogPost()
	{
		_blog = new Link<Blog>();
	}

	private Link<Blog> _blog; 

	public int Id
	{
		get;
		set;
	}

	public string Title
	{
		get;
		set;
	}

	public string Text
	{
		get;
		set;
	}

	public DateTime DateCreated
	{
		get;
		set;
	}

	public Blog Blog
	{
		get
		{
			return _blog.Value;
		}
		set
		{
			_blog.Value = value;
		}
	}
}

public class BlogRepository : KeyedDbRepository<Blog, int>
{
	public BlogRepository()
		: base(new Blog.Materializer())
	{
		ExpirationTimeout = TimeSpan.FromMinutes(10);
		DataProvider = DbDataProvider.Create<SqlDbDataProvider>(@"Initial Catalog=vekrosta.ru;Data Source=iceberg\sqlexpress;Packet Size=4096;User ID=asp;Password=asp");
	}

	protected override DbCommandDescriptor CreateEntitySelector(int key)
	{
		return new DbCommandDescriptor(@"SELECT B.* FROM [dbo].[tbBlogs] B WHERE B.[ID] = @id", 
			CommandType.Text, new { id = key });
	}

	protected override int ExtractKey(IValueSet values)
	{
		return values.GetValue<int>("Id");
	}

	protected override Blog ExecuteSave(Blog entity)
	{
		return entity;
	}

	protected override Blog ExecuteDelete(Blog entity)
	{
		return entity;
	}
}

public class BlogPostRepository : KeyedDbRepository<BlogPost, int>
{
	public BlogPostRepository()
		: base(new BlogPost.Materializer())
	{
		ExpirationTimeout = TimeSpan.FromMinutes(10);
		DataProvider = DbDataProvider.Create<SqlDbDataProvider>(@"Initial Catalog=vekrosta.ru;Data Source=iceberg\sqlexpress;Packet Size=4096;User ID=asp;Password=asp");
	}

	protected override DbCommandDescriptor CreateEntitySelector(int key)
	{
		return new DbCommandDescriptor(@"SELECT P.* FROM [dbo].[tbBlogPosts] P WHERE P.[ID] = @id", 
			CommandType.Text, new { id = key });
	}

	protected override int ExtractKey(IValueSet values)
	{
		return values.GetValue<int>("Id");
	}

	protected override BlogPost ExecuteSave(BlogPost entity)
	{
		return entity;
	}

	protected override BlogPost ExecuteDelete(BlogPost entity)
	{
		return entity;
	}

	public IEnumerable<BlogPost> Last(int count)
	{
		return Select(new DbCommandDescriptor(@"SELECT TOP (@count) P.*, B.[Title] AS [BlogTitle], B.[Description] AS [BlogDescription] 
			FROM [dbo].[tbBlogPosts] P 
			INNER JOIN [dbo].[tbBlogs] B ON P.[BlogID] = B.[ID]
			ORDER BY P.[CreationDate] DESC
			SELECT COUNT([ID]) FROM [dbo].[tbBlogPosts]", 
			CommandType.Text, new { count = count }));
	}
}
