Imports System
Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2015, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.security



	''' <summary>
	''' An AccessControlContext is used to make system resource access decisions
	''' based on the context it encapsulates.
	''' 
	''' <p>More specifically, it encapsulates a context and
	''' has a single method, {@code checkPermission},
	''' that is equivalent to the {@code checkPermission} method
	''' in the AccessController [Class], with one difference: The AccessControlContext
	''' {@code checkPermission} method makes access decisions based on the
	''' context it encapsulates,
	''' rather than that of the current execution thread.
	''' 
	''' <p>Thus, the purpose of AccessControlContext is for those situations where
	''' a security check that should be made within a given context
	''' actually needs to be done from within a
	''' <i>different</i> context (for example, from within a worker thread).
	''' 
	''' <p> An AccessControlContext is created by calling the
	''' {@code AccessController.getContext} method.
	''' The {@code getContext} method takes a "snapshot"
	''' of the current calling context, and places
	''' it in an AccessControlContext object, which it returns. A sample call is
	''' the following:
	''' 
	''' <pre>
	'''   AccessControlContext acc = AccessController.getContext()
	''' </pre>
	''' 
	''' <p>
	''' Code within a different context can subsequently call the
	''' {@code checkPermission} method on the
	''' previously-saved AccessControlContext object. A sample call is the
	''' following:
	''' 
	''' <pre>
	'''   acc.checkPermission(permission)
	''' </pre>
	''' </summary>
	''' <seealso cref= AccessController
	''' 
	''' @author Roland Schemers </seealso>

	Public NotInheritable Class AccessControlContext

		Private context As ProtectionDomain()
		' isPrivileged and isAuthorized are referenced by the VM - do not remove
		' or change their names
		Private isPrivileged_Renamed As Boolean
		Private isAuthorized_Renamed As Boolean = False

		' Note: This field is directly used by the virtual machine
		' native codes. Don't touch it.
		Private privilegedContext As AccessControlContext

		Private combiner As DomainCombiner = Nothing

		' limited privilege scope
		Private permissions As Permission()
		Private parent As AccessControlContext
		Private isWrapped As Boolean

		' is constrained by limited privilege scope?
		Private isLimited As Boolean
		Private limitedContext As ProtectionDomain()

		Private Shared debugInit As Boolean = False
        Private Shared _debug As sun.security.util.Debug = Nothing

        Friend Shared ReadOnly Property debug As sun.security.util.Debug
            Get
                If debugInit Then
                    Return _debug
                Else
                    If Policy.set Then
                        _debug = sun.security.util.Debug.getInstance("access")
                        debugInit = True
                    End If
                    Return _debug
                End If
            End Get
        End Property

        ''' <summary>
        ''' Create an AccessControlContext with the given array of ProtectionDomains.
        ''' Context must not be null. Duplicate domains will be removed from the
        ''' context.
        ''' </summary>
        ''' <param name="context"> the ProtectionDomains associated with this context.
        ''' The non-duplicate domains are copied from the array. Subsequent
        ''' changes to the array will not affect this AccessControlContext. </param>
        ''' <exception cref="NullPointerException"> if {@code context} is {@code null} </exception>
        Public Sub New(ByVal context As ProtectionDomain())
			If context.Length = 0 Then
				Me.context = Nothing
			ElseIf context.Length = 1 Then
				If context(0) IsNot Nothing Then
					Me.context = context.clone()
				Else
					Me.context = Nothing
				End If
			Else
				Dim v As IList(Of ProtectionDomain) = New List(Of ProtectionDomain)(context.Length)
				For i As Integer = 0 To context.Length - 1
					If (context(i) IsNot Nothing) AndAlso ((Not v.Contains(context(i)))) Then v.Add(context(i))
				Next i
				If v.Count > 0 Then
					Me.context = New ProtectionDomain(v.Count - 1){}
					Me.context = v.ToArray(Me.context)
				End If
			End If
		End Sub

		''' <summary>
		''' Create a new {@code AccessControlContext} with the given
		''' {@code AccessControlContext} and {@code DomainCombiner}.
		''' This constructor associates the provided
		''' {@code DomainCombiner} with the provided
		''' {@code AccessControlContext}.
		''' 
		''' <p>
		''' </summary>
		''' <param name="acc"> the {@code AccessControlContext} associated
		'''          with the provided {@code DomainCombiner}.
		''' </param>
		''' <param name="combiner"> the {@code DomainCombiner} to be associated
		'''          with the provided {@code AccessControlContext}.
		''' </param>
		''' <exception cref="NullPointerException"> if the provided
		'''          {@code context} is {@code null}.
		''' </exception>
		''' <exception cref="SecurityException"> if a security manager is installed and the
		'''          caller does not have the "createAccessControlContext"
		'''          <seealso cref="SecurityPermission"/>
		''' @since 1.3 </exception>
		Public Sub New(ByVal acc As AccessControlContext, ByVal combiner As DomainCombiner)

			Me.New(acc, combiner, False)
		End Sub

		''' <summary>
		''' package private to allow calls from ProtectionDomain without performing
		''' the security check for <seealso cref="SecurityConstants.CREATE_ACC_PERMISSION"/>
		''' permission
		''' </summary>
		Friend Sub New(ByVal acc As AccessControlContext, ByVal combiner As DomainCombiner, ByVal preauthorized As Boolean)
			If Not preauthorized Then
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then
					sm.checkPermission(sun.security.util.SecurityConstants.CREATE_ACC_PERMISSION)
					Me.isAuthorized_Renamed = True
				End If
			Else
				Me.isAuthorized_Renamed = True
			End If

			Me.context = acc.context

			' we do not need to run the combine method on the
			' provided ACC.  it was already "combined" when the
			' context was originally retrieved.
			'
			' at this point in time, we simply throw away the old
			' combiner and use the newly provided one.
			Me.combiner = combiner
		End Sub

		''' <summary>
		''' package private for AccessController
		''' 
		''' This "argument wrapper" context will be passed as the actual context
		''' parameter on an internal doPrivileged() call used in the implementation.
		''' </summary>
		Friend Sub New(ByVal caller As ProtectionDomain, ByVal combiner As DomainCombiner, ByVal parent As AccessControlContext, ByVal context As AccessControlContext, ByVal perms As Permission())
	'        
	'         * Combine the domains from the doPrivileged() context into our
	'         * wrapper context, if necessary.
	'         
			Dim callerPDs As ProtectionDomain() = Nothing
			If caller IsNot Nothing Then callerPDs = New ProtectionDomain() { caller }
			If context IsNot Nothing Then
				If combiner IsNot Nothing Then
					Me.context = combiner.combine(callerPDs, context.context)
				Else
					Me.context = combine(callerPDs, context.context)
				End If
			Else
	'            
	'             * Call combiner even if there is seemingly nothing to combine.
	'             
				If combiner IsNot Nothing Then
					Me.context = combiner.combine(callerPDs, Nothing)
				Else
					Me.context = combine(callerPDs, Nothing)
				End If
			End If
			Me.combiner = combiner

			Dim tmp As Permission() = Nothing
			If perms IsNot Nothing Then
				tmp = New Permission(perms.Length - 1){}
				For i As Integer = 0 To perms.Length - 1
					If perms(i) Is Nothing Then Throw New NullPointerException("permission can't be null")

	'                
	'                 * An AllPermission argument is equivalent to calling
	'                 * doPrivileged() without any limit permissions.
	'                 
					If perms(i).GetType() Is GetType(AllPermission) Then parent = Nothing
					tmp(i) = perms(i)
				Next i
			End If

	'        
	'         * For a doPrivileged() with limited privilege scope, initialize
	'         * the relevant fields.
	'         *
	'         * The limitedContext field contains the union of all domains which
	'         * are enclosed by this limited privilege scope. In other words,
	'         * it contains all of the domains which could potentially be checked
	'         * if none of the limiting permissions implied a requested permission.
	'         
			If parent IsNot Nothing Then
				Me.limitedContext = combine(parent.context, parent.limitedContext)
				Me.isLimited = True
				Me.isWrapped = True
				Me.permissions = tmp
				Me.parent = parent
				Me.privilegedContext = context ' used in checkPermission2()
			End If
			Me.isAuthorized_Renamed = True
		End Sub


        ''' <summary>
        ''' package private constructor for AccessController.getContext()
        ''' </summary>
        Sub New(context() As ProtectionDomain, isPrivileged As Boolean)
            Me.context = context
            Me.isPrivileged_Renamed = isPrivileged_Renamed
            Me.isAuthorized_Renamed = True
        End Sub
        ''' <summary>
        ''' Constructor for JavaSecurityAccess.doIntersectionPrivilege()
        ''' </summary>
        Sub New(context As ProtectionDomain(), privilegedContext As AccessControlContext)
            Me.context = context
            Me.privilegedContext = privilegedContext
            Me.isPrivileged_Renamed = True
        End Sub
        ''' <summary>
        ''' Returns this context's context.
        ''' </summary>
        Public Function context() As ProtectionDomain()
            Return context
        End Function

        ''' <summary>
        ''' Returns true if this context is privileged.
        ''' </summary>
        Public Function privileged() As Boolean
            Return isPrivileged_Renamed
        End Function

        ''' <summary>
        ''' get the assigned combiner from the privileged or inherited context
        ''' </summary>
        Public Function assignedCombiner() As DomainCombiner
            Dim acc As AccessControlContext
            If isPrivileged_Renamed Then
                acc = privilegedContext
            Else
                acc = AccessController.inheritedAccessControlContext
            End If
            If acc IsNot Nothing Then Return acc.combiner
            Return Nothing
        End Function
        ''' <summary>
        ''' Get the {@code DomainCombiner} associated with this
        ''' {@code AccessControlContext}.
        ''' 
        ''' <p>
        ''' </summary>
        ''' <returns> the {@code DomainCombiner} associated with this
        '''          {@code AccessControlContext}, or {@code null}
        '''          if there is none.
        ''' </returns>
        ''' <exception cref="SecurityException"> if a security manager is installed and
        '''          the caller does not have the "getDomainCombiner"
        '''          <seealso cref="SecurityPermission"/>
        ''' @since 1.3 </exception>
        Public Function domainCombiner() As DomainCombiner

            Dim sm As SecurityManager = System.securityManager
            If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.GET_COMBINER_PERMISSION)
            Return combiner
        End Function
        ''' <summary>
        ''' package private for AccessController
        ''' </summary>
        Public Function combiner() As DomainCombiner
            Return combiner
        End Function
        Public Function authorized() As Boolean
            Return isAuthorized_Renamed
        End Function
        ''' <summary>
        ''' Determines whether the access request indicated by the
        ''' specified permission should be allowed or denied, based on
        ''' the security policy currently in effect, and the context in
        ''' this object. The request is allowed only if every ProtectionDomain
        ''' in the context implies the permission. Otherwise the request is
        ''' denied.
        ''' 
        ''' <p>
        ''' This method quietly returns if the access request
        ''' is permitted, or throws a suitable AccessControlException otherwise.
        ''' </summary>
        ''' <param name="perm"> the requested permission.
        ''' </param>
        ''' <exception cref="AccessControlException"> if the specified permission
        ''' is not permitted, based on the current security policy and the
        ''' context encapsulated by this object. </exception>
        ''' <exception cref="NullPointerException"> if the permission to check for is null. </exception>
        Public Sub checkPermission(perm As Permission) 'throws AccessControlException
            Dim dumpDebug As Boolean = False

            If perm Is Nothing Then Throw New NullPointerException("permission can't be null")
            If debug IsNot Nothing Then
                ' If "codebase" is not specified, we dump the info by default.
                dumpDebug = Not sun.security.util.Debug.isOn("codebase=")
                If Not dumpDebug Then
                    ' If "codebase" is specified, only dump if the specified code
                    ' value is in the stack.
                    Dim i As Integer = 0
                    Do While context IsNot Nothing AndAlso i < context.Length
                        If context(i).codeSource IsNot Nothing AndAlso context(i).codeSource.location IsNot Nothing AndAlso sun.security.util.Debug.isOn("codebase=" & context(i).codeSource.location.ToString()) Then
                            dumpDebug = True
                            Exit Do
                        End If
                        i += 1
                    Loop
                End If

                dumpDebug = dumpDebug And (Not sun.security.util.Debug.isOn("permission=")) OrElse sun.security.util.Debug.isOn("permission=" & perm.GetType().canonicalName)

                If dumpDebug AndAlso sun.security.util.Debug.isOn("stack") Then Thread.dumpStack()

                If dumpDebug AndAlso sun.security.util.Debug.isOn("domain") Then
                    If context Is Nothing Then
                        debug.println("domain (context is null)")
                    Else
                        For i As Integer = 0 To context.Length - 1
                            debug.println("domain " & i & " " & context(i))
                        Next i
                    End If
                End If
            End If

            '        
            '         * iterate through the ProtectionDomains in the context.
            '         * Stop at the first one that doesn't allow the
            '         * requested permission (throwing an exception).
            '         *
            '         

            '         if ctxt is null, all we had on the stack were system domains,
            '           or the first domain was a Privileged system domain. This
            '           is to make the common case for system code very fast 

            If context Is Nothing Then
                checkPermission2(perm)
                Return
            End If

            For i As Integer = 0 To context.Length - 1
                If context(i) IsNot Nothing AndAlso (Not context(i).implies(perm)) Then
                    If dumpDebug Then debug.println("access denied " & perm)

                    If sun.security.util.Debug.isOn("failure") AndAlso debug IsNot Nothing Then
                        ' Want to make sure this is always displayed for failure,
                        ' but do not want to display again if already displayed
                        ' above.
                        If Not dumpDebug Then debug.println("access denied " & perm)
                        Thread.dumpStack()
                        Dim pd As ProtectionDomain = context(i)
                        Dim db As sun.security.util.Debug = debug
                        AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
                    End If
                    Throw New AccessControlException("access denied " & perm, perm)
                End If
            Next i

            ' allow if all of them allowed access
            If dumpDebug Then debug.println("access allowed " & perm)

            checkPermission2(perm)
        End Sub
        '    
        '     * Check the domains associated with the limited privilege scope.
        '     
        Private Sub checkPermission2(perm As Permission)
            If Not isLimited Then Return

            '        
            '         * Check the doPrivileged() context parameter, if present.
            '         
            If privilegedContext IsNot Nothing Then privilegedContext.checkPermission2(perm)

            '        
            '         * Ignore the limited permissions and parent fields of a wrapper
            '         * context since they were already carried down into the unwrapped
            '         * context.
            '         
            If isWrapped Then Return

            '        
            '         * Try to match any limited privilege scope.
            '         
            If permissions IsNot Nothing Then
                Dim permClass As [Class] = perm.GetType()
                For i As Integer = 0 To permissions.Length - 1
                    Dim limit As Permission = permissions(i)
                    If limit.GetType().Equals(permClass) AndAlso limit.implies(perm) Then Return
                Next i
            End If

            '        
            '         * Check the limited privilege scope up the call stack or the inherited
            '         * parent thread call stack of this ACC.
            '         
            If parent IsNot Nothing Then
                '            
                '             * As an optimization, if the parent context is the inherited call
                '             * stack context from a parent thread then checking the protection
                '             * domains of the parent context is redundant since they have
                '             * already been merged into the child thread's context by
                '             * optimize(). When parent is set to an inherited context this
                '             * context was not directly created by a limited scope
                '             * doPrivileged() and it does not have its own limited permissions.
                '             
                If permissions Is Nothing Then
                    parent.checkPermission2(perm)
                Else
                    parent.checkPermission(perm)
                End If
            End If
        End Sub
        ''' <summary>
        ''' Take the stack-based context (this) and combine it with the
        ''' privileged or inherited context, if need be. Any limited
        ''' privilege scope is flagged regardless of whether the assigned
        ''' context comes from an immediately enclosing limited doPrivileged().
        ''' The limited privilege scope can indirectly flow from the inherited
        ''' parent thread or an assigned context previously captured by getContext().
        ''' </summary>
        Public Function optimize() As AccessControlContext
            ' the assigned (privileged or inherited) context
            Dim acc As AccessControlContext
            Dim combiner_Renamed As DomainCombiner = Nothing
            Dim parent As AccessControlContext = Nothing
            Dim permissions As Permission() = Nothing

            If isPrivileged_Renamed Then
                acc = privilegedContext
                If acc IsNot Nothing Then
                    '                
                    '                 * If the context is from a limited scope doPrivileged() then
                    '                 * copy the permissions and parent fields out of the wrapper
                    '                 * context that was created to hold them.
                    '                 
                    If acc.isWrapped Then
                        permissions = acc.permissions
                        parent = acc.parent
                    End If
                End If
            Else
                acc = AccessController.inheritedAccessControlContext
                If acc IsNot Nothing Then
                    '                
                    '                 * If the inherited context is constrained by a limited scope
                    '                 * doPrivileged() then set it as our parent so we will process
                    '                 * the non-domain-related state.
                    '                 
                    If acc.isLimited Then parent = acc
                End If
            End If

            ' this.context could be null if only system code is on the stack;
            ' in that case, ignore the stack context
            Dim skipStack As Boolean = (context Is Nothing)

            ' acc.context could be null if only system code was involved;
            ' in that case, ignore the assigned context
            Dim skipAssigned As Boolean = (acc Is Nothing OrElse acc.context Is Nothing)
            Dim assigned As ProtectionDomain() = If(skipAssigned, Nothing, acc.context)
            Dim pd As ProtectionDomain()

            ' if there is no enclosing limited privilege scope on the stack or
            ' inherited from a parent thread
            Dim skipLimited As Boolean = ((acc Is Nothing OrElse (Not acc.isWrapped)) AndAlso parent Is Nothing)

            If acc IsNot Nothing AndAlso acc.combiner IsNot Nothing Then
                ' let the assigned acc's combiner do its thing
                If debug IsNot Nothing Then debug.println("AccessControlContext invoking the Combiner")

                ' No need to clone current and assigned.context
                ' combine() will not update them
                combiner_Renamed = acc.combiner
                pd = combiner_Renamed.combine(context, assigned)
            Else
                If skipStack Then
                    If skipAssigned Then
                        calculateFields(acc, parent, permissions)
                        Return Me
                    ElseIf skipLimited Then
                        Return acc
                    End If
                ElseIf assigned IsNot Nothing Then
                    If skipLimited Then
                        ' optimization: if there is a single stack domain and
                        ' that domain is already in the assigned context; no
                        ' need to combine
                        If context.Length = 1 AndAlso context(0) Is assigned(0) Then Return acc
                    End If
                End If

                pd = combine(context, assigned)
                If skipLimited AndAlso (Not skipAssigned) AndAlso pd = assigned Then
                    Return acc
                ElseIf skipAssigned AndAlso pd = context Then
                    calculateFields(acc, parent, permissions)
                    Return Me
                End If
            End If

            ' Reuse existing ACC
            Me.context = pd
            Me.combiner = combiner_Renamed
            Me.isPrivileged_Renamed = False

            calculateFields(acc, parent, permissions)
            Return Me
        End Function

        '    
        '     * Combine the current (stack) and assigned domains.
        '     
        Private Shared Function combine(current As ProtectionDomain(), assigned As ProtectionDomain()) As ProtectionDomain()

            ' current could be null if only system code is on the stack;
            ' in that case, ignore the stack context
            Dim skipStack As Boolean = (current Is Nothing)

            ' assigned could be null if only system code was involved;
            ' in that case, ignore the assigned context
            Dim skipAssigned As Boolean = (assigned Is Nothing)

            Dim slen As Integer = If(skipStack, 0, current.Length)

            ' optimization: if there is no assigned context and the stack length
            ' is less then or equal to two; there is no reason to compress the
            ' stack context, it already is
            If skipAssigned AndAlso slen <= 2 Then Return current

            Dim n As Integer = If(skipAssigned, 0, assigned.Length)

            ' now we combine both of them, and create a new context
            Dim pd As ProtectionDomain() = New ProtectionDomain(slen + n - 1) {}

            ' first copy in the assigned context domains, no need to compress
            If Not skipAssigned Then Array.Copy(assigned, 0, pd, 0, n)

            ' now add the stack context domains, discarding nulls and duplicates
outer:
            For i As Integer = 0 To slen - 1
                Dim sd As ProtectionDomain = current(i)
                If sd IsNot Nothing Then
                    For j As Integer = 0 To n - 1
                        If sd Is pd(j) Then GoTo outer
                    Next j
                    pd(n) = sd
                    n += 1
                End If
            Next i

            ' if length isn't equal, we need to shorten the array
            If n <> pd.Length Then
                ' optimization: if we didn't really combine anything
                If (Not skipAssigned) AndAlso n = assigned.Length Then
                    Return assigned
                ElseIf skipAssigned AndAlso n = slen Then
                    Return current
                End If
                Dim tmp As ProtectionDomain() = New ProtectionDomain(n - 1) {}
                Array.Copy(pd, 0, tmp, 0, n)
                pd = tmp
            End If

            Return pd
        End Function

        '    
        '     * Calculate the additional domains that could potentially be reached via
        '     * limited privilege scope. Mark the context as being subject to limited
        '     * privilege scope unless the reachable domains (if any) are already
        '     * contained in this domain context (in which case any limited
        '     * privilege scope checking would be redundant).
        '     
        Private Sub calculateFields(assigned As AccessControlContext, parent As AccessControlContext, permissions As Permission())
            Dim parentLimit As ProtectionDomain() = Nothing
            Dim assignedLimit As ProtectionDomain() = Nothing
            Dim newLimit As ProtectionDomain()

            parentLimit = If(parent IsNot Nothing, parent.limitedContext, Nothing)
            assignedLimit = If(assigned IsNot Nothing, assigned.limitedContext, Nothing)
            newLimit = combine(parentLimit, assignedLimit)
            If newLimit IsNot Nothing Then
                If context Is Nothing OrElse (Not containsAllPDs(newLimit, context)) Then
                    Me.limitedContext = newLimit
                    Me.permissions = permissions
                    Me.parent = parent
                    Me.isLimited = True
                End If
            End If
        End Sub

        ''' <summary>
        ''' Checks two AccessControlContext objects for equality.
        ''' Checks that <i>obj</i> is
        ''' an AccessControlContext and has the same set of ProtectionDomains
        ''' as this context.
        ''' <P> </summary>
        ''' <param name="obj"> the object we are testing for equality with this object. </param>
        ''' <returns> true if <i>obj</i> is an AccessControlContext, and has the
        ''' same set of ProtectionDomains as this context, false otherwise. </returns>
        Public Function Equals(obj As Object) As Boolean
            If obj Is Me Then Return True

            If Not (TypeOf obj Is AccessControlContext) Then Return False

            Dim that As AccessControlContext = CType(obj, AccessControlContext)

            If Not equalContext(that) Then Return False

            If Not equalLimitedContext(that) Then Return False

            Return True
        End Function
        '    
        '     * Compare for equality based on state that is free of limited
        '     * privilege complications.
        '     
        Private Function equalContext(that As AccessControlContext) As Boolean
            If Not equalPDs(Me.context, that.context) Then Return False

            If Me.combiner Is Nothing AndAlso that.combiner IsNot Nothing Then Return False

            If Me.combiner IsNot Nothing AndAlso (Not Me.combiner.Equals(that.combiner)) Then Return False

            Return True
        End Function
        Private Boolean equalPDs(ProtectionDomain() a, ProtectionDomain() b)
			If a Is Nothing Then Return (b Is Nothing)

			If b Is Nothing Then Return False

			If Not(containsAllPDs(a, b) AndAlso containsAllPDs(b, a)) Then Return False

			Return True

        '    
        '     * Compare for equality based on state that is captured during a
        '     * call to AccessController.getContext() when a limited privilege
        '     * scope is in effect.
        '     
        Private Function equalLimitedContext(AccessControlContext that) As Boolean
            If that Is Nothing Then Return False

            '        
            '         * If neither instance has limited privilege scope then we're done.
            '         
            If (Not Me.isLimited) AndAlso (Not that.isLimited) Then Return True

            '        
            '         * If only one instance has limited privilege scope then we're done.
            '         
            If Not (Me.isLimited AndAlso that.isLimited) Then Return False

            '        
            '         * Wrapped instances should never escape outside the implementation
            '         * this class and AccessController so this will probably never happen
            '         * but it only makes any sense to compare if they both have the same
            '         * isWrapped state.
            '         
            If (Me.isWrapped AndAlso (Not that.isWrapped)) OrElse ((Not Me.isWrapped) AndAlso that.isWrapped) Then Return False

            If Me.permissions Is Nothing AndAlso that.permissions IsNot Nothing Then Return False

            If Me.permissions IsNot Nothing AndAlso that.permissions Is Nothing Then Return False

            If Not (Me.containsAllLimits(that) AndAlso that.containsAllLimits(Me)) Then Return False

            '        
            '         * Skip through any wrapped contexts.
            '         
            Dim thisNextPC As AccessControlContext = getNextPC(Me)
            Dim thatNextPC As AccessControlContext = getNextPC(that)

            '        
            '         * The protection domains and combiner of a privilegedContext are
            '         * not relevant because they have already been included in the context
            '         * of this instance by optimize() so we only care about any limited
            '         * privilege state they may have.
            '         
            If thisNextPC Is Nothing AndAlso thatNextPC IsNot Nothing AndAlso thatNextPC.isLimited Then Return False

            If thisNextPC IsNot Nothing AndAlso (Not thisNextPC.equalLimitedContext(thatNextPC)) Then Return False

            If Me.parent Is Nothing AndAlso that.parent IsNot Nothing Then Return False

            If Me.parent IsNot Nothing AndAlso (Not Me.parent.Equals(that.parent)) Then Return False

            Return True
        End Function
        '    
        '     * Follow the privilegedContext link making our best effort to skip
        '     * through any wrapper contexts.
        '     
        Private Shared Function getNextPC(acc As AccessControlContext) As AccessControlContext
            Do While acc IsNot Nothing AndAlso acc.privilegedContext IsNot Nothing
                acc = acc.privilegedContext
                If Not acc.isWrapped Then Return acc
            Loop
            Return Nothing
        End Function
        Private Shared Function containsAllPDs(ProtectionDomain() thisContext, ProtectionDomain() thatContext) As Boolean
            Dim match As Boolean = False

            '
            ' ProtectionDomains within an ACC currently cannot be null
            ' and this is enforced by the constructor and the various
            ' optimize methods. However, historically this logic made attempts
            ' to support the notion of a null PD and therefore this logic continues
            ' to support that notion.
            Dim thisPd As ProtectionDomain
            For i As Integer = 0 To thisContext.length - 1
                match = False
                thisPd = thisContext(i)
                If thisPd Is Nothing Then
                    Dim j As Integer = 0
                    Do While (j < thatContext.length) AndAlso Not match
                        match = (thatContext(j) Is Nothing)
                        j += 1
                    Loop
                Else
                    Dim thisPdClass As [Class] = thisPd.GetType()
                    Dim thatPd As ProtectionDomain
                    Dim j As Integer = 0
                    Do While (j < thatContext.length) AndAlso Not match
                        thatPd = thatContext(j)

                        ' Class check required to avoid PD exposure (4285406)
                        match = (thatPd IsNot Nothing AndAlso thisPdClass Is thatPd.GetType() AndAlso thisPd.Equals(thatPd))
                        j += 1
                    Loop
                End If
                If Not match Then Return False
            Next i
            Return match
        End Function

        Private Function containsAllLimits(that As AccessControlContext) As Boolean
            Dim match As Boolean = False
			Dim thisPerm As Permission

			If Me.permissions Is Nothing AndAlso that.permissions Is Nothing Then Return True

			For i As Integer = 0 To Me.permissions.Length - 1
				Dim limit As Permission = Me.permissions(i)
				Class limitClass = limit.GetType()
				match = False
				Dim j As Integer = 0
				Do While (j < that.permissions.length) AndAlso Not match
					Dim perm As Permission = that.permissions(j)
					match = (limitClass.Equals(perm.GetType()) AndAlso limit.Equals(perm))
					j += 1
				Loop
				If Not match Then Return False
			Next i
			Return match
            End Function

            ''' <summary>
            ''' Returns the hash code value for this context. The hash code
            ''' is computed by exclusive or-ing the hash code of all the protection
            ''' domains in the context together.
            ''' </summary>
            ''' <returns> a hash code value for this context. </returns>

            Public Function GetHashCode() As Integer
                Dim hashCode As Integer = 0

                If context Is Nothing Then Return hashCode

                For i As Integer = 0 To context.Length - 1
                    If context(i) IsNot Nothing Then hashCode = hashCode Xor context(i).GetHashCode()
                Next i

                Return hashCode
            End Function
        End Class


	Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
		Implements PrivilegedAction(Of T)

            Public Overridable Sub run()
                db.println("domain that failed " & pd)
            End Sub
        End Class
End Namespace