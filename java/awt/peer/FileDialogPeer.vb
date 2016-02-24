'
' * Copyright (c) 1995, 2007, Oracle and/or its affiliates. All rights reserved.
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
	''' The peer interface for <seealso cref="FileDialog"/>.
	''' 
	''' The peer interfaces are intended only for use in porting
	''' the AWT. They are not intended for use by application
	''' developers, and developers should not implement peers
	''' nor invoke any of the peer methods directly on the peer
	''' instances.
	''' </summary>
	Public Interface FileDialogPeer
		Inherits DialogPeer

		''' <summary>
		''' Sets the selected file for this file dialog.
		''' </summary>
		''' <param name="file"> the file to set as selected file, or {@code null} for
		'''        no selected file
		''' </param>
		''' <seealso cref= FileDialog#setFile(String) </seealso>
		WriteOnly Property file As String

		''' <summary>
		''' Sets the current directory for this file dialog.
		''' </summary>
		''' <param name="dir"> the directory to set
		''' </param>
		''' <seealso cref= FileDialog#setDirectory(String) </seealso>
		WriteOnly Property directory As String

		''' <summary>
		''' Sets the filename filter for filtering the displayed files.
		''' </summary>
		''' <param name="filter"> the filter to set
		''' </param>
		''' <seealso cref= FileDialog#setFilenameFilter(FilenameFilter) </seealso>
		WriteOnly Property filenameFilter As java.io.FilenameFilter
	End Interface

End Namespace