'
' * Copyright (c) 1995, 1997, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.applet

	''' <summary>
	''' The <code>AudioClip</code> interface is a simple abstraction for
	''' playing a sound clip. Multiple <code>AudioClip</code> items can be
	''' playing at the same time, and the resulting sound is mixed
	''' together to produce a composite.
	''' 
	''' @author      Arthur van Hoff
	''' @since       JDK1.0
	''' </summary>
	Public Interface AudioClip
		''' <summary>
		''' Starts playing this audio clip. Each time this method is called,
		''' the clip is restarted from the beginning.
		''' </summary>
		Sub play()

		''' <summary>
		''' Starts playing this audio clip in a loop.
		''' </summary>
		Sub [loop]()

		''' <summary>
		''' Stops playing this audio clip.
		''' </summary>
		Sub [stop]()
	End Interface

End Namespace