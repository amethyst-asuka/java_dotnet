'
' * Copyright (c) 2003, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang

	''' <summary>
	''' The CharacterData class encapsulates the large tables found in
	'''    Java.lang.Character. 
	''' </summary>

	Friend Class CharacterDataUndefined
		Inherits CharacterData

		Friend Overrides Function getProperties(ByVal ch As Integer) As Integer
			Return 0
		End Function

		Friend Overrides Function [getType](ByVal ch As Integer) As Integer
		Return Character.UNASSIGNED
		End Function

		Friend Overrides Function isJavaIdentifierStart(ByVal ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function isJavaIdentifierPart(ByVal ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function isUnicodeIdentifierStart(ByVal ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function isUnicodeIdentifierPart(ByVal ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function isIdentifierIgnorable(ByVal ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function toLowerCase(ByVal ch As Integer) As Integer
			Return ch
		End Function

		Friend Overrides Function toUpperCase(ByVal ch As Integer) As Integer
			Return ch
		End Function

		Friend Overrides Function toTitleCase(ByVal ch As Integer) As Integer
			Return ch
		End Function

		Friend Overrides Function digit(ByVal ch As Integer, ByVal radix As Integer) As Integer
			Return -1
		End Function

		Friend Overrides Function getNumericValue(ByVal ch As Integer) As Integer
			Return -1
		End Function

		Friend Overrides Function isWhitespace(ByVal ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function getDirectionality(ByVal ch As Integer) As SByte
			Return Character.DIRECTIONALITY_UNDEFINED
		End Function

		Friend Overrides Function isMirrored(ByVal ch As Integer) As Boolean
			Return False
		End Function

		Friend Shared ReadOnly instance As CharacterData = New CharacterDataUndefined
		Private Sub New()
		End Sub
	End Class

End Namespace