Imports System.Collections
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



	' This AttributeSet is made entirely out of tofu and Ritz Crackers
	'   and yet has a remarkably attribute-set-like interface! 
	Friend Class MockAttributeSet
		Implements javax.swing.text.AttributeSet, javax.swing.text.MutableAttributeSet

		Public backing As java.util.Dictionary(Of Object, Object)

		Public Overridable Property empty As Boolean
			Get
				 Return backing.empty
			End Get
		End Property

		Public Overridable Property attributeCount As Integer
			Get
				 Return backing.size()
			End Get
		End Property

		Public Overridable Function isDefined(ByVal name As Object) As Boolean
			 Return (backing.get(name)) IsNot Nothing
		End Function

		Public Overridable Function isEqual(ByVal attr As javax.swing.text.AttributeSet) As Boolean
			 Throw New InternalError("MockAttributeSet: charade revealed!")
		End Function

		Public Overridable Function copyAttributes() As javax.swing.text.AttributeSet
			 Throw New InternalError("MockAttributeSet: charade revealed!")
		End Function

		Public Overridable Function getAttribute(ByVal name As Object) As Object
			Return backing.get(name)
		End Function

		Public Overridable Sub addAttribute(ByVal name As Object, ByVal value As Object)
			backing.put(name, value)
		End Sub

		Public Overridable Sub addAttributes(ByVal attr As javax.swing.text.AttributeSet)
			Dim [as] As System.Collections.IEnumerator = attr.attributeNames
			Do While [as].hasMoreElements()
				Dim el As Object = [as].nextElement()
				backing.put(el, attr.getAttribute(el))
			Loop
		End Sub

		Public Overridable Sub removeAttribute(ByVal name As Object)
			backing.remove(name)
		End Sub

		Public Overridable Sub removeAttributes(ByVal attr As javax.swing.text.AttributeSet)
			 Throw New InternalError("MockAttributeSet: charade revealed!")
		End Sub

		Public Overridable Sub removeAttributes(Of T1)(ByVal en As System.Collections.IEnumerator(Of T1))
			 Throw New InternalError("MockAttributeSet: charade revealed!")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Sub setResolveParent(ByVal pp As javax.swing.text.AttributeSet) 'JavaToDotNetTempPropertySetresolveParent
		Public Overridable Property resolveParent As javax.swing.text.AttributeSet
			Set(ByVal pp As javax.swing.text.AttributeSet)
				 Throw New InternalError("MockAttributeSet: charade revealed!")
			End Set
			Get
		End Property


		Public Overridable Property attributeNames As System.Collections.IEnumerator
			Get
				 Return backing.keys()
			End Get
		End Property

		Public Overridable Function containsAttribute(ByVal name As Object, ByVal value As Object) As Boolean
			 Throw New InternalError("MockAttributeSet: charade revealed!")
		End Function

		Public Overridable Function containsAttributes(ByVal attr As javax.swing.text.AttributeSet) As Boolean
			 Throw New InternalError("MockAttributeSet: charade revealed!")
		End Function

			 Throw New InternalError("MockAttributeSet: charade revealed!")
		End Function
	End Class

End Namespace