'
' * Copyright (c) 1999, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.lang



	''' <summary>
	''' Package-private utility class for setting up and tearing down
	''' platform-specific support for termination-triggered shutdowns.
	''' 
	''' @author   Mark Reinhold
	''' @since    1.3
	''' </summary>

	Friend Class Terminator

		Private Shared handler As sun.misc.SignalHandler = Nothing

	'     Invocations of setup and teardown are already synchronized
	'     * on the shutdown lock, so no further synchronization is needed here
	'     

		Friend Shared Sub setup()
			If handler IsNot Nothing Then Return
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.misc.SignalHandler sh = New sun.misc.SignalHandler()
	'		{
	'			public  Sub  handle(Signal sig)
	'			{
	'				Shutdown.exit(sig.getNumber() + &O200);
	'			}
	'		};
			handler = sh

			' When -Xrs is specified the user is responsible for
			' ensuring that shutdown hooks are run by calling
			' System.exit()
			Try
				sun.misc.Signal.handle(New sun.misc.Signal("INT"), sh)
			Catch e As IllegalArgumentException
			End Try
			Try
				sun.misc.Signal.handle(New sun.misc.Signal("TERM"), sh)
			Catch e As IllegalArgumentException
			End Try
		End Sub

		Friend Shared Sub teardown()
	'         The current sun.misc.Signal class does not support
	'         * the cancellation of handlers
	'         
		End Sub

	End Class

End Namespace