Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' Comparator which attempts to sort Components based on their size and
	''' position. Code adapted from original javax.swing.DefaultFocusManager
	''' implementation.
	''' 
	''' @author David Mendenhall
	''' </summary>
	<Serializable> _
	Friend NotInheritable Class LayoutComparator
		Implements IComparer(Of java.awt.Component)

		Private Const ROW_TOLERANCE As Integer = 10

		Private horizontal As Boolean = True
		Private leftToRight As Boolean = True

		Friend Property componentOrientation As java.awt.ComponentOrientation
			Set(ByVal orientation As java.awt.ComponentOrientation)
				horizontal = orientation.horizontal
				leftToRight = orientation.leftToRight
			End Set
		End Property

		Public Function compare(ByVal a As java.awt.Component, ByVal b As java.awt.Component) As Integer
			If a Is b Then Return 0

			' Row/Column algorithm only applies to siblings. If 'a' and 'b'
			' aren't siblings, then we need to find their most inferior
			' ancestors which share a parent. Compute the ancestory lists for
			' each Component and then search from the Window down until the
			' hierarchy branches.
			If a.parent <> b.parent Then
				Dim aAncestory As New LinkedList(Of java.awt.Component)

				Do While a IsNot Nothing
					aAncestory.AddLast(a)
					If TypeOf a Is java.awt.Window Then Exit Do
					a = a.parent
				Loop
				If a Is Nothing Then Throw New ClassCastException

				Dim bAncestory As New LinkedList(Of java.awt.Component)

				Do While b IsNot Nothing
					bAncestory.AddLast(b)
					If TypeOf b Is java.awt.Window Then Exit Do
					b = b.parent
				Loop
				If b Is Nothing Then Throw New ClassCastException

				Dim aIter As IEnumerator(Of java.awt.Component) = aAncestory.listIterator(aAncestory.Count)
				Dim bIter As IEnumerator(Of java.awt.Component) = bAncestory.listIterator(bAncestory.Count)
				Do
					If aIter.hasPrevious() Then
						a = aIter.previous()
					Else
						' a is an ancestor of b
						Return -1
					End If

					If bIter.hasPrevious() Then
						b = bIter.previous()
					Else
						' b is an ancestor of a
						Return 1
					End If

					If a IsNot b Then Exit Do
				Loop
			End If

			Dim ax As Integer = a.x, ay As Integer = a.y, bx As Integer = b.x, by As Integer = b.y

			Dim zOrder As Integer = a.parent.getComponentZOrder(a) - b.parent.getComponentZOrder(b)
			If horizontal Then
				If leftToRight Then

					' LT - Western Europe (optional for Japanese, Chinese, Korean)

					If Math.Abs(ay - by) < ROW_TOLERANCE Then
						Return If(ax < bx, -1, (If(ax > bx, 1, zOrder)))
					Else
						Return If(ay < by, -1, 1)
					End If ' !leftToRight
				Else

					' RT - Middle East (Arabic, Hebrew)

					If Math.Abs(ay - by) < ROW_TOLERANCE Then
						Return If(ax > bx, -1, (If(ax < bx, 1, zOrder)))
					Else
						Return If(ay < by, -1, 1)
					End If
				End If ' !horizontal
			Else
				If leftToRight Then

					' TL - Mongolian

					If Math.Abs(ax - bx) < ROW_TOLERANCE Then
						Return If(ay < by, -1, (If(ay > by, 1, zOrder)))
					Else
						Return If(ax < bx, -1, 1)
					End If ' !leftToRight
				Else

					' TR - Japanese, Chinese, Korean

					If Math.Abs(ax - bx) < ROW_TOLERANCE Then
						Return If(ay < by, -1, (If(ay > by, 1, zOrder)))
					Else
						Return If(ax > bx, -1, 1)
					End If
				End If
			End If
		End Function
	End Class

End Namespace