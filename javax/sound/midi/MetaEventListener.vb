'
' * Copyright (c) 1999, 2002, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sound.midi



	''' <summary>
	''' The <code>MetaEventListener</code> interface should be implemented
	''' by classes whose instances need to be notified when a <code><seealso cref="Sequencer"/></code>
	''' has processed a <code><seealso cref="MetaMessage"/></code>.
	''' To register a <code>MetaEventListener</code> object to receive such
	''' notifications, pass it as the argument to the
	''' <code><seealso cref="Sequencer#addMetaEventListener(MetaEventListener) addMetaEventListener"/></code>
	''' method of <code>Sequencer</code>.
	''' 
	''' @author Kara Kytle
	''' </summary>
	Public Interface MetaEventListener
		Inherits java.util.EventListener

		''' <summary>
		''' Invoked when a <code><seealso cref="Sequencer"/></code> has encountered and processed
		''' a <code>MetaMessage</code> in the <code><seealso cref="Sequence"/></code> it is processing. </summary>
		''' <param name="meta"> the meta-message that the sequencer encountered </param>
		Sub meta(ByVal meta As MetaMessage)
	End Interface

End Namespace