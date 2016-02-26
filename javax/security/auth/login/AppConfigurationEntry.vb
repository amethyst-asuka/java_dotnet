Imports System.Collections.Generic

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

Namespace javax.security.auth.login


	''' <summary>
	''' This class represents a single {@code LoginModule} entry
	''' configured for the application specified in the
	''' {@code getAppConfigurationEntry(String appName)}
	''' method in the {@code Configuration} class.  Each respective
	''' {@code AppConfigurationEntry} contains a {@code LoginModule} name,
	''' a control flag (specifying whether this {@code LoginModule} is
	''' REQUIRED, REQUISITE, SUFFICIENT, or OPTIONAL), and LoginModule-specific
	''' options.  Please refer to the {@code Configuration} class for
	''' more information on the different control flags and their semantics.
	''' </summary>
	''' <seealso cref= javax.security.auth.login.Configuration </seealso>
	Public Class AppConfigurationEntry

		Private loginModuleName As String
		Private controlFlag As LoginModuleControlFlag
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private options As IDictionary(Of String, ?)

		''' <summary>
		''' Default constructor for this class.
		''' 
		''' <p> This entry represents a single {@code LoginModule}
		''' entry configured for the application specified in the
		''' {@code getAppConfigurationEntry(String appName)}
		''' method from the {@code Configuration} class.
		''' </summary>
		''' <param name="loginModuleName"> String representing the class name of the
		'''                  {@code LoginModule} configured for the
		'''                  specified application. <p>
		''' </param>
		''' <param name="controlFlag"> either REQUIRED, REQUISITE, SUFFICIENT,
		'''                  or OPTIONAL. <p>
		''' </param>
		''' <param name="options"> the options configured for this {@code LoginModule}.
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code loginModuleName}
		'''                  is null, if {@code LoginModuleName}
		'''                  has a length of 0, if {@code controlFlag}
		'''                  is not either REQUIRED, REQUISITE, SUFFICIENT
		'''                  or OPTIONAL, or if {@code options} is null. </exception>
		Public Sub New(Of T1)(ByVal loginModuleName As String, ByVal controlFlag As LoginModuleControlFlag, ByVal options As IDictionary(Of T1))
			If loginModuleName Is Nothing OrElse loginModuleName.Length = 0 OrElse (controlFlag IsNot LoginModuleControlFlag.REQUIRED AndAlso controlFlag IsNot LoginModuleControlFlag.REQUISITE AndAlso controlFlag IsNot LoginModuleControlFlag.SUFFICIENT AndAlso controlFlag IsNot LoginModuleControlFlag.OPTIONAL) OrElse options Is Nothing Then Throw New System.ArgumentException

			Me.loginModuleName = loginModuleName
			Me.controlFlag = controlFlag
			Me.options = java.util.Collections.unmodifiableMap(options)
		End Sub

		''' <summary>
		''' Get the class name of the configured {@code LoginModule}.
		''' </summary>
		''' <returns> the class name of the configured {@code LoginModule} as
		'''          a String. </returns>
		Public Overridable Property loginModuleName As String
			Get
				Return loginModuleName
			End Get
		End Property

		''' <summary>
		''' Return the controlFlag
		''' (either REQUIRED, REQUISITE, SUFFICIENT, or OPTIONAL)
		''' for this {@code LoginModule}.
		''' </summary>
		''' <returns> the controlFlag
		'''          (either REQUIRED, REQUISITE, SUFFICIENT, or OPTIONAL)
		'''          for this {@code LoginModule}. </returns>
		Public Overridable Property controlFlag As LoginModuleControlFlag
			Get
				Return controlFlag
			End Get
		End Property

		''' <summary>
		''' Get the options configured for this {@code LoginModule}.
		''' </summary>
		''' <returns> the options configured for this {@code LoginModule}
		'''          as an unmodifiable {@code Map}. </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Overridable Property options As IDictionary(Of String, ?)
			Get
				Return options
			End Get
		End Property

		''' <summary>
		''' This class represents whether or not a {@code LoginModule}
		''' is REQUIRED, REQUISITE, SUFFICIENT or OPTIONAL.
		''' </summary>
		Public Class LoginModuleControlFlag

			Private controlFlag As String

			''' <summary>
			''' Required {@code LoginModule}.
			''' </summary>
			Public Shared ReadOnly REQUIRED As New LoginModuleControlFlag("required")

			''' <summary>
			''' Requisite {@code LoginModule}.
			''' </summary>
			Public Shared ReadOnly REQUISITE As New LoginModuleControlFlag("requisite")

			''' <summary>
			''' Sufficient {@code LoginModule}.
			''' </summary>
			Public Shared ReadOnly SUFFICIENT As New LoginModuleControlFlag("sufficient")

			''' <summary>
			''' Optional {@code LoginModule}.
			''' </summary>
			Public Shared ReadOnly [OPTIONAL] As New LoginModuleControlFlag("optional")

			Private Sub New(ByVal controlFlag As String)
				Me.controlFlag = controlFlag
			End Sub

			''' <summary>
			''' Return a String representation of this controlFlag.
			''' 
			''' <p> The String has the format, "LoginModuleControlFlag: <i>flag</i>",
			''' where <i>flag</i> is either <i>required</i>, <i>requisite</i>,
			''' <i>sufficient</i>, or <i>optional</i>.
			''' </summary>
			''' <returns> a String representation of this controlFlag. </returns>
			Public Overrides Function ToString() As String
				Return (sun.security.util.ResourcesMgr.getString("LoginModuleControlFlag.") + controlFlag)
			End Function
		End Class
	End Class

End Namespace