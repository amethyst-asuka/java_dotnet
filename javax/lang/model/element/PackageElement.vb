Imports System.Collections.Generic

'
' * Copyright (c) 2005, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.lang.model.element


	''' <summary>
	''' Represents a package program element.  Provides access to information
	''' about the package and its members.
	''' 
	''' @author Joseph D. Darcy
	''' @author Scott Seligman
	''' @author Peter von der Ah&eacute; </summary>
	''' <seealso cref= javax.lang.model.util.Elements#getPackageOf
	''' @since 1.6 </seealso>
	Public Interface PackageElement
		Inherits Element, QualifiedNameable

		''' <summary>
		''' Returns the fully qualified name of this package.
		''' This is also known as the package's <i>canonical</i> name.
		''' </summary>
		''' <returns> the fully qualified name of this package, or an
		''' empty name if this is an unnamed package
		''' @jls 6.7 Fully Qualified Names and Canonical Names </returns>
		ReadOnly Property qualifiedName As Name

        ''' <summary>
        ''' Returns the simple name of this package.  For an unnamed
        ''' package, an empty name is returned.
        ''' </summary>
        ''' <returns> the simple name of this package or an empty name if
        ''' this is an unnamed package </returns>
        ReadOnly Property simpleName As Name

        ''' <summary>
        ''' Returns the <seealso cref="NestingKind#TOP_LEVEL top-level"/>
        ''' classes and interfaces within this package.  Note that
        ''' subpackages are <em>not</em> considered to be enclosed by a
        ''' package.
        ''' </summary>
        ''' <returns> the top-level classes and interfaces within this
        ''' package </returns>
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        ReadOnly Property enclosedElements As IList(Of ? As Element)

        ''' <summary>
        ''' Returns {@code true} is this is an unnamed package and {@code
        ''' false} otherwise.
        ''' </summary>
        ''' <returns> {@code true} is this is an unnamed package and {@code
        ''' false} otherwise
        ''' @jls 7.4.2 Unnamed Packages </returns>
        ReadOnly Property unnamed As Boolean

        ''' <summary>
        ''' Returns {@code null} since a package is not enclosed by another
        ''' element.
        ''' </summary>
        ''' <returns> {@code null} </returns>
        ReadOnly Property enclosingElement As Element
    End Interface

End Namespace