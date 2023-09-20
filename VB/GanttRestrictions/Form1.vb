Imports DevExpress.XtraScheduler
Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Windows.Forms

Namespace GanttRestrictions

    Public Partial Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs)
            ' TODO: This line of code loads data into the 'gantTest01DataSet.TaskDependencies' table. You can move, or remove it, as needed.
            taskDependenciesTableAdapter.Fill(gantTest01DataSet.TaskDependencies)
            ' TODO: This line of code loads data into the 'gantTest01DataSet.Resources' table. You can move, or remove it, as needed.
            resourcesTableAdapter.Fill(gantTest01DataSet.Resources)
            ' TODO: This line of code loads data into the 'gantTest01DataSet.Appointments' table. You can move, or remove it, as needed.
            appointmentsTableAdapter.Fill(gantTest01DataSet.Appointments)
            AddHandler appointmentsTableAdapter.Adapter.RowUpdated, New SqlRowUpdatedEventHandler(AddressOf appointmentsTableAdapter_RowUpdated)
            schedulerControl1.OptionsCustomization.AllowAppointmentConflicts = AppointmentConflictsMode.Custom
            AddHandler schedulerControl1.AllowAppointmentConflicts, New AppointmentConflictEventHandler(AddressOf schedulerControl1_AllowAppointmentConflicts)
        End Sub

#Region "#enforcedependencies"
        Private Sub schedulerControl1_AllowAppointmentConflicts(ByVal sender As Object, ByVal e As AppointmentConflictEventArgs)
            e.Conflicts.Clear()
            Dim depCollectionDep As AppointmentDependencyBaseCollection = schedulerDataStorage1.AppointmentDependencies.Items.GetDependenciesByDependentId(e.Appointment.Id)
            If depCollectionDep.Count > 0 Then
                If CheckForInvalidDependenciesAsDependent(depCollectionDep, e.AppointmentClone) Then e.Conflicts.Add(e.AppointmentClone)
            End If

            Dim depCollectionPar As AppointmentDependencyBaseCollection = schedulerDataStorage1.AppointmentDependencies.Items.GetDependenciesByParentId(e.Appointment.Id)
            If depCollectionPar.Count > 0 Then
                If CheckForInvalidDependenciesAsParent(depCollectionPar, e.AppointmentClone) Then e.Conflicts.Add(e.AppointmentClone)
            End If
        End Sub

        Private Function CheckForInvalidDependenciesAsDependent(ByVal depCollection As AppointmentDependencyBaseCollection, ByVal apt As Appointment) As Boolean
            For Each dep As AppointmentDependency In depCollection
                If dep.Type = AppointmentDependencyType.FinishToStart Then
                    Dim checkTime As Date = schedulerDataStorage1.Appointments.Items.GetAppointmentById(dep.ParentId).End
                    If apt.Start < checkTime Then Return True
                End If
            Next

            Return False
        End Function

        Private Function CheckForInvalidDependenciesAsParent(ByVal depCollection As AppointmentDependencyBaseCollection, ByVal apt As Appointment) As Boolean
            For Each dep As AppointmentDependency In depCollection
                If dep.Type = AppointmentDependencyType.FinishToStart Then
                    Dim checkTime As Date = schedulerDataStorage1.Appointments.Items.GetAppointmentById(dep.DependentId).Start
                    If apt.End > checkTime Then Return True
                End If
            Next

            Return False
        End Function

#End Region  ' #enforcedependencies
#Region "#RowUpdatedHandlers"
        Private id As Integer = 0

        Private Sub appointmentsTableAdapter_RowUpdated(ByVal sender As Object, ByVal e As SqlRowUpdatedEventArgs)
            If e.Status = UpdateStatus.Continue AndAlso e.StatementType = StatementType.Insert Then
                id = 0
                Using cmd As SqlCommand = New SqlCommand("SELECT @@IDENTITY", appointmentsTableAdapter.Connection)
                    id = Convert.ToInt32(cmd.ExecuteScalar())
                    e.Row("UniqueId") = id
                End Using
            End If
        End Sub

#End Region  ' #RowUpdatedHandlers
#Region "#Appointment"
        Private Sub schedulerDataStorage1_AppointmentsChanged(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
            CommitTask()
        End Sub

        Private Sub schedulerDataStorage1_AppointmentsDeleted(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
            CommitTask()
        End Sub

        Private Sub schedulerDataStorage1_AppointmentsInserted(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
            CommitTask()
            schedulerDataStorage1.SetAppointmentId(CType(e.Objects(0), Appointment), id)
        End Sub

        Private Sub CommitTask()
            appointmentsTableAdapter.Update(gantTest01DataSet)
            gantTest01DataSet.AcceptChanges()
        End Sub

#End Region  ' #Appointment
#Region "#TaskDependencies"
        Private Sub schedulerDataStorage1_AppointmentDependenciesChanged(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
            CommitTaskDependency()
        End Sub

        Private Sub schedulerDataStorage1_AppointmentDependenciesDeleted(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
            CommitTaskDependency()
        End Sub

        Private Sub schedulerDataStorage1_AppointmentDependenciesInserted(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
            CommitTaskDependency()
        End Sub

        Private Sub CommitTaskDependency()
            taskDependenciesTableAdapter.Update(gantTest01DataSet)
            gantTest01DataSet.AcceptChanges()
        End Sub
#End Region  ' #TaskDependencies
    End Class
End Namespace
