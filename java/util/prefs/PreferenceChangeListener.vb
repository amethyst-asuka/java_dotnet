'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util.prefs

	''' <summary>
	''' A listener for receiving preference change events.
	''' 
	''' @author  Josh Bloch </summary>
	''' <seealso cref= Preferences </seealso>
	''' <seealso cref= PreferenceChangeEvent </seealso>
	''' <seealso cref= NodeChangeListener
	''' @since   1.4 </seealso>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	Public Interface PreferenceChangeListener
		Inherits java.util.EventListener

		''' <summary>
		''' This method gets called when a preference is added, removed or when
		''' its value is changed.
		''' <p> </summary>
		''' <param name="evt"> A PreferenceChangeEvent object describing the event source
		'''          and the preference that has changed. </param>
		Sub preferenceChange(  evt As PreferenceChangeEvent)
	End Interface

End Namespace