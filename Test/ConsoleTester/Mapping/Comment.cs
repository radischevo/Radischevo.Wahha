using System;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Data.Mapping;

namespace ConsoleTester
{
	[Table(Name = "Wahha.Comments", Schema = "dbo", Projection = "Wahha.Topics.Default")]
	public class Comment
	{
		#region Instance Fields
		[Column(Name = "id", IsKey = true, AutoGenerated = true)]
		private long _id;
		
		private string _content;
		private DateTime _dateCreated;
		
		[Association(Persistent = true, ThisKey = "topic", OtherKey = "id", Prefix = "topic.", CanBeNull = false)]
		private Link<Topic> _topic;
		
		[Column(Name = "attachment")]
		private Link<byte[]> _veryLongAttachment;
		#endregion
		
		#region Constructors
		public Comment ()
		{
			_topic = new Link<Topic>();
			_veryLongAttachment = new Link<byte[]>();
		}
		#endregion
		
		#region Instance Properties
		public long Id 
		{
			get
			{
				return _id;
			}
		}
		
		public DateTime DateCreated
		{
			get
			{
				return _dateCreated;
			}
			set
			{
				_dateCreated = value;
			}
		}
		
		[Column(Name = "content")]
		public string Content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
			}
		}
		
		public Topic Topic
		{
			get
			{
				return _topic.Value;
			}
			set
			{
				_topic.Value = value;
			}
		}
		
		public byte[] VeryLongAttachment
		{
			get
			{
				return _veryLongAttachment.Value;
			}
			set
			{
				_veryLongAttachment.Value = value;
			}
		}
		#endregion
	}
}

