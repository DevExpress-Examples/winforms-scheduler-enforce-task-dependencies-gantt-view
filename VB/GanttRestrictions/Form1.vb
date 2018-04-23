Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler
Imports System.Data.SqlClient

Namespace GanttRestrictions
	Partial Public Class Form1
		Inherits Form
		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            'TODO: This line of code loads data into the 'GantTest01DataSet1.TaskDependencies' table. You can move, or remove it, as needed.
            Me.TaskDependenciesTableAdapter1.Fill(Me.GantTest01DataSet1.TaskDependencies)
            'TODO: This line of code loads data into the 'GantTest01DataSet1.Resources' table. You can move, or remove it, as needed.
            Me.ResourcesTableAdapter1.Fill(Me.GantTest01DataSet1.Resources)
            'TODO: This line of code loads data into the 'GantTest01DataSet1.Appointments' table. You can move, or remove it, as needed.
            Me.AppointmentsTableAdapter1.Fill(Me.GantTest01DataSet1.Appointments)

			schedulerStorage1.Appointments.CommitIdToDataSource = False

            AddHandler AppointmentsTableAdapter1.Adapter.RowUpdated, AddressOf appointmentsTableAdapter_RowUpdated

			Me.schedulerControl1.OptionsCustomization.AllowAppointmentConflicts = AppointmentConflictsMode.Custom
			AddHandler schedulerControl1.AllowAppointmentConflicts, AddressOf schedulerControl1_AllowAppointmentConflicts
		End Sub

		#Region "#enforcedependencies"
		Private Sub schedulerControl1_AllowAppointmentConflicts(ByVal sender As Object, ByVal e As AppointmentConflictEventArgs)
			e.Conflicts.Clear()

			Dim depCollectionDep As AppointmentDependencyBaseCollection = schedulerStorage1.AppointmentDependencies.Items.GetDependenciesByDependentId(e.Appointment.Id)
			If depCollectionDep.Count > 0 Then
				If CheckForInvalidDependenciesAsDependent(depCollectionDep, e.AppointmentClone) Then
					e.Conflicts.Add(e.AppointmentClone)
				End If
			End If

			Dim depCollectionPar As AppointmentDependencyBaseCollection = schedulerStorage1.AppointmentDependencies.Items.GetDependenciesByParentId(e.Appointment.Id)
			If depCollectionPar.Count > 0 Then
				If CheckForInvalidDependenciesAsParent(depCollectionPar, e.AppointmentClone) Then
					e.Conflicts.Add(e.AppointmentClone)
				End If
			End If
		End Sub

		Private Function CheckForInvalidDependenciesAsDependent(ByVal depCollection As AppointmentDependencyBaseCollection, ByVal apt As Appointment) As Boolean
			For Each dep As AppointmentDependency In depCollection
				If dep.Type = AppointmentDependencyType.FinishToStart Then
					Dim checkTime As DateTime = schedulerStorage1.Appointments.Items.GetAppointmentById(dep.ParentId).End
					If apt.Start < checkTime Then
						Return True
					End If
				End If
			Next dep
			Return False
		End Function

		Private Function CheckForInvalidDependenciesAsParent(ByVal depCollection As AppointmentDependencyBaseCollection, ByVal apt As Appointment) As Boolean
			For Each dep As AppointmentDependency In depCollection
				If dep.Type = AppointmentDependencyType.FinishToStart Then
					Dim checkTime As DateTime = schedulerStorage1.Appointments.Items.GetAppointmentById(dep.DependentId).Start
					If apt.End > checkTime Then
						Return True
					End If
				End If
			Next dep
			Return False
		End Function
		#End Region ' #enforcedependencies


		#Region "#RowUpdatedHandlers"
		Private id As Integer = 0
		Private Sub appointmentsTableAdapter_RowUpdated(ByVal sender As Object, ByVal e As SqlRowUpdatedEventArgs)
			If e.Status = UpdateStatus.Continue AndAlso e.StatementType = StatementType.Insert Then
				id = 0
                Using cmd As New SqlCommand("SELECT @@IDENTITY", AppointmentsTableAdapter1.Connection)
                    id = Convert.ToInt32(cmd.ExecuteScalar())
                    e.Row("UniqueId") = id
                End Using
			End If
		End Sub
		#End Region ' #RowUpdatedHandlers

		#Region "#Appointment"
		Private Sub schedulerStorage1_AppointmentsChanged(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs) Handles schedulerStorage1.AppointmentsChanged
			CommitTask()
		End Sub

		Private Sub schedulerStorage1_AppointmentsDeleted(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs) Handles schedulerStorage1.AppointmentsDeleted
			CommitTask()
		End Sub
		Private Sub schedulerStorage1_AppointmentsInserted(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs) Handles schedulerStorage1.AppointmentsInserted

			CommitTask()
			schedulerStorage1.SetAppointmentId((CType(e.Objects(0), Appointment)), id)
		End Sub
		Private Sub CommitTask()

            AppointmentsTableAdapter1.Update(GantTest01DataSet1)
            Me.GantTest01DataSet1.AcceptChanges()
		End Sub
		#End Region ' #Appointment
		#Region "#TaskDependencies"
		Private Sub schedulerStorage1_AppointmentDependenciesChanged(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs) Handles schedulerStorage1.AppointmentDependenciesChanged
			CommitTaskDependency()
		End Sub

		Private Sub schedulerStorage1_AppointmentDependenciesDeleted(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs) Handles schedulerStorage1.AppointmentDependenciesDeleted
			CommitTaskDependency()
		End Sub

		Private Sub schedulerStorage1_AppointmentDependenciesInserted(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs) Handles schedulerStorage1.AppointmentDependenciesInserted
			CommitTaskDependency()
		End Sub
		Private Sub CommitTaskDependency()
            TaskDependenciesTableAdapter1.Update(Me.GantTest01DataSet1)
            Me.GantTest01DataSet1.AcceptChanges()
		End Sub
		#End Region ' #TaskDependencies
	End Class
End Namespace
