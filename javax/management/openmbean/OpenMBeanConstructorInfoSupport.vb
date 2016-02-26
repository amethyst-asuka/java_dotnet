Imports System
Imports System.Text

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


Namespace javax.management.openmbean


	' java import
	'



	''' <summary>
	''' Describes a constructor of an Open MBean.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class OpenMBeanConstructorInfoSupport
		Inherits javax.management.MBeanConstructorInfo
		Implements OpenMBeanConstructorInfo

		' Serial version 
		Friend Shadows Const serialVersionUID As Long = -4400441579007477003L


		' As this instance is immutable,
		' these two values need only be calculated once.
		<NonSerialized> _
		Private myHashCode As Integer? = Nothing
		<NonSerialized> _
		Private myToString As String = Nothing

		''' <summary>
		''' <p>Constructs an {@code OpenMBeanConstructorInfoSupport}
		''' instance, which describes the constructor of a class of open
		''' MBeans with the specified {@code name}, {@code description} and
		''' {@code signature}.</p>
		''' 
		''' <p>The {@code signature} array parameter is internally copied,
		''' so that subsequent changes to the array referenced by {@code
		''' signature} have no effect on this instance.</p>
		''' </summary>
		''' <param name="name"> cannot be a null or empty string.
		''' </param>
		''' <param name="description"> cannot be a null or empty string.
		''' </param>
		''' <param name="signature"> can be null or empty if there are no
		''' parameters to describe.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string.
		''' </exception>
		''' <exception cref="ArrayStoreException"> If {@code signature} is not an
		''' array of instances of a subclass of {@code MBeanParameterInfo}. </exception>
		Public Sub New(ByVal name As String, ByVal description As String, ByVal signature As OpenMBeanParameterInfo())
			Me.New(name, description, signature, CType(Nothing, javax.management.Descriptor))
		End Sub

		''' <summary>
		''' <p>Constructs an {@code OpenMBeanConstructorInfoSupport}
		''' instance, which describes the constructor of a class of open
		''' MBeans with the specified {@code name}, {@code description},
		''' {@code signature}, and {@code descriptor}.</p>
		''' 
		''' <p>The {@code signature} array parameter is internally copied,
		''' so that subsequent changes to the array referenced by {@code
		''' signature} have no effect on this instance.</p>
		''' </summary>
		''' <param name="name"> cannot be a null or empty string.
		''' </param>
		''' <param name="description"> cannot be a null or empty string.
		''' </param>
		''' <param name="signature"> can be null or empty if there are no
		''' parameters to describe.
		''' </param>
		''' <param name="descriptor"> The descriptor for the constructor.  This may
		''' be null which is equivalent to an empty descriptor.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string.
		''' </exception>
		''' <exception cref="ArrayStoreException"> If {@code signature} is not an
		''' array of instances of a subclass of {@code MBeanParameterInfo}.
		''' 
		''' @since 1.6 </exception>
		Public Sub New(ByVal name As String, ByVal description As String, ByVal signature As OpenMBeanParameterInfo(), ByVal descriptor As javax.management.Descriptor)
			MyBase.New(name, description, arrayCopyCast(signature), descriptor) ' may throw an ArrayStoreException

			' check parameters that should not be null or empty
			' (unfortunately it is not done in superclass :-( ! )
			'
			If name Is Nothing OrElse name.Trim().Equals("") Then Throw New System.ArgumentException("Argument name cannot be " & "null or empty")
			If description Is Nothing OrElse description.Trim().Equals("") Then Throw New System.ArgumentException("Argument description cannot " & "be null or empty")

		End Sub

		Private Shared Function arrayCopyCast(ByVal src As OpenMBeanParameterInfo()) As javax.management.MBeanParameterInfo()
			If src Is Nothing Then Return Nothing

			Dim dst As javax.management.MBeanParameterInfo() = New javax.management.MBeanParameterInfo(src.Length - 1){}
			Array.Copy(src, 0, dst, 0, src.Length)
			' may throw an ArrayStoreException
			Return dst
		End Function


		' ***  Commodity methods from java.lang.Object  *** 


		''' <summary>
		''' <p>Compares the specified {@code obj} parameter with this
		''' {@code OpenMBeanConstructorInfoSupport} instance for
		''' equality.</p>
		''' 
		''' <p>Returns {@code true} if and only if all of the following
		''' statements are true:
		''' 
		''' <ul>
		''' <li>{@code obj} is non null,</li>
		''' <li>{@code obj} also implements the {@code
		''' OpenMBeanConstructorInfo} interface,</li>
		''' <li>their names are equal</li>
		''' <li>their signatures are equal.</li>
		''' </ul>
		''' 
		''' This ensures that this {@code equals} method works properly for
		''' {@code obj} parameters which are different implementations of
		''' the {@code OpenMBeanConstructorInfo} interface.
		''' </summary>
		''' <param name="obj"> the object to be compared for equality with this
		''' {@code OpenMBeanConstructorInfoSupport} instance;
		''' </param>
		''' <returns> {@code true} if the specified object is equal to this
		''' {@code OpenMBeanConstructorInfoSupport} instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			' if obj is null, return false
			'
			If obj Is Nothing Then Return False

			' if obj is not a OpenMBeanConstructorInfo, return false
			'
			Dim other As OpenMBeanConstructorInfo
			Try
				other = CType(obj, OpenMBeanConstructorInfo)
			Catch e As ClassCastException
				Return False
			End Try

			' Now, really test for equality between this
			' OpenMBeanConstructorInfo implementation and the other:
			'

			' their Name should be equal
			If Not Me.name.Equals(other.name) Then Return False

			' their Signatures should be equal
			If Not java.util.Arrays.Equals(Me.signature, other.signature) Then Return False

			' All tests for equality were successfull
			'
			Return True
		End Function

		''' <summary>
		''' <p>Returns the hash code value for this {@code
		''' OpenMBeanConstructorInfoSupport} instance.</p>
		''' 
		''' <p>The hash code of an {@code OpenMBeanConstructorInfoSupport}
		''' instance is the sum of the hash codes of all elements of
		''' information used in {@code equals} comparisons (ie: its name
		''' and signature, where the signature hashCode is calculated by a
		''' call to {@code
		''' java.util.Arrays.asList(this.getSignature).hashCode()}).</p>
		''' 
		''' <p>This ensures that {@code t1.equals(t2)} implies that {@code
		''' t1.hashCode()==t2.hashCode()} for any two {@code
		''' OpenMBeanConstructorInfoSupport} instances {@code t1} and
		''' {@code t2}, as required by the general contract of the method
		''' <seealso cref="Object#hashCode() Object.hashCode()"/>.</p>
		''' 
		''' <p>However, note that another instance of a class implementing
		''' the {@code OpenMBeanConstructorInfo} interface may be equal to
		''' this {@code OpenMBeanConstructorInfoSupport} instance as
		''' defined by <seealso cref="#equals(java.lang.Object)"/>, but may have a
		''' different hash code if it is calculated differently.</p>
		''' 
		''' <p>As {@code OpenMBeanConstructorInfoSupport} instances are
		''' immutable, the hash code for this instance is calculated once,
		''' on the first call to {@code hashCode}, and then the same value
		''' is returned for subsequent calls.</p>
		''' </summary>
		''' <returns> the hash code value for this {@code
		''' OpenMBeanConstructorInfoSupport} instance </returns>
		Public Overrides Function GetHashCode() As Integer

			' Calculate the hash code value if it has not yet been done
			' (ie 1st call to hashCode())
			'
			If myHashCode Is Nothing Then
				Dim value As Integer = 0
				value += Me.name.GetHashCode()
				value += java.util.Arrays.asList(Me.signature).GetHashCode()
				myHashCode = Convert.ToInt32(value)
			End If

			' return always the same hash code for this instance (immutable)
			'
			Return myHashCode
		End Function

		''' <summary>
		''' <p>Returns a string representation of this {@code
		''' OpenMBeanConstructorInfoSupport} instance.</p>
		''' 
		''' <p>The string representation consists of the name of this class
		''' (ie {@code
		''' javax.management.openmbean.OpenMBeanConstructorInfoSupport}),
		''' the name and signature of the described constructor and the
		''' string representation of its descriptor.</p>
		''' 
		''' <p>As {@code OpenMBeanConstructorInfoSupport} instances are
		''' immutable, the string representation for this instance is
		''' calculated once, on the first call to {@code toString}, and
		''' then the same value is returned for subsequent calls.</p>
		''' </summary>
		''' <returns> a string representation of this {@code
		''' OpenMBeanConstructorInfoSupport} instance </returns>
		Public Overrides Function ToString() As String

			' Calculate the string value if it has not yet been done (ie
			' 1st call to toString())
			'
			If myToString Is Nothing Then myToString = (New StringBuilder).Append(Me.GetType().name).append("(name=").append(Me.name).append(",signature=").append(java.util.Arrays.asList(Me.signature).ToString()).append(",descriptor=").append(Me.descriptor).append(")").ToString()

			' return always the same string representation for this
			' instance (immutable)
			'
			Return myToString
		End Function

	End Class

End Namespace