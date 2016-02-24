'
' * Copyright (c) 1997, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.datatransfer



	''' <summary>
	''' A Multipurpose Internet Mail Extension (MIME) type, as defined
	''' in RFC 2045 and 2046.
	''' 
	''' THIS IS *NOT* - REPEAT *NOT* - A PUBLIC CLASS! DataFlavor IS
	''' THE PUBLIC INTERFACE, AND THIS IS PROVIDED AS A ***PRIVATE***
	''' (THAT IS AS IN *NOT* PUBLIC) HELPER CLASS!
	''' </summary>
	Friend Class MimeType
		Implements java.io.Externalizable, Cloneable

	'    
	'     * serialization support
	'     

		Friend Const serialVersionUID As Long = -6568722458793895906L

		''' <summary>
		''' Constructor for externalization; this constructor should not be
		''' called directly by an application, since the result will be an
		''' uninitialized, immutable <code>MimeType</code> object.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Builds a <code>MimeType</code> from a <code>String</code>.
		''' </summary>
		''' <param name="rawdata"> text used to initialize the <code>MimeType</code> </param>
		''' <exception cref="NullPointerException"> if <code>rawdata</code> is null </exception>
		Public Sub New(ByVal rawdata As String)
			parse(rawdata)
		End Sub

		''' <summary>
		''' Builds a <code>MimeType</code> with the given primary and sub
		''' type but has an empty parameter list.
		''' </summary>
		''' <param name="primary"> the primary type of this <code>MimeType</code> </param>
		''' <param name="sub"> the subtype of this <code>MimeType</code> </param>
		''' <exception cref="NullPointerException"> if either <code>primary</code> or
		'''         <code>sub</code> is null </exception>
		Public Sub New(ByVal primary As String, ByVal [sub] As String)
			Me.New(primary, [sub], New MimeTypeParameterList)
		End Sub

		''' <summary>
		''' Builds a <code>MimeType</code> with a pre-defined
		''' and valid (or empty) parameter list.
		''' </summary>
		''' <param name="primary"> the primary type of this <code>MimeType</code> </param>
		''' <param name="sub"> the subtype of this <code>MimeType</code> </param>
		''' <param name="mtpl"> the requested parameter list </param>
		''' <exception cref="NullPointerException"> if either <code>primary</code>,
		'''         <code>sub</code> or <code>mtpl</code> is null </exception>
		Public Sub New(ByVal primary As String, ByVal [sub] As String, ByVal mtpl As MimeTypeParameterList)
			'    check to see if primary is valid
			If isValidToken(primary) Then
				primaryType = primary.ToLower(java.util.Locale.ENGLISH)
			Else
				Throw New MimeTypeParseException("Primary type is invalid.")
			End If

			'    check to see if sub is valid
			If isValidToken([sub]) Then
				subType = [sub].ToLower(java.util.Locale.ENGLISH)
			Else
				Throw New MimeTypeParseException("Sub type is invalid.")
			End If

			parameters = CType(mtpl.clone(), MimeTypeParameterList)
		End Sub

		Public Overrides Function GetHashCode() As Integer

			' We sum up the hash codes for all of the strings. This
			' way, the order of the strings is irrelevant
			Dim code As Integer = 0
			code += primaryType.GetHashCode()
			code += subType.GetHashCode()
			code += parameters.GetHashCode()
			Return code
		End Function ' hashCode()

		''' <summary>
		''' <code>MimeType</code>s are equal if their primary types,
		''' subtypes, and  parameters are all equal. No default values
		''' are taken into account. </summary>
		''' <param name="thatObject"> the object to be evaluated as a
		'''    <code>MimeType</code> </param>
		''' <returns> <code>true</code> if <code>thatObject</code> is
		'''    a <code>MimeType</code>; otherwise returns <code>false</code> </returns>
		Public Overrides Function Equals(ByVal thatObject As Object) As Boolean
			If Not(TypeOf thatObject Is MimeType) Then Return False
			Dim that As MimeType = CType(thatObject, MimeType)
			Dim isIt As Boolean = ((Me.primaryType.Equals(that.primaryType)) AndAlso (Me.subType.Equals(that.subType)) AndAlso (Me.parameters.Equals(that.parameters)))
			Return isIt
		End Function ' equals()

		''' <summary>
		''' A routine for parsing the MIME type out of a String.
		''' </summary>
		''' <exception cref="NullPointerException"> if <code>rawdata</code> is null </exception>
		Private Sub parse(ByVal rawdata As String)
			Dim slashIndex As Integer = rawdata.IndexOf("/"c)
			Dim semIndex As Integer = rawdata.IndexOf(";"c)
			If (slashIndex < 0) AndAlso (semIndex < 0) Then
				'    neither character is present, so treat it
				'    as an error
				Throw New MimeTypeParseException("Unable to find a sub type.")
			ElseIf (slashIndex < 0) AndAlso (semIndex >= 0) Then
				'    we have a ';' (and therefore a parameter list),
				'    but no '/' indicating a sub type is present
				Throw New MimeTypeParseException("Unable to find a sub type.")
			ElseIf (slashIndex >= 0) AndAlso (semIndex < 0) Then
				'    we have a primary and sub type but no parameter list
				primaryType = rawdata.Substring(0,slashIndex).Trim().ToLower(java.util.Locale.ENGLISH)
				subType = rawdata.Substring(slashIndex + 1).Trim().ToLower(java.util.Locale.ENGLISH)
				parameters = New MimeTypeParameterList
			ElseIf slashIndex < semIndex Then
				'    we have all three items in the proper sequence
				primaryType = rawdata.Substring(0, slashIndex).Trim().ToLower(java.util.Locale.ENGLISH)
				subType = rawdata.Substring(slashIndex + 1, semIndex - (slashIndex + 1)).Trim().ToLower(java.util.Locale.ENGLISH)
				parameters = New MimeTypeParameterList(rawdata.Substring(semIndex))
			Else
				'    we have a ';' lexically before a '/' which means we have a primary type
				'    & a parameter list but no sub type
				Throw New MimeTypeParseException("Unable to find a sub type.")
			End If

			'    now validate the primary and sub types

			'    check to see if primary is valid
			If Not isValidToken(primaryType) Then Throw New MimeTypeParseException("Primary type is invalid.")

			'    check to see if sub is valid
			If Not isValidToken(subType) Then Throw New MimeTypeParseException("Sub type is invalid.")
		End Sub

		''' <summary>
		''' Retrieve the primary type of this object.
		''' </summary>
		Public Overridable Property primaryType As String
			Get
				Return primaryType
			End Get
		End Property

		''' <summary>
		''' Retrieve the sub type of this object.
		''' </summary>
		Public Overridable Property subType As String
			Get
				Return subType
			End Get
		End Property

		''' <summary>
		''' Retrieve a copy of this object's parameter list.
		''' </summary>
		Public Overridable Property parameters As MimeTypeParameterList
			Get
				Return CType(parameters.clone(), MimeTypeParameterList)
			End Get
		End Property

		''' <summary>
		''' Retrieve the value associated with the given name, or null if there
		''' is no current association.
		''' </summary>
		Public Overridable Function getParameter(ByVal name As String) As String
			Return parameters.get(name)
		End Function

		''' <summary>
		''' Set the value to be associated with the given name, replacing
		''' any previous association.
		''' 
		''' @throw IllegalArgumentException if parameter or value is illegal
		''' </summary>
		Public Overridable Sub setParameter(ByVal name As String, ByVal value As String)
			parameters.set(name, value)
		End Sub

		''' <summary>
		''' Remove any value associated with the given name.
		''' 
		''' @throw IllegalArgumentExcpetion if parameter may not be deleted
		''' </summary>
		Public Overridable Sub removeParameter(ByVal name As String)
			parameters.remove(name)
		End Sub

		''' <summary>
		''' Return the String representation of this object.
		''' </summary>
		Public Overrides Function ToString() As String
			Return baseType + parameters.ToString()
		End Function

		''' <summary>
		''' Return a String representation of this object
		''' without the parameter list.
		''' </summary>
		Public Overridable Property baseType As String
			Get
				Return primaryType & "/" & subType
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the primary type and the
		''' subtype of this object are the same as the specified
		''' <code>type</code>; otherwise returns <code>false</code>.
		''' </summary>
		''' <param name="type"> the type to compare to <code>this</code>'s type </param>
		''' <returns> <code>true</code> if the primary type and the
		'''    subtype of this object are the same as the
		'''    specified <code>type</code>; otherwise returns
		'''    <code>false</code> </returns>
		Public Overridable Function match(ByVal type As MimeType) As Boolean
			If type Is Nothing Then Return False
			Return primaryType.Equals(type.primaryType) AndAlso (subType.Equals("*") OrElse type.subType.Equals("*") OrElse (subType.Equals(type.subType)))
		End Function

		''' <summary>
		''' Returns <code>true</code> if the primary type and the
		''' subtype of this object are the same as the content type
		''' described in <code>rawdata</code>; otherwise returns
		''' <code>false</code>.
		''' </summary>
		''' <param name="rawdata"> the raw data to be examined </param>
		''' <returns> <code>true</code> if the primary type and the
		'''    subtype of this object are the same as the content type
		'''    described in <code>rawdata</code>; otherwise returns
		'''    <code>false</code>; if <code>rawdata</code> is
		'''    <code>null</code>, returns <code>false</code> </returns>
		Public Overridable Function match(ByVal rawdata As String) As Boolean
			If rawdata Is Nothing Then Return False
			Return match(New MimeType(rawdata))
		End Function

		''' <summary>
		''' The object implements the writeExternal method to save its contents
		''' by calling the methods of DataOutput for its primitive values or
		''' calling the writeObject method of ObjectOutput for objects, strings
		''' and arrays. </summary>
		''' <exception cref="IOException"> Includes any I/O exceptions that may occur </exception>
		Public Overridable Sub writeExternal(ByVal out As java.io.ObjectOutput)
			Dim s As String = ToString() ' contains ASCII chars only
			' one-to-one correspondence between ASCII char and byte in UTF string
			If s.length() <= 65535 Then ' 65535 is max length of UTF string
				out.writeUTF(s)
			Else
				out.writeByte(0)
				out.writeByte(0)
				out.writeInt(s.length())
				out.write(s.bytes)
			End If
		End Sub

		''' <summary>
		''' The object implements the readExternal method to restore its
		''' contents by calling the methods of DataInput for primitive
		''' types and readObject for objects, strings and arrays.  The
		''' readExternal method must read the values in the same sequence
		''' and with the same types as were written by writeExternal. </summary>
		''' <exception cref="ClassNotFoundException"> If the class for an object being
		'''              restored cannot be found. </exception>
		Public Overridable Sub readExternal(ByVal [in] As java.io.ObjectInput)
			Dim s As String = [in].readUTF()
			If s Is Nothing OrElse s.length() = 0 Then ' long mime type
				Dim ba As SByte() = New SByte([in].readInt() - 1){}
				[in].readFully(ba)
				s = New String(ba)
			End If
			Try
				parse(s)
			Catch e As MimeTypeParseException
				Throw New java.io.IOException(e.ToString())
			End Try
		End Sub

		''' <summary>
		''' Returns a clone of this object. </summary>
		''' <returns> a clone of this object </returns>

		Public Overridable Function clone() As Object
			Dim newObj As MimeType = Nothing
			Try
				newObj = CType(MyBase.clone(), MimeType)
			Catch cannotHappen As CloneNotSupportedException
			End Try
			newObj.parameters = CType(parameters.clone(), MimeTypeParameterList)
			Return newObj
		End Function

		Private primaryType As String
		Private subType As String
		Private parameters As MimeTypeParameterList

		'    below here be scary parsing related things

		''' <summary>
		''' Determines whether or not a given character belongs to a legal token.
		''' </summary>
		Private Shared Function isTokenChar(ByVal c As Char) As Boolean
			Return ((AscW(c) > &O40) AndAlso (AscW(c) < &O177)) AndAlso (TSPECIALS.IndexOf(c) < 0)
		End Function

		''' <summary>
		''' Determines whether or not a given string is a legal token.
		''' </summary>
		''' <exception cref="NullPointerException"> if <code>s</code> is null </exception>
		Private Function isValidToken(ByVal s As String) As Boolean
			Dim len As Integer = s.length()
			If len > 0 Then
				For i As Integer = 0 To len - 1
					Dim c As Char = s.Chars(i)
					If Not isTokenChar(c) Then Return False
				Next i
				Return True
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' A string that holds all the special chars.
		''' </summary>

		Private Shared ReadOnly TSPECIALS As String = "()<>@,;:\""/[]?="

	End Class ' class MimeType

End Namespace