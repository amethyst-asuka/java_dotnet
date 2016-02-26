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
	''' A {@code SubjectDomainCombiner} updates ProtectionDomains
	''' with Principals from the {@code Subject} associated with this
	''' {@code SubjectDomainCombiner}.
	''' 
	''' </summary>
	Public Class SubjectDomainCombiner
		Implements java.security.DomainCombiner

		Private ___subject As Subject
		Private cachedPDs As New WeakKeyValueMap(Of java.security.ProtectionDomain, java.security.ProtectionDomain)
		Private principalSet As java.util.Set(Of java.security.Principal)
		Private principals As java.security.Principal()

		Private Shared ReadOnly debug As sun.security.util.Debug = sun.security.util.Debug.getInstance("combiner", vbTab & "[SubjectDomainCombiner]")

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared ReadOnly useJavaxPolicy As Boolean = javax.security.auth.Policy.isCustomPolicySet(debug)
		' Note: check only at classloading time, not dynamically during combine()

		' Relevant only when useJavaxPolicy is true
		Private Shared ReadOnly allowCaching As Boolean = (useJavaxPolicy AndAlso cachePolicy())

		''' <summary>
		''' Associate the provided {@code Subject} with this
		''' {@code SubjectDomainCombiner}.
		''' 
		''' <p>
		''' </summary>
		''' <param name="subject"> the {@code Subject} to be associated with
		'''          with this {@code SubjectDomainCombiner}. </param>
		Public Sub New(ByVal ___subject As Subject)
			Me.___subject = ___subject

			If ___subject.readOnly Then
				principalSet = ___subject.principals
				principals = principalSet.ToArray(New java.security.Principal(principalSet.size() - 1){})
			End If
		End Sub

		''' <summary>
		''' Get the {@code Subject} associated with this
		''' {@code SubjectDomainCombiner}.
		''' 
		''' <p>
		''' </summary>
		''' <returns> the {@code Subject} associated with this
		'''          {@code SubjectDomainCombiner}, or {@code null}
		'''          if no {@code Subject} is associated with this
		'''          {@code SubjectDomainCombiner}.
		''' </returns>
		''' <exception cref="SecurityException"> if the caller does not have permission
		'''          to get the {@code Subject} associated with this
		'''          {@code SubjectDomainCombiner}. </exception>
		Public Overridable Property subject As Subject
			Get
				Dim sm As java.lang.SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(New AuthPermission("getSubjectFromDomainCombiner"))
				Return ___subject
			End Get
		End Property

		''' <summary>
		''' Update the relevant ProtectionDomains with the Principals
		''' from the {@code Subject} associated with this
		''' {@code SubjectDomainCombiner}.
		''' 
		''' <p> A new {@code ProtectionDomain} instance is created
		''' for each {@code ProtectionDomain} in the
		''' <i>currentDomains</i> array.  Each new {@code ProtectionDomain}
		''' instance is created using the {@code CodeSource},
		''' {@code Permission}s and {@code ClassLoader}
		''' from the corresponding {@code ProtectionDomain} in
		''' <i>currentDomains</i>, as well as with the Principals from
		''' the {@code Subject} associated with this
		''' {@code SubjectDomainCombiner}.
		''' 
		''' <p> All of the newly instantiated ProtectionDomains are
		''' combined into a new array.  The ProtectionDomains from the
		''' <i>assignedDomains</i> array are appended to this new array,
		''' and the result is returned.
		''' 
		''' <p> Note that optimizations such as the removal of duplicate
		''' ProtectionDomains may have occurred.
		''' In addition, caching of ProtectionDomains may be permitted.
		''' 
		''' <p>
		''' </summary>
		''' <param name="currentDomains"> the ProtectionDomains associated with the
		'''          current execution Thread, up to the most recent
		'''          privileged {@code ProtectionDomain}.
		'''          The ProtectionDomains are are listed in order of execution,
		'''          with the most recently executing {@code ProtectionDomain}
		'''          residing at the beginning of the array. This parameter may
		'''          be {@code null} if the current execution Thread
		'''          has no associated ProtectionDomains.<p>
		''' </param>
		''' <param name="assignedDomains"> the ProtectionDomains inherited from the
		'''          parent Thread, or the ProtectionDomains from the
		'''          privileged <i>context</i>, if a call to
		'''          AccessController.doPrivileged(..., <i>context</i>)
		'''          had occurred  This parameter may be {@code null}
		'''          if there were no ProtectionDomains inherited from the
		'''          parent Thread, or from the privileged <i>context</i>.
		''' </param>
		''' <returns> a new array consisting of the updated ProtectionDomains,
		'''          or {@code null}. </returns>
		Public Overridable Function combine(ByVal currentDomains As java.security.ProtectionDomain(), ByVal assignedDomains As java.security.ProtectionDomain()) As java.security.ProtectionDomain()
			If debug IsNot Nothing Then
				If ___subject Is Nothing Then
					debug.println("null subject")
				Else
					Dim s As Subject = ___subject
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'					java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Void>()
	'				{
	'					public Void run()
	'					{
	'						debug.println(s.toString());
	'						Return Nothing;
	'					}
	'				});
				End If
				printInputDomains(currentDomains, assignedDomains)
			End If

			If currentDomains Is Nothing OrElse currentDomains.Length = 0 Then Return assignedDomains

			' optimize currentDomains
			'
			' No need to optimize assignedDomains because it should
			' have been previously optimized (when it was set).

			currentDomains = optimize(currentDomains)
			If debug IsNot Nothing Then
				debug.println("after optimize")
				printInputDomains(currentDomains, assignedDomains)
			End If

			If currentDomains Is Nothing AndAlso assignedDomains Is Nothing Then Return Nothing

			' maintain backwards compatibility for developers who provide
			' their own custom javax.security.auth.Policy implementations
			If useJavaxPolicy Then Return combineJavaxPolicy(currentDomains, assignedDomains)

			Dim cLen As Integer = (If(currentDomains Is Nothing, 0, currentDomains.Length))
			Dim aLen As Integer = (If(assignedDomains Is Nothing, 0, assignedDomains.Length))

			' the ProtectionDomains for the new AccessControlContext
			' that we will return
			Dim newDomains As java.security.ProtectionDomain() = New java.security.ProtectionDomain(cLen + aLen - 1){}

			Dim allNew As Boolean = True
			SyncLock cachedPDs
				If (Not ___subject.readOnly) AndAlso (Not ___subject.principals.Equals(principalSet)) Then

					' if the Subject was mutated, clear the PD cache
					Dim newSet As java.util.Set(Of java.security.Principal) = ___subject.principals
					SyncLock newSet
						principalSet = New HashSet(Of java.security.Principal)(newSet)
					End SyncLock
					principals = principalSet.ToArray(New java.security.Principal(principalSet.size() - 1){})
					cachedPDs.clear()

					If debug IsNot Nothing Then debug.println("Subject mutated - clearing cache")
				End If

				Dim subjectPd As java.security.ProtectionDomain
				For i As Integer = 0 To cLen - 1
					Dim pd As java.security.ProtectionDomain = currentDomains(i)

					subjectPd = cachedPDs.getValue(pd)

					If subjectPd Is Nothing Then
						subjectPd = New java.security.ProtectionDomain(pd.codeSource, pd.permissions, pd.classLoader, principals)
						cachedPDs.putValue(pd, subjectPd)
					Else
						allNew = False
					End If
					newDomains(i) = subjectPd
				Next i
			End SyncLock

			If debug IsNot Nothing Then
				debug.println("updated current: ")
				For i As Integer = 0 To cLen - 1
					debug.println(vbTab & "updated[" & i & "] = " & printDomain(newDomains(i)))
				Next i
			End If

			' now add on the assigned domains
			If aLen > 0 Then
				Array.Copy(assignedDomains, 0, newDomains, cLen, aLen)

				' optimize the result (cached PDs might exist in assignedDomains)
				If Not allNew Then newDomains = optimize(newDomains)
			End If

			' if aLen == 0 || allNew, no need to further optimize newDomains

			If debug IsNot Nothing Then
				If newDomains Is Nothing OrElse newDomains.Length = 0 Then
					debug.println("returning null")
				Else
					debug.println("combinedDomains: ")
					For i As Integer = 0 To newDomains.Length - 1
						debug.println("newDomain " & i & ": " & printDomain(newDomains(i)))
					Next i
				End If
			End If

			' return the new ProtectionDomains
			If newDomains Is Nothing OrElse newDomains.Length = 0 Then
				Return Nothing
			Else
				Return newDomains
			End If
		End Function

		''' <summary>
		''' Use the javax.security.auth.Policy implementation
		''' </summary>
		Private Function combineJavaxPolicy(ByVal currentDomains As java.security.ProtectionDomain(), ByVal assignedDomains As java.security.ProtectionDomain()) As java.security.ProtectionDomain()

			If Not allowCaching Then
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'				java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<Void>()
	'			{
	'					@SuppressWarnings("deprecation") public Void run()
	'					{
	'						' Call refresh only caching is disallowed
	'						javax.security.auth.Policy.getPolicy().refresh();
	'						Return Nothing;
	'					}
	'				});
			End If


			Dim cLen As Integer = (If(currentDomains Is Nothing, 0, currentDomains.Length))
			Dim aLen As Integer = (If(assignedDomains Is Nothing, 0, assignedDomains.Length))

			' the ProtectionDomains for the new AccessControlContext
			' that we will return
			Dim newDomains As java.security.ProtectionDomain() = New java.security.ProtectionDomain(cLen + aLen - 1){}

			SyncLock cachedPDs
				If (Not ___subject.readOnly) AndAlso (Not ___subject.principals.Equals(principalSet)) Then

					' if the Subject was mutated, clear the PD cache
					Dim newSet As java.util.Set(Of java.security.Principal) = ___subject.principals
					SyncLock newSet
						principalSet = New HashSet(Of java.security.Principal)(newSet)
					End SyncLock
					principals = principalSet.ToArray(New java.security.Principal(principalSet.size() - 1){})
					cachedPDs.clear()

					If debug IsNot Nothing Then debug.println("Subject mutated - clearing cache")
				End If

				For i As Integer = 0 To cLen - 1
					Dim pd As java.security.ProtectionDomain = currentDomains(i)
					Dim subjectPd As java.security.ProtectionDomain = cachedPDs.getValue(pd)

					If subjectPd Is Nothing Then

						' XXX
						' we must first add the original permissions.
						' that way when we later add the new JAAS permissions,
						' any unresolved JAAS-related permissions will
						' automatically get resolved.

						' get the original perms
						Dim perms As New java.security.Permissions
						Dim coll As java.security.PermissionCollection = pd.permissions
						Dim e As System.Collections.IEnumerator(Of java.security.Permission)
						If coll IsNot Nothing Then
							SyncLock coll
								e = coll.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
									Dim newPerm As java.security.Permission = e.nextElement()
									 perms.add(newPerm)
								Loop
							End SyncLock
						End If

						' get perms from the policy

						Dim finalCs As java.security.CodeSource = pd.codeSource
						Dim finalS As Subject = ___subject
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'						java.security.PermissionCollection newPerms = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<java.security.PermissionCollection>()
	'					{
	'						@SuppressWarnings("deprecation") public PermissionCollection run()
	'						{
	'						  Return javax.security.auth.Policy.getPolicy().getPermissions(finalS, finalCs);
	'						}
	'					});

						' add the newly granted perms,
						' avoiding duplicates
						SyncLock newPerms
							e = newPerms.elements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
							Do While e.hasMoreElements()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
								Dim newPerm As java.security.Permission = e.nextElement()
								If Not perms.implies(newPerm) Then
									perms.add(newPerm)
									If debug IsNot Nothing Then debug.println("Adding perm " & newPerm & vbLf)
								End If
							Loop
						End SyncLock
						subjectPd = New java.security.ProtectionDomain(finalCs, perms, pd.classLoader, principals)

						If allowCaching Then cachedPDs.putValue(pd, subjectPd)
					End If
					newDomains(i) = subjectPd
				Next i
			End SyncLock

			If debug IsNot Nothing Then
				debug.println("updated current: ")
				For i As Integer = 0 To cLen - 1
					debug.println(vbTab & "updated[" & i & "] = " & newDomains(i))
				Next i
			End If

			' now add on the assigned domains
			If aLen > 0 Then Array.Copy(assignedDomains, 0, newDomains, cLen, aLen)

			If debug IsNot Nothing Then
				If newDomains Is Nothing OrElse newDomains.Length = 0 Then
					debug.println("returning null")
				Else
					debug.println("combinedDomains: ")
					For i As Integer = 0 To newDomains.Length - 1
						debug.println("newDomain " & i & ": " & newDomains(i).ToString())
					Next i
				End If
			End If

			' return the new ProtectionDomains
			If newDomains Is Nothing OrElse newDomains.Length = 0 Then
				Return Nothing
			Else
				Return newDomains
			End If
		End Function

		Private Shared Function optimize(ByVal domains As java.security.ProtectionDomain()) As java.security.ProtectionDomain()
			If domains Is Nothing OrElse domains.Length = 0 Then Return Nothing

			Dim optimized As java.security.ProtectionDomain() = New java.security.ProtectionDomain(domains.Length - 1){}
			Dim pd As java.security.ProtectionDomain
			Dim num As Integer = 0
			For i As Integer = 0 To domains.Length - 1

				' skip domains with AllPermission
				' XXX
				'
				'  if (domains[i].implies(ALL_PERMISSION))
				'  continue;

				' skip System Domains
				pd = domains(i)
				If pd IsNot Nothing Then

					' remove duplicates
					Dim found As Boolean = False
					Dim j As Integer = 0
					Do While j < num AndAlso Not found
						found = (optimized(j) Is pd)
						j += 1
					Loop
					If Not found Then
						optimized(num) = pd
						num += 1
					End If
				End If
			Next i

			' resize the array if necessary
			If num > 0 AndAlso num < domains.Length Then
				Dim downSize As java.security.ProtectionDomain() = New java.security.ProtectionDomain(num - 1){}
				Array.Copy(optimized, 0, downSize, 0, downSize.Length)
				optimized = downSize
			End If

			Return (If(num = 0 OrElse optimized.Length = 0, Nothing, optimized))
		End Function

		Private Shared Function cachePolicy() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			String s = java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<String>()
	'		{
	'			public String run()
	'			{
	'				Return Security.getProperty("cache.auth.policy");
	'			}
	'		});
			If s IsNot Nothing Then Return Convert.ToBoolean(s)

			' cache by default
			Return True
		End Function

		Private Shared Sub printInputDomains(ByVal currentDomains As java.security.ProtectionDomain(), ByVal assignedDomains As java.security.ProtectionDomain())
			If currentDomains Is Nothing OrElse currentDomains.Length = 0 Then
				debug.println("currentDomains null or 0 length")
			Else
				Dim i As Integer = 0
				Do While currentDomains IsNot Nothing AndAlso i < currentDomains.Length
					If currentDomains(i) Is Nothing Then
						debug.println("currentDomain " & i & ": SystemDomain")
					Else
						debug.println("currentDomain " & i & ": " & printDomain(currentDomains(i)))
					End If
					i += 1
				Loop
			End If

			If assignedDomains Is Nothing OrElse assignedDomains.Length = 0 Then
				debug.println("assignedDomains null or 0 length")
			Else
				debug.println("assignedDomains = ")
				Dim i As Integer = 0
				Do While assignedDomains IsNot Nothing AndAlso i < assignedDomains.Length
					If assignedDomains(i) Is Nothing Then
						debug.println("assignedDomain " & i & ": SystemDomain")
					Else
						debug.println("assignedDomain " & i & ": " & printDomain(assignedDomains(i)))
					End If
					i += 1
				Loop
			End If
		End Sub

		Private Shared Function printDomain(ByVal pd As java.security.ProtectionDomain) As String
			If pd Is Nothing Then Return "null"
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			Return java.security.AccessController.doPrivileged(New java.security.PrivilegedAction<String>()
	'		{
	'			public String run()
	'			{
	'				Return pd.toString();
	'			}
	'		});
		End Function

		''' <summary>
		''' A HashMap that has weak keys and values.
		''' 
		''' Key objects in this map are the "current" ProtectionDomain instances
		''' received via the combine method.  Each "current" PD is mapped to a
		''' new PD instance that holds both the contents of the "current" PD,
		''' as well as the principals from the Subject associated with this combiner.
		''' 
		''' The newly created "principal-based" PD values must be stored as
		''' WeakReferences since they contain strong references to the
		''' corresponding key object (the "current" non-principal-based PD),
		''' which will prevent the key from being GC'd.  Specifically,
		''' a "principal-based" PD contains strong references to the CodeSource,
		''' signer certs, PermissionCollection and ClassLoader objects
		''' in the "current PD".
		''' </summary>
		Private Class WeakKeyValueMap(Of K, V)
			Inherits java.util.WeakHashMap(Of K, WeakReference(Of V))

			Public Overridable Function getValue(ByVal key As K) As V
				Dim wr As WeakReference(Of V) = MyBase.get(key)
				If wr IsNot Nothing Then Return wr.get()
				Return Nothing
			End Function

			Public Overridable Function putValue(ByVal key As K, ByVal value As V) As V
				Dim wr As WeakReference(Of V) = MyBase.put(key, New WeakReference(Of V)(value))
				If wr IsNot Nothing Then Return wr.get()
				Return Nothing
			End Function
		End Class
	End Class

End Namespace