using System;
using Radischevo.Wahha.Core;

namespace ConsoleTester
{
	public class Item
	{
		public Item()
		{
			_data = new Link<ItemData>();
			_values = new EnumerableLink<ItemData>();
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

		private EnumerableLink<ItemData> _values;

		public EnumerableLink<ItemData> Values
		{
			get
			{
				return _values;
			}
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

	public struct Money
	{
		private decimal _value;
		private string _currency;

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

	public class ItemData
	{
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
