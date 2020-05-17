#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;
using Newtonsoft.Json.Linq;

#endregion

namespace Renoir {


	/// <summary>
	/// Maintains a registered properties
	/// </summary>
	public class PropertyPool {

		/// <summary>
		/// Holds all global properties
		/// </summary>
		private static readonly Dictionary<string, object> pool = new Dictionary<string, object>();

		/// <summary>
		/// Holds all subscriptions
		/// </summary>
		private static readonly Dictionary<string, IPropertyChangeHandler> subscriptions = new Dictionary<string, IPropertyChangeHandler>();

		/// <summary>
		/// Current property id counter
		/// </summary>
		public static int CurrentId { get; set; } = 100;


		/// <summary>
		/// Add untyped property to pool
		/// </summary>
		/// <param name="property"></param>
		/// <param name="name"></param>
		public static void Register(string name, object property) {
			Logger.trace($"Registering property: {property}");
			if (!pool.ContainsKey(name)) pool.Add(name, property);
			else pool[name] = property;
		}


		/// <summary>
		/// Registers a new property at the property-pool.
		/// </summary>
		/// <param name="property">The property to update</param>
		public static void Register<T>(Property<T> property) {
			Logger.trace($"Registering property: {property}");
			if (pool.ContainsKey(property.Alias)) pool[property.Alias] = property;
			else pool.Add(property.Alias, property);
		}


		/// <summary>
		/// Matches all subscriptions by the given alias
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static IEnumerable<IPropertyChangeHandler> MatchSubscriptions(string alias) {
			var matched = new List<IPropertyChangeHandler>();

			foreach (var subscription in subscriptions) {
				var r = @"^" + subscription.Key.Trim().Replace(".", @"\.").Replace("*", @".*") + "$";
				var matches = Regex.Matches(alias, r, RegexOptions.Singleline);

				if (matches.Count > 0) matched.Add(subscription.Value);
			}

			return matched;
		}


		/// <summary>
		/// Get a property by it's name ID
		/// </summary>
		/// <param name="id">The properties name ID</param>
		/// <returns></returns>
		public static Property<T> GetProperty<T>(string id) {
			var tmp = pool[id];

			if (tmp is Property<T> t) return t;

			return (Property<T>) tmp;
		}


		/// <summary>
		/// Get with no type-cast
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static object Get(string name) {
			return pool[name];
		}


		/// <summary>
		/// Removes a property from the pool.
		/// </summary>
		/// <param name="property">The property to update</param>
		public static void Unregister<T>(Property<T> property) {
			pool.Remove(property.Alias);
		}


		/// <summary>
		/// Returns the pool's content as string representation.
		/// Rather a feature for debug issues to dump the content.
		/// </summary>
		/// <returns></returns>
		public static string AsString() {
			var s = "[";
			foreach (var property in pool) s += property + (property.Key != pool.Last().Key ? ", " : "");
			return s;
		}


		/// <summary>
		/// Clear property pool
		/// </summary>
		public static void Clear() {
			pool.Clear();
			subscriptions.Clear();
		}


		/// <summary>
		/// Check of property already registered
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static bool Exists(string name) {
			return pool.ContainsKey(name);
		}


		/// <summary>
		/// Add a subscription to one or more properties defined by a matching string.
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="subscriber"></param>
		public static void AddSubscription(string alias, IPropertyChangeHandler subscriber) {
			subscriptions.Add(alias, subscriber);
			/* update property event handler */
			foreach (var o in pool)
				if (o.Value is BaseProperty bp)
					bp.UpdateSubscriber();
		}

	}


	/// <summary>
	/// Interface for subscriber classes.
	/// </summary>
	public interface IPropertyChangeHandler {

		/// <summary>
		/// Called upon an event has been fired.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		void OnPropertyChange<T>(Property<T> sender, PropertyEventArgs<T> args);

	}


	/// <summary>
	/// Property change event data.
	/// </summary>
	public class PropertyEventArgs<T> : EventArgs {

		public PropertyEventArgs(T old, T @new) {
			Old = old;
			New = @new;
		}


		public T Old { get; }
		public T New { get; }


		/// <summary>
		/// Pretty print this event.
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			var o = Old != null ? Old.ToString() : "<empty>";
			var n = New != null ? New.ToString() : "<empty>";
			return $"({o} => {n})";
		}

	}


	/// <summary>
	/// Encapsulates a property, binds it to a name and provide an event to catch manipulating.
	/// </summary>
	public class Property<T> : BaseProperty, ITokenizeable {

		/// <summary>
		/// Event handler delegate.
		/// </summary>
		/// <param name="sender">The property where the event is raised.</param>
		/// <param name="args">Change data.</param>
		public delegate void ChangeEventHandler(Property<T> sender, PropertyEventArgs<T> args);


		/// <summary>
		/// generic typed property value
		/// </summary>
		private T _value;


		/// <summary>
		/// Basic parameterless constructor
		/// </summary>
		public Property() {
			// Logging created at: 18:43:22
			Logger.debug($"default is: {Default}");
			if (Default != null) {
				Set(Default);
				Logger.debug($"set default value for: {this}");
			}
		}


		/// <summary>
		/// Constructs a property.
		/// </summary>
		/// <param name="alias">The name of the property</param>
		/// <param name="value">The properties value</param>
		/// <param name="locked">Write-lock (default is false)</param>
		public Property(string alias, T value, bool locked = false) {
			Alias = alias;
			_value = value;
			Locked = locked;
			ID = PropertyPool.CurrentId++;
		}


		/// <summary>
		/// Constructs a property with a default value.
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="locked"></param>
		public Property(string alias, bool locked = false) {
			Alias = alias;
			Locked = locked;
			ID = PropertyPool.CurrentId++;
		}


		public T Value {
			get => _value;
			set {
				if (value == null || _value != null && _value.Equals(value)) return;

				// ExecuteTrigger(newVal);
				// ExecuteTransformTrigger(newVal);
				OnPropertyChange(new PropertyEventArgs<T>(_value, value));
				_value = value;
			}
		}


		/// <summary>
		/// Setter for untyped values.
		/// Checks the given type for valid casting and assigns it.
		/// Returns true if value can be casted and assigned.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Set(object value) {
			// Logging created at: 18:39:20
			Logger.debug($"try to set default value: {value} for {this}");
			if (!(value is T t)) return false;

			Logger.debug("default value has been set.");
			
			_value = t;
			return true;
		}


		/// <summary>
		/// Check for new subscriber
		/// </summary>
		public override void UpdateSubscriber() {
			foreach (var _propertyChangeListener in PropertyPool.MatchSubscriptions(Alias)) Subscribe(_propertyChangeListener);
		}


		/// <summary>
		/// Dynamic transformation trigger list. Contains all transformation trigger.
		/// </summary>
		public List<(Func<T, bool>, Func<T, T>)> TransformTriggers { get; } = new List<(Func<T, bool>, Func<T, T>)>();

		/// <summary>
		/// Dynamic trigger list. Contains all constraint trigger.
		/// </summary>
		public List<(Func<T, bool>, Action<T>)> Triggers { get; } = new List<(Func<T, bool>, Action<T>)>();

		/// <summary>
		/// The change event bound to the event handler delegate.
		/// </summary>
		public event ChangeEventHandler RaiseChangeEvent;


		/// <summary>
		/// Subscribe to a change event.
		/// </summary>
		/// <param name="handler"></param>
		public void Subscribe(IPropertyChangeHandler handler) {
			RaiseChangeEvent += handler.OnPropertyChange;
		}


		/// <summary>
		/// Check for delegate
		/// </summary>
		/// <param name="delegate"></param>
		/// <returns></returns>
		private bool DelegateSubscribed(Delegate @delegate) {
			return RaiseChangeEvent.GetInvocationList().Any(evdel => evdel == @delegate);
		}


		/// <summary>
		/// UnSubscribe to event.
		/// </summary>
		/// <param name="handler"></param>
		public void Unsubscribe(IPropertyChangeHandler handler) {
			RaiseChangeEvent -= handler.OnPropertyChange;
		}


		/// <summary>
		/// Return the properties value as formatted string using
		/// the internal setup Format property.
		/// </summary>
		/// <returns></returns>
		public string Formatted() {
			return string.Format(Format, _value);
		}


		/// <summary>
		/// Return the properties value as formatted string using
		/// the given format specifier.
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public string Formatted(string format) {
			return string.Format(format, _value);
		}


		/// <summary>
		/// Event handler. Propagate event to all subscriber.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnPropertyChange(PropertyEventArgs<T> e) {
			RaiseChangeEvent?.Invoke(this, e);
		}


		/// <summary>
		/// String representation of the property.
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			var typeStr = typeof(T).ToString().Split('.');
			var valStr = Value != null ? Value.ToString() : "null";

			if (Format.Length > 0 && Value != null) valStr = Formatted();

			return $"[Alias='{Alias}' ID={ID} value={typeStr[typeStr.Length - 1]}({valStr})" + (Locked ? " locked=true" : "") +
				   $" handler={RaiseChangeEvent?.GetInvocationList()?.Length}" +
				   $" trigger={Triggers.Count} transforms={TransformTriggers.Count}" + "]";
		}


		/// <summary>
		/// Transform value to JToken
		/// </summary>
		/// <returns></returns>
		public JToken ToToken() {
			var val = _value ?? (object) "null";
			return JToken.FromObject(val);
		}


		/// <summary>
		/// Adds a trigger handler which is raised if 'cond' becomes true and transforms
		/// it's value via 'handler'.
		/// </summary>
		/// <param name="cond">The condition closure.</param>
		/// <param name="handler">The handler closure. Should return null on no transformation.</param>
		public void AddTransformTrigger(Func<T, bool> cond, Func<T, T> handler) {
			TransformTriggers.Add((cond, handler));
		}


		/// <summary>
		/// Checks all transform-triggers for match and applies it's
		/// transformation on success.
		/// </summary>
		/// <param name="t">The value to check against all riggers.</param>
		private void ExecuteTransformTrigger(T t) {
			foreach (var (cond, handler) in TransformTriggers)
				if (cond(t)) {
					var tmp = handler(t);
					if (tmp != null) t = tmp;
				}
		}


		/// <summary>
		/// Adds a trigger handler which is raised if 'cond' becomes true.
		/// Does not transforms anything.
		/// </summary>
		/// <param name="cond">The condition closure.</param>
		/// <param name="handler">The handler closure. Should return null on no transformation.</param>
		public void AddTrigger(Func<T, bool> cond, Action<T> handler) {
			Triggers.Add((cond, handler));
		}


		/// <summary>
		/// Checks all triggers for match and call it's handler on success.
		/// </summary>
		/// <param name="t">The value to check against all riggers.</param>
		private void ExecuteTrigger(T t) {
			foreach (var (cond, handler) in Triggers)
				if (cond(t))
					handler(t);
		}


		/*===== IMPLICIT CONVERSATIONS =================================================================================*/
		public static implicit operator Vector2(Property<T> p) {
			return p._value switch {
				Vector2 v => v,
				float f => new Vector2(f, f),
				int i => new Vector2(i, i),
				_ => new Vector2()
			};
		}


		/// <summary>
		/// Unboxing the inner value.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public static explicit operator T(Property<T> p) {
			return p._value;
		}


		/// <summary>
		/// Implicit conversation to JToken for JSON support.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public static implicit operator JToken(Property<T> p) {
			var t = JToken.FromObject(p._value);
			return t;
		}


		public static implicit operator string(Property<T> p) {
			if ((object) p == null) return "<NIL>";
			if (p._value == null) return "<null>";
			return p.Format.Length > 1 ? p.Formatted() : p.Value.ToString();
		}
		/*===== IMPLICIT CONVERSATIONS =================================================================================*/


		/*===== OPERATOR CONVERSATIONS =================================================================================*/
		public static Property<T> operator +(Property<T> p, dynamic n) {
			p.Value = p._value + n;
			return p;
		}


		public static Property<T> operator -(Property<T> p, dynamic n) {
			p.Value = p._value - n;
			return p;
		}


		public static Property<T> operator *(Property<T> p, dynamic n) {
			p.Value = p._value * n;
			return p;
		}


		public static Property<T> operator /(Property<T> p, dynamic n) {
			p.Value = p._value / n;
			return p;
		}


		public static Property<T> operator ==(Property<T> p, dynamic n) {
			return p._value == n;
		}


		public static Property<T> operator !=(Property<T> p, dynamic n) {
			return p._value != n;
		}


		public static Property<T> operator >(Property<T> p, dynamic n) {
			p.Value = p._value > n;
			return p;
		}


		public static Property<T> operator <(Property<T> p, dynamic n) {
			p.Value = p._value < n;
			return p;
		}


		public static Property<T> operator >=(Property<T> p, dynamic n) {
			p.Value = p._value >= n;
			return p;
		}


		public static Property<T> operator <=(Property<T> p, dynamic n) {
			p.Value = p._value <= n;
			return p;
		}


		public static Property<T> operator ++(Property<T> p) {
			return p + 1;
		}


		public static Property<T> operator --(Property<T> p) {
			return p - 1;
		}


		public static Property<T> operator -(Property<T> p) {
			if (p is Property<int> i) {
				i._value = -i._value;
				return p;
			}

			if (p is Property<long> l) {
				l._value = -l._value;
				return p;
			}

			if (p is Property<float> f) {
				f._value = -f._value;
				return p;
			}

			if (p is Property<double> d) {
				d._value = -d._value;
				return p;
			}

			throw new InvalidCastException(
				$"Type: {p.GetType()} can't be casted to int|long|float|double to apply unary (-) operator.");
		}


		public static Property<T> operator !(Property<T> p) {
			if (p is Property<bool> pbool)
				pbool.Value = !pbool.Value;
			else
				throw new InvalidCastException($"Type: {p.GetType()} can't be casted to bool to apply unary (!) operator.");

			return p;
		}
		/*===== OPERATOR CONVERSATIONS =================================================================================*/

	}


	/// <summary>
	/// Basic property properties
	/// </summary>
	public abstract class BaseProperty {

		public string Alias { get; set; }
		public string Format { get; set; } = "";
		public object Default { get; set; }
		public long ID { get; set; }
		public bool Locked { get; set; }

		public abstract void UpdateSubscriber();

	}

}
