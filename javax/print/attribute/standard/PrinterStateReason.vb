Imports System

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.print.attribute.standard


	''' <summary>
	''' Class PrinterStateReason is a printing attribute class, an enumeration,
	''' that provides additional information about the printer's current state,
	''' i.e., information that augments the value of the printer's
	''' <seealso cref="PrinterState PrinterState"/> attribute.
	''' Class PrinterStateReason defines standard printer
	''' state reason values. A Print Service implementation only needs to report
	''' those printer state reasons which are appropriate for the particular
	''' implementation; it does not have to report every defined printer state
	''' reason.
	''' <P>
	''' Instances of PrinterStateReason do not appear in a Print Service's
	''' attribute set directly.
	''' Rather, a <seealso cref="PrinterStateReasons PrinterStateReasons"/>
	''' attribute appears in the Print Service's attribute set. The {@link
	''' PrinterStateReasons PrinterStateReasons} attribute contains zero, one, or
	''' more than one PrinterStateReason objects which pertain to the
	''' Print Service's status, and each PrinterStateReason object is
	''' associated with a <seealso cref="Severity Severity"/> level of REPORT (least severe),
	''' WARNING, or ERROR (most severe). The printer adds a PrinterStateReason
	''' object to the Print Service's
	''' <seealso cref="PrinterStateReasons PrinterStateReasons"/> attribute when the
	''' corresponding condition becomes true of the printer, and the printer
	''' removes the PrinterStateReason object again when the corresponding
	''' condition becomes false, regardless of whether the Print Service's overall
	''' <seealso cref="PrinterState PrinterState"/> also changed.
	''' <P>
	''' <B>IPP Compatibility:</B>
	''' The string values returned by each individual <seealso cref="PrinterStateReason"/> and
	''' associated <seealso cref="Severity"/> object's <CODE>toString()</CODE>
	''' methods, concatenated together with a hyphen (<CODE>"-"</CODE>) in
	''' between, gives the IPP keyword value for a <seealso cref="PrinterStateReasons"/>.
	''' The category name returned by <CODE>getName()</CODE> gives the IPP
	''' attribute name.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	Public Class PrinterStateReason
		Inherits javax.print.attribute.EnumSyntax
		Implements javax.print.attribute.Attribute

		Private Const serialVersionUID As Long = -1623720656201472593L

		''' <summary>
		''' The printer has detected an error other than ones listed below.
		''' </summary>
		Public Shared ReadOnly OTHER As New PrinterStateReason(0)

		''' <summary>
		''' A tray has run out of media.
		''' </summary>
		Public Shared ReadOnly MEDIA_NEEDED As New PrinterStateReason(1)

		''' <summary>
		''' The device has a media jam.
		''' </summary>
		Public Shared ReadOnly MEDIA_JAM As New PrinterStateReason(2)

		''' <summary>
		''' Someone has paused the printer, but the device(s) are taking an
		''' appreciable time to stop. Later, when all output has stopped,
		''' the <seealso cref=" PrinterState PrinterState"/> becomes STOPPED,
		''' and the PAUSED value replaces
		''' the MOVING_TO_PAUSED value in the {@link PrinterStateReasons
		''' PrinterStateReasons} attribute. This value must be supported if the
		''' printer can be paused and the implementation takes significant time to
		''' pause a device in certain circumstances.
		''' </summary>
		Public Shared ReadOnly MOVING_TO_PAUSED As New PrinterStateReason(3)

		''' <summary>
		''' Someone has paused the printer and the printer's {@link PrinterState
		''' PrinterState} is STOPPED. In this state, a printer must not produce
		''' printed output, but it must perform other operations requested by a
		''' client. If a printer had been printing a job when the printer was
		''' paused,
		''' the Printer must resume printing that job when the printer is no longer
		''' paused and leave no evidence in the printed output of such a pause.
		''' This value must be supported if the printer can be paused.
		''' </summary>
		Public Shared ReadOnly PAUSED As New PrinterStateReason(4)

		''' <summary>
		''' Someone has removed a printer from service, and the device may be
		''' powered down or physically removed.
		''' In this state, a printer must not produce
		''' printed output, and unless the printer is realized by a print server
		''' that is still active, the printer must perform no other operations
		''' requested by a client.
		''' If a printer had been printing a job when it was shut down,
		''' the printer need not resume printing that job when the printer is no
		''' longer shut down. If the printer resumes printing such a job, it may
		''' leave evidence in the printed output of such a shutdown, e.g. the part
		''' printed before the shutdown may be printed a second time after the
		''' shutdown.
		''' </summary>
		Public Shared ReadOnly SHUTDOWN As New PrinterStateReason(5)

		''' <summary>
		''' The printer has scheduled a job on the output device and is in the
		''' process of connecting to a shared network output device (and might not
		''' be able to actually start printing the job for an arbitrarily long time
		''' depending on the usage of the output device by other servers on the
		''' network).
		''' </summary>
		Public Shared ReadOnly CONNECTING_TO_DEVICE As New PrinterStateReason(6)

		''' <summary>
		''' The server was able to connect to the output device (or is always
		''' connected), but was unable to get a response from the output device.
		''' </summary>
		Public Shared ReadOnly TIMED_OUT As New PrinterStateReason(7)

		''' <summary>
		''' The printer is in the process of stopping the device and will be
		''' stopped in a while.
		''' When the device is stopped, the printer will change the
		''' <seealso cref="PrinterState PrinterState"/> to STOPPED. The STOPPING reason is
		''' never an error, even for a printer with a single output device. When an
		''' output device ceases accepting jobs, the printer's {@link
		''' PrinterStateReasons PrinterStateReasons} will have this reason while
		''' the output device completes printing.
		''' </summary>
		Public Shared ReadOnly STOPPING As New PrinterStateReason(8)

		''' <summary>
		''' When a printer controls more than one output device, this reason
		''' indicates that one or more output devices are stopped. If the reason's
		''' severity is a report, fewer than half of the output devices are
		''' stopped.
		''' If the reason's severity is a warning, half or more but fewer than
		''' all of the output devices are stopped.
		''' </summary>
		Public Shared ReadOnly STOPPED_PARTLY As New PrinterStateReason(9)

		''' <summary>
		''' The device is low on toner.
		''' </summary>
		Public Shared ReadOnly TONER_LOW As New PrinterStateReason(10)

		''' <summary>
		''' The device is out of toner.
		''' </summary>
		Public Shared ReadOnly TONER_EMPTY As New PrinterStateReason(11)

		''' <summary>
		''' The limit of persistent storage allocated for spooling has been
		''' reached.
		''' The printer is temporarily unable to accept more jobs. The printer will
		''' remove this reason when it is able to accept more jobs.
		''' This value should  be used by a non-spooling printer that only
		''' accepts one or a small number
		''' jobs at a time or a spooling printer that has filled the spool space.
		''' </summary>
		Public Shared ReadOnly SPOOL_AREA_FULL As New PrinterStateReason(12)

		''' <summary>
		''' One or more covers on the device are open.
		''' </summary>
		Public Shared ReadOnly COVER_OPEN As New PrinterStateReason(13)

		''' <summary>
		''' One or more interlock devices on the printer are unlocked.
		''' </summary>
		Public Shared ReadOnly INTERLOCK_OPEN As New PrinterStateReason(14)

		''' <summary>
		''' One or more doors on the device are open.
		''' </summary>
		Public Shared ReadOnly DOOR_OPEN As New PrinterStateReason(15)

		''' <summary>
		''' One or more input trays are not in the device.
		''' </summary>
		Public Shared ReadOnly INPUT_TRAY_MISSING As New PrinterStateReason(16)

		''' <summary>
		''' At least one input tray is low on media.
		''' </summary>
		Public Shared ReadOnly MEDIA_LOW As New PrinterStateReason(17)

		''' <summary>
		''' At least one input tray is empty.
		''' </summary>
		Public Shared ReadOnly MEDIA_EMPTY As New PrinterStateReason(18)

		''' <summary>
		''' One or more output trays are not in the device.
		''' </summary>
		Public Shared ReadOnly OUTPUT_TRAY_MISSING As New PrinterStateReason(19)

		''' <summary>
		''' One or more output areas are almost full
		''' (e.g. tray, stacker, collator).
		''' </summary>
		Public Shared ReadOnly OUTPUT_AREA_ALMOST_FULL As New PrinterStateReason(20)

		''' <summary>
		''' One or more output areas are full (e.g. tray, stacker, collator).
		''' </summary>
		Public Shared ReadOnly OUTPUT_AREA_FULL As New PrinterStateReason(21)

		''' <summary>
		''' The device is low on at least one marker supply (e.g. toner, ink,
		''' ribbon).
		''' </summary>
		Public Shared ReadOnly MARKER_SUPPLY_LOW As New PrinterStateReason(22)

		''' <summary>
		''' The device is out of at least one marker supply (e.g. toner, ink,
		''' ribbon).
		''' </summary>
		Public Shared ReadOnly MARKER_SUPPLY_EMPTY As New PrinterStateReason(23)

		''' <summary>
		''' The device marker supply waste receptacle is almost full.
		''' </summary>
		Public Shared ReadOnly MARKER_WASTE_ALMOST_FULL As New PrinterStateReason(24)

		''' <summary>
		''' The device marker supply waste receptacle is full.
		''' </summary>
		Public Shared ReadOnly MARKER_WASTE_FULL As New PrinterStateReason(25)

		''' <summary>
		''' The fuser temperature is above normal.
		''' </summary>
		Public Shared ReadOnly FUSER_OVER_TEMP As New PrinterStateReason(26)

		''' <summary>
		''' The fuser temperature is below normal.
		''' </summary>
		Public Shared ReadOnly FUSER_UNDER_TEMP As New PrinterStateReason(27)

		''' <summary>
		''' The optical photo conductor is near end of life.
		''' </summary>
		Public Shared ReadOnly OPC_NEAR_EOL As New PrinterStateReason(28)

		''' <summary>
		''' The optical photo conductor is no longer functioning.
		''' </summary>
		Public Shared ReadOnly OPC_LIFE_OVER As New PrinterStateReason(29)

		''' <summary>
		''' The device is low on developer.
		''' </summary>
		Public Shared ReadOnly DEVELOPER_LOW As New PrinterStateReason(30)

		''' <summary>
		''' The device is out of developer.
		''' </summary>
		Public Shared ReadOnly DEVELOPER_EMPTY As New PrinterStateReason(31)

		''' <summary>
		''' An interpreter resource is unavailable (e.g., font, form).
		''' </summary>
		Public Shared ReadOnly INTERPRETER_RESOURCE_UNAVAILABLE As New PrinterStateReason(32)

		''' <summary>
		''' Construct a new printer state reason enumeration value with
		''' the given integer value.
		''' </summary>
		''' <param name="value">  Integer value. </param>
		Protected Friend Sub New(ByVal value As Integer)
			MyBase.New(value)
		End Sub

		Private Shared ReadOnly myStringTable As String() = { "other", "media-needed", "media-jam", "moving-to-paused", "paused", "shutdown", "connecting-to-device", "timed-out", "stopping", "stopped-partly", "toner-low", "toner-empty", "spool-area-full", "cover-open", "interlock-open", "door-open", "input-tray-missing", "media-low", "media-empty", "output-tray-missing", "output-area-almost-full", "output-area-full", "marker-supply-low", "marker-supply-empty", "marker-waste-almost-full", "marker-waste-full", "fuser-over-temp", "fuser-under-temp", "opc-near-eol", "opc-life-over", "developer-low", "developer-empty", "interpreter-resource-unavailable" }

		Private Shared ReadOnly myEnumValueTable As PrinterStateReason() = { OTHER, MEDIA_NEEDED, MEDIA_JAM, MOVING_TO_PAUSED, PAUSED, SHUTDOWN, CONNECTING_TO_DEVICE, TIMED_OUT, STOPPING, STOPPED_PARTLY, TONER_LOW, TONER_EMPTY, SPOOL_AREA_FULL, COVER_OPEN, INTERLOCK_OPEN, DOOR_OPEN, INPUT_TRAY_MISSING, MEDIA_LOW, MEDIA_EMPTY, OUTPUT_TRAY_MISSING, OUTPUT_AREA_ALMOST_FULL, OUTPUT_AREA_FULL, MARKER_SUPPLY_LOW, MARKER_SUPPLY_EMPTY, MARKER_WASTE_ALMOST_FULL, MARKER_WASTE_FULL, FUSER_OVER_TEMP, FUSER_UNDER_TEMP, OPC_NEAR_EOL, OPC_LIFE_OVER, DEVELOPER_LOW, DEVELOPER_EMPTY, INTERPRETER_RESOURCE_UNAVAILABLE }

		''' <summary>
		''' Returns the string table for class PrinterStateReason.
		''' </summary>
		Protected Friend Property Overrides stringTable As String()
			Get
				Return CType(myStringTable.clone(), String())
			End Get
		End Property

		''' <summary>
		''' Returns the enumeration value table for class PrinterStateReason.
		''' </summary>
		Protected Friend Property Overrides enumValueTable As javax.print.attribute.EnumSyntax()
			Get
				Return CType(myEnumValueTable.clone(), javax.print.attribute.EnumSyntax())
			End Get
		End Property


		''' <summary>
		''' Get the printing attribute class which is to be used as the "category"
		''' for this printing attribute value.
		''' <P>
		''' For class PrinterStateReason and any vendor-defined subclasses, the
		''' category is class PrinterStateReason itself.
		''' </summary>
		''' <returns>  Printing attribute class (category), an instance of class
		'''          <seealso cref="java.lang.Class java.lang.Class"/>. </returns>
		Public Property category As Type
			Get
				Return GetType(PrinterStateReason)
			End Get
		End Property

		''' <summary>
		''' Get the name of the category of which this attribute value is an
		''' instance.
		''' <P>
		''' For class PrinterStateReason and any vendor-defined subclasses, the
		''' category name is <CODE>"printer-state-reason"</CODE>.
		''' </summary>
		''' <returns>  Attribute category name. </returns>
		Public Property name As String
			Get
				Return "printer-state-reason"
			End Get
		End Property

	End Class

End Namespace