Imports javax.swing

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
Namespace javax.swing.event

	''' <summary>
	''' An event reported to a child component that originated from an
	''' ancestor in the component hierarchy.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author Dave Moore
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class AncestorEvent
		Inherits AWTEvent

		''' <summary>
		''' An ancestor-component was added to the hierarchy of
		''' visible objects (made visible), and is currently being displayed.
		''' </summary>
		Public Const ANCESTOR_ADDED As Integer = 1
		''' <summary>
		''' An ancestor-component was removed from the hierarchy
		''' of visible objects (hidden) and is no longer being displayed.
		''' </summary>
		Public Const ANCESTOR_REMOVED As Integer = 2
		''' <summary>
		''' An ancestor-component changed its position on the screen. </summary>
		Public Const ANCESTOR_MOVED As Integer = 3

		Friend ancestor As Container
		Friend ancestorParent As Container

		''' <summary>
		''' Constructs an AncestorEvent object to identify a change
		''' in an ancestor-component's display-status.
		''' </summary>
		''' <param name="source">          the JComponent that originated the event
		'''                        (typically <code>this</code>) </param>
		''' <param name="id">              an int specifying <seealso cref="#ANCESTOR_ADDED"/>,
		'''                        <seealso cref="#ANCESTOR_REMOVED"/> or <seealso cref="#ANCESTOR_MOVED"/> </param>
		''' <param name="ancestor">        a Container object specifying the ancestor-component
		'''                        whose display-status changed </param>
		''' <param name="ancestorParent">  a Container object specifying the ancestor's parent </param>
		Public Sub New(ByVal source As JComponent, ByVal id As Integer, ByVal ancestor As Container, ByVal ancestorParent As Container)
			MyBase.New(source, id)
			Me.ancestor = ancestor
			Me.ancestorParent = ancestorParent
		End Sub

		''' <summary>
		''' Returns the ancestor that the event actually occurred on.
		''' </summary>
		Public Overridable Property ancestor As Container
			Get
				Return ancestor
			End Get
		End Property

		''' <summary>
		''' Returns the parent of the ancestor the event actually occurred on.
		''' This is most interesting in an ANCESTOR_REMOVED event, as
		''' the ancestor may no longer be in the component hierarchy.
		''' </summary>
		Public Overridable Property ancestorParent As Container
			Get
				Return ancestorParent
			End Get
		End Property

		''' <summary>
		''' Returns the component that the listener was added to.
		''' </summary>
		Public Overridable Property component As JComponent
			Get
				Return CType(source, JComponent)
			End Get
		End Property
	End Class

End Namespace