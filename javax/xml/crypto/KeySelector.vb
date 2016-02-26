'
' * Copyright (c) 2005, 2013, Oracle and/or its affiliates. All rights reserved.
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
' * $Id: KeySelector.java,v 1.6 2005/05/10 15:47:42 mullan Exp $
' 
Namespace javax.xml.crypto


	''' <summary>
	''' A selector that finds and returns a key using the data contained in a
	''' <seealso cref="KeyInfo"/> object. An example of an implementation of
	''' this class is one that searches a <seealso cref="java.security.KeyStore"/> for
	''' trusted keys that match information contained in a <code>KeyInfo</code>.
	''' 
	''' <p>Whether or not the returned key is trusted and the mechanisms
	''' used to determine that is implementation-specific.
	''' 
	''' @author Sean Mullan
	''' @author JSR 105 Expert Group
	''' @since 1.6
	''' </summary>
	Public MustInherit Class KeySelector

		''' <summary>
		''' The purpose of the key that is to be selected.
		''' </summary>
		Public Class Purpose

			Private ReadOnly name As String

			Private Sub New(ByVal name As String)
				Me.name = name
			End Sub

			''' <summary>
			''' Returns a string representation of this purpose ("sign",
			''' "verify", "encrypt", or "decrypt").
			''' </summary>
			''' <returns> a string representation of this purpose </returns>
			Public Overrides Function ToString() As String
				Return name
			End Function

			''' <summary>
			''' A key for signing.
			''' </summary>
			Public Shared ReadOnly SIGN As New Purpose("sign")
			''' <summary>
			''' A key for verifying.
			''' </summary>
			Public Shared ReadOnly VERIFY As New Purpose("verify")
			''' <summary>
			''' A key for encrypting.
			''' </summary>
			Public Shared ReadOnly ENCRYPT As New Purpose("encrypt")
			''' <summary>
			''' A key for decrypting.
			''' </summary>
			Public Shared ReadOnly DECRYPT As New Purpose("decrypt")
		End Class

		''' <summary>
		''' Default no-args constructor; intended for invocation by subclasses only.
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Attempts to find a key that satisfies the specified constraints.
		''' </summary>
		''' <param name="keyInfo"> a <code>KeyInfo</code> (may be <code>null</code>) </param>
		''' <param name="purpose"> the key's purpose (<seealso cref="Purpose#SIGN"/>,
		'''    <seealso cref="Purpose#VERIFY"/>, <seealso cref="Purpose#ENCRYPT"/>, or
		'''    <seealso cref="Purpose#DECRYPT"/>) </param>
		''' <param name="method"> the algorithm method that this key is to be used for.
		'''    Only keys that are compatible with the algorithm and meet the
		'''    constraints of the specified algorithm should be returned. </param>
		''' <param name="context"> an <code>XMLCryptoContext</code> that may contain
		'''    useful information for finding an appropriate key. If this key
		'''    selector supports resolving <seealso cref="RetrievalMethod"/> types, the
		'''    context's <code>baseURI</code> and <code>dereferencer</code>
		'''    parameters (if specified) should be used by the selector to
		'''    resolve and dereference the URI. </param>
		''' <returns> the result of the key selector </returns>
		''' <exception cref="KeySelectorException"> if an exceptional condition occurs while
		'''    attempting to find a key. Note that an inability to find a key is not
		'''    considered an exception (<code>null</code> should be
		'''    returned in that case). However, an error condition (ex: network
		'''    communications failure) that prevented the <code>KeySelector</code>
		'''    from finding a potential key should be considered an exception. </exception>
		''' <exception cref="ClassCastException"> if the data type of <code>method</code>
		'''    is not supported by this key selector </exception>
		Public MustOverride Function [select](ByVal keyInfo As javax.xml.crypto.dsig.keyinfo.KeyInfo, ByVal purpose As Purpose, ByVal method As AlgorithmMethod, ByVal context As XMLCryptoContext) As KeySelectorResult

		''' <summary>
		''' Returns a <code>KeySelector</code> that always selects the specified
		''' key, regardless of the <code>KeyInfo</code> passed to it.
		''' </summary>
		''' <param name="key"> the sole key to be stored in the key selector </param>
		''' <returns> a key selector that always selects the specified key </returns>
		''' <exception cref="NullPointerException"> if <code>key</code> is <code>null</code> </exception>
		Public Shared Function singletonKeySelector(ByVal key As java.security.Key) As KeySelector
			Return New SingletonKeySelector(key)
		End Function

		Private Class SingletonKeySelector
			Inherits KeySelector

			Private ReadOnly key As java.security.Key

			Friend Sub New(ByVal key As java.security.Key)
				If key Is Nothing Then Throw New NullPointerException
				Me.key = key
			End Sub

			Public Overrides Function [select](ByVal keyInfo As javax.xml.crypto.dsig.keyinfo.KeyInfo, ByVal ___purpose As Purpose, ByVal method As AlgorithmMethod, ByVal context As XMLCryptoContext) As KeySelectorResult

				Return New KeySelectorResultAnonymousInnerClassHelper
			End Function

			Private Class KeySelectorResultAnonymousInnerClassHelper
				Implements KeySelectorResult

				Public Overridable Property key As java.security.Key Implements KeySelectorResult.getKey
					Get
						Return outerInstance.key
					End Get
				End Property
			End Class
		End Class
	End Class

End Namespace