Imports System.Runtime.CompilerServices

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
' *
' 

Namespace java.util


	''' <summary>
	''' <code>ListResourceBundle</code> is an abstract subclass of
	''' <code>ResourceBundle</code> that manages resources for a locale
	''' in a convenient and easy to use list. See <code>ResourceBundle</code> for
	''' more information about resource bundles in general.
	''' 
	''' <P>
	''' Subclasses must override <code>getContents</code> and provide an array,
	''' where each item in the array is a pair of objects.
	''' The first element of each pair is the key, which must be a
	''' <code>String</code>, and the second element is the value associated with
	''' that key.
	''' 
	''' <p>
	''' The following <a name="sample">example</a> shows two members of a resource
	''' bundle family with the base name "MyResources".
	''' "MyResources" is the default member of the bundle family, and
	''' "MyResources_fr" is the French member.
	''' These members are based on <code>ListResourceBundle</code>
	''' (a related <a href="PropertyResourceBundle.html#sample">example</a> shows
	''' how you can add a bundle to this family that's based on a properties file).
	''' The keys in this example are of the form "s1" etc. The actual
	''' keys are entirely up to your choice, so long as they are the same as
	''' the keys you use in your program to retrieve the objects from the bundle.
	''' Keys are case-sensitive.
	''' <blockquote>
	''' <pre>
	''' 
	''' public class MyResources extends ListResourceBundle {
	'''     protected Object[][] getContents() {
	'''         return new Object[][] {
	'''         // LOCALIZE THIS
	'''             {"s1", "The disk \"{1}\" contains {0}."},  // MessageFormat pattern
	'''             {"s2", "1"},                               // location of {0} in pattern
	'''             {"s3", "My Disk"},                         // sample disk name
	'''             {"s4", "no files"},                        // first ChoiceFormat choice
	'''             {"s5", "one file"},                        // second ChoiceFormat choice
	'''             {"s6", "{0,number} files"},                // third ChoiceFormat choice
	'''             {"s7", "3 Mar 96"},                        // sample date
	'''             {"s8", new Dimension(1,5)}                 // real object, not just string
	'''         // END OF MATERIAL TO LOCALIZE
	'''         };
	'''     }
	''' }
	''' 
	''' public class MyResources_fr extends ListResourceBundle {
	'''     protected Object[][] getContents() {
	'''         return new Object[][] {
	'''         // LOCALIZE THIS
	'''             {"s1", "Le disque \"{1}\" {0}."},          // MessageFormat pattern
	'''             {"s2", "1"},                               // location of {0} in pattern
	'''             {"s3", "Mon disque"},                      // sample disk name
	'''             {"s4", "ne contient pas de fichiers"},     // first ChoiceFormat choice
	'''             {"s5", "contient un fichier"},             // second ChoiceFormat choice
	'''             {"s6", "contient {0,number} fichiers"},    // third ChoiceFormat choice
	'''             {"s7", "3 mars 1996"},                     // sample date
	'''             {"s8", new Dimension(1,3)}                 // real object, not just string
	'''         // END OF MATERIAL TO LOCALIZE
	'''         };
	'''     }
	''' }
	''' </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' The implementation of a {@code ListResourceBundle} subclass must be thread-safe
	''' if it's simultaneously used by multiple threads. The default implementations
	''' of the methods in this class are thread-safe.
	''' </summary>
	''' <seealso cref= ResourceBundle </seealso>
	''' <seealso cref= PropertyResourceBundle
	''' @since JDK1.1 </seealso>
	Public MustInherit Class ListResourceBundle
		Inherits ResourceBundle

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Public Sub New()
		End Sub

		' Implements java.util.ResourceBundle.handleGetObject; inherits javadoc specification.
		Public NotOverridable Overrides Function handleGetObject(ByVal key As String) As Object
			' lazily load the lookup hashtable.
			If lookup Is Nothing Then loadLookup()
			If key Is Nothing Then Throw New NullPointerException
			Return lookup.get(key) ' this class ignores locales
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
				' lazily load the lookup hashtable.
				If lookup Is Nothing Then loadLookup()
    
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
			If lookup Is Nothing Then loadLookup()
			Return lookup.Keys
		End Function

		''' <summary>
		''' Returns an array in which each item is a pair of objects in an
		''' <code>Object</code> array. The first element of each pair is
		''' the key, which must be a <code>String</code>, and the second
		''' element is the value associated with that key.  See the class
		''' description for details.
		''' </summary>
		''' <returns> an array of an <code>Object</code> array representing a
		''' key-value pair. </returns>
		Protected Friend MustOverride ReadOnly Property contents As Object()()

		' ==================privates====================

		''' <summary>
		''' We lazily load the lookup hashtable.  This function does the
		''' loading.
		''' </summary>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Private Sub loadLookup()
			If lookup IsNot Nothing Then Return

			Dim contents_Renamed As Object()() = contents
			Dim temp As New HashMap(Of String, Object)(contents_Renamed.Length)
			For i As Integer = 0 To contents_Renamed.Length - 1
				' key must be non-null String, value must be non-null
				Dim key As String = CStr(contents_Renamed(i)(0))
				Dim value As Object = contents_Renamed(i)(1)
				If key Is Nothing OrElse value Is Nothing Then Throw New NullPointerException
				temp.put(key, value)
			Next i
			lookup = temp
		End Sub

		Private lookup As Map(Of String, Object) = Nothing
	End Class

End Namespace