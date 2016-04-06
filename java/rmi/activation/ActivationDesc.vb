Imports System
Imports java.lang

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
    ''' An activation descriptor contains the information necessary to
    ''' activate an object: <ul>
    ''' <li> the object's group identifier,
    ''' <li> the object's fully-qualified class name,
    ''' <li> the object's code location (the location of the [Class]), a codebase URL
    ''' path,
    ''' <li> the object's restart "mode", and,
    ''' <li> a "marshalled" object that can contain object specific
    ''' initialization data. </ul>
    ''' 
    ''' <p>A descriptor registered with the activation system can be used to
    ''' recreate/activate the object specified by the descriptor. The
    ''' <code>MarshalledObject</code> in the object's descriptor is passed
    ''' as the second argument to the remote object's constructor for
    ''' object to use during reinitialization/activation.
    ''' 
    ''' @author      Ann Wollrath
    ''' @since       1.2 </summary>
    ''' <seealso cref=         java.rmi.activation.Activatable </seealso>
    <Serializable>
    Public NotInheritable Class ActivationDesc

        ''' <summary>
        ''' @serial the group's identifier
        ''' </summary>
        Private groupID As ActivationGroupID

        ''' <summary>
        ''' @serial the object's class name
        ''' </summary>
        Private className As String

        ''' <summary>
        ''' @serial the object's code location
        ''' </summary>
        Private location As String

        ''' <summary>
        ''' @serial the object's initialization data
        ''' </summary>
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Private data As java.rmi.MarshalledObject(Of ?)

        ''' <summary>
        ''' @serial indicates whether the object should be restarted
        ''' </summary>
        Private restart As Boolean

        ''' <summary>
        ''' indicate compatibility with the Java 2 SDK v1.2 version of class </summary>
        Private Const serialVersionUID As Long = 7455834104417690957L

        ''' <summary>
        ''' Constructs an object descriptor for an object whose class name
        ''' is <code>className</code>, that can be loaded from the
        ''' code <code>location</code> and whose initialization
        ''' information is <code>data</code>. If this form of the constructor
        ''' is used, the <code>groupID</code> defaults to the current id for
        ''' <code>ActivationGroup</code> for this VM. All objects with the
        ''' same <code>ActivationGroupID</code> are activated in the same VM.
        ''' 
        ''' <p>Note that objects specified by a descriptor created with this
        ''' constructor will only be activated on demand (by default, the restart
        ''' mode is <code>false</code>).  If an activatable object requires restart
        ''' services, use one of the <code>ActivationDesc</code> constructors that
        ''' takes a boolean parameter, <code>restart</code>.
        ''' 
        ''' <p> This constructor will throw <code>ActivationException</code> if
        ''' there is no current activation group for this VM.  To create an
        ''' <code>ActivationGroup</code> use the
        ''' <code>ActivationGroup.createGroup</code> method.
        ''' </summary>
        ''' <param name="className"> the object's fully package qualified class name </param>
        ''' <param name="location"> the object's code location (from where the class is
        ''' loaded) </param>
        ''' <param name="data"> the object's initialization (activation) data contained
        ''' in marshalled form. </param>
        ''' <exception cref="ActivationException"> if the current group is nonexistent </exception>
        ''' <exception cref="UnsupportedOperationException"> if and only if activation is
        ''' not supported by this implementation
        ''' @since 1.2 </exception>
        Public Sub New(Of T1)(  className As String,   location As String,   data As java.rmi.MarshalledObject(Of T1))
            Me.New(ActivationGroup.internalCurrentGroupID(), className, location, data, False)
        End Sub

        ''' <summary>
        ''' Constructs an object descriptor for an object whose class name
        ''' is <code>className</code>, that can be loaded from the
        ''' code <code>location</code> and whose initialization
        ''' information is <code>data</code>. If this form of the constructor
        ''' is used, the <code>groupID</code> defaults to the current id for
        ''' <code>ActivationGroup</code> for this VM. All objects with the
        ''' same <code>ActivationGroupID</code> are activated in the same VM.
        ''' 
        ''' <p>This constructor will throw <code>ActivationException</code> if
        ''' there is no current activation group for this VM.  To create an
        ''' <code>ActivationGroup</code> use the
        ''' <code>ActivationGroup.createGroup</code> method.
        ''' </summary>
        ''' <param name="className"> the object's fully package qualified class name </param>
        ''' <param name="location"> the object's code location (from where the class is
        ''' loaded) </param>
        ''' <param name="data"> the object's initialization (activation) data contained
        ''' in marshalled form. </param>
        ''' <param name="restart"> if true, the object is restarted (reactivated) when
        ''' either the activator is restarted or the object's activation group
        ''' is restarted after an unexpected crash; if false, the object is only
        ''' activated on demand.  Specifying <code>restart</code> to be
        ''' <code>true</code> does not force an initial immediate activation of
        ''' a newly registered object;  initial activation is lazy. </param>
        ''' <exception cref="ActivationException"> if the current group is nonexistent </exception>
        ''' <exception cref="UnsupportedOperationException"> if and only if activation is
        ''' not supported by this implementation
        ''' @since 1.2 </exception>
        Public Sub New(Of T1)(  className As String,   location As String,   data As java.rmi.MarshalledObject(Of T1),   restart As Boolean)
            Me.New(ActivationGroup.internalCurrentGroupID(), className, location, data, restart)
        End Sub

        ''' <summary>
        ''' Constructs an object descriptor for an object whose class name
        ''' is <code>className</code> that can be loaded from the
        ''' code <code>location</code> and whose initialization
        ''' information is <code>data</code>. All objects with the same
        ''' <code>groupID</code> are activated in the same Java VM.
        ''' 
        ''' <p>Note that objects specified by a descriptor created with this
        ''' constructor will only be activated on demand (by default, the restart
        ''' mode is <code>false</code>).  If an activatable object requires restart
        ''' services, use one of the <code>ActivationDesc</code> constructors that
        ''' takes a boolean parameter, <code>restart</code>.
        ''' </summary>
        ''' <param name="groupID"> the group's identifier (obtained from registering
        ''' <code>ActivationSystem.registerGroup</code> method). The group
        ''' indicates the VM in which the object should be activated. </param>
        ''' <param name="className"> the object's fully package-qualified class name </param>
        ''' <param name="location"> the object's code location (from where the class is
        ''' loaded) </param>
        ''' <param name="data">  the object's initialization (activation) data contained
        ''' in marshalled form. </param>
        ''' <exception cref="IllegalArgumentException"> if <code>groupID</code> is null </exception>
        ''' <exception cref="UnsupportedOperationException"> if and only if activation is
        ''' not supported by this implementation
        ''' @since 1.2 </exception>
        Public Sub New(Of T1)(  groupID As ActivationGroupID,   className As String,   location As String,   data As java.rmi.MarshalledObject(Of T1))
            Me.New(groupID, className, location, data, False)
        End Sub

        ''' <summary>
        ''' Constructs an object descriptor for an object whose class name
        ''' is <code>className</code> that can be loaded from the
        ''' code <code>location</code> and whose initialization
        ''' information is <code>data</code>. All objects with the same
        ''' <code>groupID</code> are activated in the same Java VM.
        ''' </summary>
        ''' <param name="groupID"> the group's identifier (obtained from registering
        ''' <code>ActivationSystem.registerGroup</code> method). The group
        ''' indicates the VM in which the object should be activated. </param>
        ''' <param name="className"> the object's fully package-qualified class name </param>
        ''' <param name="location"> the object's code location (from where the class is
        ''' loaded) </param>
        ''' <param name="data">  the object's initialization (activation) data contained
        ''' in marshalled form. </param>
        ''' <param name="restart"> if true, the object is restarted (reactivated) when
        ''' either the activator is restarted or the object's activation group
        ''' is restarted after an unexpected crash; if false, the object is only
        ''' activated on demand.  Specifying <code>restart</code> to be
        ''' <code>true</code> does not force an initial immediate activation of
        ''' a newly registered object;  initial activation is lazy. </param>
        ''' <exception cref="IllegalArgumentException"> if <code>groupID</code> is null </exception>
        ''' <exception cref="UnsupportedOperationException"> if and only if activation is
        ''' not supported by this implementation
        ''' @since 1.2 </exception>
        Public Sub New(Of T1)(  groupID As ActivationGroupID,   className As String,   location As String,   data As java.rmi.MarshalledObject(Of T1),   restart As Boolean)
            If groupID Is Nothing Then Throw New IllegalArgumentException("groupID can't be null")
            Me.groupID = groupID
            Me.className = className
            Me.location = location
            Me.data = data
            Me.restart = restart
        End Sub

        ''' <summary>
        ''' Returns the group identifier for the object specified by this
        ''' descriptor. A group provides a way to aggregate objects into a
        ''' single Java virtual machine. RMI creates/activates objects with
        ''' the same <code>groupID</code> in the same virtual machine.
        ''' </summary>
        ''' <returns> the group identifier
        ''' @since 1.2 </returns>
        Public Property groupID As ActivationGroupID
            Get
                Return groupID
            End Get
        End Property

        ''' <summary>
        ''' Returns the class name for the object specified by this
        ''' descriptor. </summary>
        ''' <returns> the class name
        ''' @since 1.2 </returns>
        Public Property className As String
            Get
                Return className
            End Get
        End Property

        ''' <summary>
        ''' Returns the code location for the object specified by
        ''' this descriptor. </summary>
        ''' <returns> the code location
        ''' @since 1.2 </returns>
        Public Property location As String
            Get
                Return location
            End Get
        End Property

        ''' <summary>
        ''' Returns a "marshalled object" containing intialization/activation
        ''' data for the object specified by this descriptor. </summary>
        ''' <returns> the object specific "initialization" data
        ''' @since 1.2 </returns>
        'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        Public Property data As java.rmi.MarshalledObject(Of ?)
            Get
                Return data
            End Get
        End Property

        ''' <summary>
        ''' Returns the "restart" mode of the object associated with
        ''' this activation descriptor.
        ''' </summary>
        ''' <returns> true if the activatable object associated with this
        ''' activation descriptor is restarted via the activation
        ''' daemon when either the daemon comes up or the object's group
        ''' is restarted after an unexpected crash; otherwise it returns false,
        ''' meaning that the object is only activated on demand via a
        ''' method call.  Note that if the restart mode is <code>true</code>, the
        ''' activator does not force an initial immediate activation of
        ''' a newly registered object;  initial activation is lazy.
        ''' @since 1.2 </returns>
        Public Property restartMode As Boolean
            Get
                Return restart
            End Get
        End Property

        ''' <summary>
        ''' Compares two activation descriptors for content equality.
        ''' </summary>
        ''' <param name="obj">     the Object to compare with </param>
        ''' <returns>  true if these Objects are equal; false otherwise. </returns>
        ''' <seealso cref=             java.util.Hashtable
        ''' @since 1.2 </seealso>
        Public Overrides Function Equals(  obj As Object) As Boolean

            If TypeOf obj Is ActivationDesc Then
                Dim desc As ActivationDesc = CType(obj, ActivationDesc)
                Return ((If(groupID Is Nothing, desc.groupID Is Nothing, groupID.Equals(desc.groupID))) AndAlso (If(className Is Nothing, desc.className Is Nothing, className.Equals(desc.className))) AndAlso (If(location Is Nothing, desc.location Is Nothing, location.Equals(desc.location))) AndAlso (If(data Is Nothing, desc.data Is Nothing, data.Equals(desc.data))) AndAlso (restart = desc.restart))

            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Return the same hashCode for similar <code>ActivationDesc</code>s. </summary>
        ''' <returns> an integer </returns>
        ''' <seealso cref= java.util.Hashtable </seealso>
        Public Overrides Function GetHashCode() As Integer
            Return ((If(location Is Nothing, 0, location.GetHashCode() << 24)) Xor (If(groupID Is Nothing, 0, groupID.GetHashCode() << 16)) Xor (If(className Is Nothing, 0, className.GetHashCode() << 9)) Xor (If(data Is Nothing, 0, data.GetHashCode() << 1)) Xor (If(restart, 1, 0)))
        End Function
    End Class

End Namespace