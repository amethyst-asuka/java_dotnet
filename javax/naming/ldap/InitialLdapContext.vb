Imports System
Imports System.Collections.Generic
Imports javax.naming
Imports javax.naming.directory

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

Namespace javax.naming.ldap


	''' <summary>
	''' This class is the starting context for performing
	''' LDAPv3-style extended operations and controls.
	''' <p>
	''' See <tt>javax.naming.InitialContext</tt> and
	''' <tt>javax.naming.InitialDirContext</tt> for details on synchronization,
	''' and the policy for how an initial context is created.
	'''  
	''' <h1>Request Controls</h1>
	''' When you create an initial context (<tt>InitialLdapContext</tt>),
	''' you can specify a list of request controls.
	''' These controls will be used as the request controls for any
	''' implicit LDAP "bind" operation performed by the context or contexts
	''' derived from the context. These are called <em>connection request controls</em>.
	''' Use <tt>getConnectControls()</tt> to get a context's connection request
	''' controls.
	''' <p>
	''' The request controls supplied to the initial context constructor
	''' are <em>not</em> used as the context request controls
	''' for subsequent context operations such as searches and lookups.
	''' Context request controls are set and updated by using
	''' <tt>setRequestControls()</tt>.
	''' <p>
	''' As shown, there can be two different sets of request controls
	''' associated with a context: connection request controls and context
	''' request controls.
	''' This is required for those applications needing to send critical
	''' controls that might not be applicable to both the context operation and
	''' any implicit LDAP "bind" operation.
	''' A typical user program would do the following:
	''' <blockquote><pre>
	''' InitialLdapContext lctx = new InitialLdapContext(env, critConnCtls);
	''' lctx.setRequestControls(critModCtls);
	''' lctx.modifyAttributes(name, mods);
	''' Controls[] respCtls =  lctx.getResponseControls();
	''' </pre></blockquote>
	''' It specifies first the critical controls for creating the initial context
	''' (<tt>critConnCtls</tt>), and then sets the context's request controls
	''' (<tt>critModCtls</tt>) for the context operation. If for some reason
	''' <tt>lctx</tt> needs to reconnect to the server, it will use
	''' <tt>critConnCtls</tt>. See the <tt>LdapContext</tt> interface for
	''' more discussion about request controls.
	''' <p>
	''' Service provider implementors should read the "Service Provider" section
	''' in the <tt>LdapContext</tt> class description for implementation details.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @author Vincent Ryan
	''' </summary>
	''' <seealso cref= LdapContext </seealso>
	''' <seealso cref= javax.naming.InitialContext </seealso>
	''' <seealso cref= javax.naming.directory.InitialDirContext </seealso>
	''' <seealso cref= javax.naming.spi.NamingManager#setInitialContextFactoryBuilder
	''' @since 1.3 </seealso>

	Public Class InitialLdapContext(Of T1)
		Inherits InitialDirContext
		Implements LdapContext

		Private Const BIND_CONTROLS_PROPERTY As String = "java.naming.ldap.control.connect"

		''' <summary>
		''' Constructs an initial context using no environment properties or
		''' connection request controls.
		''' Equivalent to <tt>new InitialLdapContext(null, null)</tt>.
		''' </summary>
		''' <exception cref="NamingException"> if a naming exception is encountered </exception>
		Public Sub New()
			MyBase.New(Nothing)
		End Sub

		''' <summary>
		''' Constructs an initial context
		''' using environment properties and connection request controls.
		''' See <tt>javax.naming.InitialContext</tt> for a discussion of
		''' environment properties.
		''' 
		''' <p> This constructor will not modify its parameters or
		''' save references to them, but may save a clone or copy.
		''' Caller should not modify mutable keys and values in
		''' <tt>environment</tt> after it has been passed to the constructor.
		''' 
		''' <p> <tt>connCtls</tt> is used as the underlying context instance's
		''' connection request controls.  See the class description
		''' for details.
		''' </summary>
		''' <param name="environment">
		'''          environment used to create the initial DirContext.
		'''          Null indicates an empty environment. </param>
		''' <param name="connCtls">
		'''          connection request controls for the initial context.
		'''          If null, no connection request controls are used.
		''' </param>
		''' <exception cref="NamingException"> if a naming exception is encountered
		''' </exception>
		''' <seealso cref= #reconnect </seealso>
		''' <seealso cref= LdapContext#reconnect </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(ByVal environment As Dictionary(Of T1), ByVal connCtls As Control())
			MyBase.New(True) ' don't initialize yet

			' Clone environment since caller owns it.
			Dim env As Dictionary(Of Object, Object) = If(environment Is Nothing, New Dictionary(Of Object, Object)(11), CType(environment.clone(), Dictionary(Of Object, Object)))

			' Put connect controls into environment.  Copy them first since
			' caller owns the array.
			If connCtls IsNot Nothing Then
				Dim copy As Control() = New Control(connCtls.Length - 1){}
				Array.Copy(connCtls, 0, copy, 0, connCtls.Length)
				env(BIND_CONTROLS_PROPERTY) = copy
			End If
			' set version to LDAPv3
			env("java.naming.ldap.version") = "3"

			' Initialize with updated environment
			init(env)
		End Sub

		''' <summary>
		''' Retrieves the initial LDAP context.
		''' </summary>
		''' <returns> The non-null cached initial context. </returns>
		''' <exception cref="NotContextException"> If the initial context is not an
		''' instance of <tt>LdapContext</tt>. </exception>
		''' <exception cref="NamingException"> If a naming exception was encountered. </exception>
	ReadOnly	Private Property defaultLdapInitCtx As LdapContext
			Get
				Dim answer As Context = defaultInitCtx
    
				If Not(TypeOf answer Is LdapContext) Then
					If answer Is Nothing Then
						Throw New NoInitialContextException
					Else
						Throw New NotContextException("Not an instance of LdapContext")
					End If
				End If
				Return CType(answer, LdapContext)
			End Get
		End Property

	' LdapContext methods
	' Most Javadoc is deferred to the LdapContext interface.

		Public Overridable Function extendedOperation(ByVal request As ExtendedRequest) As ExtendedResponse Implements LdapContext.extendedOperation
			Return defaultLdapInitCtx.extendedOperation(request)
		End Function

		Public Overridable Function newInstance(ByVal reqCtls As Control()) As LdapContext Implements LdapContext.newInstance
				Return defaultLdapInitCtx.newInstance(reqCtls)
		End Function

		Public Overridable Sub reconnect(ByVal connCtls As Control()) Implements LdapContext.reconnect
			defaultLdapInitCtx.reconnect(connCtls)
		End Sub

	ReadOnly	Public Overridable Property connectControls As Control() Implements LdapContext.getConnectControls
			Get
				Return defaultLdapInitCtx.connectControls
			End Get
		End Property

		Public Overridable Property requestControls Implements LdapContext.setRequestControls As Control()
			Set(ByVal requestControls As Control())
					defaultLdapInitCtx.requestControls = requestControls
			End Set
			Get
				Return defaultLdapInitCtx.requestControls
			End Get
		End Property


	readonly	Public Overridable Property responseControls As Control() Implements LdapContext.getResponseControls
			Get
				Return defaultLdapInitCtx.responseControls
			End Get
		End Property
	End Class

End Namespace