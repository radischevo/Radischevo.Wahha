using System;
using Radischevo.Wahha.Core;
using System.Reflection;

namespace ConsoleTester
{
	[Serializable]
	public class Item
	{
		public Item()
		{
			_data = new Link<ItemData>();
			RawData = new ItemData();
		}

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

		public string Alias
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public DateTime DateCreated
		{
			get;
			set;
		}

		public DateTime DateLastModified
		{
			get;
			set;
		}

		public ItemData RawData
		{
			get;
			set;
		}

		private Link<ItemData> _data;

		public Link<ItemData> Data
		{
			get
			{
				return _data;
			}
		}
	}

	[Serializable]
	public class Money
	{
		private decimal _value;
		private string _currency;

		public Money()
		{
		}

		public Money(decimal value, 
			string currency)
		{
			_value = value;
			_currency = currency;
		}

		public decimal Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public string Currency
		{
			get
			{
				return _currency;
			}
			set
			{
				_currency = value;
			}
		}
	}

	[Serializable]
	public class ItemData
	{
		public ItemData()
		{
			Amount = new Money();
		}

		public long Item
		{
			get;
			set;
		}

		public Money Amount
		{
			get;
			set;
		}

		public int Length
		{
			get;
			set;
		}

		public float Percent
		{
			get;
			set;
		}

		public string Comments
		{
			get;
			set;
		}
	}
}
