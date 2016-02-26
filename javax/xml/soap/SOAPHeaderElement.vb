'
' * Copyright (c) 2004, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.soap

	''' <summary>
	''' An object representing the contents in the SOAP header part of the
	''' SOAP envelope.
	''' The immediate children of a <code>SOAPHeader</code> object can
	''' be represented only as <code>SOAPHeaderElement</code> objects.
	''' <P>
	''' A <code>SOAPHeaderElement</code> object can have other
	''' <code>SOAPElement</code> objects as its children.
	''' </summary>
	Public Interface SOAPHeaderElement
		Inherits SOAPElement

		''' <summary>
		''' Sets the actor associated with this <code>SOAPHeaderElement</code>
		''' object to the specified actor. The default value of an actor is:
		'''          <code>SOAPConstants.URI_SOAP_ACTOR_NEXT</code>
		''' <P>
		''' If this <code>SOAPHeaderElement</code> supports SOAP 1.2 then this call is
		''' equivalent to <seealso cref="#setRole(String)"/>
		''' </summary>
		''' <param name="actorURI"> a <code>String</code> giving the URI of the actor
		'''           to set
		''' </param>
		''' <exception cref="IllegalArgumentException"> if there is a problem in
		''' setting the actor.
		''' </exception>
		''' <seealso cref= #getActor </seealso>
		Property actor As String

		''' <summary>
		''' Sets the <code>Role</code> associated with this <code>SOAPHeaderElement</code>
		''' object to the specified <code>Role</code>.
		''' </summary>
		''' <param name="uri"> - the URI of the <code>Role</code>
		''' </param>
		''' <exception cref="SOAPException"> if there is an error in setting the role
		''' </exception>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Fault Role.
		''' 
		''' @since SAAJ 1.3 </exception>
		Property role As String



		''' <summary>
		''' Sets the mustUnderstand attribute for this <code>SOAPHeaderElement</code>
		''' object to be either true or false.
		''' <P>
		''' If the mustUnderstand attribute is on, the actor who receives the
		''' <code>SOAPHeaderElement</code> must process it correctly. This
		''' ensures, for example, that if the <code>SOAPHeaderElement</code>
		''' object modifies the message, that the message is being modified correctly.
		''' </summary>
		''' <param name="mustUnderstand"> <code>true</code> to set the mustUnderstand
		'''        attribute to true; <code>false</code> to set it to false
		''' </param>
		''' <exception cref="IllegalArgumentException"> if there is a problem in
		''' setting the mustUnderstand attribute </exception>
		''' <seealso cref= #getMustUnderstand </seealso>
		''' <seealso cref= #setRelay </seealso>
		Property mustUnderstand As Boolean


		''' <summary>
		''' Sets the <i>relay</i> attribute for this <code>SOAPHeaderElement</code> to be
		''' either true or false.
		''' <P>
		''' The SOAP relay attribute is set to true to indicate that the SOAP header
		''' block must be relayed by any node that is targeted by the header block
		''' but not actually process it. This attribute is ignored on header blocks
		''' whose mustUnderstand attribute is set to true or that are targeted at
		''' the ultimate reciever (which is the default). The default value of this
		''' attribute is <code>false</code>.
		''' </summary>
		''' <param name="relay"> the new value of the <i>relay</i> attribute
		''' </param>
		''' <exception cref="SOAPException"> if there is a problem in setting the
		''' relay attribute. </exception>
		''' <exception cref="UnsupportedOperationException"> if this message does not
		'''      support the SOAP 1.2 concept of Relay attribute.
		''' </exception>
		''' <seealso cref= #setMustUnderstand </seealso>
		''' <seealso cref= #getRelay
		''' 
		''' @since SAAJ 1.3 </seealso>
		Property relay As Boolean

	End Interface

End Namespace