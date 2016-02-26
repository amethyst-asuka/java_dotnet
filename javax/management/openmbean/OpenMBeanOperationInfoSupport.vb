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
	''' Describes an operation of an Open MBean.
	''' 
	''' 
	''' @since 1.5
	''' </summary>
	Public Class OpenMBeanOperationInfoSupport
		Inherits javax.management.MBeanOperationInfo
		Implements OpenMBeanOperationInfo

		' Serial version 
		Friend Shadows Const serialVersionUID As Long = 4996859732565369366L

		''' <summary>
		''' @serial The <i>open type</i> of the values returned by the operation
		'''         described by this <seealso cref="OpenMBeanOperationInfo"/> instance
		''' 
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private returnOpenType As OpenType(Of ?)


		' As this instance is immutable,
		' these two values need only be calculated once.
		<NonSerialized> _
		Private myHashCode As Integer? = Nothing
		<NonSerialized> _
		Private myToString As String = Nothing


		''' <summary>
		''' <p>Constructs an {@code OpenMBeanOperationInfoSupport}
		''' instance, which describes the operation of a class of open
		''' MBeans, with the specified {@code name}, {@code description},
		''' {@code signature}, {@code returnOpenType} and {@code
		''' impact}.</p>
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
		''' <param name="returnOpenType"> cannot be null: use {@code
		''' SimpleType.VOID} for operations that return nothing.
		''' </param>
		''' <param name="impact"> must be one of {@code ACTION}, {@code
		''' ACTION_INFO}, {@code INFO}, or {@code UNKNOWN}.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string, or {@code
		''' returnOpenType} is null, or {@code impact} is not one of {@code
		''' ACTION}, {@code ACTION_INFO}, {@code INFO}, or {@code UNKNOWN}.
		''' </exception>
		''' <exception cref="ArrayStoreException"> If {@code signature} is not an
		''' array of instances of a subclass of {@code MBeanParameterInfo}. </exception>
		Public Sub New(Of T1)(ByVal name As String, ByVal description As String, ByVal signature As OpenMBeanParameterInfo(), ByVal returnOpenType As OpenType(Of T1), ByVal impact As Integer)
			Me.New(name, description, signature, returnOpenType, impact, CType(Nothing, javax.management.Descriptor))
		End Sub

		''' <summary>
		''' <p>Constructs an {@code OpenMBeanOperationInfoSupport}
		''' instance, which describes the operation of a class of open
		''' MBeans, with the specified {@code name}, {@code description},
		''' {@code signature}, {@code returnOpenType}, {@code
		''' impact}, and {@code descriptor}.</p>
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
		''' <param name="returnOpenType"> cannot be null: use {@code
		''' SimpleType.VOID} for operations that return nothing.
		''' </param>
		''' <param name="impact"> must be one of {@code ACTION}, {@code
		''' ACTION_INFO}, {@code INFO}, or {@code UNKNOWN}.
		''' </param>
		''' <param name="descriptor"> The descriptor for the operation.  This may
		''' be null, which is equivalent to an empty descriptor.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} or {@code
		''' description} are null or empty string, or {@code
		''' returnOpenType} is null, or {@code impact} is not one of {@code
		''' ACTION}, {@code ACTION_INFO}, {@code INFO}, or {@code UNKNOWN}.
		''' </exception>
		''' <exception cref="ArrayStoreException"> If {@code signature} is not an
		''' array of instances of a subclass of {@code MBeanParameterInfo}.
		''' 
		''' @since 1.6 </exception>
		Public Sub New(Of T1)(ByVal name As String, ByVal description As String, ByVal signature As OpenMBeanParameterInfo(), ByVal returnOpenType As OpenType(Of T1), ByVal impact As Integer, ByVal descriptor As javax.management.Descriptor)
			MyBase.New(name, description, arrayCopyCast(signature),If(returnOpenType Is Nothing, Nothing, returnOpenType.className), impact, javax.management.ImmutableDescriptor.union(descriptor,If(returnOpenType Is Nothing, Nothing, returnOpenType.descriptor)))
				  ' must prevent NPE here - we will throw IAE later on if
				  ' returnOpenType is null
					' must prevent NPE here - we will throw IAE later on if
					' returnOpenType is null

			' check parameters that should not be null or empty
			' (unfortunately it is not done in superclass :-( ! )
			'
			If name Is Nothing OrElse name.Trim().Equals("") Then Throw New System.ArgumentException("Argument name cannot " & "be null or empty")
			If description Is Nothing OrElse description.Trim().Equals("") Then Throw New System.ArgumentException("Argument description cannot " & "be null or empty")
			If returnOpenType Is Nothing Then Throw New System.ArgumentException("Argument returnOpenType " & "cannot be null")

			If impact <> ACTION AndAlso impact <> ACTION_INFO AndAlso impact <> INFO AndAlso impact <> UNKNOWN Then Throw New System.ArgumentException("Argument impact can only be " & "one of ACTION, ACTION_INFO, " & "INFO, or UNKNOWN: " & impact)

			Me.returnOpenType = returnOpenType
		End Sub


		' Converts an array of OpenMBeanParameterInfo objects extending
		' MBeanParameterInfo into an array of MBeanParameterInfo.
		'
		Private Shared Function arrayCopyCast(ByVal src As OpenMBeanParameterInfo()) As javax.management.MBeanParameterInfo()
			If src Is Nothing Then Return Nothing

			Dim dst As javax.management.MBeanParameterInfo() = New javax.management.MBeanParameterInfo(src.Length - 1){}
			Array.Copy(src, 0, dst, 0, src.Length)
			' may throw an ArrayStoreException
			Return dst
		End Function

		' Converts an array of MBeanParameterInfo objects implementing
		' OpenMBeanParameterInfo into an array of OpenMBeanParameterInfo.
		'
		Private Shared Function arrayCopyCast(ByVal src As javax.management.MBeanParameterInfo()) As OpenMBeanParameterInfo()
			If src Is Nothing Then Return Nothing

			Dim dst As OpenMBeanParameterInfo() = New OpenMBeanParameterInfo(src.Length - 1){}
			Array.Copy(src, 0, dst, 0, src.Length)
			' may throw an ArrayStoreException
			Return dst
		End Function


		' [JF]: should we add constructor with java.lang.reflect.Method
		' method parameter ?  would need to add consistency check between
		' OpenType<?> returnOpenType and method.getReturnType().


		''' <summary>
		''' Returns the <i>open type</i> of the values returned by the
		''' operation described by this {@code OpenMBeanOperationInfo}
		''' instance.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property returnOpenType As OpenType(Of ?) Implements OpenMBeanOperationInfo.getReturnOpenType
			Get
    
				Return returnOpenType
			End Get
		End Property



		' ***  Commodity methods from java.lang.Object  *** 


		''' <summary>
		''' <p>Compares the specified {@code obj} parameter with this
		''' {@code OpenMBeanOperationInfoSupport} instance for
		''' equality.</p>
		''' 
		''' <p>Returns {@code true} if and only if all of the following
		''' statements are true:
		''' 
		''' <ul>
		''' <li>{@code obj} is non null,</li>
		''' <li>{@code obj} also implements the {@code
		''' OpenMBeanOperationInfo} interface,</li>
		''' <li>their names are equal</li>
		''' <li>their signatures are equal</li>
		''' <li>their return open types are equal</li>
		''' <li>their impacts are equal</li>
		''' </ul>
		''' 
		''' This ensures that this {@code equals} method works properly for
		''' {@code obj} parameters which are different implementations of
		''' the {@code OpenMBeanOperationInfo} interface.
		''' </summary>
		''' <param name="obj"> the object to be compared for equality with this
		''' {@code OpenMBeanOperationInfoSupport} instance;
		''' </param>
		''' <returns> {@code true} if the specified object is equal to this
		''' {@code OpenMBeanOperationInfoSupport} instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			' if obj is null, return false
			'
			If obj Is Nothing Then Return False

			' if obj is not a OpenMBeanOperationInfo, return false
			'
			Dim other As OpenMBeanOperationInfo
			Try
				other = CType(obj, OpenMBeanOperationInfo)
			Catch e As ClassCastException
				Return False
			End Try

			' Now, really test for equality between this
			' OpenMBeanOperationInfo implementation and the other:
			'

			' their Name should be equal
			If Not Me.name.Equals(other.name) Then Return False

			' their Signatures should be equal
			If Not java.util.Arrays.Equals(Me.signature, other.signature) Then Return False

			' their return open types should be equal
			If Not Me.returnOpenType.Equals(other.returnOpenType) Then Return False

			' their impacts should be equal
			If Me.impact <> other.impact Then Return False

			' All tests for equality were successfull
			'
			Return True
		End Function

		''' <summary>
		''' <p>Returns the hash code value for this {@code
		''' OpenMBeanOperationInfoSupport} instance.</p>
		''' 
		''' <p>The hash code of an {@code OpenMBeanOperationInfoSupport}
		''' instance is the sum of the hash codes of all elements of
		''' information used in {@code equals} comparisons (ie: its name,
		''' return open type, impact and signature, where the signature
		''' hashCode is calculated by a call to {@code
		''' java.util.Arrays.asList(this.getSignature).hashCode()}).</p>
		''' 
		''' <p>This ensures that {@code t1.equals(t2) } implies that {@code
		''' t1.hashCode()==t2.hashCode() } for any two {@code
		''' OpenMBeanOperationInfoSupport} instances {@code t1} and {@code
		''' t2}, as required by the general contract of the method {@link
		''' Object#hashCode() Object.hashCode()}.</p>
		''' 
		''' <p>However, note that another instance of a class implementing
		''' the {@code OpenMBeanOperationInfo} interface may be equal to
		''' this {@code OpenMBeanOperationInfoSupport} instance as defined
		''' by <seealso cref="#equals(java.lang.Object)"/>, but may have a different
		''' hash code if it is calculated differently.</p>
		''' 
		''' <p>As {@code OpenMBeanOperationInfoSupport} instances are
		''' immutable, the hash code for this instance is calculated once,
		''' on the first call to {@code hashCode}, and then the same value
		''' is returned for subsequent calls.</p>
		''' </summary>
		''' <returns> the hash code value for this {@code
		''' OpenMBeanOperationInfoSupport} instance </returns>
		Public Overrides Function GetHashCode() As Integer

			' Calculate the hash code value if it has not yet been done
			' (ie 1st call to hashCode())
			'
			If myHashCode Is Nothing Then
				Dim value As Integer = 0
				value += Me.name.GetHashCode()
				value += java.util.Arrays.asList(Me.signature).GetHashCode()
				value += Me.returnOpenType.GetHashCode()
				value += Me.impact
				myHashCode = Convert.ToInt32(value)
			End If

			' return always the same hash code for this instance (immutable)
			'
			Return myHashCode
		End Function

		''' <summary>
		''' <p>Returns a string representation of this {@code
		''' OpenMBeanOperationInfoSupport} instance.</p>
		''' 
		''' <p>The string representation consists of the name of this class
		''' (ie {@code
		''' javax.management.openmbean.OpenMBeanOperationInfoSupport}), and
		''' the name, signature, return open type and impact of the
		''' described operation and the string representation of its descriptor.</p>
		''' 
		''' <p>As {@code OpenMBeanOperationInfoSupport} instances are
		''' immutable, the string representation for this instance is
		''' calculated once, on the first call to {@code toString}, and
		''' then the same value is returned for subsequent calls.</p>
		''' </summary>
		''' <returns> a string representation of this {@code
		''' OpenMBeanOperationInfoSupport} instance </returns>
		Public Overrides Function ToString() As String

			' Calculate the hash code value if it has not yet been done
			' (ie 1st call to toString())
			'
			If myToString Is Nothing Then myToString = (New StringBuilder).Append(Me.GetType().name).append("(name=").append(Me.name).append(",signature=").append(java.util.Arrays.asList(Me.signature).ToString()).append(",return=").append(Me.returnOpenType.ToString()).append(",impact=").append(Me.impact).append(",descriptor=").append(Me.descriptor).append(")").ToString()

			' return always the same string representation for this
			' instance (immutable)
			'
			Return myToString
		End Function

		''' <summary>
		''' An object serialized in a version of the API before Descriptors were
		''' added to this class will have an empty or null Descriptor.
		''' For consistency with our
		''' behavior in this version, we must replace the object with one
		''' where the Descriptors reflect the same value of returned openType.
		''' 
		''' </summary>
		Private Function readResolve() As Object
			If descriptor.fieldNames.Length = 0 Then
				' This constructor will construct the expected default Descriptor.
				'
				Return New OpenMBeanOperationInfoSupport(name, description, arrayCopyCast(signature), returnOpenType, impact)
			Else
				Return Me
			End If
		End Function

	End Class

End Namespace