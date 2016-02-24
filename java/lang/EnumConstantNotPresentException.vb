'
' * Copyright (c) 2004, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' Thrown when an application tries to access an enum constant by name
	''' and the enum type contains no constant with the specified name.
	''' This exception can be thrown by the {@linkplain
	''' java.lang.reflect.AnnotatedElement API used to read annotations
	''' reflectively}.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref=     java.lang.reflect.AnnotatedElement
	''' @since   1.5 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Class EnumConstantNotPresentException
		Inherits RuntimeException
 ' rawtypes are part of the public api
		Private Shadows Const serialVersionUID As Long = -6046998521960521108L

        ''' <summary>
        ''' The type of the missing enum constant.
        ''' </summary>
        Private enumType_Renamed As [Class]

        ''' <summary>
        ''' The name of the missing enum constant.
        ''' </summary>
        Private constantName_Renamed As String

        ''' <summary>
        ''' Constructs an <tt>EnumConstantNotPresentException</tt> for the
        ''' specified constant.
        ''' </summary>
        ''' <param name="enumType"> the type of the missing enum constant </param>
        ''' <param name="constantName"> the name of the missing enum constant </param>
        Public Sub New(ByVal enumType As [Class], ByVal constantName As String)
            MyBase.New(enumType.name & "." & constantName)
            Me.enumType_Renamed = enumType
            Me.constantName_Renamed = constantName
        End Sub

        ''' <summary>
        ''' Returns the type of the missing enum constant.
        ''' </summary>
        ''' <returns> the type of the missing enum constant </returns>
        Public Overridable Function enumType() As [Class]
            Return enumType_Renamed
        End Function

        ''' <summary>
        ''' Returns the name of the missing enum constant.
        ''' </summary>
        ''' <returns> the name of the missing enum constant </returns>
        Public Overridable Function constantName() As String
			Return constantName_Renamed
		End Function
	End Class

End Namespace