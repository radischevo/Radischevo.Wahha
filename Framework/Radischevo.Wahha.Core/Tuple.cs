using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Radischevo.Wahha.Core
{
	public abstract class Tuple : IStructuralEquatable, 
		IStructuralComparable, IComparable, ITuple
	{
		#region Constructors
		protected Tuple()
		{
		}
		#endregion

		#region Instance Properties
		protected abstract int Size
		{
			get;
		}
		#endregion

		#region Static Methods
		protected static int CombineHashCodes(int h1, int h2)
		{
			return (((h1 << 5) + h1) ^ h2);
		}
		#endregion

		#region Factory Methods
		public static Tuple<T1> Create<T1>(T1 item1)
		{
			return new Tuple<T1>(item1);
		}

		public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
		{
			return new Tuple<T1, T2>(item1, item2);
		}

		public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
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
			return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, 
				item3, item4, item5, item6, item7);
		}

		public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(
			T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(item1, item2, 
				item3, item4, item5, item6, item7, new Tuple<T8>(item8));
		}
		#endregion

		#region Instance Methods
		protected abstract int CompareTo(object other, IComparer comparer);

		protected abstract bool Equals(object other, IEqualityComparer comparer);

		protected abstract int GetHashCode(IEqualityComparer comparer);

		protected abstract string ToString(StringBuilder builder);

		public override bool Equals(object obj)
		{
			return Equals(obj, EqualityComparer<object>.Default);
		}

		public override int GetHashCode()
		{
			return GetHashCode(EqualityComparer<object>.Default);
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("(");

			return ((ITuple)this).ToString(builder);
		}
		#endregion

		#region Interface Implementations
		int ITuple.Size
		{
			get
			{
				return Size;
			}
		}

		int IComparable.CompareTo(object obj)
		{
			return CompareTo(obj, Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			return CompareTo(other, comparer);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			return Equals(other, comparer);
		}

		int ITuple.GetHashCode(IEqualityComparer comparer)
		{
			return GetHashCode(comparer);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return GetHashCode(comparer);
		}

		string ITuple.ToString(StringBuilder builder)
		{
			return ToString(builder);
		}
		#endregion
	}

	[Serializable]
	public class Tuple<T1> : Tuple
	{
		#region Instance Fields
		private readonly T1 _item1;
		#endregion

		#region Constructors
		public Tuple(T1 item1)
			: base()
		{
			_item1 = item1;
		}
		#endregion

		#region Instance Properties
		protected override int Size
		{
			get
			{
				return 1;
			}
		}

		public T1 Item1
		{
			get
			{
				return _item1;
			}
		}
		#endregion

		#region Instance Methods
		protected override int CompareTo(object other, IComparer comparer)
		{
			if (other == null)
				return 1;

			Tuple<T1> tuple = (other as Tuple<T1>);
			Precondition.Require(tuple, () => Error.TypeIsNotTuple(other, "other"));

			return comparer.Compare(Item1, tuple.Item1);
		}

		protected override bool Equals(object other, IEqualityComparer comparer)
		{
			Tuple<T1> tuple = (other as Tuple<T1>);
			return (tuple == null) ? false : comparer.Equals(Item1, tuple.Item1);
		}

		protected override int GetHashCode(IEqualityComparer comparer)
		{
			return comparer.GetHashCode(Item1);
		}

		protected override string ToString(StringBuilder builder)
		{
			return builder.Append(Item1).Append(")").ToString();
		}
		#endregion
	}

	[Serializable]
	public class Tuple<T1, T2> : Tuple<T1>
	{
		#region Instance Fields
		private readonly T2 _item2;
		#endregion

		#region Constructors
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
		}

		protected override int Size
		{
			get
			{
				return 2;
			}
		}
		#endregion

		#region Instance Methods
		protected override int CompareTo(object other, IComparer comparer)
		{
			if (other == null)
				return 1;

			Tuple<T1, T2> tuple = (other as Tuple<T1, T2>);
			Precondition.Require(tuple, () => Error.TypeIsNotTuple(other, "other"));

			int result = base.CompareTo(other, comparer);
			if (result != 0)
				return result;

			return comparer.Compare(Item2, tuple.Item2);
		}

		protected override bool Equals(object other, IEqualityComparer comparer)
		{
			if (other == null)
				return false;

			Tuple<T1, T2> tuple = (other as Tuple<T1, T2>);
			if (tuple == null)
				return false;

			return base.Equals(other, comparer) &&
				comparer.Equals(Item2, tuple.Item2);
		}

		protected override int GetHashCode(IEqualityComparer comparer)
		{
			return CombineHashCodes(base.GetHashCode(comparer), 
				comparer.GetHashCode(Item2));
		}

		protected override string ToString(StringBuilder builder)
		{
			return builder.Append(Item1).Append(", ")
				.Append(Item2).Append(")").ToString();
		}
		#endregion
	}

	[Serializable]
	public class Tuple<T1, T2, T3> : Tuple<T1, T2>
	{
		#region Instance Fields
		private readonly T3 _item3;
		#endregion

		#region Constructors
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
		}

		protected override int Size
		{
			get
			{
				return 3;
			}
		}
		#endregion

		#region Instance Methods
		protected override int CompareTo(object other, IComparer comparer)
		{
			if (other == null)
				return 1;

			Tuple<T1, T2, T3> tuple = (other as Tuple<T1, T2, T3>);
			Precondition.Require(tuple, () => Error.TypeIsNotTuple(other, "other"));

			int result = base.CompareTo(other, comparer);
			if (result != 0)
				return result;

			return comparer.Compare(Item3, tuple.Item3);
		}

		protected override bool Equals(object other, IEqualityComparer comparer)
		{
			if (other == null)
				return false;

			Tuple<T1, T2, T3> tuple = (other as Tuple<T1, T2, T3>);
			if (tuple == null)
				return false;

			return base.Equals(other, comparer) &&
				comparer.Equals(Item3, tuple.Item3);
		}

		protected override int GetHashCode(IEqualityComparer comparer)
		{
			return Tuple.CombineHashCodes(
				base.GetHashCode(comparer),
				comparer.GetHashCode(Item3));
		}

		protected override string ToString(StringBuilder builder)
		{
			return builder.Append(Item1)
				.Append(", ").Append(Item2).Append(", ")
				.Append(Item3).Append(")").ToString();
		}
		#endregion
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
	{
		#region Instance Fields
		private readonly T4 _item4;
		#endregion

		#region Constructors
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
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
		}

		protected override int Size
		{
			get
			{
				return 4;
			}
		}
		#endregion

		#region Instance Methods
		protected override int CompareTo(object other, IComparer comparer)
		{
			if (other == null)
				return 1;

			Tuple<T1, T2, T3, T4> tuple = (other as Tuple<T1, T2, T3, T4>);
			Precondition.Require(tuple, () => Error.TypeIsNotTuple(other, "other"));

			int result = base.CompareTo(other, comparer);
			if(result != 0)
				return result;

			return comparer.Compare(Item4, tuple.Item4);
		}

		protected override bool Equals(object other, IEqualityComparer comparer)
		{
			if (other == null)
				return false;

			Tuple<T1, T2, T3, T4> tuple = (other as Tuple<T1, T2, T3, T4>);
			if (tuple == null)
				return false;

			return base.Equals(other, comparer) &&
				comparer.Equals(Item4, tuple.Item4);
		}

		protected override int GetHashCode(IEqualityComparer comparer)
		{
			return CombineHashCodes(
				base.GetHashCode(comparer),
				comparer.GetHashCode(Item4));
		}

		protected override string ToString(StringBuilder builder)
		{
			return builder.Append(Item1).Append(", ")
				.Append(Item2).Append(", ")
				.Append(Item3).Append(", ")
				.Append(Item4).Append(")")
				.ToString();
		}
		#endregion
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5> : Tuple<T1, T2, T3, T4>
	{
		#region Instance Fields
		private readonly T5 _item5;
		#endregion

		#region Constructors
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
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
		}

		protected override int Size
		{
			get
			{
				return 5;
			}
		}
		#endregion

		#region Instance Methods
		protected override int CompareTo(object other, IComparer comparer)
		{
			if (other == null)
				return 1;

			Tuple<T1, T2, T3, T4, T5> tuple = (other as Tuple<T1, T2, T3, T4, T5>);
			Precondition.Require(tuple, () => Error.TypeIsNotTuple(other, "other"));

			int result = base.CompareTo(other, comparer);
			if (result != 0)
				return result;

			return comparer.Compare(Item5, tuple.Item5);
		}

		protected override bool Equals(object other, IEqualityComparer comparer)
		{
			if (other == null)
				return false;

			Tuple<T1, T2, T3, T4, T5> tuple = (other as Tuple<T1, T2, T3, T4, T5>);
			if (tuple == null)
				return false;

			return base.Equals(other, comparer) && 
				comparer.Equals(Item5, tuple.Item5);
		}

		protected override int GetHashCode(IEqualityComparer comparer)
		{
			return CombineHashCodes(
				base.GetHashCode(comparer),
				comparer.GetHashCode(Item5));
		}

		protected override string ToString(StringBuilder builder)
		{
			return builder.Append(Item1).Append(", ")
				.Append(Item2).Append(", ")
				.Append(Item3).Append(", ")
				.Append(Item4).Append(", ")
				.Append(Item5).Append(")")
				.ToString();
		}
		#endregion
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6> : Tuple<T1, T2, T3, T4, T5>
	{
		#region Instance Fields
		private readonly T6 _item6;
		#endregion

		#region Constructors
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
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
		}

		protected override int Size
		{
			get
			{
				return 6;
			}
		}
		#endregion

		#region Instance Methods
		protected override int CompareTo(object other, IComparer comparer)
		{
			if (other == null)
				return 1;

			Tuple<T1, T2, T3, T4, T5, T6> tuple = (other as Tuple<T1, T2, T3, T4, T5, T6>);
			Precondition.Require(tuple, () => Error.TypeIsNotTuple(other, "other"));

			int result = base.CompareTo(other, comparer);
			if (result != 0)
				return result;

			return comparer.Compare(Item6, tuple.Item6);
		}

		protected override bool Equals(object other, IEqualityComparer comparer)
		{
			if (other == null)
				return false;

			Tuple<T1, T2, T3, T4, T5, T6> tuple = (other as Tuple<T1, T2, T3, T4, T5, T6>);
			if (tuple == null)
				return false;

			return base.Equals(other, comparer) &&
				comparer.Equals(Item6, tuple.Item6);
		}

		protected override int GetHashCode(IEqualityComparer comparer)
		{
			return CombineHashCodes(
				base.GetHashCode(comparer),
				comparer.GetHashCode(Item6));
		}

		protected override string ToString(StringBuilder builder)
		{
			return builder.Append(Item1).Append(", ")
				.Append(Item2).Append(", ")
				.Append(Item3).Append(", ")
				.Append(Item4).Append(", ")
				.Append(Item5).Append(", ")
				.Append(Item6).Append(")")
				.ToString();
		}
		#endregion		
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6, T7> : Tuple<T1, T2, T3, T4, T5, T6>
	{
		#region Instance Fields
		private readonly T7 _item7;
		#endregion

		#region Constructors
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
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
		}

		protected override int Size
		{
			get
			{
				return 7;
			}
		}
		#endregion

		#region Instance Methods
		protected override int CompareTo(object other, IComparer comparer)
		{
			if (other == null)
				return 1;

			Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = (other as Tuple<T1, T2, T3, T4, T5, T6, T7>);
			Precondition.Require(tuple, () => Error.TypeIsNotTuple(other, "other"));

			int result = base.CompareTo(other, comparer);
			if (result != 0)
				return result;

			return comparer.Compare(Item7, tuple.Item7);
		}

		protected override bool Equals(object other, IEqualityComparer comparer)
		{
			if (other == null)
				return false;

			Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = (other as Tuple<T1, T2, T3, T4, T5, T6, T7>);
			if (tuple == null)
				return false;

			return base.Equals(other, comparer) &&
				comparer.Equals(Item7, tuple.Item7);
		}

		protected override int GetHashCode(IEqualityComparer comparer)
		{
			return CombineHashCodes(
				base.GetHashCode(comparer),
				comparer.GetHashCode(Item7));
		}

		protected override string ToString(StringBuilder builder)
		{
			return builder.Append(Item1).Append(", ")
				.Append(Item2).Append(", ")
				.Append(Item3).Append(", ")
				.Append(Item4).Append(", ")
				.Append(Item5).Append(", ")
				.Append(Item6).Append(", ")
				.Append(Item7).Append(")")
				.ToString();
		}
		#endregion
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : Tuple<T1, T2, T3, T4, T5, T6, T7>
		where TRest : ITuple
	{
		#region Instance Fields
		private readonly TRest _rest;
		#endregion

		#region Constructors
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
			: base(item1, item2, item3, item4, item5, item6, item7)
		{
			_rest = rest;
		}
		#endregion

		#region Instance Properties
		public TRest Rest
		{
			get
			{
				return _rest;
			}
		}

		protected override int Size
		{
			get
			{
				return 7 + _rest.Size;
			}
		}
		#endregion

		#region Instance Methods
		protected override int CompareTo(object other, IComparer comparer)
		{
			if (other == null)
				return 1;

			Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple = 
				(other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>);
			Precondition.Require(tuple, () => Error.TypeIsNotTuple(other, "other"));

			int result = base.CompareTo(other, comparer);
			if (result != 0)
				return result;

			return comparer.Compare(Rest, tuple.Rest);
		}

		protected override bool Equals(object other, IEqualityComparer comparer)
		{
			if (other == null)
				return false;

			Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> tuple = 
				(other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>);
			if (tuple == null)
				return false;

			return base.Equals(other, comparer) && 
				comparer.Equals(Rest, tuple.Rest);
		}

		protected override int GetHashCode(IEqualityComparer comparer)
		{
			if (Rest.Size >= 8)
				return Rest.GetHashCode(comparer);

			return CombineHashCodes(base.GetHashCode(comparer), Rest.GetHashCode(comparer));
		}

		protected override string ToString(StringBuilder builder)
		{
			builder.Append(Item1).Append(", ")
				.Append(Item2).Append(", ")
				.Append(Item3).Append(", ")
				.Append(Item4).Append(", ")
				.Append(Item5).Append(", ")
				.Append(Item6).Append(", ")
				.Append(Item7).Append(", ");

			return Rest.ToString(builder);
		}
		#endregion
	}
}