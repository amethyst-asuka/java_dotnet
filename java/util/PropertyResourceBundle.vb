'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

'
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
' *
' * The original version of this source code and documentation
' * is copyrighted and owned by Taligent, Inc., a wholly-owned
' * subsidiary of IBM. These materials are provided under terms
' * of a License Agreement between Taligent and Sun. This technology
' * is protected by multiple US and International patents.
' *
' * This notice and attribution to Taligent may not be removed.
' * Taligent is a registered trademark of Taligent, Inc.
' 

Namespace java.util


	''' <summary>
	''' <code>PropertyResourceBundle</code> is a concrete subclass of
	''' <code>ResourceBundle</code> that manages resources for a locale
	''' using a set of static strings from a property file. See
	''' <seealso cref="ResourceBundle ResourceBundle"/> for more information about resource
	''' bundles.
	''' 
	''' <p>
	''' Unlike other types of resource bundle, you don't subclass
	''' <code>PropertyResourceBundle</code>.  Instead, you supply properties
	''' files containing the resource data.  <code>ResourceBundle.getBundle</code>
	''' will automatically look for the appropriate properties file and create a
	''' <code>PropertyResourceBundle</code> that refers to it. See
	''' <seealso cref="ResourceBundle#getBundle(java.lang.String, java.util.Locale, java.lang.ClassLoader) ResourceBundle.getBundle"/>
	''' for a complete description of the search and instantiation strategy.
	''' 
	''' <p>
	''' The following <a name="sample">example</a> shows a member of a resource
	''' bundle family with the base name "MyResources".
	''' The text defines the bundle "MyResources_de",
	''' the German member of the bundle family.
	''' This member is based on <code>PropertyResourceBundle</code>, and the text
	''' therefore is the content of the file "MyResources_de.properties"
	''' (a related <a href="ListResourceBundle.html#sample">example</a> shows
	''' how you can add bundles to this family that are implemented as subclasses
	''' of <code>ListResourceBundle</code>).
	''' The keys in this example are of the form "s1" etc. The actual
	''' keys are entirely up to your choice, so long as they are the same as
	''' the keys you use in your program to retrieve the objects from the bundle.
	''' Keys are case-sensitive.
	''' <blockquote>
	''' <pre>
	''' # MessageFormat pattern
	''' s1=Die Platte \"{1}\" enth&auml;lt {0}.
	''' 
	''' # location of {0} in pattern
	''' s2=1
	''' 
	''' # sample disk name
	''' s3=Meine Platte
	''' 
	''' # first ChoiceFormat choice
	''' s4=keine Dateien
	''' 
	''' # second ChoiceFormat choice
	''' s5=eine Datei
	''' 
	''' # third ChoiceFormat choice
	''' s6={0,number} Dateien
	''' 
	''' # sample date
	''' s7=3. M&auml;rz 1996
	''' </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' The implementation of a {@code PropertyResourceBundle} subclass must be
	''' thread-safe if it's simultaneously used by multiple threads. The default
	''' implementations of the non-abstract methods in this class are thread-safe.
	''' 
	''' <p>
	''' <strong>Note:</strong> PropertyResourceBundle can be constructed either
	''' from an InputStream or a Reader, which represents a property file.
	''' Constructing a PropertyResourceBundle instance from an InputStream requires
	''' that the input stream be encoded in ISO-8859-1.  In that case, characters
	''' that cannot be represented in ISO-8859-1 encoding must be represented by Unicode Escapes
	''' as defined in section 3.3 of
	''' <cite>The Java&trade; Language Specification</cite>
	''' whereas the other constructor which takes a Reader does not have that limitation.
	''' </summary>
	''' <seealso cref= ResourceBundle </seealso>
	''' <seealso cref= ListResourceBundle </seealso>
	''' <seealso cref= Properties
	''' @since JDK1.1 </seealso>
	Public Class PropertyResourceBundle
		Inherits ResourceBundle

		''' <summary>
		''' Creates a property resource bundle from an {@link java.io.InputStream
		''' InputStream}.  The property file read with this constructor
		''' must be encoded in ISO-8859-1.
		''' </summary>
		''' <param name="stream"> an InputStream that represents a property file
		'''        to read from. </param>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		''' <exception cref="NullPointerException"> if <code>stream</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if {@code stream} contains a
		'''     malformed Unicode escape sequence. </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(  stream As java.io.InputStream)
			Dim properties As New Properties
			properties.load(stream)
			lookup = New HashMap(properties)
		End Sub

		''' <summary>
		''' Creates a property resource bundle from a {@link java.io.Reader
		''' Reader}.  Unlike the constructor
		''' <seealso cref="#PropertyResourceBundle(java.io.InputStream) PropertyResourceBundle(InputStream)"/>,
		''' there is no limitation as to the encoding of the input property file.
		''' </summary>
		''' <param name="reader"> a Reader that represents a property file to
		'''        read from. </param>
		''' <exception cref="IOException"> if an I/O error occurs </exception>
		''' <exception cref="NullPointerException"> if <code>reader</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if a malformed Unicode escape sequence appears
		'''     from {@code reader}.
		''' @since 1.6 </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Public Sub New(  reader As java.io.Reader)
			Dim properties As New Properties
			properties.load(reader)
			lookup = New HashMap(properties)
		End Sub

		' Implements java.util.ResourceBundle.handleGetObject; inherits javadoc specification.
		Public Overrides Function handleGetObject(  key As String) As Object
			If key Is Nothing Then Throw New NullPointerException
			Return lookup.get(key)
		End Function

		''' <summary>
		''' Returns an <code>Enumeration</code> of the keys contained in
		''' this <code>ResourceBundle</code> and its parent bundles.
		''' </summary>
		''' <returns> an <code>Enumeration</code> of the keys contained in
		'''         this <code>ResourceBundle</code> and its parent bundles. </returns>
		''' <seealso cref= #keySet() </seealso>
		Public  Overrides ReadOnly Property  keys As Enumeration(Of String)
			Get
				Dim parent_Renamed As ResourceBundle = Me.parent
				Return New sun.util.ResourceBundleEnumeration(lookup.Keys,If(parent_Renamed IsNot Nothing, parent_Renamed.keys, Nothing))
			End Get
		End Property

		''' <summary>
		''' Returns a <code>Set</code> of the keys contained
		''' <em>only</em> in this <code>ResourceBundle</code>.
		''' </summary>
		''' <returns> a <code>Set</code> of the keys contained only in this
		'''         <code>ResourceBundle</code>
		''' @since 1.6 </returns>
		''' <seealso cref= #keySet() </seealso>
		Protected Friend Overrides Function handleKeySet() As [Set](Of String)
			Return lookup.Keys
		End Function

		' ==================privates====================

		Private lookup As Map(Of String, Object)
	End Class

End Namespace