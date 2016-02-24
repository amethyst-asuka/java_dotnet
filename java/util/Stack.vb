Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1994, 2010, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util

	''' <summary>
	''' The <code>Stack</code> class represents a last-in-first-out
	''' (LIFO) stack of objects. It extends class <tt>Vector</tt> with five
	''' operations that allow a vector to be treated as a stack. The usual
	''' <tt>push</tt> and <tt>pop</tt> operations are provided, as well as a
	''' method to <tt>peek</tt> at the top item on the stack, a method to test
	''' for whether the stack is <tt>empty</tt>, and a method to <tt>search</tt>
	''' the stack for an item and discover how far it is from the top.
	''' <p>
	''' When a stack is first created, it contains no items.
	''' 
	''' <p>A more complete and consistent set of LIFO stack operations is
	''' provided by the <seealso cref="Deque"/> interface and its implementations, which
	''' should be used in preference to this class.  For example:
	''' <pre>   {@code
	'''   Deque<Integer> stack = new ArrayDeque<Integer>();}</pre>
	''' 
	''' @author  Jonathan Payne
	''' @since   JDK1.0
	''' </summary>
	Public Class Stack(Of E)
		Inherits Vector(Of E)

		''' <summary>
		''' Creates an empty Stack.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Pushes an item onto the top of this stack. This has exactly
		''' the same effect as:
		''' <blockquote><pre>
		''' addElement(item)</pre></blockquote>
		''' </summary>
		''' <param name="item">   the item to be pushed onto this stack. </param>
		''' <returns>  the <code>item</code> argument. </returns>
		''' <seealso cref=     java.util.Vector#addElement </seealso>
		Public Overridable Function push(ByVal item As E) As E
			addElement(item)

			Return item
		End Function

		''' <summary>
		''' Removes the object at the top of this stack and returns that
		''' object as the value of this function.
		''' </summary>
		''' <returns>  The object at the top of this stack (the last item
		'''          of the <tt>Vector</tt> object). </returns>
		''' <exception cref="EmptyStackException">  if this stack is empty. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function pop() As E
			Dim obj As E
			Dim len As Integer = size()

			obj = peek()
			removeElementAt(len - 1)

			Return obj
		End Function

		''' <summary>
		''' Looks at the object at the top of this stack without removing it
		''' from the stack.
		''' </summary>
		''' <returns>  the object at the top of this stack (the last item
		'''          of the <tt>Vector</tt> object). </returns>
		''' <exception cref="EmptyStackException">  if this stack is empty. </exception>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function peek() As E
			Dim len As Integer = size()

			If len = 0 Then Throw New EmptyStackException
			Return elementAt(len - 1)
		End Function

		''' <summary>
		''' Tests if this stack is empty.
		''' </summary>
		''' <returns>  <code>true</code> if and only if this stack contains
		'''          no items; <code>false</code> otherwise. </returns>
		Public Overridable Function empty() As Boolean
			Return size() = 0
		End Function

		''' <summary>
		''' Returns the 1-based position where an object is on this stack.
		''' If the object <tt>o</tt> occurs as an item in this stack, this
		''' method returns the distance from the top of the stack of the
		''' occurrence nearest the top of the stack; the topmost item on the
		''' stack is considered to be at distance <tt>1</tt>. The <tt>equals</tt>
		''' method is used to compare <tt>o</tt> to the
		''' items in this stack.
		''' </summary>
		''' <param name="o">   the desired object. </param>
		''' <returns>  the 1-based position from the top of the stack where
		'''          the object is located; the return value <code>-1</code>
		'''          indicates that the object is not on the stack. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function search(ByVal o As Object) As Integer
			Dim i As Integer = LastIndexOf(o)

			If i >= 0 Then Return size() - i
			Return -1
		End Function

		''' <summary>
		''' use serialVersionUID from JDK 1.0.2 for interoperability </summary>
		Private Const serialVersionUID As Long = 1224463164541339165L
	End Class

End Namespace