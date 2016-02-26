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
	''' This exception is used to describe problems encounter while resolving links.
	''' Addition information is added to the base NamingException for pinpointing
	''' the problem with the link.
	''' <p>
	''' Analogous to how NamingException captures name resolution information,
	''' LinkException captures "link"-name resolution information pinpointing
	''' the problem encountered while resolving a link. All these fields may
	''' be null.
	''' <ul>
	''' <li> Link Resolved Name. Portion of link name that has been resolved.
	''' <li> Link Resolved Object. Object to which resolution of link name proceeded.
	''' <li> Link Remaining Name. Portion of link name that has not been resolved.
	''' <li> Link Explanation. Detail explaining why link resolution failed.
	''' </ul>
	''' 
	''' <p>
	''' A LinkException instance is not synchronized against concurrent
	''' multithreaded access. Multiple threads trying to access and modify
	''' a single LinkException instance should lock the object.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= Context#lookupLink </seealso>
	''' <seealso cref= LinkRef
	''' @since 1.3 </seealso>


	'  <p>
	'  * The serialized form of a LinkException object consists of the
	'  * serialized fields of its NamingException superclass, the link resolved
	'  * name (a Name object), the link resolved object, link remaining name
	'  * (a Name object), and the link explanation String.
	'


	Public Class LinkException
		Inherits NamingException

		''' <summary>
		''' Contains the part of the link that has been successfully resolved.
		''' It is a composite name and can be null.
		''' This field is initialized by the constructors.
		''' You should access and manipulate this field
		''' through its get and set methods.
		''' @serial </summary>
		''' <seealso cref= #getLinkResolvedName </seealso>
		''' <seealso cref= #setLinkResolvedName </seealso>
		Protected Friend linkResolvedName As Name

		''' <summary>
		''' Contains the object to which resolution of the part of the link was successful.
		''' Can be null. This field is initialized by the constructors.
		''' You should access and manipulate this field
		''' through its get and set methods.
		''' @serial </summary>
		''' <seealso cref= #getLinkResolvedObj </seealso>
		''' <seealso cref= #setLinkResolvedObj </seealso>
		Protected Friend linkResolvedObj As Object

		''' <summary>
		''' Contains the remaining link name that has not been resolved yet.
		''' It is a composite name and can be null.
		''' This field is initialized by the constructors.
		''' You should access and manipulate this field
		''' through its get and set methods.
		''' @serial </summary>
		''' <seealso cref= #getLinkRemainingName </seealso>
		''' <seealso cref= #setLinkRemainingName </seealso>
		Protected Friend linkRemainingName As Name

		''' <summary>
		''' Contains the exception of why resolution of the link failed.
		''' Can be null. This field is initialized by the constructors.
		''' You should access and manipulate this field
		''' through its get and set methods.
		''' @serial </summary>
		''' <seealso cref= #getLinkExplanation </seealso>
		''' <seealso cref= #setLinkExplanation </seealso>
		Protected Friend linkExplanation As String

		''' <summary>
		''' Constructs a new instance of LinkException with an explanation
		''' All the other fields are initialized to null. </summary>
		''' <param name="explanation">     A possibly null string containing additional
		'''                         detail about this exception. </param>
		''' <seealso cref= java.lang.Throwable#getMessage </seealso>
		Public Sub New(ByVal explanation As String)
			MyBase.New(explanation)
			linkResolvedName = Nothing
			linkResolvedObj = Nothing
			linkRemainingName = Nothing
			linkExplanation = Nothing
		End Sub

		''' <summary>
		''' Constructs a new instance of LinkException.
		''' All the non-link-related and link-related fields are initialized to null.
		''' </summary>
		Public Sub New()
			MyBase.New()
			linkResolvedName = Nothing
			linkResolvedObj = Nothing
			linkRemainingName = Nothing
			linkExplanation = Nothing
		End Sub

		''' <summary>
		''' Retrieves the leading portion of the link name that was resolved
		''' successfully.
		''' </summary>
		''' <returns> The part of the link name that was resolved successfully.
		'''          It is a composite name. It can be null, which means
		'''          the link resolved name field has not been set. </returns>
		''' <seealso cref= #getLinkResolvedObj </seealso>
		''' <seealso cref= #setLinkResolvedName </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getLinkResolvedName() As Name 'JavaToDotNetTempPropertyGetlinkResolvedName
		Public Overridable Property linkResolvedName As Name
			Get
				Return Me.linkResolvedName
			End Get
			Set(ByVal name As Name)
		End Property

		''' <summary>
		''' Retrieves the remaining unresolved portion of the link name. </summary>
		''' <returns> The part of the link name that has not been resolved.
		'''          It is a composite name. It can be null, which means
		'''          the link remaining name field has not been set. </returns>
		''' <seealso cref= #setLinkRemainingName </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getLinkRemainingName() As Name 'JavaToDotNetTempPropertyGetlinkRemainingName
		Public Overridable Property linkRemainingName As Name
			Get
				Return Me.linkRemainingName
			End Get
			Set(ByVal name As Name)
		End Property

		''' <summary>
		''' Retrieves the object to which resolution was successful.
		''' This is the object to which the resolved link name is bound.
		''' </summary>
		''' <returns> The possibly null object that was resolved so far.
		''' If null, it means the link resolved object field has not been set. </returns>
		''' <seealso cref= #getLinkResolvedName </seealso>
		''' <seealso cref= #setLinkResolvedObj </seealso>
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Overridable Function getLinkResolvedObj() As Object 'JavaToDotNetTempPropertyGetlinkResolvedObj
		Public Overridable Property linkResolvedObj As Object
			Get
				Return Me.linkResolvedObj
			End Get
			Set(ByVal obj As Object)
		End Property

		''' <summary>
		''' Retrieves the explanation associated with the problem encounter
		''' when resolving a link.
		''' </summary>
		''' <returns> The possibly null detail string explaining more about the problem
		''' with resolving a link.
		'''         If null, it means there is no
		'''         link detail message for this exception. </returns>
		''' <seealso cref= #setLinkExplanation </seealso>
		Public Overridable Property linkExplanation As String
			Get
				Return Me.linkExplanation
			End Get
			Set(ByVal msg As String)
				Me.linkExplanation = msg
			End Set
		End Property


			If name IsNot Nothing Then
				Me.linkResolvedName = CType(name.clone(), Name)
			Else
				Me.linkResolvedName = Nothing
			End If
		End Sub

			If name IsNot Nothing Then
				Me.linkRemainingName = CType(name.clone(), Name)
			Else
				Me.linkRemainingName = Nothing
			End If
		End Sub

			Me.linkResolvedObj = obj
		End Sub

		''' <summary>
		''' Generates the string representation of this exception.
		''' This string consists of the NamingException information plus
		''' the link's remaining name.
		''' This string is used for debugging and not meant to be interpreted
		''' programmatically. </summary>
		''' <returns> The non-null string representation of this link exception. </returns>
		Public Overrides Function ToString() As String
			Return MyBase.ToString() & "; Link Remaining Name: '" & Me.linkRemainingName & "'"
		End Function

		''' <summary>
		''' Generates the string representation of this exception.
		''' This string consists of the NamingException information plus
		''' the additional information of resolving the link.
		''' If 'detail' is true, the string also contains information on
		''' the link resolved object. If false, this method is the same
		''' as the form of toString() that accepts no parameters.
		''' This string is used for debugging and not meant to be interpreted
		''' programmatically.
		''' </summary>
		''' <param name="detail">  If true, add information about the link resolved
		'''                  object. </param>
		''' <returns> The non-null string representation of this link exception. </returns>
		Public Overrides Function ToString(ByVal detail As Boolean) As String
			If (Not detail) OrElse Me.linkResolvedObj Is Nothing Then Return Me.ToString()

			Return Me.ToString() & "; Link Resolved Object: " & Me.linkResolvedObj
		End Function

		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -7967662604076777712L
	End Class

End Namespace