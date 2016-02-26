Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2011, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.beans



	''' <summary>
	''' A utility class which generates unique names for object instances.
	''' The name will be a concatenation of the unqualified class name
	''' and an instance number.
	''' <p>
	''' For example, if the first object instance javax.swing.JButton
	''' is passed into <code>instanceName</code> then the returned
	''' string identifier will be &quot;JButton0&quot;.
	''' 
	''' @author Philip Milne
	''' </summary>
	Friend Class NameGenerator

		Private valueToName As IDictionary(Of Object, String)
		Private nameToCount As IDictionary(Of String, Integer?)

		Public Sub New()
			valueToName = New java.util.IdentityHashMap(Of )
			nameToCount = New Dictionary(Of )
		End Sub

		''' <summary>
		''' Clears the name cache. Should be called to near the end of
		''' the encoding cycle.
		''' </summary>
		Public Overridable Sub clear()
			valueToName.Clear()
			nameToCount.Clear()
		End Sub

		''' <summary>
		''' Returns the root name of the class.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Shared Function unqualifiedClassName(ByVal type As [Class]) As String
			If type.array Then Return unqualifiedClassName(type.componentType) & "Array"
			Dim name As String = type.name
			Return name.Substring(name.LastIndexOf("."c)+1)
		End Function

		''' <summary>
		''' Returns a String which capitalizes the first letter of the string.
		''' </summary>
		Public Shared Function capitalize(ByVal name As String) As String
			If name Is Nothing OrElse name.length() = 0 Then Return name
			Return name.Substring(0, 1).ToUpper(ENGLISH) + name.Substring(1)
		End Function

		''' <summary>
		''' Returns a unique string which identifies the object instance.
		''' Invocations are cached so that if an object has been previously
		''' passed into this method then the same identifier is returned.
		''' </summary>
		''' <param name="instance"> object used to generate string </param>
		''' <returns> a unique string representing the object </returns>
		Public Overridable Function instanceName(ByVal instance As Object) As String
			If instance Is Nothing Then Return "null"
			If TypeOf instance Is Class Then
				Return unqualifiedClassName(CType(instance, [Class]))
			Else
				Dim result As String = valueToName(instance)
				If result IsNot Nothing Then Return result
				Dim type As  [Class] = instance.GetType()
				Dim className As String = unqualifiedClassName(type)

				Dim size As Integer? = nameToCount(className)
				Dim instanceNumber As Integer = If(size Is Nothing, 0, (size) + 1)
				nameToCount(className) = New Integer?(instanceNumber)

				result = className + instanceNumber
				valueToName(instance) = result
				Return result
			End If
		End Function
	End Class

End Namespace