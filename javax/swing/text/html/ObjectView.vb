Imports System
Imports System.Threading
Imports javax.swing
Imports javax.swing.text

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
Namespace javax.swing.text.html



	''' <summary>
	''' Component decorator that implements the view interface
	''' for &lt;object&gt; elements.
	''' <p>
	''' This view will try to load the class specified by the
	''' <code>classid</code> attribute.  If possible, the Classloader
	''' used to load the associated Document is used.
	''' This would typically be the same as the ClassLoader
	''' used to load the EditorKit.  If the document's
	''' ClassLoader is null, <code>Class.forName</code> is used.
	''' <p>
	''' If the class can successfully be loaded, an attempt will
	''' be made to create an instance of it by calling
	''' <code>Class.newInstance</code>.  An attempt will be made
	''' to narrow the instance to type <code>java.awt.Component</code>
	''' to display the object.
	''' <p>
	''' This view can also manage a set of parameters with limitations.
	''' The parameters to the &lt;object&gt; element are expected to
	''' be present on the associated elements attribute set as simple
	''' strings.  Each bean property will be queried as a key on
	''' the AttributeSet, with the expectation that a non-null value
	''' (of type String) will be present if there was a parameter
	''' specification for the property.  Reflection is used to
	''' set the parameter.  Currently, this is limited to a very
	''' simple single parameter of type String.
	''' <p>
	''' A simple example HTML invocation is:
	''' <pre>
	'''      &lt;object classid="javax.swing.JLabel"&gt;
	'''      &lt;param name="text" value="sample text"&gt;
	'''      &lt;/object&gt;
	''' </pre>
	''' 
	''' @author Timothy Prinzing
	''' </summary>
	Public Class ObjectView
		Inherits ComponentView

		''' <summary>
		''' Creates a new ObjectView object.
		''' </summary>
		''' <param name="elem"> the element to decorate </param>
		Public Sub New(ByVal elem As Element)
			MyBase.New(elem)
		End Sub

		''' <summary>
		''' Create the component.  The classid is used
		''' as a specification of the classname, which
		''' we try to load.
		''' </summary>
		Protected Friend Overrides Function createComponent() As Component
			Dim attr As AttributeSet = element.attributes
			Dim classname As String = CStr(attr.getAttribute(HTML.Attribute.CLASSID))
			Try
				sun.reflect.misc.ReflectUtil.checkPackageAccess(classname)
				Dim c As Type = Type.GetType(classname, True,Thread.CurrentThread.contextClassLoader)
				Dim o As Object = c.newInstance()
				If TypeOf o Is Component Then
					Dim comp As Component = CType(o, Component)
					parametersers(comp, attr)
					Return comp
				End If
			Catch e As Exception
				' couldn't create a component... fall through to the
				' couldn't load representation.
			End Try

			Return unloadableRepresentation
		End Function

		''' <summary>
		''' Fetch a component that can be used to represent the
		''' object if it can't be created.
		''' </summary>
		Friend Overridable Property unloadableRepresentation As Component
			Get
				' PENDING(prinz) get some artwork and return something
				' interesting here.
				Dim comp As Component = New JLabel("??")
				comp.foreground = Color.red
				Return comp
			End Get
		End Property

		''' <summary>
		''' Initialize this component according the KEY/VALUEs passed in
		''' via the &lt;param&gt; elements in the corresponding
		''' &lt;object&gt; element.
		''' </summary>
		Private Sub setParameters(ByVal comp As Component, ByVal attr As AttributeSet)
			Dim k As Type = comp.GetType()
			Dim bi As BeanInfo
			Try
				bi = Introspector.getBeanInfo(k)
			Catch ex As IntrospectionException
				Console.Error.WriteLine("introspector failed, ex: " & ex)
				Return ' quit for now
			End Try
			Dim props As PropertyDescriptor() = bi.propertyDescriptors
			For i As Integer = 0 To props.Length - 1
				'      System.err.println("checking on props[i]: "+props[i].getName());
				Dim v As Object = attr.getAttribute(props(i).name)
				If TypeOf v Is String Then
					' found a property parameter
					Dim value As String = CStr(v)
					Dim writer As Method = props(i).writeMethod
					If writer Is Nothing Then Return ' for now
					Dim params As Type() = writer.parameterTypes
					If params.Length <> 1 Then Return ' for now
					Dim args As Object() = { value }
					Try
						sun.reflect.misc.MethodUtil.invoke(writer, comp, args)
					Catch ex As Exception
						Console.Error.WriteLine("Invocation failed")
						' invocation code
					End Try
				End If
			Next i
		End Sub

	End Class

End Namespace