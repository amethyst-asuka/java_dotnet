'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.peer



	''' <summary>
	''' The {@code DesktopPeer} interface provides methods for the operation
	''' of open, edit, print, browse and mail with the given URL or file, by
	''' launching the associated application.
	''' <p>
	''' Each platform has an implementation class for this interface.
	''' 
	''' </summary>
	Public Interface DesktopPeer

		''' <summary>
		''' Returns whether the given action is supported on the current platform. </summary>
		''' <param name="action"> the action type to be tested if it's supported on the
		'''        current platform. </param>
		''' <returns> {@code true} if the given action is supported on
		'''         the current platform; {@code false} otherwise. </returns>
		Function isSupported(ByVal action As java.awt.Desktop.Action) As Boolean

		''' <summary>
		''' Launches the associated application to open the given file. The
		''' associated application is registered to be the default file viewer for
		''' the file type of the given file.
		''' </summary>
		''' <param name="file"> the given file. </param>
		''' <exception cref="IOException"> If the given file has no associated application,
		'''         or the associated application fails to be launched. </exception>
		Sub open(ByVal file As java.io.File)

		''' <summary>
		''' Launches the associated editor and opens the given file for editing. The
		''' associated editor is registered to be the default editor for the file
		''' type of the given file.
		''' </summary>
		''' <param name="file"> the given file. </param>
		''' <exception cref="IOException"> If the given file has no associated editor, or
		'''         the associated application fails to be launched. </exception>
		Sub edit(ByVal file As java.io.File)

		''' <summary>
		''' Prints the given file with the native desktop printing facility, using
		''' the associated application's print command.
		''' </summary>
		''' <param name="file"> the given file. </param>
		''' <exception cref="IOException"> If the given file has no associated application
		'''         that can be used to print it. </exception>
		Sub print(ByVal file As java.io.File)

		''' <summary>
		''' Launches the mail composing window of the user default mail client,
		''' filling the message fields including to, cc, etc, with the values
		''' specified by the given mailto URL.
		''' </summary>
		''' <param name="mailtoURL"> represents a mailto URL with specified values of the message.
		'''        The syntax of mailto URL is defined by
		'''        <a href="http://www.ietf.org/rfc/rfc2368.txt">RFC2368: The mailto
		'''        URL scheme</a> </param>
		''' <exception cref="IOException"> If the user default mail client is not found,
		'''         or it fails to be launched. </exception>
		Sub mail(ByVal mailtoURL As java.net.URI)

		''' <summary>
		''' Launches the user default browser to display the given URI.
		''' </summary>
		''' <param name="uri"> the given URI. </param>
		''' <exception cref="IOException"> If the user default browser is not found,
		'''         or it fails to be launched. </exception>
		Sub browse(ByVal uri As java.net.URI)
	End Interface

End Namespace