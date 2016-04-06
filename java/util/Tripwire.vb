'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.util



	''' <summary>
	''' Utility class for detecting inadvertent uses of boxing in
	''' {@code java.util} classes.  The detection is turned on or off based on
	''' whether the system property {@code org.openjdk.java.util.stream.tripwire} is
	''' considered {@code true} according to <seealso cref="Boolean#getBoolean(String)"/>.
	''' This should normally be turned off for production use.
	''' 
	''' @apiNote
	''' Typical usage would be for boxing code to do:
	''' <pre>{@code
	'''     if (Tripwire.ENABLED)
	'''         Tripwire.trip(getClass(), "{0} calling PrimitiveIterator.OfInt.nextInt()");
	''' }</pre>
	''' 
	''' @since 1.8
	''' </summary>
	Friend NotInheritable Class Tripwire
		Private Const TRIPWIRE_PROPERTY As String = "org.openjdk.java.util.stream.tripwire"

		''' <summary>
		''' Should debugging checks be enabled? </summary>
		Friend Shared ReadOnly ENABLED As Boolean = java.security.AccessController.doPrivileged(CType(, java.security.PrivilegedAction(Of Boolean?)) ->  java.lang.[Boolean].getBoolean(TRIPWIRE_PROPERTY))

		Private Sub New()
		End Sub

		''' <summary>
		''' Produces a log warning, using {@code PlatformLogger.getLogger(className)},
		''' using the supplied message.  The class name of {@code trippingClass} will
		''' be used as the first parameter to the message.
		''' </summary>
		''' <param name="trippingClass"> Name of the class generating the message </param>
		''' <param name="msg"> A message format string of the type expected by
		''' <seealso cref="PlatformLogger"/> </param>
		Friend Shared Sub trip(  trippingClass As [Class],   msg As String)
			sun.util.logging.PlatformLogger.getLogger(trippingClass.name).warning(msg, trippingClass.name)
		End Sub
	End Class

End Namespace