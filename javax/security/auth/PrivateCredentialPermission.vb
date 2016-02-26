Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.security.auth


	''' <summary>
	''' This class is used to protect access to private Credentials
	''' belonging to a particular {@code Subject}.  The {@code Subject}
	''' is represented by a Set of Principals.
	''' 
	''' <p> The target name of this {@code Permission} specifies
	''' a Credential class name, and a Set of Principals.
	''' The only valid value for this Permission's actions is, "read".
	''' The target name must abide by the following syntax:
	''' 
	''' <pre>
	'''      CredentialClass {PrincipalClass "PrincipalName"}*
	''' </pre>
	''' 
	''' For example, the following permission grants access to the
	''' com.sun.PrivateCredential owned by Subjects which have
	''' a com.sun.Principal with the name, "duke".  Note that although
	''' this example, as well as all the examples below, do not contain
	''' Codebase, SignedBy, or Principal information in the grant statement
	''' (for simplicity reasons), actual policy configurations should
	''' specify that information when appropriate.
	''' 
	''' <pre>
	''' 
	'''    grant {
	'''      permission javax.security.auth.PrivateCredentialPermission
	'''              "com.sun.PrivateCredential com.sun.Principal \"duke\"",
	'''              "read";
	'''    };
	''' </pre>
	''' 
	''' If CredentialClass is "*", then access is granted to
	''' all private Credentials belonging to the specified
	''' {@code Subject}.
	''' If "PrincipalName" is "*", then access is granted to the
	''' specified Credential owned by any {@code Subject} that has the
	''' specified {@code Principal} (the actual PrincipalName doesn't matter).
	''' For example, the following grants access to the
	''' a.b.Credential owned by any {@code Subject} that has
	''' an a.b.Principal.
	''' 
	''' <pre>
	'''    grant {
	'''      permission javax.security.auth.PrivateCredentialPermission
	'''              "a.b.Credential a.b.Principal "*"",
	'''              "read";
	'''    };
	''' </pre>
	''' 
	''' If both the PrincipalClass and "PrincipalName" are "*",
	''' then access is granted to the specified Credential owned by
	''' any {@code Subject}.
	''' 
	''' <p> In addition, the PrincipalClass/PrincipalName pairing may be repeated:
	''' 
	''' <pre>
	'''    grant {
	'''      permission javax.security.auth.PrivateCredentialPermission
	'''              "a.b.Credential a.b.Principal "duke" c.d.Principal "dukette"",
	'''              "read";
	'''    };
	''' </pre>
	''' 
	''' The above grants access to the private Credential, "a.b.Credential",
	''' belonging to a {@code Subject} with at least two associated Principals:
	''' "a.b.Principal" with the name, "duke", and "c.d.Principal", with the name,
	''' "dukette".
	''' 
	''' </summary>
	Public NotInheritable Class PrivateCredentialPermission
		Inherits java.security.Permission

		Private Const serialVersionUID As Long = 5284372143517237068L

		Private Shared ReadOnly EMPTY_PRINCIPALS As CredOwner() = New CredOwner(){}

		''' <summary>
		''' @serial
		''' </summary>
		Private credentialClass As String

		''' <summary>
		''' @serial The Principals associated with this permission.
		'''          The set contains elements of type,
		'''          {@code PrivateCredentialPermission.CredOwner}.
		''' </summary>
		Private principals As [Set](Of java.security.Principal) ' ignored - kept around for compatibility
		<NonSerialized> _
		Private credOwners As CredOwner()

		''' <summary>
		''' @serial
		''' </summary>
		Private testing As Boolean = False

		''' <summary>
		''' Create a new {@code PrivateCredentialPermission}
		''' with the specified {@code credentialClass} and Principals.
		''' </summary>
		Friend Sub New(ByVal credentialClass As String, ByVal principals As [Set](Of java.security.Principal))

			MyBase.New(credentialClass)
			Me.credentialClass = credentialClass

			SyncLock principals
				If principals.size() = 0 Then
					Me.credOwners = EMPTY_PRINCIPALS
				Else
					Me.credOwners = New CredOwner(principals.size() - 1){}
					Dim index As Integer = 0
					Dim i As IEnumerator(Of java.security.Principal) = principals.GetEnumerator()
					Do While i.MoveNext()
						Dim p As java.security.Principal = i.Current
						Me.credOwners(index) = New CredOwner(p.GetType().name, p.name)
						index += 1
					Loop
				End If
			End SyncLock
		End Sub

		''' <summary>
		''' Creates a new {@code PrivateCredentialPermission}
		''' with the specified {@code name}.  The {@code name}
		''' specifies both a Credential class and a {@code Principal} Set.
		''' 
		''' <p>
		''' </summary>
		''' <param name="name"> the name specifying the Credential class and
		'''          {@code Principal} Set. <p>
		''' </param>
		''' <param name="actions"> the actions specifying that the Credential can be read.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code name} does not conform
		'''          to the correct syntax or if {@code actions} is not "read". </exception>
		Public Sub New(ByVal name As String, ByVal actions As String)
			MyBase.New(name)

			If Not "read".equalsIgnoreCase(actions) Then Throw New System.ArgumentException(sun.security.util.ResourcesMgr.getString("actions.can.only.be.read."))
			init(name)
		End Sub

		''' <summary>
		''' Returns the Class name of the Credential associated with this
		''' {@code PrivateCredentialPermission}.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the Class name of the Credential associated with this
		'''          {@code PrivateCredentialPermission}. </returns>
		Public Property credentialClass As String
			Get
				Return credentialClass
			End Get
		End Property

		''' <summary>
		''' Returns the {@code Principal} classes and names
		''' associated with this {@code PrivateCredentialPermission}.
		''' The information is returned as a two-dimensional array (array[x][y]).
		''' The 'x' value corresponds to the number of {@code Principal}
		''' class and name pairs.  When (y==0), it corresponds to
		''' the {@code Principal} class value, and when (y==1),
		''' it corresponds to the {@code Principal} name value.
		''' For example, array[0][0] corresponds to the class name of
		''' the first {@code Principal} in the array.  array[0][1]
		''' corresponds to the {@code Principal} name of the
		''' first {@code Principal} in the array.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the {@code Principal} class and names associated
		'''          with this {@code PrivateCredentialPermission}. </returns>
		Public Property principals As String()()
			Get
    
				If credOwners Is Nothing OrElse credOwners.Length = 0 Then Return New String(){}
    
				Dim pArray As String()() = RectangularArrays.ReturnRectangularStringArray(credOwners.Length, 2)
				For i As Integer = 0 To credOwners.Length - 1
					pArray(i)(0) = credOwners(i).principalClass
					pArray(i)(1) = credOwners(i).principalName
				Next i
				Return pArray
			End Get
		End Property

		''' <summary>
		''' Checks if this {@code PrivateCredentialPermission} implies
		''' the specified {@code Permission}.
		''' 
		''' <p>
		''' 
		''' This method returns true if:
		''' <ul>
		''' <li> <i>p</i> is an instanceof PrivateCredentialPermission and
		''' <li> the target name for <i>p</i> is implied by this object's
		'''          target name.  For example:
		''' <pre>
		'''  [* P1 "duke"] implies [a.b.Credential P1 "duke"].
		'''  [C1 P1 "duke"] implies [C1 P1 "duke" P2 "dukette"].
		'''  [C1 P2 "dukette"] implies [C1 P1 "duke" P2 "dukette"].
		''' </pre>
		''' </ul>
		''' 
		''' <p>
		''' </summary>
		''' <param name="p"> the {@code Permission} to check against.
		''' </param>
		''' <returns> true if this {@code PrivateCredentialPermission} implies
		''' the specified {@code Permission}, false if not. </returns>
		Public Function implies(ByVal p As java.security.Permission) As Boolean

			If p Is Nothing OrElse Not(TypeOf p Is PrivateCredentialPermission) Then Return False

			Dim that As PrivateCredentialPermission = CType(p, PrivateCredentialPermission)

			If Not impliesCredentialClass(credentialClass, that.credentialClass) Then Return False

			Return impliesPrincipalSet(credOwners, that.credOwners)
		End Function

		''' <summary>
		''' Checks two {@code PrivateCredentialPermission} objects for
		''' equality.  Checks that <i>obj</i> is a
		''' {@code PrivateCredentialPermission},
		''' and has the same credential class as this object,
		''' as well as the same Principals as this object.
		''' The order of the Principals in the respective Permission's
		''' target names is not relevant.
		''' 
		''' <p>
		''' </summary>
		''' <param name="obj"> the object we are testing for equality with this object.
		''' </param>
		''' <returns> true if obj is a {@code PrivateCredentialPermission},
		'''          has the same credential class as this object,
		'''          and has the same Principals as this object. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True

			If Not(TypeOf obj Is PrivateCredentialPermission) Then Return False

			Dim that As PrivateCredentialPermission = CType(obj, PrivateCredentialPermission)

			Return (Me.implies(that) AndAlso that.implies(Me))
		End Function

		''' <summary>
		''' Returns the hash code value for this object.
		''' </summary>
		''' <returns> a hash code value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return Me.credentialClass.GetHashCode()
		End Function

		''' <summary>
		''' Returns the "canonical string representation" of the actions.
		''' This method always returns the String, "read".
		''' 
		''' <p>
		''' </summary>
		''' <returns> the actions (always returns "read"). </returns>
		Public Property actions As String
			Get
				Return "read"
			End Get
		End Property

		''' <summary>
		''' Return a homogeneous collection of PrivateCredentialPermissions
		''' in a {@code PermissionCollection}.
		''' No such {@code PermissionCollection} is defined,
		''' so this method always returns {@code null}.
		''' 
		''' <p>
		''' </summary>
		''' <returns> null in all cases. </returns>
		Public Function newPermissionCollection() As java.security.PermissionCollection
			Return Nothing
		End Function

		Private Sub init(ByVal name As String)

			If name Is Nothing OrElse name.Trim().Length = 0 Then Throw New System.ArgumentException("invalid empty name")

			Dim pList As New List(Of CredOwner)
			Dim tokenizer As New StringTokenizer(name, " ", True)
			Dim principalClass As String = Nothing
			Dim principalName As String = Nothing

			If testing Then Console.WriteLine("whole name = " & name)

			' get the Credential Class
			credentialClass = tokenizer.nextToken()
			If testing Then Console.WriteLine("Credential Class = " & credentialClass)

			If tokenizer.hasMoreTokens() = False Then
				Dim form As New java.text.MessageFormat(sun.security.util.ResourcesMgr.getString("permission.name.name.syntax.invalid."))
				Dim source As Object() = {name}
				Throw New System.ArgumentException(form.format(source) + sun.security.util.ResourcesMgr.getString("Credential.Class.not.followed.by.a.Principal.Class.and.Name"))
			End If

			Do While tokenizer.hasMoreTokens()

				' skip delimiter
				tokenizer.nextToken()

				' get the Principal Class
				principalClass = tokenizer.nextToken()
				If testing Then Console.WriteLine("    Principal Class = " & principalClass)

				If tokenizer.hasMoreTokens() = False Then
					Dim form As New java.text.MessageFormat(sun.security.util.ResourcesMgr.getString("permission.name.name.syntax.invalid."))
					Dim source As Object() = {name}
					Throw New System.ArgumentException(form.format(source) + sun.security.util.ResourcesMgr.getString("Principal.Class.not.followed.by.a.Principal.Name"))
				End If

				' skip delimiter
				tokenizer.nextToken()

				' get the Principal Name
				principalName = tokenizer.nextToken()

				If Not principalName.StartsWith("""") Then
					Dim form As New java.text.MessageFormat(sun.security.util.ResourcesMgr.getString("permission.name.name.syntax.invalid."))
					Dim source As Object() = {name}
					Throw New System.ArgumentException(form.format(source) + sun.security.util.ResourcesMgr.getString("Principal.Name.must.be.surrounded.by.quotes"))
				End If

				If Not principalName.EndsWith("""") Then

					' we have a name with spaces in it --
					' keep parsing until we find the end quote,
					' and keep the spaces in the name

					Do While tokenizer.hasMoreTokens()
						principalName = principalName + tokenizer.nextToken()
						If principalName.EndsWith("""") Then Exit Do
					Loop

					If Not principalName.EndsWith("""") Then
						Dim form As New java.text.MessageFormat(sun.security.util.ResourcesMgr.getString("permission.name.name.syntax.invalid."))
						Dim source As Object() = {name}
						Throw New System.ArgumentException(form.format(source) + sun.security.util.ResourcesMgr.getString("Principal.Name.missing.end.quote"))
					End If
				End If

				If testing Then Console.WriteLine(vbTab & "principalName = '" & principalName & "'")

				principalName = principalName.Substring(1, principalName.Length - 1 - 1)

				If principalClass.Equals("*") AndAlso (Not principalName.Equals("*")) Then Throw New System.ArgumentException(sun.security.util.ResourcesMgr.getString("PrivateCredentialPermission.Principal.Class.can.not.be.a.wildcard.value.if.Principal.Name.is.not.a.wildcard.value"))

				If testing Then Console.WriteLine(vbTab & "principalName = '" & principalName & "'")

				pList.Add(New CredOwner(principalClass, principalName))
			Loop

			Me.credOwners = New CredOwner(pList.Count - 1){}
			pList.ToArray(Me.credOwners)
		End Sub

		Private Function impliesCredentialClass(ByVal thisC As String, ByVal thatC As String) As Boolean

			' this should never happen
			If thisC Is Nothing OrElse thatC Is Nothing Then Return False

			If testing Then Console.WriteLine("credential class comparison: " & thisC & "/" & thatC)

			If thisC.Equals("*") Then Return True

			''' <summary>
			''' XXX let's not enable this for now --
			'''      if people want it, we'll enable it later
			''' </summary>
	'        
	'        if (thisC.endsWith("*")) {
	'            String cClass = thisC.substring(0, thisC.length() - 2);
	'            return thatC.startsWith(cClass);
	'        }
	'        

			Return thisC.Equals(thatC)
		End Function

		Private Function impliesPrincipalSet(ByVal thisP As CredOwner(), ByVal thatP As CredOwner()) As Boolean

			' this should never happen
			If thisP Is Nothing OrElse thatP Is Nothing Then Return False

			If thatP.Length = 0 Then Return True

			If thisP.Length = 0 Then Return False

			For i As Integer = 0 To thisP.Length - 1
				Dim foundMatch As Boolean = False
				For j As Integer = 0 To thatP.Length - 1
					If thisP(i).implies(thatP(j)) Then
						foundMatch = True
						Exit For
					End If
				Next j
				If Not foundMatch Then Return False
			Next i
			Return True
		End Function

		''' <summary>
		''' Reads this object from a stream (i.e., deserializes it)
		''' </summary>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)

			s.defaultReadObject()

			' perform new initialization from the permission name

			If name.IndexOf(" ") = -1 AndAlso name.IndexOf("""") = -1 Then

				' name only has a credential class specified
				credentialClass = name
				credOwners = EMPTY_PRINCIPALS

			Else

				' perform regular initialization
				init(name)
			End If
		End Sub

		''' <summary>
		''' @serial include
		''' </summary>
		<Serializable> _
		Friend Class CredOwner

			Private Const serialVersionUID As Long = -5607449830436408266L

			''' <summary>
			''' @serial
			''' </summary>
			Friend principalClass As String
			''' <summary>
			''' @serial
			''' </summary>
			Friend principalName As String

			Friend Sub New(ByVal principalClass As String, ByVal principalName As String)
				Me.principalClass = principalClass
				Me.principalName = principalName
			End Sub

			Public Overridable Function implies(ByVal obj As Object) As Boolean
				If obj Is Nothing OrElse Not(TypeOf obj Is CredOwner) Then Return False

				Dim that As CredOwner = CType(obj, CredOwner)

				If principalClass.Equals("*") OrElse principalClass.Equals(that.principalClass) Then

					If principalName.Equals("*") OrElse principalName.Equals(that.principalName) Then Return True
				End If

				''' <summary>
				''' XXX no code yet to support a.b.*
				''' </summary>

				Return False
			End Function

			Public Overrides Function ToString() As String
				Dim form As New java.text.MessageFormat(sun.security.util.ResourcesMgr.getString("CredOwner.Principal.Class.class.Principal.Name.name"))
				Dim source As Object() = {principalClass, principalName}
				Return (form.format(source))
			End Function
		End Class
	End Class

End Namespace

'----------------------------------------------------------------------------------------
'	Copyright © 2007 - 2012 Tangible Software Solutions Inc.
'	This class can be used by anyone provided that the copyright notice remains intact.
'
'	This class provides the logic to simulate Java rectangular arrays, which are jagged
'	arrays with inner arrays of the same length.
'----------------------------------------------------------------------------------------
Partial Friend Class RectangularArrays
    Friend Shared Function ReturnRectangularStringArray(ByVal Size1 As Integer, ByVal Size2 As Integer) As String()()
        Dim Array As String()() = New String(Size1 - 1)() {}
        For Array1 As Integer = 0 To Size1 - 1
            Array(Array1) = New String(Size2 - 1) {}
        Next Array1
        Return Array
    End Function
End Class