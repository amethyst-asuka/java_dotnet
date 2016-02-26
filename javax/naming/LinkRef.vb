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
	''' This class represents a Reference whose contents is a name, called the link name,
	''' that is bound to an atomic name in a context.
	''' <p>
	''' The name is a URL, or a name to be resolved relative to the initial
	''' context, or if the first character of the name is ".", the name
	''' is relative to the context in which the link is bound.
	''' <p>
	''' Normal resolution of names in context operations always follow links.
	''' Resolution of the link name itself may cause resolution to pass through
	''' other  links. This gives rise to the possibility of a cycle of links whose
	''' resolution could not terminate normally. As a simple means to avoid such
	''' non-terminating resolutions, service providers may define limits on the
	''' number of links that may be involved in any single operation invoked
	''' by the caller.
	''' <p>
	''' A LinkRef contains a single StringRefAddr, whose type is "LinkAddress",
	''' and whose contents is the link name. The class name field of the
	''' Reference is that of this (LinkRef) class.
	''' <p>
	''' LinkRef is bound to a name using the normal Context.bind()/rebind(), and
	''' DirContext.bind()/rebind(). Context.lookupLink() is used to retrieve the link
	''' itself if the terminal atomic name is bound to a link.
	''' <p>
	''' Many naming systems support a native notion of link that may be used
	''' within the naming system itself. JNDI does not specify whether
	''' there is any relationship between such native links and JNDI links.
	''' <p>
	''' A LinkRef instance is not synchronized against concurrent access by multiple
	''' threads. Threads that need to access a LinkRef instance concurrently should
	''' synchronize amongst themselves and provide the necessary locking.
	'''  
	''' @author Rosanna Lee
	''' @author Scott Seligman
	''' </summary>
	''' <seealso cref= LinkException </seealso>
	''' <seealso cref= LinkLoopException </seealso>
	''' <seealso cref= MalformedLinkException </seealso>
	''' <seealso cref= Context#lookupLink
	''' @since 1.3 </seealso>

	'  <p>
	'  * The serialized form of a LinkRef object consists of the serialized
	'  * fields of its Reference superclass.
	'  

	Public Class LinkRef
		Inherits Reference

		' code for link handling 
		Friend Shared ReadOnly linkClassName As String = GetType(LinkRef).name
		Friend Const linkAddrType As String = "LinkAddress"

		''' <summary>
		''' Constructs a LinkRef for a name. </summary>
		''' <param name="linkName"> The non-null name for which to create this link. </param>
		Public Sub New(ByVal linkName As Name)
			MyBase.New(linkClassName, New StringRefAddr(linkAddrType, linkName.ToString()))
		End Sub

		''' <summary>
		''' Constructs a LinkRef for a string name. </summary>
		''' <param name="linkName"> The non-null name for which to create this link. </param>
		Public Sub New(ByVal linkName As String)
			MyBase.New(linkClassName, New StringRefAddr(linkAddrType, linkName))
		End Sub

		''' <summary>
		''' Retrieves the name of this link.
		''' </summary>
		''' <returns> The non-null name of this link. </returns>
		''' <exception cref="MalformedLinkException"> If a link name could not be extracted </exception>
		''' <exception cref="NamingException"> If a naming exception was encountered. </exception>
	ReadOnly	Public Overridable Property linkName As String
			Get
				If className IsNot Nothing AndAlso className.Equals(linkClassName) Then
					Dim addr As RefAddr = [get](linkAddrType)
					If addr IsNot Nothing AndAlso TypeOf addr Is StringRefAddr Then Return CStr(CType(addr, StringRefAddr).content)
				End If
				Throw New MalformedLinkException
			End Get
		End Property
		''' <summary>
		''' Use serialVersionUID from JNDI 1.1.1 for interoperability
		''' </summary>
		Private Const serialVersionUID As Long = -5386290613498931298L
	End Class

End Namespace