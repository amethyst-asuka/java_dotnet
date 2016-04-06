Imports System
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2015, Oracle and/or its affiliates. All rights reserved.
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
	''' This class provides some general purpose beans control methods.
	''' </summary>

	Public Class Beans

		''' <summary>
		''' <p>
		''' Instantiate a JavaBean.
		''' </p> </summary>
		''' <returns> a JavaBean </returns>
		''' <param name="cls">         the class-loader from which we should create
		'''                        the bean.  If this is null, then the system
		'''                        class-loader is used. </param>
		''' <param name="beanName">    the name of the bean within the class-loader.
		'''                        For example "sun.beanbox.foobah"
		''' </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized
		'''              object could not be found. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>

		Public Shared Function instantiate(  cls As  ClassLoader,   beanName As String) As Object
			Return Beans.instantiate(cls, beanName, Nothing, Nothing)
		End Function

		''' <summary>
		''' <p>
		''' Instantiate a JavaBean.
		''' </p> </summary>
		''' <returns> a JavaBean
		''' </returns>
		''' <param name="cls">         the class-loader from which we should create
		'''                        the bean.  If this is null, then the system
		'''                        class-loader is used. </param>
		''' <param name="beanName">    the name of the bean within the class-loader.
		'''                        For example "sun.beanbox.foobah" </param>
		''' <param name="beanContext"> The BeanContext in which to nest the new bean
		''' </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized
		'''              object could not be found. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>

		Public Shared Function instantiate(  cls As  ClassLoader,   beanName As String,   beanContext As java.beans.beancontext.BeanContext) As Object
			Return Beans.instantiate(cls, beanName, beanContext, Nothing)
		End Function

		''' <summary>
		''' Instantiate a bean.
		''' <p>
		''' The bean is created based on a name relative to a class-loader.
		''' This name should be a dot-separated name such as "a.b.c".
		''' <p>
		''' In Beans 1.0 the given name can indicate either a serialized object
		''' or a class.  Other mechanisms may be added in the future.  In
		''' beans 1.0 we first try to treat the beanName as a serialized object
		''' name then as a class name.
		''' <p>
		''' When using the beanName as a serialized object name we convert the
		''' given beanName to a resource pathname and add a trailing ".ser" suffix.
		''' We then try to load a serialized object from that resource.
		''' <p>
		''' For example, given a beanName of "x.y", Beans.instantiate would first
		''' try to read a serialized object from the resource "x/y.ser" and if
		''' that failed it would try to load the class "x.y" and create an
		''' instance of that class.
		''' <p>
		''' If the bean is a subtype of java.applet.Applet, then it is given
		''' some special initialization.  First, it is supplied with a default
		''' AppletStub and AppletContext.  Second, if it was instantiated from
		''' a classname the applet's "init" method is called.  (If the bean was
		''' deserialized this step is skipped.)
		''' <p>
		''' Note that for beans which are applets, it is the caller's responsiblity
		''' to call "start" on the applet.  For correct behaviour, this should be done
		''' after the applet has been added into a visible AWT container.
		''' <p>
		''' Note that applets created via beans.instantiate run in a slightly
		''' different environment than applets running inside browsers.  In
		''' particular, bean applets have no access to "parameters", so they may
		''' wish to provide property get/set methods to set parameter values.  We
		''' advise bean-applet developers to test their bean-applets against both
		''' the JDK appletviewer (for a reference browser environment) and the
		''' BDK BeanBox (for a reference bean container).
		''' </summary>
		''' <returns> a JavaBean </returns>
		''' <param name="cls">         the class-loader from which we should create
		'''                        the bean.  If this is null, then the system
		'''                        class-loader is used. </param>
		''' <param name="beanName">    the name of the bean within the class-loader.
		'''                        For example "sun.beanbox.foobah" </param>
		''' <param name="beanContext"> The BeanContext in which to nest the new bean </param>
		''' <param name="initializer"> The AppletInitializer for the new bean
		''' </param>
		''' <exception cref="ClassNotFoundException"> if the class of a serialized
		'''              object could not be found. </exception>
		''' <exception cref="IOException"> if an I/O error occurs. </exception>

		Public Shared Function instantiate(  cls As  ClassLoader,   beanName As String,   beanContext As java.beans.beancontext.BeanContext,   initializer As AppletInitializer) As Object

			Dim ins As java.io.InputStream
			Dim oins As java.io.ObjectInputStream = Nothing
			Dim result As Object = Nothing
			Dim serialized As Boolean = False
			Dim serex As java.io.IOException = Nothing

			' If the given classloader is null, we check if an
			' system classloader is available and (if so)
			' use that instead.
			' Note that calls on the system class loader will
			' look in the bootstrap class loader first.
			If cls Is Nothing Then
				Try
					cls = ClassLoader.systemClassLoader
				Catch ex As SecurityException
					' We're not allowed to access the system class loader.
					' Drop through.
				End Try
			End If

			' Try to find a serialized object with this name
			Dim serName As String = beanName.replace("."c,"/"c) & ".ser"
			If cls Is Nothing Then
				ins = ClassLoader.getSystemResourceAsStream(serName)
			Else
				ins = cls.getResourceAsStream(serName)
			End If
			If ins IsNot Nothing Then
				Try
					If cls Is Nothing Then
						oins = New java.io.ObjectInputStream(ins)
					Else
						oins = New ObjectInputStreamWithLoader(ins, cls)
					End If
					result = oins.readObject()
					serialized = True
					oins.close()
				Catch ex As java.io.IOException
					ins.close()
					' Drop through and try opening the class.  But remember
					' the exception in case we can't find the class either.
					serex = ex
				Catch ex As  ClassNotFoundException
					ins.close()
					Throw ex
				End Try
			End If

			If result Is Nothing Then
				' No serialized object, try just instantiating the class
				Dim cl As  [Class]

				Try
					cl = com.sun.beans.finder.ClassFinder.findClass(beanName, cls)
				Catch ex As  ClassNotFoundException
					' There is no appropriate class.  If we earlier tried to
					' deserialize an object and got an IO exception, throw that,
					' otherwise rethrow the ClassNotFoundException.
					If serex IsNot Nothing Then Throw serex
					Throw ex
				End Try

				If Not Modifier.isPublic(cl.modifiers) Then Throw New ClassNotFoundException("" & cl & " : no public access")

	'            
	'             * Try to instantiate the class.
	'             

				Try
					result = cl.newInstance()
				Catch ex As Exception
					' We have to remap the exception to one in our signature.
					' But we pass extra information in the detail message.
					Throw New ClassNotFoundException("" & cl & " : " & ex, ex)
				End Try
			End If

			If result IsNot Nothing Then

				' Ok, if the result is an applet initialize it.

				Dim stub As java.applet.AppletStub = Nothing

				If TypeOf result Is java.applet.Applet Then
					Dim applet As java.applet.Applet = CType(result, java.applet.Applet)
					Dim needDummies As Boolean = initializer Is Nothing

					If needDummies Then

						' Figure our the codebase and docbase URLs.  We do this
						' by locating the URL for a known resource, and then
						' massaging the URL.

						' First find the "resource name" corresponding to the bean
						' itself.  So a serialzied bean "a.b.c" would imply a
						' resource name of "a/b/c.ser" and a classname of "x.y"
						' would imply a resource name of "x/y.class".

						Dim resourceName As String

						If serialized Then
							' Serialized bean
							resourceName = beanName.replace("."c,"/"c) & ".ser"
						Else
							' Regular class
							resourceName = beanName.replace("."c,"/"c) & ".class"
						End If

						Dim objectUrl As java.net.URL = Nothing
						Dim codeBase As java.net.URL = Nothing
						Dim docBase As java.net.URL = Nothing

						' Now get the URL correponding to the resource name.
						If cls Is Nothing Then
							objectUrl = ClassLoader.getSystemResource(resourceName)
						Else
							objectUrl = cls.getResource(resourceName)
						End If

						' If we found a URL, we try to locate the docbase by taking
						' of the final path name component, and the code base by taking
						' of the complete resourceName.
						' So if we had a resourceName of "a/b/c.class" and we got an
						' objectURL of "file://bert/classes/a/b/c.class" then we would
						' want to set the codebase to "file://bert/classes/" and the
						' docbase to "file://bert/classes/a/b/"

						If objectUrl IsNot Nothing Then
							Dim s As String = objectUrl.toExternalForm()

							If s.EndsWith(resourceName) Then
								Dim ix As Integer = s.length() - resourceName.length()
								codeBase = New java.net.URL(s.Substring(0,ix))
								docBase = codeBase

								ix = s.LastIndexOf("/"c)

								If ix >= 0 Then docBase = New java.net.URL(s.Substring(0,ix+1))
							End If
						End If

						' Setup a default context and stub.
						Dim context As New BeansAppletContext(applet)

						stub = CType(New BeansAppletStub(applet, context, codeBase, docBase), java.applet.AppletStub)
						applet.stub = stub
					Else
						initializer.initialize(applet, beanContext)
					End If

					' now, if there is a BeanContext, add the bean, if applicable.

					If beanContext IsNot Nothing Then unsafeBeanContextAdd(beanContext, result)

					' If it was deserialized then it was already init-ed.
					' Otherwise we need to initialize it.

					If Not serialized Then
						' We need to set a reasonable initial size, as many
						' applets are unhappy if they are started without
						' having been explicitly sized.
						applet.sizeize(100,100)
						applet.init()
					End If

					If needDummies Then
					  CType(stub, BeansAppletStub).active = True
					Else
						initializer.activate(applet)
					End If

				ElseIf beanContext IsNot Nothing Then
					unsafeBeanContextAdd(beanContext, result)
				End If
			End If

			Return result
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Shared Sub unsafeBeanContextAdd(  beanContext As java.beans.beancontext.BeanContext,   res As Object)
			beanContext.add(res)
		End Sub

		''' <summary>
		''' From a given bean, obtain an object representing a specified
		''' type view of that source object.
		''' <p>
		''' The result may be the same object or a different object.  If
		''' the requested target view isn't available then the given
		''' bean is returned.
		''' <p>
		''' This method is provided in Beans 1.0 as a hook to allow the
		''' addition of more flexible bean behaviour in the future.
		''' </summary>
		''' <returns> an object representing a specified type view of the
		''' source object </returns>
		''' <param name="bean">        Object from which we want to obtain a view. </param>
		''' <param name="targetType">  The type of view we'd like to get.
		'''  </param>
		Public Shared Function getInstanceOf(  bean As Object,   targetType As [Class]) As Object
			Return bean
		End Function

		''' <summary>
		''' Check if a bean can be viewed as a given target type.
		''' The result will be true if the Beans.getInstanceof method
		''' can be used on the given bean to obtain an object that
		''' represents the specified targetType type view.
		''' </summary>
		''' <param name="bean">  Bean from which we want to obtain a view. </param>
		''' <param name="targetType">  The type of view we'd like to get. </param>
		''' <returns> "true" if the given bean supports the given targetType.
		'''  </returns>
		Public Shared Function isInstanceOf(  bean As Object,   targetType As [Class]) As Boolean
			Return Introspector.isSubclass(bean.GetType(), targetType)
		End Function

		''' <summary>
		''' Test if we are in design-mode.
		''' </summary>
		''' <returns>  True if we are running in an application construction
		'''          environment.
		''' </returns>
		''' <seealso cref= DesignMode </seealso>
		PublicShared ReadOnly PropertydesignTime As Boolean
			Get
				Return ThreadGroupContext.context.designTime
			End Get
			Set(  isDesignTime As Boolean)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPropertiesAccess()
				ThreadGroupContext.context.designTime = isDesignTime
			End Set
		End Property

		''' <summary>
		''' Determines whether beans can assume a GUI is available.
		''' </summary>
		''' <returns>  True if we are running in an environment where beans
		'''     can assume that an interactive GUI is available, so they
		'''     can pop up dialog boxes, etc.  This will normally return
		'''     true in a windowing environment, and will normally return
		'''     false in a server environment or if an application is
		'''     running as part of a batch job.
		''' </returns>
		''' <seealso cref= Visibility
		'''  </seealso>
		PublicShared ReadOnly PropertyguiAvailable As Boolean
			Get
				Return ThreadGroupContext.context.guiAvailable
			End Get
			Set(  isGuiAvailable As Boolean)
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPropertiesAccess()
				ThreadGroupContext.context.guiAvailable = isGuiAvailable
			End Set
		End Property




	End Class

	''' <summary>
	''' This subclass of ObjectInputStream delegates loading of classes to
	''' an existing ClassLoader.
	''' </summary>

	Friend Class ObjectInputStreamWithLoader
		Inherits java.io.ObjectInputStream

		Private loader As  ClassLoader

		''' <summary>
		''' Loader must be non-null;
		''' </summary>

		Public Sub New(  [in] As java.io.InputStream,   loader As  ClassLoader)

			MyBase.New([in])
			If loader Is Nothing Then Throw New IllegalArgumentException("Illegal null argument to ObjectInputStreamWithLoader")
			Me.loader = loader
		End Sub

		''' <summary>
		''' Use the given ClassLoader rather than using the system class
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Protected Friend Overridable Function resolveClass(  classDesc As java.io.ObjectStreamClass) As  [Class]

			Dim cname As String = classDesc.name
			Return com.sun.beans.finder.ClassFinder.resolveClass(cname, Me.loader)
		End Function
	End Class

	''' <summary>
	''' Package private support class.  This provides a default AppletContext
	''' for beans which are applets.
	''' </summary>

	Friend Class BeansAppletContext
		Implements java.applet.AppletContext

		Friend target As java.applet.Applet
		Friend imageCache As New Dictionary(Of java.net.URL, Object)

		Friend Sub New(  target As java.applet.Applet)
			Me.target = target
		End Sub

		Public Overridable Function getAudioClip(  url As java.net.URL) As java.applet.AudioClip
			' We don't currently support audio clips in the Beans.instantiate
			' applet context, unless by some luck there exists a URL content
			' class that can generate an AudioClip from the audio URL.
			Try
				Return CType(url.content, java.applet.AudioClip)
			Catch ex As Exception
				Return Nothing
			End Try
		End Function

		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Function getImage(  url As java.net.URL) As java.awt.Image
			Dim o As Object = imageCache(url)
			If o IsNot Nothing Then Return CType(o, java.awt.Image)
			Try
				o = url.content
				If o Is Nothing Then Return Nothing
				If TypeOf o Is java.awt.Image Then
					imageCache(url) = o
					Return CType(o, java.awt.Image)
				End If
				' Otherwise it must be an ImageProducer.
				Dim img As java.awt.Image = target.createImage(CType(o, java.awt.image.ImageProducer))
				imageCache(url) = img
				Return img

			Catch ex As Exception
				Return Nothing
			End Try
		End Function

		Public Overridable Function getApplet(  name As String) As java.applet.Applet
			Return Nothing
		End Function

		Public Overridable Property applets As System.Collections.IEnumerator(Of java.applet.Applet)
			Get
				Dim applets_Renamed As New List(Of java.applet.Applet)
				applets_Renamed.Add(target)
				Return applets_Renamed.elements()
			End Get
		End Property

		Public Overridable Sub showDocument(  url As java.net.URL)
			' We do nothing.
		End Sub

		Public Overridable Sub showDocument(  url As java.net.URL,   target As String)
			' We do nothing.
		End Sub

		Public Overridable Sub showStatus(  status As String)
			' We do nothing.
		End Sub

		Public Overridable Sub setStream(  key As String,   stream As java.io.InputStream)
			' We do nothing.
		End Sub

		Public Overridable Function getStream(  key As String) As java.io.InputStream
			' We do nothing.
			Return Nothing
		End Function

		Public Overridable Property streamKeys As IEnumerator(Of String)
			Get
				' We do nothing.
				Return Nothing
			End Get
		End Property
	End Class

	''' <summary>
	''' Package private support class.  This provides an AppletStub
	''' for beans which are applets.
	''' </summary>
	Friend Class BeansAppletStub
		Implements java.applet.AppletStub

		<NonSerialized> _
		Friend active As Boolean
		<NonSerialized> _
		Friend target As java.applet.Applet
		<NonSerialized> _
		Friend context As java.applet.AppletContext
		<NonSerialized> _
		Friend codeBase As java.net.URL
		<NonSerialized> _
		Friend docBase As java.net.URL

		Friend Sub New(  target As java.applet.Applet,   context As java.applet.AppletContext,   codeBase As java.net.URL,   docBase As java.net.URL)
			Me.target = target
			Me.context = context
			Me.codeBase = codeBase
			Me.docBase = docBase
		End Sub

		Public Overridable Property active As Boolean
			Get
				Return active
			End Get
		End Property

		Public Overridable Property documentBase As java.net.URL
			Get
				' use the root directory of the applet's class-loader
				Return docBase
			End Get
		End Property

		Public Overridable Property codeBase As java.net.URL
			Get
				' use the directory where we found the class or serialized object.
				Return codeBase
			End Get
		End Property

		Public Overridable Function getParameter(  name As String) As String
			Return Nothing
		End Function

		Public Overridable Property appletContext As java.applet.AppletContext
			Get
				Return context
			End Get
		End Property

		Public Overridable Sub appletResize(  width As Integer,   height As Integer)
			' we do nothing.
		End Sub
	End Class

End Namespace