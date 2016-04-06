'
' * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans

	''' <summary>
	''' The PropertyEditorManager can be used to locate a property editor for
	''' any given type name.  This property editor must support the
	''' java.beans.PropertyEditor interface for editing a given object.
	''' <P>
	''' The PropertyEditorManager uses three techniques for locating an editor
	''' for a given type.  First, it provides a registerEditor method to allow
	''' an editor to be specifically registered for a given type.  Second it
	''' tries to locate a suitable class by adding "Editor" to the full
	''' qualified classname of the given type (e.g. "foo.bah.FozEditor").
	''' Finally it takes the simple classname (without the package name) adds
	''' "Editor" to it and looks in a search-path of packages for a matching
	''' class.
	''' <P>
	''' So for an input class foo.bah.Fred, the PropertyEditorManager would
	''' first look in its tables to see if an editor had been registered for
	''' foo.bah.Fred and if so use that.  Then it will look for a
	''' foo.bah.FredEditor class.  Then it will look for (say)
	''' standardEditorsPackage.FredEditor class.
	''' <p>
	''' Default PropertyEditors will be provided for the Java primitive types
	''' "boolean", "byte", "short", "int", "long", "float", and "double"; and
	''' for the classes java.lang.String. java.awt.Color, and java.awt.Font.
	''' </summary>

	Public Class PropertyEditorManager

		''' <summary>
		''' Registers an editor class to edit values of the given target class.
		''' If the editor class is {@code null},
		''' then any existing definition will be removed.
		''' Thus this method can be used to cancel the registration.
		''' The registration is canceled automatically
		''' if either the target or editor class is unloaded.
		''' <p>
		''' If there is a security manager, its {@code checkPropertiesAccess}
		''' method is called. This could result in a <seealso cref="SecurityException"/>.
		''' </summary>
		''' <param name="targetType">   the class object of the type to be edited </param>
		''' <param name="editorClass">  the class object of the editor class </param>
		''' <exception cref="SecurityException">  if a security manager exists and
		'''                            its {@code checkPropertiesAccess} method
		'''                            doesn't allow setting of system properties
		''' </exception>
		''' <seealso cref= SecurityManager#checkPropertiesAccess </seealso>
		Public Shared Sub registerEditor(  targetType As [Class],   editorClass As [Class])
			Dim sm As SecurityManager = System.securityManager
			If sm IsNot Nothing Then sm.checkPropertiesAccess()
			ThreadGroupContext.context.propertyEditorFinder.register(targetType, editorClass)
		End Sub

		''' <summary>
		''' Locate a value editor for a given target type.
		''' </summary>
		''' <param name="targetType">  The Class object for the type to be edited </param>
		''' <returns> An editor object for the given target class.
		''' The result is null if no suitable editor can be found. </returns>
		Public Shared Function findEditor(  targetType As [Class]) As PropertyEditor
			Return ThreadGroupContext.context.propertyEditorFinder.find(targetType)
		End Function

		''' <summary>
		''' Gets the package names that will be searched for property editors.
		''' </summary>
		''' <returns>  The array of package names that will be searched in
		'''          order to find property editors.
		''' <p>     The default value for this array is implementation-dependent,
		'''         e.g. Sun implementation initially sets to  {"sun.beans.editors"}. </returns>
		PublicShared ReadOnly PropertyeditorSearchPath As String()
			Get
				Return ThreadGroupContext.context.propertyEditorFinder.packages
			End Get
			Set(  path As String())
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPropertiesAccess()
				ThreadGroupContext.context.propertyEditorFinder.packages = path
			End Set
		End Property

	End Class

End Namespace