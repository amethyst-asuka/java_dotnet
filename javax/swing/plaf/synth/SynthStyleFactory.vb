'
' * Copyright (c) 2002, 2003, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.synth


	''' <summary>
	''' Factory used for obtaining <code>SynthStyle</code>s.  Each of the
	''' Synth <code>ComponentUI</code>s will call into the current
	''' <code>SynthStyleFactory</code> to obtain a <code>SynthStyle</code>
	''' for each of the distinct regions they have.
	''' <p>
	''' The following example creates a custom <code>SynthStyleFactory</code>
	''' that returns a different style based on the <code>Region</code>:
	''' <pre>
	''' class MyStyleFactory extends SynthStyleFactory {
	'''     public SynthStyle getStyle(JComponent c, Region id) {
	'''         if (id == Region.BUTTON) {
	'''             return buttonStyle;
	'''         }
	'''         else if (id == Region.TREE) {
	'''             return treeStyle;
	'''         }
	'''         return defaultStyle;
	'''     }
	''' }
	''' SynthLookAndFeel laf = new SynthLookAndFeel();
	''' UIManager.setLookAndFeel(laf);
	''' SynthLookAndFeel.setStyleFactory(new MyStyleFactory());
	''' </pre>
	''' </summary>
	''' <seealso cref= SynthStyleFactory </seealso>
	''' <seealso cref= SynthStyle
	''' 
	''' @since 1.5
	''' @author Scott Violet </seealso>
	Public MustInherit Class SynthStyleFactory
		''' <summary>
		''' Creates a <code>SynthStyleFactory</code>.
		''' </summary>
		Public Sub New()
		End Sub

		''' <summary>
		''' Returns the style for the specified Component.
		''' </summary>
		''' <param name="c"> Component asking for </param>
		''' <param name="id"> Region identifier </param>
		''' <returns> SynthStyle for region. </returns>
		Public MustOverride Function getStyle(ByVal c As javax.swing.JComponent, ByVal id As Region) As SynthStyle
	End Class

End Namespace