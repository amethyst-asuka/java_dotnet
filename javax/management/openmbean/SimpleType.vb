Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.management.openmbean


	' java import
	'

	' jmx import
	'


	''' <summary>
	''' The <code>SimpleType</code> class is the <i>open type</i> class whose instances describe
	''' all <i>open data</i> values which are neither arrays,
	''' nor <seealso cref="CompositeData CompositeData"/> values,
	''' nor <seealso cref="TabularData TabularData"/> values.
	''' It predefines all its possible instances as static fields, and has no public constructor.
	''' <p>
	''' Given a <code>SimpleType</code> instance describing values whose Java class name is <i>className</i>,
	''' the internal fields corresponding to the name and description of this <code>SimpleType</code> instance
	''' are also set to <i>className</i>.
	''' In other words, its methods <code>getClassName</code>, <code>getTypeName</code> and <code>getDescription</code>
	''' all return the same string value <i>className</i>.
	''' 
	''' @since 1.5
	''' </summary>
	Public NotInheritable Class SimpleType(Of T)
		Inherits OpenType(Of T)

		' Serial version 
		Friend Const serialVersionUID As Long = 2215577471957694503L

		' SimpleType instances.
		' IF YOU ADD A SimpleType, YOU MUST UPDATE OpenType and typeArray

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.lang.Void</code>.
		''' </summary>
		Public Shared ReadOnly VOID As New SimpleType(Of Void)(GetType(Void))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.lang.Boolean</code>.
		''' </summary>
		Public Shared ReadOnly [BOOLEAN] As New SimpleType(Of Boolean?)(GetType(Boolean))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.lang.Character</code>.
		''' </summary>
		Public Shared ReadOnly CHARACTER As New SimpleType(Of Char?)(GetType(Char))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.lang.Byte</code>.
		''' </summary>
		Public Shared ReadOnly [BYTE] As New SimpleType(Of SByte?)(GetType(SByte))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.lang.Short</code>.
		''' </summary>
		Public Shared ReadOnly [SHORT] As New SimpleType(Of Short?)(GetType(Short))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.lang.Integer</code>.
		''' </summary>
		Public Shared ReadOnly [INTEGER] As New SimpleType(Of Integer?)(GetType(Integer))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.lang.Long</code>.
		''' </summary>
		Public Shared ReadOnly [LONG] As New SimpleType(Of Long?)(GetType(Long))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.lang.Float</code>.
		''' </summary>
		Public Shared ReadOnly FLOAT As New SimpleType(Of Single?)(GetType(Single))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.lang.Double</code>.
		''' </summary>
		Public Shared ReadOnly [DOUBLE] As New SimpleType(Of Double?)(GetType(Double))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.lang.String</code>.
		''' </summary>
		Public Shared ReadOnly [STRING] As New SimpleType(Of String)(GetType(String))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.math.BigDecimal</code>.
		''' </summary>
		Public Shared ReadOnly BIGDECIMAL As New SimpleType(Of Decimal)(GetType(Decimal))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.math.BigInteger</code>.
		''' </summary>
		Public Shared ReadOnly BIGINTEGER As New SimpleType(Of System.Numerics.BigInteger)(GetType(System.Numerics.BigInteger))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>java.util.Date</code>.
		''' </summary>
		Public Shared ReadOnly [DATE] As New SimpleType(Of DateTime)(GetType(DateTime))

		''' <summary>
		''' The <code>SimpleType</code> instance describing values whose
		''' Java class name is <code>javax.management.ObjectName</code>.
		''' </summary>
		Public Shared ReadOnly OBJECTNAME As New SimpleType(Of javax.management.ObjectName)(GetType(javax.management.ObjectName))

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared ReadOnly typeArray As SimpleType(Of ?)() = { VOID, [BOOLEAN], CHARACTER, [BYTE], [SHORT], [INTEGER], [LONG], FLOAT, [DOUBLE], [STRING], BIGDECIMAL, BIGINTEGER, [DATE], OBJECTNAME }


		<NonSerialized> _
		Private myHashCode As Integer? = Nothing ' As this instance is immutable, these two values
		<NonSerialized> _
		Private myToString As String = Nothing ' need only be calculated once.


		' *** Constructor *** 

		Private Sub New(ByVal valueClass As Type)
			MyBase.New(valueClass.name, valueClass.name, valueClass.name, False)
		End Sub


		' *** SimpleType specific information methods *** 

		''' <summary>
		''' Tests whether <var>obj</var> is a value for this
		''' <code>SimpleType</code> instance.  <p> This method returns
		''' <code>true</code> if and only if <var>obj</var> is not null and
		''' <var>obj</var>'s class name is the same as the className field
		''' defined for this <code>SimpleType</code> instance (ie the class
		''' name returned by the {@link OpenType#getClassName()
		''' getClassName} method).
		''' </summary>
		''' <param name="obj"> the object to be tested.
		''' </param>
		''' <returns> <code>true</code> if <var>obj</var> is a value for this
		''' <code>SimpleType</code> instance. </returns>
		Public Function isValue(ByVal obj As Object) As Boolean

			' if obj is null, return false
			'
			If obj Is Nothing Then Return False

			' Test if obj's class name is the same as for this instance
			'
			Return Me.className.Equals(obj.GetType().name)
		End Function


		' *** Methods overriden from class Object *** 

		''' <summary>
		''' Compares the specified <code>obj</code> parameter with this <code>SimpleType</code> instance for equality.
		''' <p>
		''' Two <code>SimpleType</code> instances are equal if and only if their
		''' <seealso cref="OpenType#getClassName() getClassName"/> methods return the same value.
		''' </summary>
		''' <param name="obj">  the object to be compared for equality with this <code>SimpleType</code> instance;
		'''              if <var>obj</var> is <code>null</code> or is not an instance of the class <code>SimpleType</code>,
		'''              <code>equals</code> returns <code>false</code>.
		''' </param>
		''' <returns>  <code>true</code> if the specified object is equal to this <code>SimpleType</code> instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

	'         If it weren't for readReplace(), we could replace this method
	'           with just:
	'           return (this == obj);
	'        

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			If Not(TypeOf obj Is SimpleType(Of ?)) Then Return False

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim other As SimpleType(Of ?) = CType(obj, SimpleType(Of ?))

			' Test if other's className field is the same as for this instance
			'
			Return Me.className.Equals(other.className)
		End Function

		''' <summary>
		''' Returns the hash code value for this <code>SimpleType</code> instance.
		''' The hash code of a <code>SimpleType</code> instance is the the hash code of
		''' the string value returned by the <seealso cref="OpenType#getClassName() getClassName"/> method.
		''' <p>
		''' As <code>SimpleType</code> instances are immutable, the hash code for this instance is calculated once,
		''' on the first call to <code>hashCode</code>, and then the same value is returned for subsequent calls.
		''' </summary>
		''' <returns>  the hash code value for this <code>SimpleType</code> instance </returns>
		Public Overrides Function GetHashCode() As Integer

			' Calculate the hash code value if it has not yet been done (ie 1st call to hashCode())
			'
			If myHashCode Is Nothing Then myHashCode = Convert.ToInt32(Me.className.GetHashCode())

			' return always the same hash code for this instance (immutable)
			'
			Return myHashCode
		End Function

		''' <summary>
		''' Returns a string representation of this <code>SimpleType</code> instance.
		''' <p>
		''' The string representation consists of
		''' the name of this class (ie <code>javax.management.openmbean.SimpleType</code>) and the type name
		''' for this instance (which is the java class name of the values this <code>SimpleType</code> instance represents).
		''' <p>
		''' As <code>SimpleType</code> instances are immutable, the string representation for this instance is calculated once,
		''' on the first call to <code>toString</code>, and then the same value is returned for subsequent calls.
		''' </summary>
		''' <returns>  a string representation of this <code>SimpleType</code> instance </returns>
		Public Overrides Function ToString() As String

			' Calculate the string representation if it has not yet been done (ie 1st call to toString())
			'
			If myToString Is Nothing Then myToString = Me.GetType().name & "(name=" & typeName & ")"

			' return always the same string representation for this instance (immutable)
			'
			Return myToString
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared ReadOnly canonicalTypes As IDictionary(Of SimpleType(Of ?), SimpleType(Of ?)) = New Dictionary(Of SimpleType(Of ?), SimpleType(Of ?))
		Shared Sub New()
			For i As Integer = 0 To typeArray.Length - 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim type As SimpleType(Of ?) = typeArray(i)
				canonicalTypes(type) = type
			Next i
		End Sub

		''' <summary>
		''' Replace an object read from an {@link
		''' java.io.ObjectInputStream} with the unique instance for that
		''' value.
		''' </summary>
		''' <returns> the replacement object.
		''' </returns>
		''' <exception cref="ObjectStreamException"> if the read object cannot be
		''' resolved. </exception>
		Public Function readResolve() As Object
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			Dim canonical As SimpleType(Of ?) = canonicalTypes(Me)
			If canonical Is Nothing Then Throw New java.io.InvalidObjectException("Invalid SimpleType: " & Me)
			Return canonical
		End Function
	End Class

End Namespace