Imports System

'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' Package private supporting class for <seealso cref="Comparator"/>.
	''' </summary>
	Friend Class Comparators
		Private Sub New()
			Throw New AssertionError("no instances")
		End Sub

		''' <summary>
		''' Compares <seealso cref="Comparable"/> objects in natural order.
		''' </summary>
		''' <seealso cref= Comparable </seealso>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot implement interfaces in .NET:
		Friend Enum NaturalOrderComparator
			INSTANCE

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public int compare(Comparable(Of Object) c1, Comparable(Of Object) c2)
	'		{
	'			Return c1.compareTo(c2);
	'		}

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'			public Comparator(Of Comparable(Of Object)) reversed()
	'		{
	'			Return Comparator.reverseOrder();
	'		}
		End Enum

		''' <summary>
		''' Null-friendly comparators
		''' </summary>
		<Serializable> _
		Friend NotInheritable Class NullComparator(Of T)
			Implements Comparator(Of T)

			Private Const serialVersionUID As Long = -7569533591570686392L
			Private ReadOnly nullFirst As Boolean
			' if null, non-null Ts are considered equal
			Private ReadOnly real As Comparator(Of T)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Friend Sub New(Of T1)(ByVal nullFirst As Boolean, ByVal real As Comparator(Of T1))
				Me.nullFirst = nullFirst
				Me.real = CType(real, Comparator(Of T))
			End Sub

			Public Overrides Function compare(ByVal a As T, ByVal b As T) As Integer Implements Comparator(Of T).compare
				If a Is Nothing Then
					Return If(b Is Nothing, 0, (If(nullFirst, -1, 1)))
				ElseIf b Is Nothing Then
					Return If(nullFirst, 1, -1)
				Else
					Return If(real Is Nothing, 0, real.Compare(a, b))
				End If
			End Function

'JAVA TO VB CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
			Public Overrides Function thenComparing(Of T1)(ByVal other As Comparator(Of T1)) As Comparator(Of T) Implements Comparator(Of T).thenComparing
				Objects.requireNonNull(other)
				Return New NullComparator(Of )(nullFirst,If(real Is Nothing, other, real.thenComparing(other)))
			End Function

			Public Overrides Function reversed() As Comparator(Of T) Implements Comparator(Of T).reversed
				Return New NullComparator(Of )((Not nullFirst),If(real Is Nothing, Nothing, real.reversed()))
			End Function
		End Class
	End Class

End Namespace