<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128634819/18.1.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E3579)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->

# WinForms Scheduler - Enforce task dependencies in the Gantt view

This example handles the [AllowAppointmentConflicts](https://docs.devexpress.com/WPF/DevExpress.Xpf.Scheduler.SchedulerControl.AllowAppointmentConflicts) event to implement restrictions on appointment editing (appointments linked with a dependency should follow dependency rules).

If two appointments are linked with a [Finish-to-Start](https://docs.devexpress.com/CoreLibraries/DevExpress.XtraScheduler.AppointmentDependencyType) dependency, the `Finish` value of the parent appointment should always be less than or equal to the `Start` value of the dependent appointment. The example uses [GetDependenciesByDependentId](https://docs.devexpress.com/CoreLibraries/DevExpress.XtraScheduler.AppointmentDependencyBaseCollection.GetDependenciesByDependentId(System.Object)) and [GetDependenciesByParentId](https://docs.devexpress.com/CoreLibraries/DevExpress.XtraScheduler.AppointmentDependencyBaseCollection.GetDependenciesByParentId(System.Object)) methods to obtain dependency collections.

```csharp
private void schedulerControl1_AllowAppointmentConflicts(object sender, AppointmentConflictEventArgs e) {
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
```
