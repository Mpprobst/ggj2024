using Godot;
using System;

public static class GDExtensions
{
	public static T GetChildOfType<T>(this Node node, bool recursive = true)
	where T : Node
	{
		int childCount = node.GetChildCount();
		for (int i = 0; i < childCount; i++)
		{
			Node child = node.GetChild(i);
			if (child.GetType() == typeof(T))
				return child as T;
			if (recursive && child.GetChildCount() > 0)
			{
				T recursiveResult = child.GetChildOfType<T>(true);
				if (recursiveResult != null)
					return recursiveResult;
			}
		}
		return null;
	}
	
	public static T GetParentOfType<T>(this Node node, bool recursive = true)
	where T : Node
	{
		Node parent = node.GetParent();
		if (!recursive)
			return parent as T;
		
		while (parent != null)
		{
			if (parent.GetType() == typeof(T))
				return parent as T;
			parent = parent.GetParent();
		}
		return null;
	}
}
