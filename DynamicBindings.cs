using System;
using System.Reflection;
using Godot;

/// <summary>
///     Custom Attribute to bind a local property to a Godot Node in the scene-graph
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class GNodeAttribute : Attribute {
    /// <summary>
    ///     Standard constructor
    /// </summary>
    public GNodeAttribute() {
    }


    /// <summary>
    ///     Construct NoteBindingAttribute
    /// </summary>
    /// <param name="bindTo">Name (and path) of the target note.</param>
    public GNodeAttribute(string bindTo) {
        BindTo = bindTo;
    }


    /// <summary>
    ///     Name (and path) of the target note)
    /// </summary>
    public string BindTo { get; set; } = "";
}

/// <summary>
///     Contain helper methods for dynamic node bindings.
/// </summary>
public static class DynamicBindings {
    private static readonly BindingFlags bindingFlags
        = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


    /// <summary>
    ///     Extension method to be used with nodes.
    ///     Must use with 'this.' to work: this.AutobindNotes()
    /// </summary>
    /// <param name="node"></param>
    public static void SetupNodeBindings(this Node node) {
        SetupBindings(node);
    }


    /// <summary>
    ///     Setup dynamic bindings for a given node instance.
    ///     Please call this in every '_Ready()' method in Godot nodes
    ///     to use automatic bindings.
    /// </summary>
    /// <param name="node">The node to setup.</param>
    /// <exception cref="Exception"></exception>
    public static void SetupBindings(Node node) {
        var t = node.GetType();

        Logger.trace($"Setup dynamic node-bindings for: '{t}'");

        foreach (var field in t.GetFields(bindingFlags))
        foreach (var attr in field.GetCustomAttributes())
            if (attr is GNodeAttribute binder) {
                var nodePath = "";

                // node path or node name not set, so use field name
                if (string.IsNullOrEmpty(binder.BindTo))
                    nodePath = field.Name;
                else
                    nodePath = binder.BindTo;

                var bindNode = node.GetNode(nodePath);

                if (bindNode == null)
                    throw new Exception($"Unable to bind field: '{field}' to node via node-path: '{binder.BindTo}'!");

                field.SetValue(node, bindNode);

                Logger.trace($"'{field}' has been bound to node: '{binder.BindTo}'");
            }
    }
}