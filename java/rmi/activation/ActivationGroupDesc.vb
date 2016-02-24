Imports System

'
' * Copyright (c) 1997, 2008, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.rmi.activation


	''' <summary>
	''' An activation group descriptor contains the information necessary to
	''' create/recreate an activation group in which to activate objects.
	''' Such a descriptor contains: <ul>
	''' <li> the group's class name,
	''' <li> the group's code location (the location of the group's class), and
	''' <li> a "marshalled" object that can contain group specific
	''' initialization data. </ul> <p>
	''' 
	''' The group's class must be a concrete subclass of
	''' <code>ActivationGroup</code>. A subclass of
	''' <code>ActivationGroup</code> is created/recreated via the
	''' <code>ActivationGroup.createGroup</code> static method that invokes
	''' a special constructor that takes two arguments: <ul>
	''' 
	''' <li> the group's <code>ActivationGroupID</code>, and
	''' <li> the group's initialization data (in a
	''' <code>java.rmi.MarshalledObject</code>)</ul><p>
	''' 
	''' @author      Ann Wollrath
	''' @since       1.2 </summary>
	''' <seealso cref=         ActivationGroup </seealso>
	''' <seealso cref=         ActivationGroupID </seealso>
	<Serializable> _
	Public NotInheritable Class ActivationGroupDesc

		''' <summary>
		''' @serial The group's fully package qualified class name.
		''' </summary>
		Private className As String

		''' <summary>
		''' @serial The location from where to load the group's class.
		''' </summary>
		Private location As String

		''' <summary>
		''' @serial The group's initialization data.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private data As java.rmi.MarshalledObject(Of ?)

		''' <summary>
		''' @serial The controlling options for executing the VM in
		''' another process.
		''' </summary>
		Private env As CommandEnvironment

		''' <summary>
		''' @serial A properties map which will override those set
		''' by default in the subprocess environment.
		''' </summary>
		Private props As java.util.Properties

		''' <summary>
		''' indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
		Private Const serialVersionUID As Long = -4936225423168276595L

		''' <summary>
		''' Constructs a group descriptor that uses the system defaults for group
		''' implementation and code location.  Properties specify Java
		''' environment overrides (which will override system properties in
		''' the group implementation's VM).  The command
		''' environment can control the exact command/options used in
		''' starting the child VM, or can be <code>null</code> to accept
		''' rmid's default.
		''' 
		''' <p>This constructor will create an <code>ActivationGroupDesc</code>
		''' with a <code>null</code> group class name, which indicates the system's
		''' default <code>ActivationGroup</code> implementation.
		''' </summary>
		''' <param name="overrides"> the set of properties to set when the group is
		''' recreated. </param>
		''' <param name="cmd"> the controlling options for executing the VM in
		''' another process (or <code>null</code>).
		''' @since 1.2 </param>
		Public Sub New(ByVal [overrides] As java.util.Properties, ByVal cmd As CommandEnvironment)
			Me.New(Nothing, Nothing, Nothing, [overrides], cmd)
		End Sub

		''' <summary>
		''' Specifies an alternate group implementation and execution
		''' environment to be used for the group.
		''' </summary>
		''' <param name="className"> the group's package qualified class name or
		''' <code>null</code>. A <code>null</code> group class name indicates
		''' the system's default <code>ActivationGroup</code> implementation. </param>
		''' <param name="location"> the location from where to load the group's
		''' class </param>
		''' <param name="data"> the group's initialization data contained in
		''' marshalled form (could contain properties, for example) </param>
		''' <param name="overrides"> a properties map which will override those set
		''' by default in the subprocess environment (will be translated
		''' into <code>-D</code> options), or <code>null</code>. </param>
		''' <param name="cmd"> the controlling options for executing the VM in
		''' another process (or <code>null</code>).
		''' @since 1.2 </param>
		Public Sub New(Of T1)(ByVal className As String, ByVal location As String, ByVal data As java.rmi.MarshalledObject(Of T1), ByVal [overrides] As java.util.Properties, ByVal cmd As CommandEnvironment)
			Me.props = [overrides]
			Me.env = cmd
			Me.data = data
			Me.location = location
			Me.className = className
		End Sub

		''' <summary>
		''' Returns the group's class name (possibly <code>null</code>).  A
		''' <code>null</code> group class name indicates the system's default
		''' <code>ActivationGroup</code> implementation. </summary>
		''' <returns> the group's class name
		''' @since 1.2 </returns>
		Public Property className As String
			Get
				Return className
			End Get
		End Property

		''' <summary>
		''' Returns the group's code location. </summary>
		''' <returns> the group's code location
		''' @since 1.2 </returns>
		Public Property location As String
			Get
				Return location
			End Get
		End Property

		''' <summary>
		''' Returns the group's initialization data. </summary>
		''' <returns> the group's initialization data
		''' @since 1.2 </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Property data As java.rmi.MarshalledObject(Of ?)
			Get
				Return data
			End Get
		End Property

		''' <summary>
		''' Returns the group's property-override list. </summary>
		''' <returns> the property-override list, or <code>null</code>
		''' @since 1.2 </returns>
		Public Property propertyOverrides As java.util.Properties
			Get
				Return If(props IsNot Nothing, CType(props.clone(), java.util.Properties), Nothing)
			End Get
		End Property

		''' <summary>
		''' Returns the group's command-environment control object. </summary>
		''' <returns> the command-environment object, or <code>null</code>
		''' @since 1.2 </returns>
		Public Property commandEnvironment As CommandEnvironment
			Get
				Return Me.env
			End Get
		End Property


		''' <summary>
		''' Startup options for ActivationGroup implementations.
		''' 
		''' This class allows overriding default system properties and
		''' specifying implementation-defined options for ActivationGroups.
		''' @since 1.2
		''' </summary>
		<Serializable> _
		Public Class CommandEnvironment
			Private Const serialVersionUID As Long = 6165754737887770191L

			''' <summary>
			''' @serial
			''' </summary>
			Private command As String

			''' <summary>
			''' @serial
			''' </summary>
			Private options As String()

			''' <summary>
			''' Create a CommandEnvironment with all the necessary
			''' information.
			''' </summary>
			''' <param name="cmdpath"> the name of the java executable, including
			''' the full path, or <code>null</code>, meaning "use rmid's default".
			''' The named program <em>must</em> be able to accept multiple
			''' <code>-Dpropname=value</code> options (as documented for the
			''' "java" tool)
			''' </param>
			''' <param name="argv"> extra options which will be used in creating the
			''' ActivationGroup.  Null has the same effect as an empty
			''' list.
			''' @since 1.2 </param>
			Public Sub New(ByVal cmdpath As String, ByVal argv As String())
				Me.command = cmdpath ' might be null

				' Hold a safe copy of argv in this.options
				If argv Is Nothing Then
					Me.options = New String(){}
				Else
					Me.options = New String(argv.Length - 1){}
					Array.Copy(argv, 0, Me.options, 0, argv.Length)
				End If
			End Sub

			''' <summary>
			''' Fetch the configured path-qualified java command name.
			''' </summary>
			''' <returns> the configured name, or <code>null</code> if configured to
			''' accept the default
			''' @since 1.2 </returns>
			Public Overridable Property commandPath As String
				Get
					Return (Me.command)
				End Get
			End Property

			''' <summary>
			''' Fetch the configured java command options.
			''' </summary>
			''' <returns> An array of the command options which will be passed
			''' to the new child command by rmid.
			''' Note that rmid may add other options before or after these
			''' options, or both.
			''' Never returns <code>null</code>.
			''' @since 1.2 </returns>
			Public Overridable Property commandOptions As String()
				Get
					Return options.clone()
				End Get
			End Property

			''' <summary>
			''' Compares two command environments for content equality.
			''' </summary>
			''' <param name="obj">     the Object to compare with </param>
			''' <returns>      true if these Objects are equal; false otherwise. </returns>
			''' <seealso cref=         java.util.Hashtable
			''' @since 1.2 </seealso>
			Public Overrides Function Equals(ByVal obj As Object) As Boolean

				If TypeOf obj Is CommandEnvironment Then
					Dim env As CommandEnvironment = CType(obj, CommandEnvironment)
					Return ((If(command Is Nothing, env.command Is Nothing, command.Equals(env.command))) AndAlso java.util.Arrays.Equals(options, env.options))
				Else
					Return False
				End If
			End Function

			''' <summary>
			''' Return identical values for similar
			''' <code>CommandEnvironment</code>s. </summary>
			''' <returns> an integer </returns>
			''' <seealso cref= java.util.Hashtable </seealso>
			Public Overrides Function GetHashCode() As Integer
				' hash command and ignore possibly expensive options
				Return (If(command Is Nothing, 0, command.GetHashCode()))
			End Function

			''' <summary>
			''' <code>readObject</code> for custom serialization.
			''' 
			''' <p>This method reads this object's serialized form for this
			''' class as follows:
			''' 
			''' <p>This method first invokes <code>defaultReadObject</code> on
			''' the specified object input stream, and if <code>options</code>
			''' is <code>null</code>, then <code>options</code> is set to a
			''' zero-length array of <code>String</code>.
			''' </summary>
			Private Sub readObject(ByVal [in] As java.io.ObjectInputStream)
				[in].defaultReadObject()
				If options Is Nothing Then options = New String(){}
			End Sub
		End Class

		''' <summary>
		''' Compares two activation group descriptors for content equality.
		''' </summary>
		''' <param name="obj">     the Object to compare with </param>
		''' <returns>  true if these Objects are equal; false otherwise. </returns>
		''' <seealso cref=             java.util.Hashtable
		''' @since 1.2 </seealso>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean

			If TypeOf obj Is ActivationGroupDesc Then
				Dim desc As ActivationGroupDesc = CType(obj, ActivationGroupDesc)
				Return ((If(className Is Nothing, desc.className Is Nothing, className.Equals(desc.className))) AndAlso (If(location Is Nothing, desc.location Is Nothing, location.Equals(desc.location))) AndAlso (If(data Is Nothing, desc.data Is Nothing, data.Equals(desc.data))) AndAlso (If(env Is Nothing, desc.env Is Nothing, env.Equals(desc.env))) AndAlso (If(props Is Nothing, desc.props Is Nothing, props.Equals(desc.props))))
			Else
				Return False
			End If
		End Function

		''' <summary>
		''' Produce identical numbers for similar <code>ActivationGroupDesc</code>s. </summary>
		''' <returns> an integer </returns>
		''' <seealso cref= java.util.Hashtable </seealso>
		Public Overrides Function GetHashCode() As Integer
			' hash location, className, data, and env
			' but omit props (may be expensive)
			Return ((If(location Is Nothing, 0, location.GetHashCode() << 24)) Xor (If(env Is Nothing, 0, env.GetHashCode() << 16)) Xor (If(className Is Nothing, 0, className.GetHashCode() << 8)) Xor (If(data Is Nothing, 0, data.GetHashCode())))
		End Function
	End Class

End Namespace