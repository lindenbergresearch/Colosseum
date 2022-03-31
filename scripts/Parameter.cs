#region header

// 
//    _____
//   (, /   )            ,
//     /__ /  _ __   ___   __
//  ) /   \__(/_/ (_(_)_(_/ (_  CORE LIBRARY
// (_/ ______________________________________/
// 
// 
// Renoir Core Library for the Godot Game-Engine.
// Copyright 2020-2022 by Lindenberg Research.
// 
// www.lindenberg-research.com
// www.godotengine.org
// 

#endregion

#region

using System;
using System.Collections.Generic;
using Godot;

#endregion

namespace Renoir;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public class Parameter<T> {
	private T _value;

	protected List<Action> actions = new();
	protected List<Func<T, T>> hooks = new();

	/*---------------------------------------------------------------------*/

	public Parameter(T value) {
		Format = "";
		_value = value;
	}

	public Parameter(T value, string format) {
		_value = value;
		Format = format;
	}


	public T Value {
		get => _value;
		set => Update(value);
	}


	public string Format { get; set; }

	/*---------------------------------------------------------------------*/

	public void AddAction(Action action) {
		actions.Add(action);
	}


	public void AddHook(Func<T, T> hook) {
		hooks.Add(hook);
	}


	public void Update(T val) {
		foreach (var hook in hooks) val = hook(val);

		foreach (var action in actions) action();

		_value = val;
	}


	public string Formatted() {
		return string.Format(Format, _value);
	}

	public override string ToString() {
		return Format.Length > 1
			? Formatted()
			: _value.ToString();
	}


	#region Implicits

	/*===== IMPLICIT CONVERSATIONS =================================================================================*/
	public static implicit operator Vector2(Parameter<T> p) {
		return p._value switch {
			Vector2 v => v,
			float f => new Vector2(f, f),
			int i => new Vector2(i, i),
			_ => new Vector2()
		};
	}


	public static explicit operator T(Parameter<T> p) {
		return p._value;
	}


	public static implicit operator string(Parameter<T> p) {
		if ((object) p == null) return "<NIL>";
		if (p._value == null) return "<null>";
		return p.Format.Length > 1
			? p.Formatted()
			: p.Value.ToString();
	}

	/*===== IMPLICIT CONVERSATIONS =================================================================================*/

	#endregion

	#region Operators

	/*===== OPERATOR CONVERSATIONS =================================================================================*/
	public static Parameter<T> operator +(Parameter<T> p, dynamic n) {
		p.Value = p._value + n;
		return p;
	}


	public static Parameter<T> operator -(Parameter<T> p, dynamic n) {
		p.Value = p._value - n;
		return p;
	}


	public static Parameter<T> operator *(Parameter<T> p, dynamic n) {
		p.Value = p._value * n;
		return p;
	}


	public static Parameter<T> operator /(Parameter<T> p, dynamic n) {
		p.Value = p._value / n;
		return p;
	}


	public static bool operator ==(Parameter<T> p, dynamic n) {
		return p._value == n;
	}


	public static bool operator !=(Parameter<T> p, dynamic n) {
		return p._value != n;
	}


	public static Parameter<T> operator >(Parameter<T> p, dynamic n) {
		p.Value = p._value > n;
		return p;
	}


	public static Parameter<T> operator <(Parameter<T> p, dynamic n) {
		p.Value = p._value < n;
		return p;
	}


	public static Parameter<T> operator >=(Parameter<T> p, dynamic n) {
		p.Value = p._value >= n;
		return p;
	}


	public static Parameter<T> operator <=(Parameter<T> p, dynamic n) {
		p.Value = p._value <= n;
		return p;
	}


	public static Parameter<T> operator ++(Parameter<T> p) {
		return p + 1;
	}


	public static Parameter<T> operator --(Parameter<T> p) {
		return p - 1;
	}


	public static Parameter<T> operator -(Parameter<T> p) {
		if (p is Parameter<int> i) {
			i._value = -i._value;
			return p;
		}

		if (p is Parameter<long> l) {
			l._value = -l._value;
			return p;
		}

		if (p is Parameter<float> f) {
			f._value = -f._value;
			return p;
		}

		if (p is Parameter<double> d) {
			d._value = -d._value;
			return p;
		}

		throw new InvalidCastException(
			$"Type: {p.GetType()} can't be casted to int|long|float|double to apply unary (-) operator.");
	}


	public static Parameter<T> operator !(Parameter<T> p) {
		if (p is Parameter<bool> pbool)
			pbool.Value = !pbool.Value;
		else
			throw new InvalidCastException($"Type: {p.GetType()} can't be casted to bool to apply unary (!) operator.");

		return p;
	}

	/*===== OPERATOR CONVERSATIONS =================================================================================*/

	#endregion
}