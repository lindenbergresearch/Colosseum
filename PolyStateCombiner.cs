using System;
using System.Collections.Generic;
using System.Linq;

public class EList<T> : List<T> {
	/// <summary>
	/// Returns the head of the list
	/// </summary>
/*	public T Head {
		get => this.First();
	}

	/// <summary>
	/// Returns the tail of the list
	/// </summary>
	public EList<T> Tail {
		get => GetRange(1, this.Count - 1);
	}*/


	/* private T foldHelper(EList<T> l, T t, Func<T, T, T> f) {
	     if (l.Count == 1) return f();
	     
	     foldHelper(l.Head)

	 }


	public static implicit operator List(EList<T> el) {
		return new List();
	}*/
}


public class PolyStateCombiner<T> {
	/// <summary>
	/// 
	/// </summary>
	public List<Func<T>> Constraints { get; } = new List<Func<T>>();

	/// <summary>
	/// 
	/// </summary>
	public Func<T, Func<T>, Func<T>> Combinator { get; set; }


	/// <summary>
	/// 
	/// </summary>
	/// <param name="constraint"></param>
	public void AddConstraint(Func<T> constraint) => Constraints.Add(constraint);


	// public T Resolve()
}
