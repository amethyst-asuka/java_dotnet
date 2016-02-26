'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
' * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' 

Namespace javax.accessibility

	''' <summary>
	''' The AccessibleAction interface should be supported by any object
	''' that can perform one or more actions.  This interface
	''' provides the standard mechanism for an assistive technology to determine
	''' what those actions are as well as tell the object to perform them.
	''' Any object that can be manipulated should support this
	''' interface.  Applications can determine if an object supports the
	''' AccessibleAction interface by first obtaining its AccessibleContext (see
	''' <seealso cref="Accessible"/>) and then calling the <seealso cref="AccessibleContext#getAccessibleAction"/>
	''' method.  If the return value is not null, the object supports this interface.
	''' </summary>
	''' <seealso cref= Accessible </seealso>
	''' <seealso cref= Accessible#getAccessibleContext </seealso>
	''' <seealso cref= AccessibleContext </seealso>
	''' <seealso cref= AccessibleContext#getAccessibleAction
	''' 
	''' @author      Peter Korn
	''' @author      Hans Muller
	''' @author      Willie Walker
	''' @author      Lynn Monsanto </seealso>
	Public Interface AccessibleAction

		''' <summary>
		''' An action which causes a tree node to
		''' collapse if expanded and expand if collapsed.
		''' @since 1.5
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String TOGGLE_EXPAND = New String("toggleexpand");

		''' <summary>
		''' An action which increments a value.
		''' @since 1.5
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String INCREMENT = New String("increment");


		''' <summary>
		''' An action which decrements a value.
		''' @since 1.5
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String DECREMENT = New String("decrement");

		''' <summary>
		''' An action which causes a component to execute its default action.
		''' @since 1.6
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String CLICK = New String("click");

		''' <summary>
		''' An action which causes a popup to become visible if it is hidden and
		''' hidden if it is visible.
		''' @since 1.6
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final String TOGGLE_POPUP = New String("toggle popup");

		''' <summary>
		''' Returns the number of accessible actions available in this object
		''' If there are more than one, the first one is considered the "default"
		''' action of the object.
		''' </summary>
		''' <returns> the zero-based number of Actions in this object </returns>
		ReadOnly Property accessibleActionCount As Integer

		''' <summary>
		''' Returns a description of the specified action of the object.
		''' </summary>
		''' <param name="i"> zero-based index of the actions </param>
		''' <returns> a String description of the action </returns>
		''' <seealso cref= #getAccessibleActionCount </seealso>
		Function getAccessibleActionDescription(ByVal i As Integer) As String

		''' <summary>
		''' Performs the specified Action on the object
		''' </summary>
		''' <param name="i"> zero-based index of actions </param>
		''' <returns> true if the action was performed; otherwise false. </returns>
		''' <seealso cref= #getAccessibleActionCount </seealso>
		Function doAccessibleAction(ByVal i As Integer) As Boolean
	End Interface

End Namespace