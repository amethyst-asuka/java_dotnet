Imports System.Diagnostics
Imports System.Collections.Generic

'
' * Copyright (c) 2007, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.file.attribute


	''' <summary>
	''' An entry in an access control list (ACL).
	''' 
	''' <p> The ACL entry represented by this class is based on the ACL model
	''' specified in <a href="http://www.ietf.org/rfc/rfc3530.txt"><i>RFC&nbsp;3530:
	''' Network File System (NFS) version 4 Protocol</i></a>. Each entry has four
	''' components as follows:
	''' 
	''' <ol>
	'''    <li><p> The <seealso cref="#type() type"/> component determines if the entry
	'''    grants or denies access. </p></li>
	''' 
	'''    <li><p> The <seealso cref="#principal() principal"/> component, sometimes called the
	'''    "who" component, is a <seealso cref="UserPrincipal"/> corresponding to the identity
	'''    that the entry grants or denies access
	'''    </p></li>
	''' 
	'''    <li><p> The <seealso cref="#permissions permissions"/> component is a set of
	'''    <seealso cref="AclEntryPermission permissions"/>
	'''    </p></li>
	''' 
	'''    <li><p> The <seealso cref="#flags flags"/> component is a set of {@link AclEntryFlag
	'''    flags} to indicate how entries are inherited and propagated </p></li>
	''' </ol>
	''' 
	''' <p> ACL entries are created using an associated <seealso cref="Builder"/> object by
	''' invoking its <seealso cref="Builder#build build"/> method.
	''' 
	''' <p> ACL entries are immutable and are safe for use by multiple concurrent
	''' threads.
	''' 
	''' @since 1.7
	''' </summary>

	Public NotInheritable Class AclEntry

		Private ReadOnly type_Renamed As AclEntryType
		Private ReadOnly who As UserPrincipal
		Private ReadOnly perms As [Set](Of AclEntryPermission)
		Private ReadOnly flags_Renamed As [Set](Of AclEntryFlag)

		' cached hash code
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private hash_Renamed As Integer

		' private constructor
		Private Sub New(ByVal type As AclEntryType, ByVal who As UserPrincipal, ByVal perms As [Set](Of AclEntryPermission), ByVal flags As [Set](Of AclEntryFlag))
			Me.type_Renamed = type
			Me.who = who
			Me.perms = perms
			Me.flags_Renamed = flags
		End Sub

		''' <summary>
		''' A builder of <seealso cref="AclEntry"/> objects.
		''' 
		''' <p> A {@code Builder} object is obtained by invoking one of the {@link
		''' AclEntry#newBuilder newBuilder} methods defined by the {@code AclEntry}
		''' class.
		''' 
		''' <p> Builder objects are mutable and are not safe for use by multiple
		''' concurrent threads without appropriate synchronization.
		''' 
		''' @since 1.7
		''' </summary>
		Public NotInheritable Class Builder
			Private type As AclEntryType
			Private who As UserPrincipal
			Private perms As [Set](Of AclEntryPermission)
			Private flags As [Set](Of AclEntryFlag)

			Private Sub New(ByVal type As AclEntryType, ByVal who As UserPrincipal, ByVal perms As [Set](Of AclEntryPermission), ByVal flags As [Set](Of AclEntryFlag))
				Debug.Assert(perms IsNot Nothing AndAlso flags IsNot Nothing)
				Me.type = type
				Me.who = who
				Me.perms = perms
				Me.flags = flags
			End Sub

			''' <summary>
			''' Constructs an <seealso cref="AclEntry"/> from the components of this builder.
			''' The type and who components are required to have been set in order
			''' to construct an {@code AclEntry}.
			''' </summary>
			''' <returns>  a new ACL entry
			''' </returns>
			''' <exception cref="IllegalStateException">
			'''          if the type or who component have not been set </exception>
			Public Function build() As AclEntry
				If type Is Nothing Then Throw New IllegalStateException("Missing type component")
				If who Is Nothing Then Throw New IllegalStateException("Missing who component")
				Return New AclEntry(type, who, perms, flags)
			End Function

			''' <summary>
			''' Sets the type component of this builder.
			''' </summary>
			''' <param name="type">  the component type </param>
			''' <returns>  this builder </returns>
			Public Function setType(ByVal type As AclEntryType) As Builder
				If type Is Nothing Then Throw New NullPointerException
				Me.type = type
				Return Me
			End Function

			''' <summary>
			''' Sets the principal component of this builder.
			''' </summary>
			''' <param name="who">  the principal component </param>
			''' <returns>  this builder </returns>
			Public Function setPrincipal(ByVal who As UserPrincipal) As Builder
				If who Is Nothing Then Throw New NullPointerException
				Me.who = who
				Return Me
			End Function

			' check set only contains elements of the given type
			Private Shared Sub checkSet(Of T1)(ByVal [set] As [Set](Of T1), ByVal type As [Class])
				For Each e As Object In [set]
					If e Is Nothing Then Throw New NullPointerException
					type.cast(e)
				Next e
			End Sub

			''' <summary>
			''' Sets the permissions component of this builder. On return, the
			''' permissions component of this builder is a copy of the given set.
			''' </summary>
			''' <param name="perms">  the permissions component </param>
			''' <returns>  this builder
			''' </returns>
			''' <exception cref="ClassCastException">
			'''          if the set contains elements that are not of type {@code
			'''          AclEntryPermission} </exception>
			Public Function setPermissions(ByVal perms As [Set](Of AclEntryPermission)) As Builder
				If perms.empty Then
					' EnumSet.copyOf does not allow empty set
					perms = Collections.emptySet()
				Else
					' copy and check for erroneous elements
					perms = EnumSet.copyOf(perms)
					checkSet(perms, GetType(AclEntryPermission))
				End If

				Me.perms = perms
				Return Me
			End Function

			''' <summary>
			''' Sets the permissions component of this builder. On return, the
			''' permissions component of this builder is a copy of the permissions in
			''' the given array.
			''' </summary>
			''' <param name="perms">  the permissions component </param>
			''' <returns>  this builder </returns>
			Public Function setPermissions(ParamArray ByVal perms As AclEntryPermission()) As Builder
				Dim [set] As [Set](Of AclEntryPermission) = EnumSet.noneOf(GetType(AclEntryPermission))
				' copy and check for null elements
				For Each p As AclEntryPermission In perms
					If p Is Nothing Then Throw New NullPointerException
					[set].add(p)
				Next p
				Me.perms = [set]
				Return Me
			End Function

			''' <summary>
			''' Sets the flags component of this builder. On return, the flags
			''' component of this builder is a copy of the given set.
			''' </summary>
			''' <param name="flags">  the flags component </param>
			''' <returns>  this builder
			''' </returns>
			''' <exception cref="ClassCastException">
			'''          if the set contains elements that are not of type {@code
			'''          AclEntryFlag} </exception>
			Public Function setFlags(ByVal flags As [Set](Of AclEntryFlag)) As Builder
				If flags.empty Then
					' EnumSet.copyOf does not allow empty set
					flags = Collections.emptySet()
				Else
					' copy and check for erroneous elements
					flags = EnumSet.copyOf(flags)
					checkSet(flags, GetType(AclEntryFlag))
				End If

				Me.flags = flags
				Return Me
			End Function

			''' <summary>
			''' Sets the flags component of this builder. On return, the flags
			''' component of this builder is a copy of the flags in the given
			''' array.
			''' </summary>
			''' <param name="flags">  the flags component </param>
			''' <returns>  this builder </returns>
			Public Function setFlags(ParamArray ByVal flags As AclEntryFlag()) As Builder
				Dim [set] As [Set](Of AclEntryFlag) = EnumSet.noneOf(GetType(AclEntryFlag))
				' copy and check for null elements
				For Each f As AclEntryFlag In flags
					If f Is Nothing Then Throw New NullPointerException
					[set].add(f)
				Next f
				Me.flags = [set]
				Return Me
			End Function
		End Class

		''' <summary>
		''' Constructs a new builder. The initial value of the type and who
		''' components is {@code null}. The initial value of the permissions and
		''' flags components is the empty set.
		''' </summary>
		''' <returns>  a new builder </returns>
		Public Shared Function newBuilder() As Builder
			Dim perms As [Set](Of AclEntryPermission) = Collections.emptySet()
			Dim flags As [Set](Of AclEntryFlag) = Collections.emptySet()
			Return New Builder(Nothing, Nothing, perms, flags)
		End Function

		''' <summary>
		''' Constructs a new builder with the components of an existing ACL entry.
		''' </summary>
		''' <param name="entry">  an ACL entry </param>
		''' <returns>  a new builder </returns>
		Public Shared Function newBuilder(ByVal entry As AclEntry) As Builder
			Return New Builder(entry.type_Renamed, entry.who, entry.perms, entry.flags_Renamed)
		End Function

		''' <summary>
		''' Returns the ACL entry type.
		''' </summary>
		''' <returns> the ACL entry type </returns>
		Public Function type() As AclEntryType
			Return type_Renamed
		End Function

		''' <summary>
		''' Returns the principal component.
		''' </summary>
		''' <returns> the principal component </returns>
		Public Function principal() As UserPrincipal
			Return who
		End Function

		''' <summary>
		''' Returns a copy of the permissions component.
		''' 
		''' <p> The returned set is a modifiable copy of the permissions.
		''' </summary>
		''' <returns> the permissions component </returns>
		Public Function permissions() As [Set](Of AclEntryPermission)
			Return New HashSet(Of AclEntryPermission)(perms)
		End Function

		''' <summary>
		''' Returns a copy of the flags component.
		''' 
		''' <p> The returned set is a modifiable copy of the flags.
		''' </summary>
		''' <returns> the flags component </returns>
		Public Function flags() As [Set](Of AclEntryFlag)
			Return New HashSet(Of AclEntryFlag)(flags_Renamed)
		End Function

		''' <summary>
		''' Compares the specified object with this ACL entry for equality.
		''' 
		''' <p> If the given object is not an {@code AclEntry} then this method
		''' immediately returns {@code false}.
		''' 
		''' <p> For two ACL entries to be considered equals requires that they are
		''' both the same type, their who components are equal, their permissions
		''' components are equal, and their flags components are equal.
		''' 
		''' <p> This method satisfies the general contract of the {@link
		''' java.lang.Object#equals(Object) Object.equals} method. </p>
		''' </summary>
		''' <param name="ob">   the object to which this object is to be compared
		''' </param>
		''' <returns>  {@code true} if, and only if, the given object is an AclEntry that
		'''          is identical to this AclEntry </returns>
		Public Overrides Function Equals(ByVal ob As Object) As Boolean
			If ob Is Me Then Return True
			If ob Is Nothing OrElse Not(TypeOf ob Is AclEntry) Then Return False
			Dim other As AclEntry = CType(ob, AclEntry)
			If Me.type_Renamed <> other.type_Renamed Then Return False
			If Not Me.who.Equals(other.who) Then Return False
			If Not Me.perms.Equals(other.perms) Then Return False
			If Not Me.flags_Renamed.Equals(other.flags_Renamed) Then Return False
			Return True
		End Function

		Private Shared Function hash(ByVal h As Integer, ByVal o As Object) As Integer
			Return h * 127 + o.GetHashCode()
		End Function

		''' <summary>
		''' Returns the hash-code value for this ACL entry.
		''' 
		''' <p> This method satisfies the general contract of the {@link
		''' Object#hashCode} method.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			' return cached hash if available
			If hash_Renamed <> 0 Then Return hash_Renamed
			Dim h As Integer = type_Renamed.GetHashCode()
			h = hash(h, who)
			h = hash(h, perms)
			h = hash(h, flags_Renamed)
			hash_Renamed = h
			Return hash_Renamed
		End Function

		''' <summary>
		''' Returns the string representation of this ACL entry.
		''' </summary>
		''' <returns>  the string representation of this entry </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder

			' who
			sb.append(who.name)
			sb.append(":"c)

			' permissions
			For Each perm As AclEntryPermission In perms
				sb.append(perm.name())
				sb.append("/"c)
			Next perm
			sb.length = sb.length()-1 ' drop final slash
			sb.append(":"c)

			' flags
			If Not flags_Renamed.empty Then
				For Each flag As AclEntryFlag In flags_Renamed
					sb.append(flag.name())
					sb.append("/"c)
				Next flag
				sb.length = sb.length()-1 ' drop final slash
				sb.append(":"c)
			End If

			' type
			sb.append(type_Renamed.name())
			Return sb.ToString()
		End Function
	End Class

End Namespace