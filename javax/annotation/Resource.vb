Imports System

'
' * Copyright (c) 2005, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.annotation


	''' <summary>
	''' The Resource annotation marks a resource that is needed
	''' by the application.  This annotation may be applied to an
	''' application component class, or to fields or methods of the
	''' component class.  When the annotation is applied to a
	''' field or method, the container will inject an instance
	''' of the requested resource into the application component
	''' when the component is initialized.  If the annotation is
	''' applied to the component class, the annotation declares a
	''' resource that the application will look up at runtime. <p>
	''' 
	''' Even though this annotation is not marked Inherited, deployment
	''' tools are required to examine all superclasses of any component
	''' class to discover all uses of this annotation in all superclasses.
	''' All such annotation instances specify resources that are needed
	''' by the application component.  Note that this annotation may
	''' appear on private fields and methods of superclasses; the container
	''' is required to perform injection in these cases as well.
	''' 
	''' @since Common Annotations 1.0
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to TYPE:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to FIELD:
'JAVA TO VB CONVERTER TODO TASK: There is no attribute target in .NET corresponding to METHOD:
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<AttributeUsage(<missing> Or <missing> Or <missing>, AllowMultiple := False, Inherited := False> _
	Public Class Resource
		Inherits System.Attribute

		''' <summary>
		''' The JNDI name of the resource.  For field annotations,
		''' the default is the field name.  For method annotations,
		''' the default is the JavaBeans property name corresponding
		''' to the method.  For class annotations, there is no default
		''' and this must be specified.
		''' </summary>
		public property name()  as String= ""

		''' <summary>
		''' The name of the resource that the reference points to. It can
		''' link to any compatible resource using the global JNDI names.
		''' 
		''' @since Common Annotations 1.1
		''' </summary>

		public property lookup() as String= ""

		''' <summary>
		''' The Java type of the resource.  For field annotations,
		''' the default is the type of the field.  For method annotations,
		''' the default is the type of the JavaBeans property.
		''' For class annotations, there is no default and this must be
		''' specified.
		''' </summary>
		 public property type()  as Type = GetType(Object)

		''' <summary>
		''' The two possible authentication types for a resource.
		''' </summary>
		Dim AuthenticationType As enum
				CONTAINER,
				APPLICATION

		''' <summary>
		''' The authentication type to use for this resource.
		''' This may be specified for resources representing a
		''' connection factory of any supported type, and must
		''' not be specified for resources of other types.
		''' </summary>
		public property  authenticationType()  as AuthenticationType = AuthenticationType.CONTAINER

		''' <summary>
		''' Indicates whether this resource can be shared between
		''' this component and other components.
		''' This may be specified for resources representing a
		''' connection factory of any supported type, and must
		''' not be specified for resources of other types.
		''' </summary>
		public property  shareable()  as boolean = True

		''' <summary>
		''' A product specific name that this resource should be mapped to.
		''' The name of this resource, as defined by the <code>name</code>
		''' element or defaulted, is a name that is local to the application
		''' component using the resource.  (It's a name in the JNDI
		''' <code>java:comp/env</code> namespace.)  Many application servers
		''' provide a way to map these local names to names of resources
		''' known to the application server.  This mapped name is often a
		''' <i>global</i> JNDI name, but may be a name of any form. <p>
		''' 
		''' Application servers are not required to support any particular
		''' form or type of mapped name, nor the ability to use mapped names.
		''' The mapped name is product-dependent and often installation-dependent.
		''' No use of a mapped name is portable.
		''' </summary>
		public property  mappedName()  as  string = ""

		''' <summary>
		''' Description of this resource.  The description is expected
		''' to be in the default language of the system on which the
		''' application is deployed.  The description can be presented
		''' to the Deployer to help in choosing the correct resource.
		''' </summary>
		public property description() as String = ""
	End Class

End Namespace