Imports System

'
' * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.xml.bind.annotation


	''' <summary>
	''' Instructs JAXB to also bind other classes when binding this class.
	''' 
	''' <p>
	''' Java makes it impractical/impossible to list all sub-classes of
	''' a given class. This often gets in a way of JAXB users, as it JAXB
	''' cannot automatically list up the classes that need to be known
	''' to <seealso cref="JAXBContext"/>.
	''' 
	''' <p>
	''' For example, with the following class definitions:
	''' 
	''' <pre>
	''' class Animal {}
	''' class Dog extends Animal {}
	''' class Cat extends Animal {}
	''' </pre>
	''' 
	''' <p>
	''' The user would be required to create <seealso cref="JAXBContext"/> as
	''' <tt>JAXBContext.newInstance(Dog.class,Cat.class)</tt>
	''' (<tt>Animal</tt> will be automatically picked up since <tt>Dog</tt>
	''' and <tt>Cat</tt> refers to it.)
	''' 
	''' <p>
	''' <seealso cref="XmlSeeAlso"/> annotation would allow you to write:
	''' <pre>
	''' &#64;XmlSeeAlso({Dog.class,Cat.class})
	''' class Animal {}
	''' class Dog extends Animal {}
	''' class Cat extends Animal {}
	''' </pre>
	''' 
	''' <p>
	''' This would allow you to do <tt>JAXBContext.newInstance(Animal.class)</tt>.
	''' By the help of this annotation, JAXB implementations will be able to
	''' correctly bind <tt>Dog</tt> and <tt>Cat</tt>.
	''' 
	''' @author Kohsuke Kawaguchi
	''' @since JAXB2.1
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := False, Inherited := False> _
	Public Class XmlSeeAlso
		Inherits System.Attribute

		Type() value()
	End Class

End Namespace