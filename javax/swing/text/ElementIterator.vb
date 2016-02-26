Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.text


	''' <summary>
	''' <p>
	''' ElementIterator, as the name suggests, iterates over the Element
	''' tree.  The constructor can be invoked with either Document or an Element
	''' as an argument.  If the constructor is invoked with a Document as an
	''' argument then the root of the iteration is the return value of
	''' document.getDefaultRootElement().
	''' 
	''' The iteration happens in a depth-first manner.  In terms of how
	''' boundary conditions are handled:
	''' a) if next() is called before first() or current(), the
	'''    root will be returned.
	''' b) next() returns null to indicate the end of the list.
	''' c) previous() returns null when the current element is the root
	'''    or next() has returned null.
	''' 
	''' The ElementIterator does no locking of the Element tree. This means
	''' that it does not track any changes.  It is the responsibility of the
	''' user of this class, to ensure that no changes happen during element
	''' iteration.
	''' 
	''' Simple usage example:
	''' 
	'''    public void iterate() {
	'''        ElementIterator it = new ElementIterator(root);
	'''        Element elem;
	'''        while (true) {
	'''           if ((elem = next()) != null) {
	'''               // process element
	'''               System.out.println("elem: " + elem.getName());
	'''           } else {
	'''               break;
	'''           }
	'''        }
	'''    }
	''' 
	''' @author Sunita Mani
	''' 
	''' </summary>

	Public Class ElementIterator
		Implements ICloneable


		Private root As Element
		Private elementStack As Stack(Of StackItem) = Nothing

		''' <summary>
		''' The StackItem class stores the element
		''' as well as a child index.  If the
		''' index is -1, then the element represented
		''' on the stack is the element itself.
		''' Otherwise, the index functions as as index
		''' into the vector of children of the element.
		''' In this case, the item on the stack
		''' represents the "index"th child of the element
		''' 
		''' </summary>
		Private Class StackItem
			Implements ICloneable

			Private ReadOnly outerInstance As ElementIterator

			Friend item As Element
			Friend childIndex As Integer

			Private Sub New(ByVal outerInstance As ElementIterator, ByVal elem As Element)
					Me.outerInstance = outerInstance
				''' <summary>
				''' -1 index implies a self reference,
				''' as opposed to an index into its
				''' list of children.
				''' </summary>
				Me.item = elem
				Me.childIndex = -1
			End Sub

			Private Sub incrementIndex()
				childIndex += 1
			End Sub

			Private Property element As Element
				Get
					Return item
				End Get
			End Property

			Private Property index As Integer
				Get
					Return childIndex
				End Get
			End Property

			Protected Friend Overridable Function clone() As Object
				Return MyBase.clone()
			End Function
		End Class

		''' <summary>
		''' Creates a new ElementIterator. The
		''' root element is taken to get the
		''' default root element of the document.
		''' </summary>
		''' <param name="document"> a Document. </param>
		Public Sub New(ByVal document As Document)
			root = document.defaultRootElement
		End Sub


		''' <summary>
		''' Creates a new ElementIterator.
		''' </summary>
		''' <param name="root"> the root Element. </param>
		Public Sub New(ByVal root As Element)
			Me.root = root
		End Sub


		''' <summary>
		''' Clones the ElementIterator.
		''' </summary>
		''' <returns> a cloned ElementIterator Object. </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function clone() As Object

			Try
				Dim it As New ElementIterator(root)
				If elementStack IsNot Nothing Then
					it.elementStack = New Stack(Of StackItem)
					For i As Integer = 0 To elementStack.Count - 1
						Dim item As StackItem = elementStack.elementAt(i)
						Dim clonee As StackItem = CType(item.clone(), StackItem)
						it.elementStack.Push(clonee)
					Next i
				End If
				Return it
			Catch e As CloneNotSupportedException
				Throw New InternalError(e)
			End Try
		End Function


		''' <summary>
		''' Fetches the first element.
		''' </summary>
		''' <returns> an Element. </returns>
		Public Overridable Function first() As Element
			' just in case...
			If root Is Nothing Then Return Nothing

			elementStack = New Stack(Of StackItem)
			If root.elementCount <> 0 Then elementStack.Push(New StackItem(Me, root))
			Return root
		End Function

		''' <summary>
		''' Fetches the current depth of element tree.
		''' </summary>
		''' <returns> the depth. </returns>
		Public Overridable Function depth() As Integer
			If elementStack Is Nothing Then Return 0
			Return elementStack.Count
		End Function


		''' <summary>
		''' Fetches the current Element.
		''' </summary>
		''' <returns> element on top of the stack or
		'''          <code>null</code> if the root element is <code>null</code> </returns>
		Public Overridable Function current() As Element

			If elementStack Is Nothing Then Return first()

	'        
	'          get a handle to the element on top of the stack.
	'        
			If elementStack.Count > 0 Then
				Dim item As StackItem = elementStack.Peek()
				Dim elem As Element = item.element
				Dim index As Integer = item.index
				' self reference
				If index = -1 Then Return elem
				' return the child at location "index".
				Return elem.getElement(index)
			End If
			Return Nothing
		End Function


		''' <summary>
		''' Fetches the next Element. The strategy
		''' used to locate the next element is
		''' a depth-first search.
		''' </summary>
		''' <returns> the next element or <code>null</code>
		'''          at the end of the list. </returns>
		Public Overridable Function [next]() As Element

	'         if current() has not been invoked
	'           and next is invoked, the very first
	'           element will be returned. 
			If elementStack Is Nothing Then Return first()

			' no more elements
			If elementStack.Count = 0 Then Return Nothing

			' get a handle to the element on top of the stack

			Dim item As StackItem = elementStack.Peek()
			Dim elem As Element = item.element
			Dim index As Integer = item.index

			If index+1 < elem.elementCount Then
				Dim child As Element = elem.getElement(index+1)
				If child.leaf Then
	'                 In this case we merely want to increment
	'                   the child index of the item on top of the
	'                   stack.
					item.incrementIndex()
				Else
	'                 In this case we need to push the child(branch)
	'                   on the stack so that we can iterate over its
	'                   children. 
					elementStack.Push(New StackItem(Me, child))
				End If
				Return child
			Else
	'             No more children for the item on top of the
	'               stack therefore pop the stack. 
				elementStack.Pop()
				If elementStack.Count > 0 Then
	'                 Increment the child index for the item that
	'                   is now on top of the stack. 
					Dim top As StackItem = elementStack.Peek()
					top.incrementIndex()
	'                 We now want to return its next child, therefore
	'                   call next() recursively. 
					Return [next]()
				End If
			End If
			Return Nothing
		End Function


		''' <summary>
		''' Fetches the previous Element. If however the current
		''' element is the last element, or the current element
		''' is null, then null is returned.
		''' </summary>
		''' <returns> previous <code>Element</code> if available
		'''  </returns>
		Public Overridable Function previous() As Element

			Dim stackSize As Integer
			stackSize = elementStack.Count
			If elementStack Is Nothing OrElse stackSize = 0 Then Return Nothing

			' get a handle to the element on top of the stack
			'
			Dim item As StackItem = elementStack.Peek()
			Dim elem As Element = item.element
			Dim index As Integer = item.index

			If index > 0 Then
				' return child at previous index. 
				index -= 1
				Return getDeepestLeaf(elem.getElement(index))
			ElseIf index = 0 Then
	'             this implies that current is the element's
	'               first child, therefore previous is the
	'               element itself. 
				Return elem
			ElseIf index = -1 Then
				If stackSize = 1 Then Return Nothing
	'             We need to return either the item
	'               below the top item or one of the
	'               former's children. 
				Dim top As StackItem = elementStack.Pop()
				item = elementStack.Peek()

				' restore the top item.
				elementStack.Push(top)
				elem = item.element
				index = item.index
				Return (If(index = -1, elem, getDeepestLeaf(elem.getElement(index))))
			End If
			' should never get here.
			Return Nothing
		End Function

		''' <summary>
		''' Returns the last child of <code>parent</code> that is a leaf. If the
		''' last child is a not a leaf, this method is called with the last child.
		''' </summary>
		Private Function getDeepestLeaf(ByVal parent As Element) As Element
			If parent.leaf Then Return parent
			Dim childCount As Integer = parent.elementCount
			If childCount = 0 Then Return parent
			Return getDeepestLeaf(parent.getElement(childCount - 1))
		End Function

	'    
	'      Iterates through the element tree and prints
	'      out each element and its attributes.
	'    
		Private Sub dumpTree()

			Dim elem As Element
			Do
				elem = [next]()
				If elem IsNot Nothing Then
					Console.WriteLine("elem: " & elem.name)
					Dim attr As AttributeSet = elem.attributes
					Dim s As String = ""
					Dim names As System.Collections.IEnumerator = attr.attributeNames
					Do While names.hasMoreElements()
						Dim key As Object = names.nextElement()
						Dim value As Object = attr.getAttribute(key)
						If TypeOf value Is AttributeSet Then
							' don't go recursive
							s = s + key & "=**AttributeSet** "
						Else
							s = s + key & "=" & value & " "
						End If
					Loop
					Console.WriteLine("attributes: " & s)
				Else
					Exit Do
				End If
			Loop
		End Sub
	End Class

End Namespace