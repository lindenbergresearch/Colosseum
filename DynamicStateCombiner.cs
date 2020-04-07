using System;

/// <summary>
///     Generic interface to hold a state
/// </summary>
public interface IDynamicState<T> {
    Func<T> Resolve { get; set; }
}


/// <summary>
///     Boolean implementation
/// </summary>
public interface IDynamicBoolState : IDynamicState<bool> {
    /// <summary>
    ///     Conditional checker
    /// </summary>
    /// <returns>Returns true is the state is satisfied.</returns>
    bool isSatisfied();
}


public interface ITimedPropery<T> {
    float Time { get; set; }

    void reset();

    void update(float time);

    bool isTimedOut();

    void start(float timeout, T initial, T finals);
}


/// <summary>
/// </summary>
public class DynamicStateCombiner : IDynamicBoolState {
    /// <summary>
    ///     Push function via constructor
    /// </summary>
    /// <param name="fun"></param>
    public DynamicStateCombiner(Func<bool> fun) {
        Resolve = fun;
    }


    public Func<bool> Resolve { get; set; }


    /// <summary>
    ///     Resolves the conditional state
    /// </summary>
    /// <returns></returns>
    public bool isSatisfied() {
        return Resolve();
    }


    /// <summary>
    ///     Implicit conversation to use as type
    /// </summary>
    /// <param name="fun"></param>
    /// <returns></returns>
    public static implicit operator DynamicStateCombiner(Func<bool> fun) {
        return new DynamicStateCombiner(fun);
    }


    /// <summary>
    ///     Implicit conversation to use as type
    /// </summary>
    /// <param name="dsc"></param>
    /// <returns></returns>
    public static implicit operator bool(DynamicStateCombiner dsc) {
        return dsc.isSatisfied();
    }


    /// <summary>
    ///     Static factory method
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static DynamicStateCombiner fun(Func<bool> f) {
        return new DynamicStateCombiner(f);
    }


    /// <summary>
    ///     Just some formatting
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
        return Resolve() ? "Yes" : "No";
    }
}