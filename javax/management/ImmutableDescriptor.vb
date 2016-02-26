Imports System
Imports System.Collections.Generic
Imports System.Text

'
' * Copyright (c) 2004, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.management


	''' <summary>
	''' An immutable descriptor.
	''' @since 1.6
	''' </summary>
	Public Class ImmutableDescriptor
		Implements Descriptor

		Private Const serialVersionUID As Long = 8853308591080540165L

		''' <summary>
		''' The names of the fields in this ImmutableDescriptor with their
		''' original case.  The names must be in alphabetical order as determined
		''' by <seealso cref="String#CASE_INSENSITIVE_ORDER"/>.
		''' </summary>
		Private ReadOnly names As String()
		''' <summary>
		''' The values of the fields in this ImmutableDescriptor.  The
		''' elements in this array match the corresponding elements in the
		''' {@code names} array.
		''' </summary>
		Private ReadOnly values As Object()

		<NonSerialized> _
		Private ___hashCode As Integer = -1

		''' <summary>
		''' An empty descriptor.
		''' </summary>
		Public Shared ReadOnly EMPTY_DESCRIPTOR As New ImmutableDescriptor

		''' <summary>
		''' Construct a descriptor containing the given fields and values.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if either array is null, or
		''' if the arrays have different sizes, or
		''' if a field name is null or empty, or if the same field name
		''' appears more than once. </exception>
		Public Sub New(ByVal fieldNames As String(), ByVal fieldValues As Object())
			Me.New(makeMap(fieldNames, fieldValues))
		End Sub

		''' <summary>
		''' Construct a descriptor containing the given fields.  Each String
		''' must be of the form {@code fieldName=fieldValue}.  The field name
		''' ends at the first {@code =} character; for example if the String
		''' is {@code a=b=c} then the field name is {@code a} and its value
		''' is {@code b=c}.
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if the parameter is null, or
		''' if a field name is empty, or if the same field name appears
		''' more than once, or if one of the strings does not contain
		''' an {@code =} character. </exception>
		Public Sub New(ParamArray ByVal fields As String())
			Me.New(makeMap(fields))
		End Sub

		''' <summary>
		''' <p>Construct a descriptor where the names and values of the fields
		''' are the keys and values of the given Map.</p>
		''' </summary>
		''' <exception cref="IllegalArgumentException"> if the parameter is null, or
		''' if a field name is null or empty, or if the same field name appears
		''' more than once (which can happen because field names are not case
		''' sensitive). </exception>
		Public Sub New(Of T1)(ByVal fields As IDictionary(Of T1))
			If fields Is Nothing Then Throw New System.ArgumentException("Null Map")
			Dim map As java.util.SortedMap(Of String, Object) = New SortedDictionary(Of String, Object)(String.CASE_INSENSITIVE_ORDER)
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
			For Each entry As KeyValuePair(Of String, ?) In fields
				Dim name As String = entry.Key
				If name Is Nothing OrElse name.Equals("") Then Throw New System.ArgumentException("Empty or null field name")
				If map.containsKey(name) Then Throw New System.ArgumentException("Duplicate name: " & name)
				map.put(name, entry.Value)
			Next entry
			Dim size As Integer = map.size()
			Me.names = map.Keys.ToArray(New String(size - 1){})
			Me.values = map.values().ToArray(New Object(size - 1){})
		End Sub

		''' <summary>
		''' This method can replace a deserialized instance of this
		''' class with another instance.  For example, it might replace
		''' a deserialized empty ImmutableDescriptor with
		''' <seealso cref="#EMPTY_DESCRIPTOR"/>.
		''' </summary>
		''' <returns> the replacement object, which may be {@code this}.
		''' </returns>
		''' <exception cref="InvalidObjectException"> if the read object has invalid fields. </exception>
		Private Function readResolve() As Object

			Dim bad As Boolean = False
			If names Is Nothing OrElse values Is Nothing OrElse names.Length <> values.Length Then bad = True
			If Not bad Then
				If names.Length = 0 AndAlso Me.GetType() = GetType(ImmutableDescriptor) Then Return EMPTY_DESCRIPTOR
				Dim compare As IComparer(Of String) = String.CASE_INSENSITIVE_ORDER
				Dim lastName As String = "" ' also catches illegal null name
				For i As Integer = 0 To names.Length - 1
					If names(i) Is Nothing OrElse compare.Compare(lastName, names(i)) >= 0 Then
						bad = True
						Exit For
					End If
					lastName = names(i)
				Next i
			End If
			If bad Then Throw New java.io.InvalidObjectException("Bad names or values")

			Return Me
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function makeMap(ByVal fieldNames As String(), ByVal fieldValues As Object()) As java.util.SortedMap(Of String, ?)
			If fieldNames Is Nothing OrElse fieldValues Is Nothing Then Throw New System.ArgumentException("Null array parameter")
			If fieldNames.Length <> fieldValues.Length Then Throw New System.ArgumentException("Different size arrays")
			Dim map As java.util.SortedMap(Of String, Object) = New SortedDictionary(Of String, Object)(String.CASE_INSENSITIVE_ORDER)
			For i As Integer = 0 To fieldNames.Length - 1
				Dim name As String = fieldNames(i)
				If name Is Nothing OrElse name.Equals("") Then Throw New System.ArgumentException("Empty or null field name")
				Dim old As Object = map.put(name, fieldValues(i))
				If old IsNot Nothing Then Throw New System.ArgumentException("Duplicate field name: " & name)
			Next i
			Return map
		End Function

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private Shared Function makeMap(ByVal fields As String()) As java.util.SortedMap(Of String, ?)
			If fields Is Nothing Then Throw New System.ArgumentException("Null fields parameter")
			Dim ___fieldNames As String() = New String(fields.Length - 1){}
			Dim ___fieldValues As String() = New String(fields.Length - 1){}
			For i As Integer = 0 To fields.Length - 1
				Dim ___field As String = fields(i)
				Dim eq As Integer = ___field.IndexOf("="c)
				If eq < 0 Then Throw New System.ArgumentException("Missing = character: " & ___field)
				___fieldNames(i) = ___field.Substring(0, eq)
				' makeMap will catch the case where the name is empty
				___fieldValues(i) = ___field.Substring(eq + 1)
			Next i
			Return makeMap(___fieldNames, ___fieldValues)
		End Function

		''' <summary>
		''' <p>Return an {@code ImmutableDescriptor} whose contents are the union of
		''' the given descriptors.  Every field name that appears in any of
		''' the descriptors will appear in the result with the
		''' value that it has when the method is called.  Subsequent changes
		''' to any of the descriptors do not affect the ImmutableDescriptor
		''' returned here.</p>
		''' 
		''' <p>In the simplest case, there is only one descriptor and the
		''' returned {@code ImmutableDescriptor} is a copy of its fields at the
		''' time this method is called:</p>
		''' 
		''' <pre>
		''' Descriptor d = something();
		''' ImmutableDescriptor copy = ImmutableDescriptor.union(d);
		''' </pre>
		''' </summary>
		''' <param name="descriptors"> the descriptors to be combined.  Any of the
		''' descriptors can be null, in which case it is skipped.
		''' </param>
		''' <returns> an {@code ImmutableDescriptor} that is the union of the given
		''' descriptors.  The returned object may be identical to one of the
		''' input descriptors if it is an ImmutableDescriptor that contains all of
		''' the required fields.
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if two Descriptors contain the
		''' same field name with different associated values.  Primitive array
		''' values are considered the same if they are of the same type with
		''' the same elements.  Object array values are considered the same if
		''' <seealso cref="Arrays#deepEquals(Object[],Object[])"/> returns true. </exception>
		Public Shared Function union(ParamArray ByVal descriptors As Descriptor()) As ImmutableDescriptor
			' Optimize the case where exactly one Descriptor is non-Empty
			' and it is immutable - we can just return it.
			Dim index As Integer = findNonEmpty(descriptors, 0)
			If index < 0 Then Return EMPTY_DESCRIPTOR
			If TypeOf descriptors(index) Is ImmutableDescriptor AndAlso findNonEmpty(descriptors, index + 1) < 0 Then Return CType(descriptors(index), ImmutableDescriptor)

			Dim map As IDictionary(Of String, Object) = New SortedDictionary(Of String, Object)(String.CASE_INSENSITIVE_ORDER)
			Dim biggestImmutable As ImmutableDescriptor = EMPTY_DESCRIPTOR
			For Each d As Descriptor In descriptors
				If d IsNot Nothing Then
					Dim names As String()
					If TypeOf d Is ImmutableDescriptor Then
						Dim id As ImmutableDescriptor = CType(d, ImmutableDescriptor)
						names = id.names
						If id.GetType() Is GetType(ImmutableDescriptor) AndAlso names.Length > biggestImmutable.names.Length Then biggestImmutable = id
					Else
						names = d.fieldNames
					End If
					For Each n As String In names
						Dim v As Object = d.getFieldValue(n)
							map(n) = v
							Dim old As Object = map(n)
						If old IsNot Nothing Then
							Dim equal As Boolean
							If old.GetType().IsArray Then
								equal = java.util.Arrays.deepEquals(New Object() {old}, New Object() {v})
							Else
								equal = old.Equals(v)
							End If
							If Not equal Then
								Dim msg As String = "Inconsistent values for descriptor field " & n & ": " & old & " :: " & v
								Throw New System.ArgumentException(msg)
							End If
						End If
					Next n
				End If
			Next d
			If biggestImmutable.names.Length = map.Count Then Return biggestImmutable
			Return New ImmutableDescriptor(map)
		End Function

		Private Shared Function isEmpty(ByVal d As Descriptor) As Boolean
			If d Is Nothing Then
				Return True
			ElseIf TypeOf d Is ImmutableDescriptor Then
				Return CType(d, ImmutableDescriptor).names.Length = 0
			Else
				Return (d.fieldNames.Length = 0)
			End If
		End Function

		Private Shared Function findNonEmpty(ByVal ds As Descriptor(), ByVal start As Integer) As Integer
			For i As Integer = start To ds.Length - 1
				If Not isEmpty(ds(i)) Then Return i
			Next i
			Return -1
		End Function

		Private Function fieldIndex(ByVal name As String) As Integer
			Return java.util.Arrays.binarySearch(names, name, String.CASE_INSENSITIVE_ORDER)
		End Function

		Public Function getFieldValue(ByVal fieldName As String) As Object Implements Descriptor.getFieldValue
			checkIllegalFieldName(fieldName)
			Dim i As Integer = fieldIndex(fieldName)
			If i < 0 Then Return Nothing
			Dim v As Object = values(i)
			If v Is Nothing OrElse (Not v.GetType().IsArray) Then Return v
			If TypeOf v Is Object() Then Return CType(v, Object()).clone()
			' clone the primitive array, could use an 8-way if/else here
			Dim len As Integer = Array.getLength(v)
			Dim a As Object = Array.newInstance(v.GetType().GetElementType(), len)
			Array.Copy(v, 0, a, 0, len)
			Return a
		End Function

		Public Property fields As String() Implements Descriptor.getFields
			Get
				Dim result As String() = New String(names.Length - 1){}
				For i As Integer = 0 To result.Length - 1
					Dim value As Object = values(i)
					If value Is Nothing Then
						value = ""
					ElseIf Not(TypeOf value Is String) Then
						value = "(" & value & ")"
					End If
					result(i) = names(i) & "=" & value
				Next i
				Return result
			End Get
		End Property

		Public Function getFieldValues(ParamArray ByVal fieldNames As String()) As Object()
			If fieldNames Is Nothing Then Return values.clone()
			Dim result As Object() = New Object(fieldNames.Length - 1){}
			For i As Integer = 0 To fieldNames.Length - 1
				Dim name As String = fieldNames(i)
				If name IsNot Nothing AndAlso (Not name.Equals("")) Then result(i) = getFieldValue(name)
			Next i
			Return result
		End Function

		Public Property fieldNames As String() Implements Descriptor.getFieldNames
			Get
				Return names.clone()
			End Get
		End Property

		''' <summary>
		''' Compares this descriptor to the given object.  The objects are equal if
		''' the given object is also a Descriptor, and if the two Descriptors have
		''' the same field names (possibly differing in case) and the same
		''' associated values.  The respective values for a field in the two
		''' Descriptors are equal if the following conditions hold:
		''' 
		''' <ul>
		''' <li>If one value is null then the other must be too.</li>
		''' <li>If one value is a primitive array then the other must be a primitive
		''' array of the same type with the same elements.</li>
		''' <li>If one value is an object array then the other must be too and
		''' <seealso cref="Arrays#deepEquals(Object[],Object[])"/> must return true.</li>
		''' <li>Otherwise <seealso cref="Object#equals(Object)"/> must return true.</li>
		''' </ul>
		''' </summary>
		''' <param name="o"> the object to compare with.
		''' </param>
		''' <returns> {@code true} if the objects are the same; {@code false}
		''' otherwise.
		'''  </returns>
		' Note: this Javadoc is copied from javax.management.Descriptor
		'       due to 6369229.
		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If o Is Me Then Return True
			If Not(TypeOf o Is Descriptor) Then Return False
			Dim onames As String()
			If TypeOf o Is ImmutableDescriptor Then
				onames = CType(o, ImmutableDescriptor).names
			Else
				onames = CType(o, Descriptor).fieldNames
				java.util.Arrays.sort(onames, String.CASE_INSENSITIVE_ORDER)
			End If
			If names.Length <> onames.Length Then Return False
			For i As Integer = 0 To names.Length - 1
				If (Not names(i).ToUpper()) = onames(i).ToUpper() Then Return False
			Next i
			Dim ovalues As Object()
			If TypeOf o Is ImmutableDescriptor Then
				ovalues = CType(o, ImmutableDescriptor).values
			Else
				ovalues = CType(o, Descriptor).getFieldValues(onames)
			End If
			Return java.util.Arrays.deepEquals(values, ovalues)
		End Function

		''' <summary>
		''' <p>Returns the hash code value for this descriptor.  The hash
		''' code is computed as the sum of the hash codes for each field in
		''' the descriptor.  The hash code of a field with name {@code n}
		''' and value {@code v} is {@code n.toLowerCase().hashCode() ^ h}.
		''' Here {@code h} is the hash code of {@code v}, computed as
		''' follows:</p>
		''' 
		''' <ul>
		''' <li>If {@code v} is null then {@code h} is 0.</li>
		''' <li>If {@code v} is a primitive array then {@code h} is computed using
		''' the appropriate overloading of {@code java.util.Arrays.hashCode}.</li>
		''' <li>If {@code v} is an object array then {@code h} is computed using
		''' <seealso cref="Arrays#deepHashCode(Object[])"/>.</li>
		''' <li>Otherwise {@code h} is {@code v.hashCode()}.</li>
		''' </ul>
		''' </summary>
		''' <returns> A hash code value for this object.
		'''  </returns>
		' Note: this Javadoc is copied from javax.management.Descriptor
		'       due to 6369229.
		Public Overrides Function GetHashCode() As Integer
			If ___hashCode = -1 Then ___hashCode = com.sun.jmx.mbeanserver.Util.hashCode(names, values)
			Return ___hashCode
		End Function

		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder("{")
			For i As Integer = 0 To names.Length - 1
				If i > 0 Then sb.Append(", ")
				sb.Append(names(i)).append("=")
				Dim v As Object = values(i)
				If v IsNot Nothing AndAlso v.GetType().IsArray Then
					Dim s As String = java.util.Arrays.deepToString(New Object() {v})
					s = s.Substring(1, s.Length - 1 - 1) ' remove [...]
					v = s
				End If
				sb.Append(Convert.ToString(v))
			Next i
			Return sb.Append("}").ToString()
		End Function

		''' <summary>
		''' Returns true if all of the fields have legal values given their
		''' names.  This method always returns true, but a subclass can
		''' override it to return false when appropriate.
		''' </summary>
		''' <returns> true if the values are legal.
		''' </returns>
		''' <exception cref="RuntimeOperationsException"> if the validity checking fails.
		''' The method returns false if the descriptor is not valid, but throws
		''' this exception if the attempt to determine validity fails. </exception>
		Public Overridable Property valid As Boolean Implements Descriptor.isValid
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' <p>Returns a descriptor which is equal to this descriptor.
		''' Changes to the returned descriptor will have no effect on this
		''' descriptor, and vice versa.</p>
		''' 
		''' <p>This method returns the object on which it is called.
		''' A subclass can override it
		''' to return another object provided the contract is respected.
		''' </summary>
		''' <exception cref="RuntimeOperationsException"> for illegal value for field Names
		''' or field Values.
		''' If the descriptor construction fails for any reason, this exception will
		''' be thrown. </exception>
		Public Overrides Function clone() As Descriptor
			Return Me
		End Function

		''' <summary>
		''' This operation is unsupported since this class is immutable.  If
		''' this call would change a mutable descriptor with the same contents,
		''' then a <seealso cref="RuntimeOperationsException"/> wrapping an
		''' <seealso cref="UnsupportedOperationException"/> is thrown.  Otherwise,
		''' the behavior is the same as it would be for a mutable descriptor:
		''' either an exception is thrown because of illegal parameters, or
		''' there is no effect.
		''' </summary>
		Public Sub setFields(ByVal fieldNames As String(), ByVal fieldValues As Object()) Implements Descriptor.setFields
			If fieldNames Is Nothing OrElse fieldValues Is Nothing Then illegal("Null argument")
			If fieldNames.Length <> fieldValues.Length Then illegal("Different array sizes")
			For i As Integer = 0 To fieldNames.Length - 1
				checkIllegalFieldName(fieldNames(i))
			Next i
			For i As Integer = 0 To fieldNames.Length - 1
				fieldeld(fieldNames(i), fieldValues(i))
			Next i
		End Sub

		''' <summary>
		''' This operation is unsupported since this class is immutable.  If
		''' this call would change a mutable descriptor with the same contents,
		''' then a <seealso cref="RuntimeOperationsException"/> wrapping an
		''' <seealso cref="UnsupportedOperationException"/> is thrown.  Otherwise,
		''' the behavior is the same as it would be for a mutable descriptor:
		''' either an exception is thrown because of illegal parameters, or
		''' there is no effect.
		''' </summary>
		Public Sub setField(ByVal fieldName As String, ByVal fieldValue As Object) Implements Descriptor.setField
			checkIllegalFieldName(fieldName)
			Dim i As Integer = fieldIndex(fieldName)
			If i < 0 Then unsupported()
			Dim value As Object = values(i)
			If If(value Is Nothing, (fieldValue IsNot Nothing), (Not value.Equals(fieldValue))) Then unsupported()
		End Sub

		''' <summary>
		''' Removes a field from the descriptor.
		''' </summary>
		''' <param name="fieldName"> String name of the field to be removed.
		''' If the field name is illegal or the field is not found,
		''' no exception is thrown.
		''' </param>
		''' <exception cref="RuntimeOperationsException"> if a field of the given name
		''' exists and the descriptor is immutable.  The wrapped exception will
		''' be an <seealso cref="UnsupportedOperationException"/>. </exception>
		Public Sub removeField(ByVal fieldName As String) Implements Descriptor.removeField
			If fieldName IsNot Nothing AndAlso fieldIndex(fieldName) >= 0 Then unsupported()
		End Sub

		Friend Shared Function nonNullDescriptor(ByVal d As Descriptor) As Descriptor
			If d Is Nothing Then
				Return EMPTY_DESCRIPTOR
			Else
				Return d
			End If
		End Function

		Private Shared Sub checkIllegalFieldName(ByVal name As String)
			If name Is Nothing OrElse name.Equals("") Then illegal("Null or empty field name")
		End Sub

		Private Shared Sub unsupported()
			Dim uoe As New System.NotSupportedException("Descriptor is read-only")
			Throw New RuntimeOperationsException(uoe)
		End Sub

		Private Shared Sub illegal(ByVal message As String)
			Dim iae As New System.ArgumentException(message)
			Throw New RuntimeOperationsException(iae)
		End Sub
	End Class

End Namespace