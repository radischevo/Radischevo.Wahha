using System;

namespace Radischevo.Wahha.Core
{
	public static class Tuple
	{
		#region Static Methods
		public static Tuple<T1> Create<T1>(T1 item1)
		{
			return new Tuple<T1>(item1);
		}

		public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
		{
			return new Tuple<T1, T2>(item1, item2);
		}

		public static Tuple<T1, T2, T3> Create<T1, T2, T3>(
			T1 item1, T2 item2, T3 item3)
		{
			return new Tuple<T1, T2, T3>(item1, item2, item3);
		}

		public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(
			T1 item1, T2 item2, T3 item3, T4 item4)
		{
			return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
		}

		public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(
			T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
		}

		public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(
			T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
		}

		public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(
			T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7>(
				item1, item2, item3, item4, item5, item6, item7);
		}

		public static Tuple<T1, T2, T3, T4, T5, T6, T7, TOther> Create<T1, T2, T3, T4, T5, T6, T7, TOther>(
			T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TOther item8)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7, TOther>(
				item1, item2, item3, item4, item5, item6, item7, item8);
		}
		#endregion
	}

	public class Tuple<T1>
	{
		#region Instance Fields
		private T1 _item1;
		#endregion

		#region Constructors
		public Tuple()
			: this(default(T1))
		{
		}

		public Tuple(T1 item1)
		{
			_item1 = item1;
		}
		#endregion

		#region Instance Properties
		public T1 Item1
		{
			get
			{
				return _item1;
			}
			set
			{
				_item1 = value;
			}
		}
		#endregion

		#region Instance Methods
		public override bool Equals(object obj)
		{
			if(Object.ReferenceEquals(obj, this))
				return true;

			Tuple<T1> tuple = (obj as Tuple<T1>);
			if (Object.ReferenceEquals(tuple, null))
				return false;

			if (Object.ReferenceEquals(null, _item1))
				return Object.ReferenceEquals(null, tuple._item1);

			return _item1.Equals(tuple._item1);
		}

		public override int GetHashCode()
		{
			return (GetType().GetHashCode() << 5) ^ 
				((_item1 == null) ? 0 : _item1.GetHashCode());
		}

		public override string ToString()
		{
			return String.Format("({0})", Item1);
		}
		#endregion
	}

	public class Tuple<T1, T2> 
		: Tuple<T1>
	{
		#region Instance Fields
		private T2 _item2;
		#endregion

		#region Constructors
		public Tuple()
			: this(default(T1))
		{
		}

		public Tuple(T1 item1)
			: this(item1, default(T2))
		{
		}

		public Tuple(T1 item1, T2 item2)
			: base(item1)
		{
			_item2 = item2;
		}
		#endregion

		#region Instance Properties
		public T2 Item2
		{
			get
			{
				return _item2;
			}
			set
			{
				_item2 = value;
			}
		}
		#endregion

		#region Instance Methods
		public override bool Equals(object obj)
		{
			bool equalsBase = base.Equals(obj);

			Tuple<T1, T2> tuple = (obj as Tuple<T1, T2>);
			if (Object.ReferenceEquals(tuple, null))
				return false;

			if (Object.ReferenceEquals(null, _item2))
				return (equalsBase && Object.ReferenceEquals(null, tuple._item2));
			
			return (equalsBase && _item2.Equals(tuple._item2));
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() << 4) ^
				((_item2 == null) ? 0 : _item2.GetHashCode());
		}

		public override string ToString()
		{
			return String.Format("({0}, {1})", Item1, Item2);
		}
		#endregion
	}

	public class Tuple<T1, T2, T3> 
		: Tuple<T1, T2>
	{
		#region Instance Fields
		private T3 _item3;
		#endregion

		#region Constructors
		public Tuple()
			: this(default(T1))
		{
		}

		public Tuple(T1 item1)
			: this(item1, default(T2))
		{
		}

		public Tuple(T1 item1, T2 item2)
			: this(item1, item2, default(T3))
		{
		}

		public Tuple(T1 item1, T2 item2, T3 item3)
			: base(item1, item2)
		{
			_item3 = item3;
		}
		#endregion

		#region Instance Properties
		public T3 Item3
		{
			get
			{
				return _item3;
			}
			set
			{
				_item3 = value;
			}
		}
		#endregion

		#region Instance Methods
		public override bool Equals(object obj)
		{
			bool equalsBase = base.Equals(obj);

			Tuple<T1, T2, T3> tuple = (obj as Tuple<T1, T2, T3>);
			if (Object.ReferenceEquals(tuple, null))
				return false;

			if (Object.ReferenceEquals(null, _item3))
				return (equalsBase && Object.ReferenceEquals(null, tuple._item3));

			return (equalsBase && _item3.Equals(tuple._item3));
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() << 4) ^
				((_item3 == null) ? 0 : _item3.GetHashCode());
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2})",
				Item1, Item2, Item3);
		}
		#endregion
	}

	public class Tuple<T1, T2, T3, T4> 
		: Tuple<T1, T2, T3>
	{
		#region Instance Fields
		private T4 _item4;
		#endregion

		#region Constructors
		public Tuple()
			: this(default(T1))
		{
		}

		public Tuple(T1 item1)
			: this(item1, default(T2))
		{
		}

		public Tuple(T1 item1, T2 item2)
			: this(item1, item2, default(T3))
		{
		}

		public Tuple(T1 item1, T2 item2, T3 item3)
			: this(item1, item2, item3, default(T4))
		{
		}

		public Tuple(T1 item1, T2 item2, 
			T3 item3, T4 item4)
			: base(item1, item2, item3)
		{
			_item4 = item4;
		}
		#endregion

		#region Instance Properties
		public T4 Item4
		{
			get
			{
				return _item4;
			}
			set
			{
				_item4 = value;
			}
		}
		#endregion

		#region Instance Methods
		public override bool Equals(object obj)
		{
			bool equalsBase = base.Equals(obj);

			Tuple<T1, T2, T3, T4> tuple = (obj as Tuple<T1, T2, T3, T4>);
			if (Object.ReferenceEquals(tuple, null))
				return false;

			if (Object.ReferenceEquals(null, _item4))
				return (equalsBase && Object.ReferenceEquals(null, tuple._item4));

			return (equalsBase && _item4.Equals(tuple._item4));
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() << 4) ^
				((_item4 == null) ? 0 : _item4.GetHashCode());
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2}, {3})",
				Item1, Item2, Item3, Item4);
		}
		#endregion
	}

	public class Tuple<T1, T2, T3, T4, T5> 
		: Tuple<T1, T2, T3, T4>
	{
		#region Instance Fields
		private T5 _item5;
		#endregion

		#region Constructors
		public Tuple()
			: this(default(T1))
		{
		}

		public Tuple(T1 item1)
			: this(item1, default(T2))
		{
		}

		public Tuple(T1 item1, T2 item2)
			: this(item1, item2, default(T3))
		{
		}

		public Tuple(T1 item1, T2 item2, T3 item3)
			: this(item1, item2, item3, default(T4))
		{
		}

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
			: this(item1, item2, item3, item4, default(T5))
		{
		}

		public Tuple(T1 item1, T2 item2,
			T3 item3, T4 item4, T5 item5)
			: base(item1, item2, item3, item4)
		{
			_item5 = item5;
		}
		#endregion

		#region Instance Properties
		public T5 Item5
		{
			get
			{
				return _item5;
			}
			set
			{
				_item5 = value;
			}
		}
		#endregion

		#region Instance Methods
		public override bool Equals(object obj)
		{
			bool equalsBase = base.Equals(obj);

			Tuple<T1, T2, T3, T4, T5> tuple = (obj as Tuple<T1, T2, T3, T4, T5>);
			if (Object.ReferenceEquals(tuple, null))
				return false;

			if (Object.ReferenceEquals(null, _item5))
				return (equalsBase && Object.ReferenceEquals(null, tuple._item5));

			return (equalsBase && _item5.Equals(tuple._item5));
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() << 4) ^
				((_item5 == null) ? 0 : _item5.GetHashCode());
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2}, {3}, {4})",
				Item1, Item2, Item3, Item4, Item5);
		}
		#endregion
	}

	public class Tuple<T1, T2, T3, T4, T5, T6> 
		: Tuple<T1, T2, T3, T4, T5>
	{
		#region Instance Fields
		private T6 _item6;
		#endregion

		#region Constructors
		public Tuple()
			: this(default(T1))
		{
		}

		public Tuple(T1 item1)
			: this(item1, default(T2))
		{
		}

		public Tuple(T1 item1, T2 item2)
			: this(item1, item2, default(T3))
		{
		}

		public Tuple(T1 item1, T2 item2, T3 item3)
			: this(item1, item2, item3, default(T4))
		{
		}

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
			: this(item1, item2, item3, item4, default(T5))
		{
		}

		public Tuple(T1 item1, T2 item2,
			T3 item3, T4 item4, T5 item5)
			: this(item1, item2, item3, item4, item5, 
				default(T6))
		{
		}

		public Tuple(T1 item1, T2 item2,
			T3 item3, T4 item4, T5 item5, T6 item6)
			: base(item1, item2, item3, item4, item5)
		{
			_item6 = item6;
		}
		#endregion

		#region Instance Properties
		public T6 Item6
		{
			get
			{
				return _item6;
			}
			set
			{
				_item6 = value;
			}
		}
		#endregion

		#region Instance Methods
		public override bool Equals(object obj)
		{
			bool equalsBase = base.Equals(obj);

			Tuple<T1, T2, T3, T4, T5, T6> tuple = (obj as Tuple<T1, T2, T3, T4, T5, T6>);
			if (Object.ReferenceEquals(tuple, null))
				return false;

			if (Object.ReferenceEquals(null, _item6))
				return (equalsBase && Object.ReferenceEquals(null, tuple._item6));

			return (equalsBase && _item6.Equals(tuple._item6));
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() << 4) ^
				((_item6 == null) ? 0 : _item6.GetHashCode());
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2}, {3}, {4}, {5})",
				Item1, Item2, Item3, Item4, Item5, Item6);
		}
		#endregion
	}

	public class Tuple<T1, T2, T3, T4, T5, T6, T7> 
		: Tuple<T1, T2, T3, T4, T5, T6>
	{
		#region Instance Fields
		private T7 _item7;
		#endregion

		#region Constructors
		public Tuple()
			: this(default(T1))
		{
		}

		public Tuple(T1 item1)
			: this(item1, default(T2))
		{
		}

		public Tuple(T1 item1, T2 item2)
			: this(item1, item2, default(T3))
		{
		}

		public Tuple(T1 item1, T2 item2, T3 item3)
			: this(item1, item2, item3, default(T4))
		{
		}

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
			: this(item1, item2, item3, item4, default(T5))
		{
		}

		public Tuple(T1 item1, T2 item2,
			T3 item3, T4 item4, T5 item5)
			: this(item1, item2, item3, item4, item5,
				default(T6))
		{
		}

		public Tuple(T1 item1, T2 item2,
			T3 item3, T4 item4, T5 item5, T6 item6)
			: this(item1, item2, item3, item4, item5,
				item6, default(T7))
		{
		}

		public Tuple(T1 item1, T2 item2,
			T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
			: base(item1, item2, item3, item4, item5, item6)
		{
			_item7 = item7;
		}
		#endregion

		#region Instance Properties
		public T7 Item7
		{
			get
			{
				return _item7;
			}
			set
			{
				_item7 = value;
			}
		}
		#endregion

		#region Instance Methods
		public override bool Equals(object obj)
		{
			bool equalsBase = base.Equals(obj);

			Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = (obj as Tuple<T1, T2, T3, T4, T5, T6, T7>);
			if (Object.ReferenceEquals(tuple, null))
				return false;

			if (Object.ReferenceEquals(null, _item7))
				return (equalsBase && Object.ReferenceEquals(null, tuple._item7));

			return (equalsBase && _item7.Equals(tuple._item7));
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() << 4) ^
				((_item7 == null) ? 0 : _item7.GetHashCode());
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2}, {3}, {4}, {5}, {6})",
				Item1, Item2, Item3, Item4, Item5, Item6, Item7);
		}
		#endregion
	}

	public class Tuple<T1, T2, T3, T4, T5, T6, T7, TOther> 
		: Tuple<T1, T2, T3, T4, T5, T6, T7>
	{
		#region Instance Fields
		private TOther _item8;
		#endregion

		#region Constructors
		public Tuple()
			: this(default(T1))
		{
		}

		public Tuple(T1 item1)
			: this(item1, default(T2))
		{
		}

		public Tuple(T1 item1, T2 item2)
			: this(item1, item2, default(T3))
		{
		}

		public Tuple(T1 item1, T2 item2, T3 item3)
			: this(item1, item2, item3, default(T4))
		{
		}

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
			: this(item1, item2, item3, item4, default(T5))
		{
		}

		public Tuple(T1 item1, T2 item2,
			T3 item3, T4 item4, T5 item5)
			: this(item1, item2, item3, item4, item5,
				default(T6))
		{
		}

		public Tuple(T1 item1, T2 item2,
			T3 item3, T4 item4, T5 item5, T6 item6)
			: this(item1, item2, item3, item4, item5,
				item6, default(T7))
		{
		}

		public Tuple(T1 item1, T2 item2,
			T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
			: this(item1, item2, item3, item4, item5,
				item6, item7, default(TOther))
		{
		}

		public Tuple(T1 item1, T2 item2,
			T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TOther item8)
			: base(item1, item2, item3, item4, item5, item6, item7)
		{
			_item8 = item8;
		}
		#endregion

		#region Instance Properties
		public TOther Item8
		{
			get
			{
				return _item8;
			}
			set
			{
				_item8 = value;
			}
		}
		#endregion

		#region Instance Methods
		public override bool Equals(object obj)
		{
			bool equalsBase = base.Equals(obj);

			Tuple<T1, T2, T3, T4, T5, T6, T7, TOther> tuple = 
				(obj as Tuple<T1, T2, T3, T4, T5, T6, T7, TOther>);
			if (Object.ReferenceEquals(tuple, null))
				return false;

			if (Object.ReferenceEquals(null, _item8))
				return (equalsBase && Object.ReferenceEquals(null, tuple._item8));

			return (equalsBase && _item8.Equals(tuple._item8));
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() << 4) ^
				((_item8 == null) ? 0 : _item8.GetHashCode());
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
				Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8);
		}
		#endregion
	}
}
