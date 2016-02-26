Imports System
Imports System.Collections
Imports System.Text

'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.print.attribute


	''' <summary>
	''' Class SetOfIntegerSyntax is an abstract base class providing the common
	''' implementation of all attributes whose value is a set of nonnegative
	''' integers. This includes attributes whose value is a single range of integers
	''' and attributes whose value is a set of ranges of integers.
	''' <P>
	''' You can construct an instance of SetOfIntegerSyntax by giving it in "string
	''' form." The string consists of zero or more comma-separated integer groups.
	''' Each integer group consists of either one integer, two integers separated by
	''' a hyphen (<CODE>-</CODE>), or two integers separated by a colon
	''' (<CODE>:</CODE>). Each integer consists of one or more decimal digits
	''' (<CODE>0</CODE> through <CODE>9</CODE>). Whitespace characters cannot
	''' appear within an integer but are otherwise ignored. For example:
	''' <CODE>""</CODE>, <CODE>"1"</CODE>, <CODE>"5-10"</CODE>, <CODE>"1:2,
	''' 4"</CODE>.
	''' <P>
	''' You can also construct an instance of SetOfIntegerSyntax by giving it in
	''' "array form." Array form consists of an array of zero or more integer groups
	''' where each integer group is a length-1 or length-2 array of
	''' <CODE>int</CODE>s; for example, <CODE>int[0][]</CODE>,
	''' <CODE>int[][]{{1}}</CODE>, <CODE>int[][]{{5,10}}</CODE>,
	''' <CODE>int[][]{{1,2},{4}}</CODE>.
	''' <P>
	''' In both string form and array form, each successive integer group gives a
	''' range of integers to be included in the set. The first integer in each group
	''' gives the lower bound of the range; the second integer in each group gives
	''' the upper bound of the range; if there is only one integer in the group, the
	''' upper bound is the same as the lower bound. If the upper bound is less than
	''' the lower bound, it denotes a null range (no values). If the upper bound is
	''' equal to the lower bound, it denotes a range consisting of a single value. If
	''' the upper bound is greater than the lower bound, it denotes a range
	''' consisting of more than one value. The ranges may appear in any order and are
	''' allowed to overlap. The union of all the ranges gives the set's contents.
	''' Once a SetOfIntegerSyntax instance is constructed, its value is immutable.
	''' <P>
	''' The SetOfIntegerSyntax object's value is actually stored in "<I>canonical</I>
	''' array form." This is the same as array form, except there are no null ranges;
	''' the members of the set are represented in as few ranges as possible (i.e.,
	''' overlapping ranges are coalesced); the ranges appear in ascending order; and
	''' each range is always represented as a length-two array of <CODE>int</CODE>s
	''' in the form {lower bound, upper bound}. An empty set is represented as a
	''' zero-length array.
	''' <P>
	''' Class SetOfIntegerSyntax has operations to return the set's members in
	''' canonical array form, to test whether a given integer is a member of the
	''' set, and to iterate through the members of the set.
	''' <P>
	''' 
	''' @author  David Mendenhall
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public MustInherit Class SetOfIntegerSyntax
		Implements ICloneable

		Private Const serialVersionUID As Long = 3666874174847632203L

		''' <summary>
		''' This set's members in canonical array form.
		''' @serial
		''' </summary>
		Private members As Integer()()


		''' <summary>
		''' Construct a new set-of-integer attribute with the given members in
		''' string form.
		''' </summary>
		''' <param name="members">  Set members in string form. If null, an empty set is
		'''                     constructed.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if <CODE>members</CODE> does not
		'''    obey  the proper syntax. </exception>
		Protected Friend Sub New(ByVal members As String)
			Me.members = parse(members)
		End Sub

		''' <summary>
		''' Parse the given string, returning canonical array form.
		''' </summary>
		Private Shared Function parse(ByVal members As String) As Integer()()
			' Create vector to hold int[] elements, each element being one range
			' parsed out of members.
			Dim theRanges As New ArrayList

			' Run state machine over members.
			Dim n As Integer = (If(members Is Nothing, 0, members.Length))
			Dim i As Integer = 0
			Dim state As Integer = 0
			Dim lb As Integer = 0
			Dim ub As Integer = 0
			Dim c As Char
			Dim digit As Integer
			Do While i < n
				c = members.Chars(i)
				i += 1
				Select Case state

				Case 0 ' Before first integer in first group
					If Char.IsWhiteSpace(c) Then
						state = 0
					Else
						digit = Char.digit(c, 10)
						If digit <> -1 Then
							lb = digit
							state = 1
						Else
							Throw New System.ArgumentException
						End If
						End If

				Case 1 ' In first integer in a group
					If Char.IsWhiteSpace(c) Then
							state = 2
					Else
						digit = Char.digit(c, 10)
						If digit <> -1 Then
							lb = 10 * lb + digit
							state = 1
						ElseIf c = "-"c OrElse c = ":"c Then
							state = 3
						ElseIf c = ","c Then
							accumulate(theRanges, lb, lb)
							state = 6
						Else
							Throw New System.ArgumentException
						End If
						End If

				Case 2 ' After first integer in a group
					If Char.IsWhiteSpace(c) Then
						state = 2
					ElseIf c = "-"c OrElse c = ":"c Then
						state = 3
					ElseIf c = ","c Then
						accumulate(theRanges, lb, lb)
						state = 6
					Else
						Throw New System.ArgumentException
					End If

				Case 3 ' Before second integer in a group
					If Char.IsWhiteSpace(c) Then
						state = 3
					Else
						digit = Char.digit(c, 10)
						If digit <> -1 Then
							ub = digit
							state = 4
						Else
							Throw New System.ArgumentException
						End If
						End If

				Case 4 ' In second integer in a group
					If Char.IsWhiteSpace(c) Then
						state = 5
					Else
						digit = Char.digit(c, 10)
						If digit <> -1 Then
							ub = 10 * ub + digit
							state = 4
						ElseIf c = ","c Then
							accumulate(theRanges, lb, ub)
							state = 6
						Else
							Throw New System.ArgumentException
						End If
						End If

				Case 5 ' After second integer in a group
					If Char.IsWhiteSpace(c) Then
						state = 5
					ElseIf c = ","c Then
						accumulate(theRanges, lb, ub)
						state = 6
					Else
						Throw New System.ArgumentException
					End If

				Case 6 ' Before first integer in second or later group
					If Char.IsWhiteSpace(c) Then
						state = 6
					Else
						digit = Char.digit(c, 10)
						If digit <> -1 Then
							lb = digit
							state = 1
						Else
							Throw New System.ArgumentException
						End If
						End If
				End Select
			Loop

			' Finish off the state machine.
			Select Case state
			Case 0 ' Before first integer in first group
			Case 1, 2 ' In first integer in a group
				accumulate(theRanges, lb, lb)
			Case 4, 5 ' In second integer in a group
				accumulate(theRanges, lb, ub)
			Case 3, 6 ' Before second integer in a group
				Throw New System.ArgumentException
			End Select

			' Return canonical array form.
			Return canonicalArrayForm(theRanges)
		End Function

		''' <summary>
		''' Accumulate the given range (lb .. ub) into the canonical array form
		''' into the given vector of int[] objects.
		''' </summary>
		Private Shared Sub accumulate(ByVal ranges As ArrayList, ByVal lb As Integer, ByVal ub As Integer)
			' Make sure range is non-null.
			If lb <= ub Then
				' Stick range at the back of the vector.
				ranges.Add(New Integer() {lb, ub})

				' Work towards the front of the vector to integrate the new range
				' with the existing ranges.
				For j As Integer = ranges.Count-2 To 0 Step -1
				' Get lower and upper bounds of the two ranges being compared.
					Dim rangea As Integer() = CType(ranges(j), Integer())
					Dim lba As Integer = rangea(0)
					Dim uba As Integer = rangea(1)
					Dim rangeb As Integer() = CType(ranges(j+1), Integer())
					Dim lbb As Integer = rangeb(0)
					Dim ubb As Integer = rangeb(1)

	'                 If the two ranges overlap or are adjacent, coalesce them.
	'                 * The two ranges overlap if the larger lower bound is less
	'                 * than or equal to the smaller upper bound. The two ranges
	'                 * are adjacent if the larger lower bound is one greater
	'                 * than the smaller upper bound.
	'                 
					If Math.Max(lba, lbb) - Math.Min(uba, ubb) <= 1 Then
						' The coalesced range is from the smaller lower bound to
						' the larger upper bound.
						ranges(j) = New Integer() {Math.Min(lba, lbb), Math.Max(uba, ubb)}
						ranges.RemoveAt(j+1)
					ElseIf lba > lbb Then

	'                     If the two ranges don't overlap and aren't adjacent but
	'                     * are out of order, swap them.
	'                     
						ranges(j) = rangeb
						ranges(j+1) = rangea
					Else
	'                 If the two ranges don't overlap and aren't adjacent and
	'                 * aren't out of order, we're done early.
	'                 
						Exit For
					End If
				Next j
			End If
		End Sub

		''' <summary>
		''' Convert the given vector of int[] objects to canonical array form.
		''' </summary>
		Private Shared Function canonicalArrayForm(ByVal ranges As ArrayList) As Integer()()
			Return CType(ranges.ToArray(GetType(Integer)), Integer()())
		End Function

		''' <summary>
		''' Construct a new set-of-integer attribute with the given members in
		''' array form.
		''' </summary>
		''' <param name="members">  Set members in array form. If null, an empty set is
		'''                     constructed.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (Unchecked exception) Thrown if any element of
		'''     <CODE>members</CODE> is null. </exception>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if any element of
		'''     <CODE>members</CODE> is not a length-one or length-two array or if
		'''     any non-null range in <CODE>members</CODE> has a lower bound less
		'''     than zero. </exception>
		Protected Friend Sub New(ByVal members As Integer()())
			Me.members = parse(members)
		End Sub

		''' <summary>
		''' Parse the given array form, returning canonical array form.
		''' </summary>
		Private Shared Function parse(ByVal members As Integer()()) As Integer()()
			' Create vector to hold int[] elements, each element being one range
			' parsed out of members.
			Dim ranges As New ArrayList

			' Process all integer groups in members.
			Dim n As Integer = (If(members Is Nothing, 0, members.Length))
			For i As Integer = 0 To n - 1
				' Get lower and upper bounds of the range.
				Dim lb, ub As Integer
				If members(i).Length = 1 Then
						ub = members(i)(0)
						lb = ub
				ElseIf members(i).Length = 2 Then
					lb = members(i)(0)
					ub = members(i)(1)
				Else
					Throw New System.ArgumentException
				End If

				' Verify valid bounds.
				If lb <= ub AndAlso lb < 0 Then Throw New System.ArgumentException

				' Accumulate the range.
				accumulate(ranges, lb, ub)
			Next i

					' Return canonical array form.
					Return canonicalArrayForm(ranges)
		End Function

		''' <summary>
		''' Construct a new set-of-integer attribute containing a single integer.
		''' </summary>
		''' <param name="member">  Set member.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if <CODE>member</CODE> is less than
		'''     zero. </exception>
		Protected Friend Sub New(ByVal member As Integer)
			If member < 0 Then Throw New System.ArgumentException
			members = New Integer() {{member, member}}
		End Sub

		''' <summary>
		''' Construct a new set-of-integer attribute containing a single range of
		''' integers. If the lower bound is greater than the upper bound (a null
		''' range), an empty set is constructed.
		''' </summary>
		''' <param name="lowerBound">  Lower bound of the range. </param>
		''' <param name="upperBound">  Upper bound of the range.
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     (Unchecked exception) Thrown if the range is non-null and
		'''     <CODE>lowerBound</CODE> is less than zero. </exception>
		Protected Friend Sub New(ByVal lowerBound As Integer, ByVal upperBound As Integer)
			If lowerBound <= upperBound AndAlso lowerBound < 0 Then Throw New System.ArgumentException
			members = If(lowerBound <=upperBound, New Integer() {{lowerBound, upperBound}}, New Integer(){})
		End Sub


		''' <summary>
		''' Obtain this set-of-integer attribute's members in canonical array form.
		''' The returned array is "safe;" the client may alter it without affecting
		''' this set-of-integer attribute.
		''' </summary>
		''' <returns>  This set-of-integer attribute's members in canonical array form. </returns>
		Public Overridable Property members As Integer()()
			Get
				Dim n As Integer = members.Length
				Dim result As Integer()() = New Integer(n - 1)(){}
				For i As Integer = 0 To n - 1
					result(i) = New Integer() {members(i)(0), members(i)(1)}
				Next i
				Return result
			End Get
		End Property

		''' <summary>
		''' Determine if this set-of-integer attribute contains the given value.
		''' </summary>
		''' <param name="x">  Integer value.
		''' </param>
		''' <returns>  True if this set-of-integer attribute contains the value
		'''          <CODE>x</CODE>, false otherwise. </returns>
		Public Overridable Function contains(ByVal x As Integer) As Boolean
			' Do a linear search to find the range that contains x, if any.
			Dim n As Integer = members.Length
			For i As Integer = 0 To n - 1
				If x < members(i)(0) Then
					Return False
				ElseIf x <= members(i)(1) Then
					Return True
				End If
			Next i
			Return False
		End Function

		''' <summary>
		''' Determine if this set-of-integer attribute contains the given integer
		''' attribute's value.
		''' </summary>
		''' <param name="attribute">  Integer attribute.
		''' </param>
		''' <returns>  True if this set-of-integer attribute contains
		'''          <CODE>theAttribute</CODE>'s value, false otherwise. </returns>
		Public Overridable Function contains(ByVal attribute As IntegerSyntax) As Boolean
			Return contains(attribute.value)
		End Function

		''' <summary>
		''' Determine the smallest integer in this set-of-integer attribute that is
		''' greater than the given value. If there are no integers in this
		''' set-of-integer attribute greater than the given value, <CODE>-1</CODE> is
		''' returned. (Since a set-of-integer attribute can only contain nonnegative
		''' values, <CODE>-1</CODE> will never appear in the set.) You can use the
		''' <CODE>next()</CODE> method to iterate through the integer values in a
		''' set-of-integer attribute in ascending order, like this:
		''' <PRE>
		'''     SetOfIntegerSyntax attribute = . . .;
		'''     int i = -1;
		'''     while ((i = attribute.next (i)) != -1)
		'''         {
		'''         foo (i);
		'''         }
		''' </PRE>
		''' </summary>
		''' <param name="x">  Integer value.
		''' </param>
		''' <returns>  The smallest integer in this set-of-integer attribute that is
		'''          greater than <CODE>x</CODE>, or <CODE>-1</CODE> if no integer in
		'''          this set-of-integer attribute is greater than <CODE>x</CODE>. </returns>
		Public Overridable Function [next](ByVal x As Integer) As Integer
			' Do a linear search to find the range that contains x, if any.
			Dim n As Integer = members.Length
			For i As Integer = 0 To n - 1
				If x < members(i)(0) Then
					Return members(i)(0)
				ElseIf x < members(i)(1) Then
					Return x + 1
				End If
			Next i
			Return -1
		End Function

		''' <summary>
		''' Returns whether this set-of-integer attribute is equivalent to the passed
		''' in object. To be equivalent, all of the following conditions must be
		''' true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class SetOfIntegerSyntax.
		''' <LI>
		''' This set-of-integer attribute's members and <CODE>object</CODE>'s
		''' members are the same.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this
		'''          set-of-integer attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			If [object] IsNot Nothing AndAlso TypeOf [object] Is SetOfIntegerSyntax Then
				Dim myMembers As Integer()() = Me.members
				Dim otherMembers As Integer()() = CType([object], SetOfIntegerSyntax).members
				Dim m As Integer = myMembers.Length
				Dim n As Integer = otherMembers.Length
				If m = n Then
					For i As Integer = 0 To m - 1
						If myMembers(i)(0) <> otherMembers(i)(0) OrElse myMembers(i)(1) <> otherMembers(i)(1) Then Return False
					Next i
					Return True
				Else
					Return False
				End If
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Returns a hash code value for this set-of-integer attribute. The hash
		''' code is the sum of the lower and upper bounds of the ranges in the
		''' canonical array form, or 0 for an empty set.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 0
			Dim n As Integer = members.Length
			For i As Integer = 0 To n - 1
				result += members(i)(0) + members(i)(1)
			Next i
			Return result
		End Function

		''' <summary>
		''' Returns a string value corresponding to this set-of-integer attribute.
		''' The string value is a zero-length string if this set is empty. Otherwise,
		''' the string value is a comma-separated list of the ranges in the canonical
		''' array form, where each range is represented as <CODE>"<I>i</I>"</CODE> if
		''' the lower bound equals the upper bound or
		''' <CODE>"<I>i</I>-<I>j</I>"</CODE> otherwise.
		''' </summary>
		Public Overrides Function ToString() As String
			Dim result As New StringBuilder
			Dim n As Integer = members.Length
			For i As Integer = 0 To n - 1
				If i > 0 Then result.Append(","c)
				result.Append(members(i)(0))
				If members(i)(0) <> members(i)(1) Then
					result.Append("-"c)
					result.Append(members(i)(1))
				End If
			Next i
			Return result.ToString()
		End Function

	End Class

End Namespace