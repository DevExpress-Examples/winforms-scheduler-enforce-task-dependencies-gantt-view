using DevExpress.XtraScheduler;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GanttRestrictions
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'gantTest01DataSet.TaskDependencies' table. You can move, or remove it, as needed.
            this.taskDependenciesTableAdapter.Fill(this.gantTest01DataSet.TaskDependencies);
            // TODO: This line of code loads data into the 'gantTest01DataSet.Resources' table. You can move, or remove it, as needed.
            this.resourcesTableAdapter.Fill(this.gantTest01DataSet.Resources);
            // TODO: This line of code loads data into the 'gantTest01DataSet.Appointments' table. You can move, or remove it, as needed.
            this.appointmentsTableAdapter.Fill(this.gantTest01DataSet.Appointments);
            
            this.appointmentsTableAdapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(appointmentsTableAdapter_RowUpdated);

            this.schedulerControl1.OptionsCustomization.AllowAppointmentConflicts = AppointmentConflictsMode.Custom;
            this.schedulerControl1.AllowAppointmentConflicts+=new AppointmentConflictEventHandler(schedulerControl1_AllowAppointmentConflicts);
        }

        #region #enforcedependencies
        private void schedulerControl1_AllowAppointmentConflicts(object sender, AppointmentConflictEventArgs e)
        {
            e.Conflicts.Clear();

            AppointmentDependencyBaseCollection depCollectionDep = 
                schedulerDataStorage1.AppointmentDependencies.Items.GetDependenciesByDependentId(e.Appointment.Id);
            if (depCollectionDep.Count > 0) {
                if (CheckForInvalidDependenciesAsDependent(depCollectionDep, e.AppointmentClone))
                    e.Conflicts.Add(e.AppointmentClone);
            }

            AppointmentDependencyBaseCollection depCollectionPar = 
                schedulerDataStorage1.AppointmentDependencies.Items.GetDependenciesByParentId(e.Appointment.Id);
            if (depCollectionPar.Count > 0) {
                if (CheckForInvalidDependenciesAsParent(depCollectionPar, e.AppointmentClone))
                    e.Conflicts.Add(e.AppointmentClone);
            }
        }

        private bool CheckForInvalidDependenciesAsDependent(AppointmentDependencyBaseCollection depCollection, Appointment apt)
        {
            foreach (AppointmentDependency dep in depCollection) {
                if (dep.Type == AppointmentDependencyType.FinishToStart) {
                    DateTime checkTime = schedulerDataStorage1.Appointments.Items.GetAppointmentById(dep.ParentId).End;
                    if (apt.Start < checkTime)
                        return true;
                }
            }
            return false;
        }

        private bool CheckForInvalidDependenciesAsParent(AppointmentDependencyBaseCollection depCollection, Appointment apt)
        {
            foreach (AppointmentDependency dep in depCollection) {
                if (dep.Type == AppointmentDependencyType.FinishToStart) {
                    DateTime checkTime = schedulerDataStorage1.Appointments.Items.GetAppointmentById(dep.DependentId).Start;
                    if (apt.End > checkTime)
                        return true;
                }
            }
            return false;
        }
        #endregion #enforcedependencies


        #region #RowUpdatedHandlers
        int id = 0;
        private void appointmentsTableAdapter_RowUpdated(object sender, SqlRowUpdatedEventArgs e)
        {
            if (e.Status == UpdateStatus.Continue && e.StatementType == StatementType.Insert) {
                id = 0;
                using (SqlCommand cmd = new SqlCommand("SELECT @@IDENTITY", appointmentsTableAdapter.Connection)) {
                    id = Convert.ToInt32(cmd.ExecuteScalar());
                    e.Row["UniqueId"] = id;
                }
            }
        }
        #endregion #RowUpdatedHandlers

        #region #Appointment
        private void schedulerDataStorage1_AppointmentsChanged(object sender, PersistentObjectsEventArgs e)
        {
            CommitTask();
        }

        private void schedulerDataStorage1_AppointmentsDeleted(object sender, PersistentObjectsEventArgs e)
        {
            CommitTask();
        }
        private void schedulerDataStorage1_AppointmentsInserted(object sender, PersistentObjectsEventArgs e)
        {

            CommitTask();
            schedulerDataStorage1.SetAppointmentId(((Appointment)e.Objects[0]), id);
        }
        void CommitTask()
        {

            appointmentsTableAdapter.Update(gantTest01DataSet);
            this.gantTest01DataSet.AcceptChanges();
        }
        #endregion #Appointment
        #region #TaskDependencies
        private void schedulerDataStorage1_AppointmentDependenciesChanged(object sender, PersistentObjectsEventArgs e)
        {
            CommitTaskDependency();
        }

        private void schedulerDataStorage1_AppointmentDependenciesDeleted(object sender, PersistentObjectsEventArgs e)
        {
            CommitTaskDependency();
        }

        private void schedulerDataStorage1_AppointmentDependenciesInserted(object sender, PersistentObjectsEventArgs e)
        {
            CommitTaskDependency();
        }
        void CommitTaskDependency()
        {
            taskDependenciesTableAdapter.Update(this.gantTest01DataSet);
            this.gantTest01DataSet.AcceptChanges();
        }
        #endregion #TaskDependencies
    }
}
