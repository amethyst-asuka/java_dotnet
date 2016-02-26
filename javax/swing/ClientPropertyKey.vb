'
' * Copyright (c) 2006, 2012, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.swing


	''' <summary>
	''' An enumeration for keys used as client properties within the Swing
	''' implementation.
	''' <p>
	''' This enum holds only a small subset of the keys currently used within Swing,
	''' but we may move more of them here in the future.
	''' <p>
	''' Adding an item to, and using, this class instead of {@code String} for
	''' client properties protects against conflicts with developer-set client
	''' properties. Using this class also avoids a problem with {@code StringBuilder}
	''' and {@code StringBuffer} keys, whereby the keys are not recognized upon
	''' deserialization.
	''' <p>
	''' When a client property value associated with one of these keys does not
	''' implement {@code Serializable}, the result during serialization depends
	''' on how the key is defined here. Historically, client properties with values
	''' not implementing {@code Serializable} have simply been dropped and left out
	''' of the serialized representation. To define keys with such behavior in this
	''' enum, provide a value of {@code false} for the {@code reportValueNotSerializable}
	''' property. When migrating existing properties to this enum, one may wish to
	''' consider using this by default, to preserve backward compatibility.
	''' <p>
	''' To instead have a {@code NotSerializableException} thrown when a
	''' {@code non-Serializable} property is encountered, provide the value of
	''' {@code true} for the {@code reportValueNotSerializable} property. This
	''' is useful when the property represents something that the developer
	''' needs to know about when it cannot be serialized.
	''' 
	''' @author  Shannon Hickey
	''' </summary>
	Friend Enum ClientPropertyKey

		''' <summary>
		''' Key used by JComponent for storing InputVerifier.
		''' </summary>
		JComponent_INPUT_VERIFIER = True

		''' <summary>
		''' Key used by JComponent for storing TransferHandler.
		''' </summary>
		JComponent_TRANSFER_HANDLER = True

		''' <summary>
		''' Key used by JComponent for storing AncestorNotifier.
		''' </summary>
		JComponent_ANCESTOR_NOTIFIER = True

		''' <summary>
		''' Key used by PopupFactory to force heavy weight popups for a
		''' component.
		''' </summary>
		PopupFactory_FORCE_HEAVYWEIGHT_POPUP = True


		''' <summary>
		''' Whether or not a {@code NotSerializableException} should be thrown
		''' during serialization, when the value associated with this key does
		''' not implement {@code Serializable}.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain fields in .NET:
'		private final boolean reportValueNotSerializable;

'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		static ImpliedClass()
	'	{
	'		AWTAccessor.setClientPropertyKeyAccessor(New AWTAccessor.ClientPropertyKeyAccessor()
	'		{
	'				public Object getJComponent_TRANSFER_HANDLER()
	'				{
	'					Return JComponent_TRANSFER_HANDLER;
	'				}
	'			});
	'	}

		''' <summary>
		''' Constructs a key with the {@code reportValueNotSerializable} property
		''' set to {@code false}.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private ClientPropertyKey()
	'	{
	'		Me(False);
	'	}

		''' <summary>
		''' Constructs a key with the {@code reportValueNotSerializable} property
		''' set to the given value.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		private ClientPropertyKey(boolean reportValueNotSerializable)
	'	{
	'		Me.reportValueNotSerializable = reportValueNotSerializable;
	'	}

		''' <summary>
		''' Returns whether or not a {@code NotSerializableException} should be thrown
		''' during serialization, when the value associated with this key does
		''' not implement {@code Serializable}.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Enums cannot contain methods in .NET:
'		public boolean getReportValueNotSerializable()
	'	{
	'		Return reportValueNotSerializable;
	'	}
	End Enum

End Namespace