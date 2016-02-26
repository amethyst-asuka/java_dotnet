Imports System
Imports System.Text

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
	''' A TabSet is comprised of many TabStops. It offers methods for locating the
	''' closest TabStop to a given position and finding all the potential TabStops.
	''' It is also immutable.
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
	''' @author  Scott Violet
	''' </summary>
	<Serializable> _
	Public Class TabSet
		''' <summary>
		''' TabStops this TabSet contains. </summary>
		Private tabs As TabStop()
		''' <summary>
		''' Since this class is immutable the hash code could be
		''' calculated once. MAX_VALUE means that it was not initialized
		''' yet. Hash code shouldn't has MAX_VALUE value.
		''' </summary>
		Private ___hashCode As Integer = Integer.MAX_VALUE

		''' <summary>
		''' Creates and returns an instance of TabSet. The array of Tabs
		''' passed in must be sorted in ascending order.
		''' </summary>
		Public Sub New(ByVal tabs As TabStop())
			' PENDING(sky): If this becomes a problem, make it sort.
			If tabs IsNot Nothing Then
				Dim ___tabCount As Integer = tabs.Length

				Me.tabs = New TabStop(___tabCount - 1){}
				Array.Copy(tabs, 0, Me.tabs, 0, ___tabCount)
			Else
				Me.tabs = Nothing
			End If
		End Sub

		''' <summary>
		''' Returns the number of Tab instances the receiver contains.
		''' </summary>
		Public Overridable Property tabCount As Integer
			Get
				Return If(tabs Is Nothing, 0, tabs.Length)
			End Get
		End Property

		''' <summary>
		''' Returns the TabStop at index <code>index</code>. This will throw an
		''' IllegalArgumentException if <code>index</code> is outside the range
		''' of tabs.
		''' </summary>
		Public Overridable Function getTab(ByVal index As Integer) As TabStop
			Dim numTabs As Integer = tabCount

			If index < 0 OrElse index >= numTabs Then Throw New System.ArgumentException(index & " is outside the range of tabs")
			Return tabs(index)
		End Function

		''' <summary>
		''' Returns the Tab instance after <code>location</code>. This will
		''' return null if there are no tabs after <code>location</code>.
		''' </summary>
		Public Overridable Function getTabAfter(ByVal location As Single) As TabStop
			Dim index As Integer = getTabIndexAfter(location)

			Return If(index = -1, Nothing, tabs(index))
		End Function

		''' <returns> the index of the TabStop <code>tab</code>, or -1 if
		''' <code>tab</code> is not contained in the receiver. </returns>
		Public Overridable Function getTabIndex(ByVal tab As TabStop) As Integer
			For counter As Integer = tabCount - 1 To 0 Step -1
				' should this use .equals?
				If getTab(counter) Is tab Then Return counter
			Next counter
			Return -1
		End Function

		''' <summary>
		''' Returns the index of the Tab to be used after <code>location</code>.
		''' This will return -1 if there are no tabs after <code>location</code>.
		''' </summary>
		Public Overridable Function getTabIndexAfter(ByVal location As Single) As Integer
			Dim current, min, max As Integer

			min = 0
			max = tabCount
			Do While min <> max
				current = (max - min) \ 2 + min
				If location > tabs(current).position Then
					If min = current Then
						min = max
					Else
						min = current
					End If
				Else
					If current = 0 OrElse location > tabs(current - 1).position Then Return current
					max = current
				End If
			Loop
			' no tabs after the passed in location.
			Return -1
		End Function

		''' <summary>
		''' Indicates whether this <code>TabSet</code> is equal to another one. </summary>
		''' <param name="o"> the <code>TabSet</code> instance which this instance
		'''  should be compared to. </param>
		''' <returns> <code>true</code> if <code>o</code> is the instance of
		''' <code>TabSet</code>, has the same number of <code>TabStop</code>s
		''' and they are all equal, <code>false</code> otherwise.
		''' 
		''' @since 1.5 </returns>
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If TypeOf o Is TabSet Then
				Dim ts As TabSet = CType(o, TabSet)
				Dim count As Integer = tabCount
				If ts.tabCount <> count Then Return False
				For i As Integer = 0 To count - 1
					Dim ts1 As TabStop = getTab(i)
					Dim ts2 As TabStop = ts.getTab(i)
					If (ts1 Is Nothing AndAlso ts2 IsNot Nothing) OrElse (ts1 IsNot Nothing AndAlso (Not getTab(i).Equals(ts.getTab(i)))) Then Return False
				Next i
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Returns a hashcode for this set of TabStops. </summary>
		''' <returns>  a hashcode value for this set of TabStops.
		''' 
		''' @since 1.5 </returns>
		Public Overrides Function GetHashCode() As Integer
			If ___hashCode = Integer.MaxValue Then
				___hashCode = 0
				Dim len As Integer = tabCount
				For i As Integer = 0 To len - 1
					Dim ts As TabStop = getTab(i)
					___hashCode = ___hashCode Xor If(ts IsNot Nothing, getTab(i).GetHashCode(), 0)
				Next i
				If ___hashCode = Integer.MaxValue Then ___hashCode -= 1
			End If
			Return ___hashCode
		End Function

		''' <summary>
		''' Returns the string representation of the set of tabs.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim ___tabCount As Integer = tabCount
			Dim buffer As New StringBuilder("[ ")

			For counter As Integer = 0 To ___tabCount - 1
				If counter > 0 Then buffer.Append(" - ")
				buffer.Append(getTab(counter).ToString())
			Next counter
			buffer.Append(" ]")
			Return buffer.ToString()
		End Function
	End Class

End Namespace