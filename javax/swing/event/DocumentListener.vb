'
' * Copyright (c) 1997, 1998, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.event


	''' <summary>
	''' Interface for an observer to register to receive notifications
	''' of changes to a text document.
	''' <p>
	''' The default implementation of
	''' the Document interface (AbstractDocument) supports asynchronous
	''' mutations.  If this feature is used (i.e. mutations are made
	''' from a thread other than the Swing event thread), the listeners
	''' will be notified via the mutating thread.  <em>This means that
	''' if asynchronous updates are made, the implementation of this
	''' interface must be threadsafe</em>!
	''' <p>
	''' The DocumentEvent notification is based upon the JavaBeans
	''' event model.  There is no guarantee about the order of delivery
	''' to listeners, and all listeners must be notified prior to making
	''' further mutations to the Document.  <em>This means implementations
	''' of the DocumentListener may not mutate the source of the event
	''' (i.e. the associated Document)</em>.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref= javax.swing.text.Document </seealso>
	''' <seealso cref= javax.swing.text.StyledDocument </seealso>
	''' <seealso cref= DocumentEvent </seealso>
	Public Interface DocumentListener
		Inherits java.util.EventListener

		''' <summary>
		''' Gives notification that there was an insert into the document.  The
		''' range given by the DocumentEvent bounds the freshly inserted region.
		''' </summary>
		''' <param name="e"> the document event </param>
		Sub insertUpdate(ByVal e As DocumentEvent)

		''' <summary>
		''' Gives notification that a portion of the document has been
		''' removed.  The range is given in terms of what the view last
		''' saw (that is, before updating sticky positions).
		''' </summary>
		''' <param name="e"> the document event </param>
		Sub removeUpdate(ByVal e As DocumentEvent)

		''' <summary>
		''' Gives notification that an attribute or set of attributes changed.
		''' </summary>
		''' <param name="e"> the document event </param>
		Sub changedUpdate(ByVal e As DocumentEvent)
	End Interface

End Namespace