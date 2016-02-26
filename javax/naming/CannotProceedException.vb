Imports System.Collections.Generic

'
' * Copyright (c) 1999, 2004, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.naming


	''' <summary>
	''' This exception is thrown to indicate that the operation reached
	''' a point in the name where the operation cannot proceed any further.
	''' When performing an operation on a composite name, a naming service
	''' provider may reach a part of the name that does not belong to its
	''' namespace.  At that point, it can construct a
	''' CannotProceedException and then invoke methods provided by
	''' javax.naming.spi.NamingManager (such as getContinuationContext())
	''' to locate another provider to continue the operation.  If this is
	''' not possible, this exception is raised to the caller of the
	''' context operation.
	''' <p>
	''' If the program wants to handle this exception in particular, it
	''' should catch CannotProceedException explicitly before attempting to
	''' catch NamingException.
	''' <p>
	''' A CannotProceedException instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify
	''' CannotProceedException should lock the object.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' @since 1.3
	''' </summary>

	'
	'  * The serialized form of a CannotProceedException object consists of
	'  * the serialized fields of its NamingException superclass, the remaining new
	'  * name (a Name object), the environment (a Hashtable), the altName field
	'  * (a Name object), and the serialized form of the altNameCtx field.
	'  


	Public Class CannotProceedException
		Inherits NamingException

		''' <summary>
		''' Contains the remaining unresolved part of the second
		''' "name" argument to Context.rename().
		''' This information necessary for
		''' continuing the Context.rename() operation.
		''' <p>
		''' This field is initialized to null.
		''' It should not be manipulated directly:  it should
		''' be accessed and updated using getRemainingName() and setRemainingName().
		''' @serial
		''' </summary>
		''' <seealso cref= #getRemainingNewName </seealso>
		''' <seealso cref= #setRemainingNewName </seealso>
		Protected Friend remainingNewName As Name = Nothing

		''' <summary>
		''' Contains the environment
		''' relevant for the Context or DirContext method that cannot proceed.
		''' <p>
		''' This field is initialized to null.
		''' It should not be manipulated directly:  it should be accessed
		''' and updated using getEnvironment() and setEnvironment().
		''' @serial
		''' </summary>
		''' <seealso cref= #getEnvironment </seealso>
		''' <seealso cref= #setEnvironment </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Protected Friend environment As Dictionary(Of ?, ?) = Nothing

		''' <summary>
		''' Contains the name of the resolved object, relative
		''' to the context <code>altNameCtx</code>.  It is a composite name.
		''' If null, then no name is specified.
		''' See the <code>javax.naming.spi.ObjectFactory.getObjectInstance</code>
		''' method for details on how this is used.
		''' <p>
		''' This field is initialized to null.
		''' It should not be manipulated directly:  it should
		''' be accessed and updated using getAltName() and setAltName().
		''' @serial
		''' </summary>
		''' <seealso cref= #getAltName </seealso>
		''' <seealso cref= #setAltName </seealso>
		''' <seealso cref= #altNameCtx </seealso>
		''' <seealso cref= javax.naming.spi.ObjectFactory#getObjectInstance </seealso>
		Protected Friend altName As Name = Nothing

		''' <summary>
		''' Contains the context relative to which
		''' <code>altName</code> is specified.  If null, then the default initial
		''' context is implied.
		''' See the <code>javax.naming.spi.ObjectFactory.getObjectInstance</code>
		''' method for details on how this is used.
		''' <p>
		''' This field is initialized to null.
		''' It should not be manipulated directly:  it should
		''' be accessed and updated using getAltNameCtx() and setAltNameCtx().
		''' @serial
		''' </summary>
		''' <seealso cref= #getAltNameCtx </seealso>
		''' <seealso cref= #setAltNameCtx </seealso>
		''' <seealso cref= #altName </seealso>
		''' <seealso cref= javax.naming.spi.ObjectFactory#getObjectInstance </seealso>
		Protected Friend altNameCtx As Context = Nothing

		''' <summary>
		''' Constructs a new instance of CannotProceedException using an
		''' explanation. All unspecified fields default to null.
		''' </summary>
		''' <param name="explanation">     A possibly null string containing additional
		'''                          detail about this exception.
		'''   If null, this exception has no detail message. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
		End Sub

		''' <summary>
		''' Constructs a new instance of CannotProceedException.
		''' All fields default to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Retrieves the environment that was in effect when this exception
		''' was created. </summary>
		''' <returns> Possibly null environment property set.
		'''          null means no environment was recorded for this exception. </returns>
		''' <seealso cref= #setEnvironment </seealso>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property environment As Dictionary(Of ?, ?)
			Get
				Return environment
			End Get
			Set(ByVal environment As Dictionary(Of T1))
				Me.environment = environment ' %%% clone it??
			End Set
		End Property


		''' <summary>
		''' Retrieves the "remaining new name" field of this exception, which is
		''' used when this exception is thrown during a rename() operation.
		''' </summary>
		''' <returns> The possibly null part of the new name that has not been resolved.
		'''          It is a composite name. It can be null, which means
		'''          the remaining new name field has not been set.
		''' </returns>
		''' <seealso cref= #setRemainingNewName </seealso>
		Public Overridable Property remainingNewName As Name
			Get
				Return remainingNewName
			End Get
			Set(ByVal newName As Name)
				If newName IsNot Nothing Then
					Me.remainingNewName = CType(newName.clone(), Name)
				Else
					Me.remainingNewName = Nothing
				End If
			End Set
		End Property


		''' <summary>
		''' Retrieves the <code>altName</code> field of this exception.
		''' This is the name of the resolved object, relative to the context
		''' <code>altNameCtx</code>. It will be used during a subsequent call to the
		''' <code>javax.naming.spi.ObjectFactory.getObjectInstance</code> method.
		''' </summary>
		''' <returns> The name of the resolved object, relative to
		'''          <code>altNameCtx</code>.
		'''          It is a composite name.  If null, then no name is specified.
		''' </returns>
		''' <seealso cref= #setAltName </seealso>
		''' <seealso cref= #getAltNameCtx </seealso>
		''' <seealso cref= javax.naming.spi.ObjectFactory#getObjectInstance </seealso>
		Public Overridable Property altName As Name
			Get
				Return altName
			End Get
			Set(ByVal altName As Name)
				Me.altName = altName
			End Set
		End Property


		''' <summary>
		''' Retrieves the <code>altNameCtx</code> field of this exception.
		''' This is the context relative to which <code>altName</code> is named.
		''' It will be used during a subsequent call to the
		''' <code>javax.naming.spi.ObjectFactory.getObjectInstance</code> method.
		''' </summary>
		''' <returns>  The context relative to which <code>altName</code> is named.
		'''          If null, then the default initial context is implied.
		''' </returns>
		''' <seealso cref= #setAltNameCtx </seealso>
		''' <seealso cref= #getAltName </seealso>
		''' <seealso cref= javax.naming.spi.ObjectFactory#getObjectInstance </seealso>
		Public Overridable Property altNameCtx As Context
			Get
				Return altNameCtx
			End Get
			Set(ByVal altNameCtx As Context)
				Me.altNameCtx = altNameCtx
			End Set
		End Property



		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = 1219724816191576813L
	End Class

End Namespace