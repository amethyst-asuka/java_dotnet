'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing.filechooser


	''' <summary>
	''' <code>FileFilter</code> is an abstract class used by {@code JFileChooser}
	''' for filtering the set of files shown to the user. See
	''' {@code FileNameExtensionFilter} for an implementation that filters using
	''' the file name extension.
	''' <p>
	''' A <code>FileFilter</code>
	''' can be set on a <code>JFileChooser</code> to
	''' keep unwanted files from appearing in the directory listing.
	''' For an example implementation of a simple file filter, see
	''' <code><i>yourJDK</i>/demo/jfc/FileChooserDemo/ExampleFileFilter.java</code>.
	''' For more information and examples see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/filechooser.html">How to Use File Choosers</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' </summary>
	''' <seealso cref= FileNameExtensionFilter </seealso>
	''' <seealso cref= javax.swing.JFileChooser#setFileFilter </seealso>
	''' <seealso cref= javax.swing.JFileChooser#addChoosableFileFilter
	''' 
	''' @author Jeff Dinkins </seealso>
	Public MustInherit Class FileFilter
		''' <summary>
		''' Whether the given file is accepted by this filter.
		''' </summary>
		Public MustOverride Function accept(ByVal f As java.io.File) As Boolean

		''' <summary>
		''' The description of this filter. For example: "JPG and GIF Images" </summary>
		''' <seealso cref= FileView#getName </seealso>
		Public MustOverride ReadOnly Property description As String
	End Class

End Namespace