Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text.rtf


	Friend Class RTFAttributes
		Friend Shared attributes As RTFAttribute()

		Shared Sub New()
			Dim a As New List(Of RTFAttribute)
			Dim CHR As Integer = RTFAttribute.D_CHARACTER
			Dim PGF As Integer = RTFAttribute.D_PARAGRAPH
			Dim SEC As Integer = RTFAttribute.D_SECTION
			Dim DOC As Integer = RTFAttribute.D_DOCUMENT
			Dim PST As Integer = RTFAttribute.D_META
			Dim [True] As Boolean? = Convert.ToBoolean([True])
			Dim [False] As Boolean? = Convert.ToBoolean([False])

			a.Add(New BooleanAttribute(CHR, javax.swing.text.StyleConstants.Italic, "i"))
			a.Add(New BooleanAttribute(CHR, javax.swing.text.StyleConstants.Bold, "b"))
			a.Add(New BooleanAttribute(CHR, javax.swing.text.StyleConstants.Underline, "ul"))
			a.Add(NumericAttribute.NewTwips(PGF, javax.swing.text.StyleConstants.LeftIndent, "li", 0f, 0))
			a.Add(NumericAttribute.NewTwips(PGF, javax.swing.text.StyleConstants.RightIndent, "ri", 0f, 0))
			a.Add(NumericAttribute.NewTwips(PGF, javax.swing.text.StyleConstants.FirstLineIndent, "fi", 0f, 0))

			a.Add(New AssertiveAttribute(PGF, javax.swing.text.StyleConstants.Alignment, "ql", javax.swing.text.StyleConstants.ALIGN_LEFT))
			a.Add(New AssertiveAttribute(PGF, javax.swing.text.StyleConstants.Alignment, "qr", javax.swing.text.StyleConstants.ALIGN_RIGHT))
			a.Add(New AssertiveAttribute(PGF, javax.swing.text.StyleConstants.Alignment, "qc", javax.swing.text.StyleConstants.ALIGN_CENTER))
			a.Add(New AssertiveAttribute(PGF, javax.swing.text.StyleConstants.Alignment, "qj", javax.swing.text.StyleConstants.ALIGN_JUSTIFIED))
			a.Add(NumericAttribute.NewTwips(PGF, javax.swing.text.StyleConstants.SpaceAbove, "sa", 0))
			a.Add(NumericAttribute.NewTwips(PGF, javax.swing.text.StyleConstants.SpaceBelow, "sb", 0))

			a.Add(New AssertiveAttribute(PST, RTFReader.TabAlignmentKey, "tqr", javax.swing.text.TabStop.ALIGN_RIGHT))
			a.Add(New AssertiveAttribute(PST, RTFReader.TabAlignmentKey, "tqc", javax.swing.text.TabStop.ALIGN_CENTER))
			a.Add(New AssertiveAttribute(PST, RTFReader.TabAlignmentKey, "tqdec", javax.swing.text.TabStop.ALIGN_DECIMAL))


			a.Add(New AssertiveAttribute(PST, RTFReader.TabLeaderKey, "tldot", javax.swing.text.TabStop.LEAD_DOTS))
			a.Add(New AssertiveAttribute(PST, RTFReader.TabLeaderKey, "tlhyph", javax.swing.text.TabStop.LEAD_HYPHENS))
			a.Add(New AssertiveAttribute(PST, RTFReader.TabLeaderKey, "tlul", javax.swing.text.TabStop.LEAD_UNDERLINE))
			a.Add(New AssertiveAttribute(PST, RTFReader.TabLeaderKey, "tlth", javax.swing.text.TabStop.LEAD_THICKLINE))
			a.Add(New AssertiveAttribute(PST, RTFReader.TabLeaderKey, "tleq", javax.swing.text.TabStop.LEAD_EQUALS))

			' The following aren't actually recognized by Swing 
			a.Add(New BooleanAttribute(CHR, Constants.Caps, "caps"))
			a.Add(New BooleanAttribute(CHR, Constants.Outline, "outl"))
			a.Add(New BooleanAttribute(CHR, Constants.SmallCaps, "scaps"))
			a.Add(New BooleanAttribute(CHR, Constants.Shadow, "shad"))
			a.Add(New BooleanAttribute(CHR, Constants.Hidden, "v"))
			a.Add(New BooleanAttribute(CHR, Constants.Strikethrough, "strike"))
			a.Add(New BooleanAttribute(CHR, Constants.Deleted, "deleted"))



			a.Add(New AssertiveAttribute(DOC, "saveformat", "defformat", "RTF"))
			a.Add(New AssertiveAttribute(DOC, "landscape", "landscape"))

			a.Add(NumericAttribute.NewTwips(DOC, Constants.PaperWidth, "paperw", 12240))
			a.Add(NumericAttribute.NewTwips(DOC, Constants.PaperHeight, "paperh", 15840))
			a.Add(NumericAttribute.NewTwips(DOC, Constants.MarginLeft, "margl", 1800))
			a.Add(NumericAttribute.NewTwips(DOC, Constants.MarginRight, "margr", 1800))
			a.Add(NumericAttribute.NewTwips(DOC, Constants.MarginTop, "margt", 1440))
			a.Add(NumericAttribute.NewTwips(DOC, Constants.MarginBottom, "margb", 1440))
			a.Add(NumericAttribute.NewTwips(DOC, Constants.GutterWidth, "gutter", 0))

			a.Add(New AssertiveAttribute(PGF, Constants.WidowControl, "nowidctlpar", [False]))
			a.Add(New AssertiveAttribute(PGF, Constants.WidowControl, "widctlpar", [True]))
			a.Add(New AssertiveAttribute(DOC, Constants.WidowControl, "widowctrl", [True]))


			Dim attrs As RTFAttribute() = New RTFAttribute(a.Count - 1){}
			a.CopyTo(attrs)
			attributes = attrs
		End Sub

		Friend Shared Function attributesByKeyword() As Dictionary(Of String, RTFAttribute)
			Dim d As Dictionary(Of String, RTFAttribute) = New Dictionary(Of String, RTFAttribute)(attributes.Length)

			For Each attribute As RTFAttribute In attributes
				d.put(attribute.rtfName(), attribute)
			Next attribute

			Return d
		End Function

		''' <summary>
		'''********************************************************************* </summary>
		''' <summary>
		'''********************************************************************* </summary>

		Friend MustInherit Class GenericAttribute
			Friend ___domain As Integer
			Friend ___swingName As Object
			Friend ___rtfName As String

			Protected Friend Sub New(ByVal d As Integer, ByVal s As Object, ByVal r As String)
				___domain = d
				___swingName = s
				___rtfName = r
			End Sub

			Public Overridable Function domain() As Integer
				Return ___domain
			End Function
			Public Overridable Function swingName() As Object
				Return ___swingName
			End Function
			Public Overridable Function rtfName() As String
				Return ___rtfName
			End Function

			Friend MustOverride Function [set](ByVal target As javax.swing.text.MutableAttributeSet) As Boolean
			Friend MustOverride Function [set](ByVal target As javax.swing.text.MutableAttributeSet, ByVal parameter As Integer) As Boolean
			Friend MustOverride Function setDefault(ByVal target As javax.swing.text.MutableAttributeSet) As Boolean

			Public Overridable Function write(ByVal source As javax.swing.text.AttributeSet, ByVal target As RTFGenerator, ByVal force As Boolean) As Boolean
				Return writeValue(source.getAttribute(___swingName), target, force)
			End Function

			Public Overridable Function writeValue(ByVal value As Object, ByVal target As RTFGenerator, ByVal force As Boolean) As Boolean
				Return False
			End Function
		End Class

		Friend Class BooleanAttribute
			Inherits GenericAttribute
			Implements RTFAttribute

			Friend rtfDefault As Boolean
			Friend swingDefault As Boolean

			Protected Friend Shared ReadOnly [True] As Boolean? = Boolean.valueOf([True])
			Protected Friend Shared ReadOnly [False] As Boolean? = Boolean.valueOf([False])

			Public Sub New(ByVal d As Integer, ByVal s As Object, ByVal r As String, ByVal ds As Boolean, ByVal dr As Boolean)
				MyBase.New(d, s, r)
				swingDefault = ds
				rtfDefault = dr
			End Sub

			Public Sub New(ByVal d As Integer, ByVal s As Object, ByVal r As String)
				MyBase.New(d, s, r)

				swingDefault = [False]
				rtfDefault = [False]
			End Sub

			Public Overrides Function [set](ByVal target As javax.swing.text.MutableAttributeSet) As Boolean Implements RTFAttribute.set
	'             TODO: There's some ambiguity about whether this should
	'               *set* or *toggle* the attribute. 
				target.addAttribute(___swingName, [True])

				Return [True] ' true indicates we were successful
			End Function

			Public Overrides Function [set](ByVal target As javax.swing.text.MutableAttributeSet, ByVal parameter As Integer) As Boolean Implements RTFAttribute.set
				' See above note in the case that parameter==1 
				Dim value As Boolean? = (If(parameter <> 0, [True], [False]))

				target.addAttribute(___swingName, value)

				Return [True] ' true indicates we were successful
			End Function

			Public Overrides Function setDefault(ByVal target As javax.swing.text.MutableAttributeSet) As Boolean Implements RTFAttribute.setDefault
				If swingDefault <> rtfDefault OrElse (target.getAttribute(___swingName) IsNot Nothing) Then target.addAttribute(___swingName, Convert.ToBoolean(rtfDefault))
				Return [True]
			End Function

			Public Overrides Function writeValue(ByVal o_value As Object, ByVal target As RTFGenerator, ByVal force As Boolean) As Boolean Implements RTFAttribute.writeValue
				Dim val As Boolean?

				If o_value Is Nothing Then
				  val = Convert.ToBoolean(swingDefault)
				Else
				  val = CBool(o_value)
				End If

				If force OrElse (val <> rtfDefault) Then
					If val Then
						target.writeControlWord(___rtfName)
					Else
						target.writeControlWord(___rtfName, 0)
					End If
				End If
				Return [True]
			End Function
		End Class


		Friend Class AssertiveAttribute
			Inherits GenericAttribute
			Implements RTFAttribute

			Friend swingValue As Object

			Public Sub New(ByVal d As Integer, ByVal s As Object, ByVal r As String)
				MyBase.New(d, s, r)
				swingValue = Convert.ToBoolean(True)
			End Sub

			Public Sub New(ByVal d As Integer, ByVal s As Object, ByVal r As String, ByVal v As Object)
				MyBase.New(d, s, r)
				swingValue = v
			End Sub

			Public Sub New(ByVal d As Integer, ByVal s As Object, ByVal r As String, ByVal v As Integer)
				MyBase.New(d, s, r)
				swingValue = Convert.ToInt32(v)
			End Sub

			Public Overrides Function [set](ByVal target As javax.swing.text.MutableAttributeSet) As Boolean Implements RTFAttribute.set
				If swingValue Is Nothing Then
					target.removeAttribute(___swingName)
				Else
					target.addAttribute(___swingName, swingValue)
				End If

				Return True
			End Function

			Public Overrides Function [set](ByVal target As javax.swing.text.MutableAttributeSet, ByVal parameter As Integer) As Boolean Implements RTFAttribute.set
				Return False
			End Function

			Public Overrides Function setDefault(ByVal target As javax.swing.text.MutableAttributeSet) As Boolean Implements RTFAttribute.setDefault
				target.removeAttribute(___swingName)
				Return True
			End Function

			Public Overrides Function writeValue(ByVal value As Object, ByVal target As RTFGenerator, ByVal force As Boolean) As Boolean Implements RTFAttribute.writeValue
				If value Is Nothing Then Return Not force

				If value.Equals(swingValue) Then
					target.writeControlWord(___rtfName)
					Return True
				End If

				Return Not force
			End Function
		End Class


		Friend Class NumericAttribute
			Inherits GenericAttribute
			Implements RTFAttribute

			Friend rtfDefault As Integer
			Friend swingDefault As Number
			Friend scale As Single

			Protected Friend Sub New(ByVal d As Integer, ByVal s As Object, ByVal r As String)
				MyBase.New(d, s, r)
				rtfDefault = 0
				swingDefault = Nothing
				scale = 1f
			End Sub

			Public Sub New(ByVal d As Integer, ByVal s As Object, ByVal r As String, ByVal ds As Integer, ByVal dr As Integer)
				Me.New(d, s, r, Convert.ToInt32(ds), dr, 1f)
			End Sub

			Public Sub New(ByVal d As Integer, ByVal s As Object, ByVal r As String, ByVal ds As Number, ByVal dr As Integer, ByVal sc As Single)
				MyBase.New(d, s, r)
				swingDefault = ds
				rtfDefault = dr
				scale = sc
			End Sub

			Public Shared Function NewTwips(ByVal d As Integer, ByVal s As Object, ByVal r As String, ByVal ds As Single, ByVal dr As Integer) As NumericAttribute
				Return New NumericAttribute(d, s, r, New Single?(ds), dr, 20f)
			End Function

			Public Shared Function NewTwips(ByVal d As Integer, ByVal s As Object, ByVal r As String, ByVal dr As Integer) As NumericAttribute
				Return New NumericAttribute(d, s, r, Nothing, dr, 20f)
			End Function

			Public Overrides Function [set](ByVal target As javax.swing.text.MutableAttributeSet) As Boolean Implements RTFAttribute.set
				Return False
			End Function

			Public Overrides Function [set](ByVal target As javax.swing.text.MutableAttributeSet, ByVal parameter As Integer) As Boolean Implements RTFAttribute.set
				Dim swingValue As Number

				If scale = 1f Then
					swingValue = Convert.ToInt32(parameter)
				Else
					swingValue = New Single?(parameter / scale)
				End If
				target.addAttribute(___swingName, swingValue)
				Return True
			End Function

			Public Overrides Function setDefault(ByVal target As javax.swing.text.MutableAttributeSet) As Boolean Implements RTFAttribute.setDefault
				Dim old As Number = CType(target.getAttribute(___swingName), Number)
				If old Is Nothing Then old = swingDefault
				If old IsNot Nothing AndAlso ((scale = 1f AndAlso old = rtfDefault) OrElse (Math.Round(old * scale) = rtfDefault)) Then Return True
				[set](target, rtfDefault)
				Return True
			End Function

			Public Overrides Function writeValue(ByVal o_value As Object, ByVal target As RTFGenerator, ByVal force As Boolean) As Boolean Implements RTFAttribute.writeValue
				Dim value As Number = CType(o_value, Number)
				If value Is Nothing Then value = swingDefault
				If value Is Nothing Then Return True
				Dim int_value As Integer = Math.Round(value * scale)
				If force OrElse (int_value <> rtfDefault) Then target.writeControlWord(___rtfName, int_value)
				Return True
			End Function
		End Class
	End Class

End Namespace