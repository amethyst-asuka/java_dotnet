'
' * Copyright (c) 1999, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management



	''' <summary>
	''' This class is used by the query-building mechanism to represent binary
	''' relations.
	''' @serial include
	''' 
	''' @since 1.5
	''' </summary>
	Friend Class MatchQueryExp
		Inherits QueryEval
		Implements QueryExp

		' Serial version 
		Private Const serialVersionUID As Long = -7156603696948215014L

		''' <summary>
		''' @serial The attribute value to be matched
		''' </summary>
		Private exp As AttributeValueExp

		''' <summary>
		''' @serial The pattern to be matched
		''' </summary>
		Private pattern As String


		''' <summary>
		''' Basic Constructor.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a new MatchQueryExp where the specified AttributeValueExp matches
		''' the specified pattern StringValueExp.
		''' </summary>
		Public Sub New(ByVal a As AttributeValueExp, ByVal s As StringValueExp)
			exp = a
			pattern = s.value
		End Sub


		''' <summary>
		''' Returns the attribute of the query.
		''' </summary>
		Public Overridable Property attribute As AttributeValueExp
			Get
				Return exp
			End Get
		End Property

		''' <summary>
		''' Returns the pattern of the query.
		''' </summary>
		Public Overridable Property pattern As String
			Get
				Return pattern
			End Get
		End Property

		''' <summary>
		''' Applies the MatchQueryExp on a MBean.
		''' </summary>
		''' <param name="name"> The name of the MBean on which the MatchQueryExp will be applied.
		''' </param>
		''' <returns>  True if the query was successfully applied to the MBean, false otherwise.
		''' </returns>
		''' <exception cref="BadStringOperationException"> </exception>
		''' <exception cref="BadBinaryOpValueExpException"> </exception>
		''' <exception cref="BadAttributeValueExpException"> </exception>
		''' <exception cref="InvalidApplicationException"> </exception>
		Public Overridable Function apply(ByVal name As ObjectName) As Boolean Implements QueryExp.apply

			Dim val As ValueExp = exp.apply(name)
			If Not(TypeOf val Is StringValueExp) Then Return False
			Return wildmatch(CType(val, StringValueExp).value, pattern)
		End Function

		''' <summary>
		''' Returns the string representing the object
		''' </summary>
		Public Overrides Function ToString() As String
			Return exp & " like " & New StringValueExp(pattern)
		End Function

	'    
	'     * Tests whether string s is matched by pattern p.
	'     * Supports "?", "*", "[", each of which may be escaped with "\";
	'     * character classes may use "!" for negation and "-" for range.
	'     * Not yet supported: internationalization; "\" inside brackets.<P>
	'     * Wildcard matching routine by Karl Heuer.  Public Domain.<P>
	'     
		Private Shared Function wildmatch(ByVal s As String, ByVal p As String) As Boolean
			Dim c As Char
			Dim si As Integer = 0, pi As Integer = 0
			Dim slen As Integer = s.Length
			Dim plen As Integer = p.Length

			Do While pi < plen ' While still string
				c = p.Chars(pi)
				pi += 1
				If c = "?"c Then
					si += 1
					If si > slen Then
						Return False
					End If ' Start of choice
				ElseIf c = "["c Then
					If si >= slen Then Return False
					Dim wantit As Boolean = True
					Dim seenit As Boolean = False
					If p.Chars(pi) = "!"c Then
						wantit = False
						pi += 1
					End If
					c = p.Chars(pi)
					pi += 1
					Do While c <> "]"c AndAlso pi < plen
						If p.Chars(pi) = "-"c AndAlso pi+1 < plen AndAlso p.Chars(pi+1) <> "]"c Then
							If s.Chars(si) >= p.Chars(pi-1) AndAlso s.Chars(si) <= p.Chars(pi+1) Then seenit = True
							pi += 1
						Else
							If c = s.Chars(si) Then seenit = True
						End If
						c = p.Chars(pi)
						pi += 1
					Loop
					If (pi >= plen) OrElse (wantit <> seenit) Then Return False
					pi += 1
					si += 1 ' Wildcard
				ElseIf c = "*"c Then
					If pi >= plen Then Return True
					Do
						If wildmatch(s.Substring(si), p.Substring(pi)) Then Return True
						si += 1
					Loop While si < slen
					Return False
				ElseIf c = "\"c Then
'JAVA TO VB CONVERTER TODO TASK: Assignments within expressions are not supported in VB
					Dim tempVar As Boolean = pi >= plen OrElse si >= slen OrElse p.Chars(pi++) <> s.Chars(si++)
					If tempVar Then Return False
				Else
					Dim tempVar2 As Boolean = si >= slen OrElse c <> s.Chars(si)
					si += 1
					If tempVar2 Then Return False
				End If
			Loop
			Return (si = slen)
		End Function
	End Class

End Namespace