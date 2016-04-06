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

	Friend Class CharacterDataPrivateUse
		Inherits CharacterData

		Friend Overrides Function getProperties(  ch As Integer) As Integer
			Return 0
		End Function

		Friend Overrides Function [getType](  ch As Integer) As Integer
		Return If((ch And &HFFFE) = &HFFFE, Character.UNASSIGNED, Character.PRIVATE_USE)
		End Function

		Friend Overrides Function isJavaIdentifierStart(  ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function isJavaIdentifierPart(  ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function isUnicodeIdentifierStart(  ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function isUnicodeIdentifierPart(  ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function isIdentifierIgnorable(  ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function toLowerCase(  ch As Integer) As Integer
			Return ch
		End Function

		Friend Overrides Function toUpperCase(  ch As Integer) As Integer
			Return ch
		End Function

		Friend Overrides Function toTitleCase(  ch As Integer) As Integer
			Return ch
		End Function

		Friend Overrides Function digit(  ch As Integer,   radix As Integer) As Integer
			Return -1
		End Function

		Friend Overrides Function getNumericValue(  ch As Integer) As Integer
			Return -1
		End Function

		Friend Overrides Function isWhitespace(  ch As Integer) As Boolean
			Return False
		End Function

		Friend Overrides Function getDirectionality(  ch As Integer) As SByte
		Return If((ch And &HFFFE) = &HFFFE, Character.DIRECTIONALITY_UNDEFINED, Character.DIRECTIONALITY_LEFT_TO_RIGHT)
		End Function

		Friend Overrides Function isMirrored(  ch As Integer) As Boolean
			Return False
		End Function

		Friend Shared ReadOnly instance As CharacterData = New CharacterDataPrivateUse
		Private Sub New()
		End Sub
	End Class



End Namespace