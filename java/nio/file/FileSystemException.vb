'
' * Copyright (c) 2007, 2009, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.file


	''' <summary>
	''' Thrown when a file system operation fails on one or two files. This class is
	''' the general class for file system exceptions.
	''' 
	''' @since 1.7
	''' </summary>

	Public Class FileSystemException
		Inherits java.io.IOException

		Friend Shadows Const serialVersionUID As Long = -3055425747967319812L

		Private ReadOnly file As String
		Private ReadOnly other As String

		''' <summary>
		''' Constructs an instance of this class. This constructor should be used
		''' when an operation involving one file fails and there isn't any additional
		''' information to explain the reason.
		''' </summary>
		''' <param name="file">
		'''          a string identifying the file or {@code null} if not known. </param>
		Public Sub New(ByVal file As String)
			MyBase.New(CStr(Nothing))
			Me.file = file
			Me.other = Nothing
		End Sub

		''' <summary>
		''' Constructs an instance of this class. This constructor should be used
		''' when an operation involving two files fails, or there is additional
		''' information to explain the reason.
		''' </summary>
		''' <param name="file">
		'''          a string identifying the file or {@code null} if not known. </param>
		''' <param name="other">
		'''          a string identifying the other file or {@code null} if there
		'''          isn't another file or if not known </param>
		''' <param name="reason">
		'''          a reason message with additional information or {@code null} </param>
		Public Sub New(ByVal file As String, ByVal other As String, ByVal reason As String)
			MyBase.New(reason)
			Me.file = file
			Me.other = other
		End Sub

		''' <summary>
		''' Returns the file used to create this exception.
		''' </summary>
		''' <returns>  the file (can be {@code null}) </returns>
		Public Overridable Property file As String
			Get
				Return file
			End Get
		End Property

		''' <summary>
		''' Returns the other file used to create this exception.
		''' </summary>
		''' <returns>  the other file (can be {@code null}) </returns>
		Public Overridable Property otherFile As String
			Get
				Return other
			End Get
		End Property

		''' <summary>
		''' Returns the string explaining why the file system operation failed.
		''' </summary>
		''' <returns>  the string explaining why the file system operation failed </returns>
		Public Overridable Property reason As String
			Get
				Return MyBase.message
			End Get
		End Property

		''' <summary>
		''' Returns the detail message string.
		''' </summary>
		Public Property Overrides message As String
			Get
				If file Is Nothing AndAlso other Is Nothing Then Return reason
				Dim sb As New StringBuilder
				If file IsNot Nothing Then sb.append(file)
				If other IsNot Nothing Then
					sb.append(" -> ")
					sb.append(other)
				End If
				If reason IsNot Nothing Then
					sb.append(": ")
					sb.append(reason)
				End If
				Return sb.ToString()
			End Get
		End Property
	End Class

End Namespace