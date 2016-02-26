Imports System
Imports System.Collections
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing



	''' <summary>
	''' A simple implementation of <code>SpinnerModel</code> whose
	''' values are defined by an array or a <code>List</code>.
	''' For example to create a model defined by
	''' an array of the names of the days of the week:
	''' <pre>
	''' String[] days = new DateFormatSymbols().getWeekdays();
	''' SpinnerModel model = new SpinnerListModel(Arrays.asList(days).subList(1, 8));
	''' </pre>
	''' This class only stores a reference to the array or <code>List</code>
	''' so if an element of the underlying sequence changes, it's up
	''' to the application to notify the <code>ChangeListeners</code> by calling
	''' <code>fireStateChanged</code>.
	''' <p>
	''' This model inherits a <code>ChangeListener</code>.
	''' The <code>ChangeListener</code>s are notified whenever the
	''' model's <code>value</code> or <code>list</code> properties changes.
	''' </summary>
	''' <seealso cref= JSpinner </seealso>
	''' <seealso cref= SpinnerModel </seealso>
	''' <seealso cref= AbstractSpinnerModel </seealso>
	''' <seealso cref= SpinnerNumberModel </seealso>
	''' <seealso cref= SpinnerDateModel
	''' 
	''' @author Hans Muller
	''' @since 1.4 </seealso>
	<Serializable> _
	Public Class SpinnerListModel
		Inherits AbstractSpinnerModel

		Private list As IList
		Private index As Integer


		''' <summary>
		''' Constructs a <code>SpinnerModel</code> whose sequence of
		''' values is defined by the specified <code>List</code>.
		''' The initial value (<i>current element</i>)
		''' of the model will be <code>values.get(0)</code>.
		''' If <code>values</code> is <code>null</code> or has zero
		''' size, an <code>IllegalArugmentException</code> is thrown.
		''' </summary>
		''' <param name="values"> the sequence this model represents </param>
		''' <exception cref="IllegalArgumentException"> if <code>values</code> is
		'''    <code>null</code> or zero size </exception>
		Public Sub New(Of T1)(ByVal values As IList(Of T1))
			If values Is Nothing OrElse values.Count = 0 Then Throw New System.ArgumentException("SpinnerListModel(List) expects non-null non-empty List")
			Me.list = values
			Me.index = 0
		End Sub


		''' <summary>
		''' Constructs a <code>SpinnerModel</code> whose sequence of values
		''' is defined by the specified array.  The initial value of the model
		''' will be <code>values[0]</code>.  If <code>values</code> is
		''' <code>null</code> or has zero length, an
		''' <code>IllegalArgumentException</code> is thrown.
		''' </summary>
		''' <param name="values"> the sequence this model represents </param>
		''' <exception cref="IllegalArgumentException"> if <code>values</code> is
		'''    <code>null</code> or zero length </exception>
		Public Sub New(ByVal values As Object())
			If values Is Nothing OrElse values.Length = 0 Then Throw New System.ArgumentException("SpinnerListModel(Object[]) expects non-null non-empty Object[]")
			Me.list = values
			Me.index = 0
		End Sub


		''' <summary>
		''' Constructs an effectively empty <code>SpinnerListModel</code>.
		''' The model's list will contain a single
		''' <code>"empty"</code> string element.
		''' </summary>
		Public Sub New()
			Me.New(New Object(){"empty"})
		End Sub


		''' <summary>
		''' Returns the <code>List</code> that defines the sequence for this model.
		''' </summary>
		''' <returns> the value of the <code>list</code> property </returns>
		''' <seealso cref= #setList </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property list As IList(Of ?)
			Get
				Return list
			End Get
			Set(ByVal list As IList(Of T1))
				If (list Is Nothing) OrElse (list.Count = 0) Then Throw New System.ArgumentException("invalid list")
				If Not list.Equals(Me.list) Then
					Me.list = list
					index = 0
					fireStateChanged()
				End If
			End Set
		End Property




		''' <summary>
		''' Returns the current element of the sequence.
		''' </summary>
		''' <returns> the <code>value</code> property </returns>
		''' <seealso cref= SpinnerModel#getValue </seealso>
		''' <seealso cref= #setValue </seealso>
		Public Property Overrides value As Object
			Get
				Return list(index)
			End Get
			Set(ByVal elt As Object)
				Dim index As Integer = list.IndexOf(elt)
				If index = -1 Then
					Throw New System.ArgumentException("invalid sequence element")
				ElseIf index <> Me.index Then
					Me.index = index
					fireStateChanged()
				End If
			End Set
		End Property




		''' <summary>
		''' Returns the next legal value of the underlying sequence or
		''' <code>null</code> if value is already the last element.
		''' </summary>
		''' <returns> the next legal value of the underlying sequence or
		'''     <code>null</code> if value is already the last element </returns>
		''' <seealso cref= SpinnerModel#getNextValue </seealso>
		''' <seealso cref= #getPreviousValue </seealso>
		Public Property Overrides nextValue As Object
			Get
				Return If(index >= (list.Count - 1), Nothing, list(index + 1))
			End Get
		End Property


		''' <summary>
		''' Returns the previous element of the underlying sequence or
		''' <code>null</code> if value is already the first element.
		''' </summary>
		''' <returns> the previous element of the underlying sequence or
		'''     <code>null</code> if value is already the first element </returns>
		''' <seealso cref= SpinnerModel#getPreviousValue </seealso>
		''' <seealso cref= #getNextValue </seealso>
		Public Property Overrides previousValue As Object
			Get
				Return If(index <= 0, Nothing, list(index - 1))
			End Get
		End Property


		''' <summary>
		''' Returns the next object that starts with <code>substring</code>.
		''' </summary>
		''' <param name="substring"> the string to be matched </param>
		''' <returns> the match </returns>
		Friend Overridable Function findNextMatch(ByVal substring As String) As Object
			Dim max As Integer = list.Count

			If max = 0 Then Return Nothing
			Dim counter As Integer = index

			Do
				Dim ___value As Object = list(counter)
				Dim [string] As String = ___value.ToString()

				If [string] IsNot Nothing AndAlso [string].StartsWith(Substring) Then Return ___value
				counter = (counter + 1) Mod max
			Loop While counter <> index
			Return Nothing
		End Function
	End Class

End Namespace