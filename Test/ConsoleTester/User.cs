using System;
using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data;

namespace ConsoleTester
{
	public class Container
	{
		#region Instance Fields
		private DateTime _date;
		#endregion
		
		#region Constructors
		public Container()
		{
		}
		
		public Container(DateTime date)
		{
			_date = date;
		}
		#endregion
		
		#region Instance Properties
		public DateTime Date
		{
			get 
			{
				return _date;
			}
			set
			{
				_date = value;
			}
		}
		#endregion
	}
	
	public class User
	{
		#region Instance Fields
		private Link<Speciality> _specialityLink;
		private Link<Profile> _profileLink;
		#endregion

		#region Constructors
		public User()
		{
			_specialityLink = new Link<Speciality>();
			_profileLink = new Link<Profile>(new Profile());
		}
		#endregion

		#region Instance Properties
		public long Id
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		public string ActivationKey
		{
			get;
			set;
		}

		public DateTime DateRegistered
		{
			get;
			set;
		}

		public DateTime DateLastVisited
		{
			get;
			set;
		}

		public Speciality Speciality
		{
			get
			{
				return _specialityLink.Value;
			}
			set
			{
				_specialityLink.Value = value;
			}
		}

		public Profile Profile
		{
			get
			{
				return _profileLink.Value;
			}
		}

		internal Link<Profile> ProfileLink
		{
			get
			{
				return _profileLink;
			}
		}

		internal Link<Speciality> SpecialityLink
		{
			get
			{
				return _specialityLink;
			}
		}
		#endregion
	}

	public class Profile
	{
		#region Instance Properties
		public string FirstName
		{
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}

		public string NotificationEmail
		{
			get;
			set;
		}
		#endregion
	}

	public class Speciality
	{
		#region Instance Properties
		public long Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}
		#endregion
	}

	public class UserMaterializer : DbMaterializer<User>
	{
		protected override User CreateInstance(IDbValueSet source)
		{
			return new User();
		}

		protected override User Execute(User entity, IDbValueSet source)
		{
			entity.Id = source.GetValue<long>("Id");
			entity.Email = source.GetValue<string>("Email");
			entity.ActivationKey = source.GetValue<string>("ActivationKey");
			entity.DateRegistered = source.GetValue<DateTime>("DateRegistered");
			entity.DateLastVisited = source.GetValue<DateTime>("DateLastVisited");

			Associate(entity.ProfileLink).Subset("Profile.").Apply<ProfileMaterializer>(source);
			Associate(entity.SpecialityLink).With(() => new SingleSpecialityOperation(
				source.GetValue<long>("speciality.Id"))).Subset("Speciality.")
				.Defined("Id").Apply<SpecialityMaterializer>(source);

			return entity;
		}
	}

	public class SpecialityMaterializer : DbMaterializer<Speciality>
	{
		protected override Speciality CreateInstance(IDbValueSet source)
		{
			return new Speciality();
		}

		protected override Speciality Execute(Speciality entity, IDbValueSet source)
		{
			entity.Id = source.GetValue<long>("Id");
			entity.Name = source.GetValue<string>("Name");

			return entity;
		}
	}

	public class ProfileMaterializer : DbMaterializer<Profile>
	{
		protected override Profile CreateInstance(IDbValueSet source)
		{
			throw new InvalidOperationException("Instance creation is not supported.");
		}

		protected override Profile Execute(Profile entity, IDbValueSet source)
		{
			entity.FirstName = source.GetValue<string>("FirstName");
			entity.LastName = source.GetValue<string>("LastName");
			entity.NotificationEmail = source.GetValue<string>("NotificationEmail");

			return entity;
		}
	}

	public class SingleSpecialityOperation : DbSingleOperation<Speciality>
	{
		private long _id;

		public SingleSpecialityOperation(long id)
			: base(new SpecialityMaterializer())
		{
			_id = id;
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor("SELECT [Id], [Name] FROM [dbo].[Workle.Specialities] WHERE [Id]=@id",
				new {
					id = _id
				});
		}
	}

	public class SelectUsersOperation : DbSubsetOperation<User>
	{
		private int _count;

		public SelectUsersOperation(int count)
			: base(new UserMaterializer())
		{
			_count = count;
		}

		protected override DbCommandDescriptor CreateCommand()
		{
			return new DbCommandDescriptor(@"SELECT TOP(@count) A.[Id], A.[Email], A.[ActivationKey], 
				A.[DateRegistered], A.[DateLastVisited], B.[Id] AS [Speciality.Id], B.[Name] AS [Speciality.Name], 
				C.[FirstName] AS [Profile.FirstName], C.[LastName] AS [Profile.LastName], C.[NotificationEmail] AS [Profile.NotificationEmail]
				FROM [dbo].[Workle.Users] AS [A] LEFT JOIN [dbo].[Workle.Specialities] AS [B] ON [A].[MainSpecialityId] = [B].[Id]
				JOIN [dbo].[Workle.Profiles] AS [C] ON [A].[Id] = [C].[Id] WHERE [A].[Deleted] = 0;

				SELECT COUNT(1) FROM [dbo].[Workle.Users] AS [A] LEFT JOIN [dbo].[Workle.Specialities] AS [B] ON [A].[MainSpecialityId] = [B].[Id]
				JOIN [dbo].[Workle.Profiles] AS [C] ON [A].[Id] = [C].[Id] WHERE [A].[Deleted] = 0;", new { count = _count });
				
		}
	}
}
